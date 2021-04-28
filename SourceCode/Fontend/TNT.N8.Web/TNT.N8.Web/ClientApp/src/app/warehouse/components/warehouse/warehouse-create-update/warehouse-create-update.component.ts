import { Component, OnInit, ElementRef} from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import * as $ from 'jquery';

/* COMPONENTS */
import { PopupComponent } from '../../../../shared/components/popup/popup.component';
/*End*/

/* MODELS */
import { WarehouseModel } from '../../../models/warehouse.model'
/*End*/

/* SERVICES */
import { WarehouseService } from "../../../services/warehouse.service";
import { EmployeeService} from "../../../../employee/services/employee.service";
/*End*/

/* Primeng API */
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
/* end PrimeNg API */

interface DialogResult{
  status: boolean;
  message: string;
}

class keeperModel {
  employeeId: string;
  employeeName: string;
  employeeCode: string;
}

class warehouseModel {
  warehouseId: string;
  warehouseCode: string;
  warehouseName: string;
  warehouseParent: string;
  warehouseAddress: string;
  warehousePhone: string;
  storagekeeper: string;
  warehouseDescription: string;
  active: boolean;
  createdDate: Date;
  createdById: string;
  updatedDate: Date;
  updatedById: string;
}

@Component({
  selector: 'app-warehouse-create-update',
  templateUrl: './warehouse-create-update.component.html',
  styleUrls: ['./warehouse-create-update.component.css'],
  providers: [ConfirmationService, MessageService]
})

export class WarehouseCreateUpdateComponent implements OnInit {
  loading: boolean = false;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  emptyGuid = '00000000-0000-0000-0000-000000000000';
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  //master data
  listKeeper: Array<keeperModel> = [];
  listWarehouseCode: Array<string> = [];
  //pop up 
  isEdit: boolean = false;
  editedWarehouseModel: warehouseModel;
  listWarehouseNameEqualLevel: Array<string> = [];
  parentId: string;
  //forms
  createWarehouseForm: FormGroup;
  constructor(
    private warehouseService: WarehouseService,
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private el: ElementRef,
  ) { }

  async ngOnInit() {
    this.initForm();
    this.getMasterdata();
    this.isEdit = this.config.data.isEdit;
    this.listWarehouseNameEqualLevel = this.config.data.listWarehouseNameEqualLevel;
    this.parentId = this.config.data.parentId;
 
    if (this.isEdit == true) {
      this.editedWarehouseModel =  this.config.data.editedWarehouseModel;
      this.mappingModelToForm(this.editedWarehouseModel);
      this.listWarehouseNameEqualLevel =  this.listWarehouseNameEqualLevel.filter(e => e !== this.editedWarehouseModel.warehouseName);
    } 

    if (this.parentId !== null){
      this.createWarehouseForm.get('Keeper').disable();
    } 

    this.createWarehouseForm.get('Name').setValidators([Validators.required, checkDuplicateCode(this.listWarehouseNameEqualLevel), checkBlankString()]);
    this.createWarehouseForm.updateValueAndValidity();
  }

  async getMasterdata() {
    this.loading = true;
     let result: any = await this.warehouseService.createUpdateWarehouseMasterdata(null, this.auth.UserId);
     this.loading = false;
     if (result.statusCode === 200) {
       this.listKeeper = result.listEmployeeEntityModel;
       this.listWarehouseCode = result.listWarehouseCode;   
       if (this.isEdit == true) {
         this.listWarehouseCode = this.listWarehouseCode.filter(e => e !== this.editedWarehouseModel.warehouseCode);
        this.getdataAfterMasterdata();
       }    
       this.createWarehouseForm.get('Code').setValidators([Validators.required, checkDuplicateCode(this.listWarehouseCode), checkBlankString()]);
       this.createWarehouseForm.updateValueAndValidity();
     } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
     }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  initForm() {
    this.createWarehouseForm = new FormGroup({
        'Name': new FormControl('', Validators.required),
        'Code': new FormControl('', Validators.required),
        'Address': new FormControl(''),
        'Phone': new FormControl('', [Validators.pattern(this.getPhonePattern())]),
        'Keeper': new FormControl(null),
        'Note': new FormControl('')
    });
  }

  resetForm() {
    this.createWarehouseForm.reset();
    this.createWarehouseForm.patchValue({
      'Name': new FormControl('', Validators.required),
      'Code': new FormControl('', Validators.required),
      'Address': new FormControl(''),
      'Phone': new FormControl(''),
      'Keeper': new FormControl(''),
      'Note': new FormControl('')
    });
  }

  cancel() {   
    this.confirmationService.confirm({
      message: 'Các thay đổi sẽ không được lưu lại. Hành động này không thể được hoàn tác, bạn có chắc chắn muốn hủy?',
      accept: () => {
        this.ref.close();
      }
    });
  }

  getdataAfterMasterdata(){
    let keeperValue =  this.editedWarehouseModel.storagekeeper;
    if (keeperValue == null) {
      this.createWarehouseForm.get('Keeper').setValue(null);
    } else {
      this.createWarehouseForm.get('Keeper').setValue(this.listKeeper.find(e => e.employeeId == keeperValue));
    }
  }

  async createWarehouse() {
    if (!this.createWarehouseForm.valid) {
      Object.keys(this.createWarehouseForm.controls).forEach(key => {
        if (!this.createWarehouseForm.controls[key].valid) {
          this.createWarehouseForm.controls[key].markAsTouched();
        }
      });
      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else { 
      //valid
      let warehouse: WarehouseModel = this.mappingFormToModel();
      this.loading = true;
      let result: any = await this.warehouseService.createWareHouseAsync(warehouse);
      this.loading = false;
      if (result.statusCode === 200) {
        if (this.isEdit == false) {
          //tao moi
          let resultDialog: DialogResult =  {
            status: true,
            message: "Tạo thành công"
           };
          this.ref.close(resultDialog);
        } else {
          //edit
          let resultDialog: DialogResult =  {
            status: true,
            message: "Chỉnh sửa thành công"
           };
          this.ref.close(resultDialog);
        }
      } else {
        let resultDialog: DialogResult = {
          status: false,
          message: result.messageCode
         };
        this.ref.close(resultDialog);
      }  
    }
  }

  mappingModelToForm(editedWarehouseModel: warehouseModel) {
    this.createWarehouseForm.get('Name').setValue(editedWarehouseModel.warehouseName);
    this.createWarehouseForm.get('Code').setValue(editedWarehouseModel.warehouseCode);
    this.createWarehouseForm.get('Address').setValue(editedWarehouseModel.warehouseAddress);
    this.createWarehouseForm.get('Phone').setValue(editedWarehouseModel.warehousePhone); 
    this.createWarehouseForm.get('Note').setValue(editedWarehouseModel.warehouseDescription);
  }

  mappingFormToModel(): WarehouseModel{
    let warehouseModel = new WarehouseModel();
    if (this.isEdit == false) {
      warehouseModel.WarehouseId = this.emptyGuid;
    } else {
      warehouseModel.WarehouseId = this.editedWarehouseModel.warehouseId;
    }
    //form values
    warehouseModel.WarehouseCode = this.createWarehouseForm.get('Code').value;
    warehouseModel.WarehouseName = this.createWarehouseForm.get('Name').value;
    warehouseModel.WarehouseAddress =  this.createWarehouseForm.get('Address').value;
    warehouseModel.WarehousePhone =  this.createWarehouseForm.get('Phone').value;
    let keeperValue = this.createWarehouseForm.get('Keeper').value;
    if (keeperValue == null) {
      warehouseModel.Storagekeeper = null;
    } else {
      warehouseModel.Storagekeeper =  this.createWarehouseForm.get('Keeper').value.employeeId;
    }
    warehouseModel.WarehouseDescription = this.createWarehouseForm.get('Note').value;
    //default value
    warehouseModel.WarehouseParent = this.parentId;
    warehouseModel.Active = true;
    warehouseModel.CreatedDate = new Date();
    warehouseModel.CreatedById = this.auth.UserId;
    warehouseModel.UpdatedDate = new Date();
    warehouseModel.UpdatedById =  this.auth.UserId;
    
    return warehouseModel;
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }
  /*end*/
}

function checkBlankString(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() === "") {
        return { 'blankString': true };
      }
    }
    return null;
  }
}

function checkDuplicateCode(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() !== "") {
        let duplicateCode = array.find(e => e.trim().toLowerCase() === control.value.trim().toLowerCase());
        if (duplicateCode !== undefined) {
          return { 'duplicateCode': true };
        }
      }
    }
    return null;
  }
}
import { Component, OnInit, ElementRef, Inject } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission';
import { VendorModel } from '../../../vendor/models/vendor.model';
import { VendorService } from '../../../vendor/services/vendor.service';

import { ProductService } from '../../services/product.service';
import * as $ from 'jquery';

/* Primeng API */
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
/* end PrimeNg API */


interface DialogResult {
  status: boolean;
  vendorModel: vendorModel2;
}

class vendorCategory {
  categoryId: string;
  categoryName: string;
  isDefauld: boolean;
}

class vendorModel2 {
  public vendorId: string;
  public vendorName: string;
  public vendorCode: string;
}

@Component({
  selector: 'app-quick-create-vendor',
  templateUrl: './quick-create-vendor.component.html',
  styleUrls: ['./quick-create-vendor.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})

export class QuickCreateVendorComponent implements OnInit {
  vendorCodePattern = '[a-zA-Z0-9]+$';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  //master data
  listVendorCategory: Array<vendorCategory> = [];
  listVendorCode: Array<string> = [];
  //form controls
  createVendorForm: FormGroup;

  constructor(private el: ElementRef,
    private translate: TranslateService,
    private vendorService: VendorService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private productService: ProductService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
  ) { translate.setDefaultLang('vi'); }

  async ngOnInit() {
    this.initForm();
    this.getMasterData();
  }

  createVendor() {
  }

  initForm() {
    this.createVendorForm = new FormGroup({
      'VendorGroup': new FormControl(null, Validators.required),
      'VendorName': new FormControl('', [Validators.required, checkBlankString()]),
      'VendorCode': new FormControl('', [Validators.required, Validators.pattern(this.vendorCodePattern)])
    });
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.vendorService.quickCreateVendorMasterdata();
    this.loading = false;
    if (result.statusCode === 200) {
      this.listVendorCategory = result.listVendorCategory;
      this.listVendorCode = result.listVendorCode;
      this.createVendorForm.get('VendorCode').setValidators([Validators.required, checkDuplicateCode(this.listVendorCode), checkBlankString(), Validators.pattern(this.vendorCodePattern)]);
      this.createVendorForm.updateValueAndValidity();
      this.createVendorForm.updateValueAndValidity();
    }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  onCancel() {
    this.confirmationService.confirm({
      message: 'Hành động này không thể được hoàn tác, bạn có chắc chắn muốn hủy?',
      accept: () => {
        let dialogResult: DialogResult = {
          status: false,
          vendorModel: null
        }
        this.ref.close(dialogResult)
      }
    });
  }

  onSave() {
    if (!this.createVendorForm.valid) {
      Object.keys(this.createVendorForm.controls).forEach(key => {
        if (!this.createVendorForm.controls[key].valid) {
          this.createVendorForm.controls[key].markAsTouched();
        }
      });
      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {
      //valid form
      let vendorModel = this.mappingFormToModel();
      this.loading = true;
      this.vendorService.quickCreateVendor(vendorModel, '', '', '').subscribe(response => {
        this.loading = false;
        let result = <any>response;

        if (result.statusCode == 200) {
          let newVendor: vendorModel2 = new vendorModel2();
          newVendor.vendorId = result.vendorId;
          newVendor.vendorName = vendorModel.VendorName;
          newVendor.vendorCode = vendorModel.VendorCode;
          let dialogResult: DialogResult = {
            status: true,
            vendorModel: newVendor
          }
          this.ref.close(dialogResult);
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      }, error => { this.loading = false; });
    }
  }

  mappingFormToModel(): VendorModel {
    let vendorModel = new VendorModel();
    vendorModel.VendorId = this.emptyGuid;
    vendorModel.VendorName = this.createVendorForm.get('VendorName').value;
    vendorModel.VendorCode = this.createVendorForm.get('VendorCode').value;
    vendorModel.VendorGroupId = this.createVendorForm.get('VendorGroup').value.categoryId;
    vendorModel.PaymentId = this.emptyGuid;
    vendorModel.TotalPurchaseValue = 0;
    vendorModel.TotalPayableValue = 0;
    vendorModel.NearestDateTransaction = null;
    vendorModel.CreatedById = this.auth.UserId;
    vendorModel.CreatedDate = new Date();
    vendorModel.UpdatedById = null;
    vendorModel.UpdatedDate = null;
    vendorModel.Active = true;
    return vendorModel;
  }
  /* end */
}


function checkDuplicateCode(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() !== "") {
        let duplicateCode = array.find(e => e.toLowerCase() === control.value.trim().toLowerCase());
        if (duplicateCode !== undefined) {
          return { 'duplicateCode': true };
        }
      }
    }
    return null;
  }
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

import { Component, OnInit, ElementRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl, FormArray } from '@angular/forms';
import * as $ from 'jquery';
import { TranslateService } from '@ngx-translate/core';
import { VendorService } from "../../services/vendor.service";
import { VendorModel } from "../../models/vendor.model";
import { ContactModel } from "../../../shared/models/contact.model";
import { VendorBankAccountModel } from "../../models/vendorBankAccount.model";
import { GetPermission } from '../../../shared/permission/get-permission';
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService } from 'primeng/api';

import { AddVendorContactDialogComponent } from '../add-vendor-contact-dialog/add-vendor-contact-dialog.component';

class vendorGroupModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}

class provinceModel {
  provinceId: string;
  provinceName: string;
  provinceType: string;
  provinceCode: string;
}

class districtModel {
  districtId: string;
  districtName: string;
  districtType: string;
  districtCode: string;
  provinceId: string;
}

class wardModel {
  wardId: string;
  wardCode: string;
  wardType: string;
  wardName: string;
  districtId: string;
}

class contactVendorDialogModel {
  FullName: string;
  GenderDisplay: string;
  GenderName: string;
  Role: string;
  Phone: string;
  Email: string;
}

@Component({
  selector: 'app-create-vendor',
  templateUrl: './create-vendor.component.html',
  styleUrls: ['./create-vendor.component.css'],
  // providers: [ConfirmationService, MessageService, DialogService]
})

export class CreateVendorComponent implements OnInit {
  emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
  vendorCodePattern = '[a-zA-Z0-9]+$';
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));

  genders = [{
    code: 'NAM', name: 'Ông'
  },
  {
    code: 'NU', name: 'Bà'
  }];

  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  createVendorForm: FormGroup;
  //master data
  listVendorGroup: Array<vendorGroupModel> = [];
  listProvince: Array<provinceModel> = [];
  listDistrict: Array<districtModel> = [];
  listCurrentDistrict: Array<districtModel> = [];
  listWard: Array<wardModel> = [];
  listCurrentWard: Array<wardModel> = [];
  listVendorCode: Array<string> = [];
  //table
  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  listContact: Array<contactVendorDialogModel> = [];
  rows: number = 10;
  selectedContact: ContactModel;
  isInVietNam: boolean = true;
  // let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
  // let bankAccPattern = '^[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$';

  constructor(private translate: TranslateService,
    private getPermission: GetPermission,
    private vendorService: VendorService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private route: ActivatedRoute,
    private router: Router,
    public builder: FormBuilder,
    private el: ElementRef,
  ) {
    translate.setDefaultLang('vi');
  }

  async ngOnInit() {
    let resource = "buy/vendor/create/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }

      this.initForm();
      this.initTable();
      this.getMasterData();
    }
  }

  initForm() {
    this.createVendorForm = new FormGroup({
      'VendorGroup': new FormControl(null, [Validators.required]),
      'VendorCode': new FormControl('', [Validators.required, Validators.pattern(this.vendorCodePattern)]),
      'VendorName': new FormControl('', [Validators.required, checkBlankString()]),
      'Email': new FormControl('', [Validators.pattern(this.emailPattern)]),
      'Phone': new FormControl('', [Validators.pattern(this.getPhonePattern())]),
      'Location': new FormControl("1"),
      'Province': new FormControl(null),
      'District': new FormControl(null),
      'Ward': new FormControl(null),
      'Address': new FormControl('')
    });
  }

  initTable() {
    this.columns = [
      { field: 'FullName', header: 'Họ tên', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'GenderName', header: 'Giới tính', width: '150px', textAlign: 'center', color: '#f44336' },
      { field: 'Role', header: 'Chức vụ', width: '50px', textAlign: 'center', color: '#f44336' },
      { field: 'Phone', header: 'Số điện thoại', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'Email', header: 'Email', width: '50px', textAlign: 'left', color: '#f44336' }
    ];
    this.selectedColumns = this.columns;
  }

  resetForm() {
    this.createVendorForm.reset();
    this.createVendorForm.get('VendorGroup').patchValue(null);
    this.createVendorForm.get('VendorCode').patchValue('');
    this.createVendorForm.get('VendorName').patchValue('');
    this.createVendorForm.get('Email').patchValue('');
    this.createVendorForm.get('Phone').patchValue('');
    this.createVendorForm.get('Location').patchValue('1');
    this.createVendorForm.get('Province').patchValue(null);
    this.createVendorForm.get('District').patchValue(null);
    this.createVendorForm.get('Ward').patchValue(null);
    this.createVendorForm.get('Address').patchValue('');
  }

  resetTable() {
    this.listContact = [];
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  fixed: boolean = false;
  withFiexd: string = "";
  // @HostListener('document:scroll', [])
  // onScroll(): void {
  //   let num = window.pageYOffset;
  //   if (num > 110) {
  //     this.fixed = true;
  //     var width: number = $('#parent').width();
  //     this.withFiexd = width + 'px';
  //   } else {
  //     this.fixed = false;
  //     this.withFiexd = "";
  //   }
  // }

  changeProvince(event: any): boolean {
    //reset list district and ward
    this.listCurrentDistrict = [];
    this.listCurrentWard = [];
    this.createVendorForm.get('District').setValue(null);
    this.createVendorForm.get('Ward').setValue(null);
    if (event.value === null) return false;
    let currentProvince: provinceModel = event.value;
    this.listCurrentDistrict = this.listDistrict.filter(e => e.provinceId === currentProvince.provinceId);
    return false;
  }

  changeDistrict(event: any): boolean {
    //reset ward
    this.listCurrentWard = [];
    this.createVendorForm.get('Ward').setValue(null);
    if (event.value === null) return false;
    let currentDistrict: districtModel = event.value;
    this.listCurrentWard = this.listWard.filter(e => e.districtId === currentDistrict.districtId);
    return false
  }

  changeLocation() {
    let value = this.createVendorForm.get('Location').value;
    switch (value) {
      case "1":
        //trong nuoc
        this.isInVietNam = true;
        break;
      case "2":
        //nuoc ngoai: clear province, district, ward
        this.createVendorForm.get('Province').reset();
        this.createVendorForm.get('District').reset();
        this.createVendorForm.get('Ward').reset();
        this.createVendorForm.get('Province').patchValue(null);
        this.createVendorForm.get('District').patchValue(null);
        this.createVendorForm.get('Ward').patchValue(null);
        this.isInVietNam = false;
        break;
      default:
        break;
    }
  }

  deleteContact(rowData: contactVendorDialogModel) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.listContact = this.listContact.filter(e => e != rowData);
      }
    });
  }

  goBackToList() {
    if (this.createVendorForm.pristine && this.listContact.length == 0) {
      this.router.navigate(['/vendor/list']);
    } else {
      //confirm dialog
      this.confirmationService.confirm({
        message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác, bạn có chắc chắn muốn hủy?',
        accept: () => {
          this.router.navigate(['/vendor/list']);
        }
      });
    }
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.vendorService.getDataCreateVendor(this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listProvince = result.listProvince;
      this.listDistrict = result.listDistrict;
      this.listWard = result.listWard;
      this.listVendorGroup = result.listVendorGroup;
      this.listVendorCode = result.listVendorCode;
      //set duplicate code validation
      this.createVendorForm.get('VendorCode').setValidators([Validators.required, checkDuplicateCode(this.listVendorCode), checkBlankString(), Validators.pattern(this.vendorCodePattern)]);
      this.createVendorForm.updateValueAndValidity();
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
    }
  }

  addVendorContact() {
    let ref = this.dialogService.open(AddVendorContactDialogComponent, {
      header: 'Thêm người liên hệ',
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px",
        "max-height": "600px",
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status === true) {
          let newContact = result.contactModel;
          this.listContact = [...this.listContact, newContact];
        }
      }
    });
  }

  editContact(rowData: contactVendorDialogModel) {
    let ref = this.dialogService.open(AddVendorContactDialogComponent, {
      header: 'Chỉnh sửa người liên hệ',
      data: {
        isEdit: true,
        contact: rowData
      },
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px",
        "max-height": "600px",
      }
    });

    ref.onClose.subscribe(async (result: any) => {
      if (result) {
        if (result.status === true) {
          //replace rowdata cũ
          let newContact: contactVendorDialogModel = result.contactModel;
          const index = this.listContact.indexOf(rowData);
          this.listContact[index] = newContact;
          this.listContact = [...this.listContact];
        }
      }
    });
  }

  async createVendor(type: boolean) {
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
      //valid
      let vendorModel: VendorModel = this.mapFormToVendorModel();
      let contactModel: ContactModel = this.mapFormToVendorContact();
      let listVendorContactList: Array<ContactModel> = this.mapListContact();
      this.loading = true;
      this.vendorService.createVendor(vendorModel, contactModel, listVendorContactList, this.auth.UserId).subscribe(response => {
        this.loading = false;
        let result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          switch (type) {
            case true:
              //lưu và tạo mới
              this.resetForm();
              this.resetTable();
              this.clearToast();
              this.showToast('success', 'Thông báo', 'Tạo nhà cung cấp thành công');
              break;
            case false:
              //lưu
              this.router.navigate(['/vendor/detail', { vendorId: result.vendorId, contactId: result.contactId }]);
              break;
            default:
              break;
          }
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      }, error => { this.loading = false; });

    }
  }

  mapFormToVendorModel(): VendorModel {
    let vendorModel = new VendorModel();
    vendorModel.VendorId = this.emptyGuid;
    vendorModel.VendorName = this.createVendorForm.get('VendorName').value;
    vendorModel.VendorCode = this.createVendorForm.get('VendorCode').value;
    let vendorGroup = this.createVendorForm.get('VendorGroup').value
    vendorModel.VendorGroupId = vendorGroup !== null ? vendorGroup.categoryId : this.emptyGuid;
    vendorModel.PaymentId = this.emptyGuid;
    vendorModel.TotalPurchaseValue = 0;
    vendorModel.TotalPayableValue = 0;

    vendorModel.Active = true;
    vendorModel.CreatedById = this.auth.UserId;
    vendorModel.CreatedDate = new Date();
    vendorModel.UpdatedById = null;
    vendorModel.UpdatedDate = null;

    return vendorModel;
  }

  mapFormToVendorContact(): ContactModel {
    let contactModel = new ContactModel();
    contactModel.ContactId = this.emptyGuid;
    contactModel.ObjectId = this.emptyGuid;
    contactModel.ObjectType = "VEN";
    contactModel.Email = this.createVendorForm.get('Email').value;
    contactModel.Phone = this.createVendorForm.get('Phone').value;
    let _province = this.createVendorForm.get('Province').value;
    let _district = this.createVendorForm.get('District').value;
    let _ward = this.createVendorForm.get('Ward').value;
    contactModel.ProvinceId = _province !== null ? _province.provinceId : null;
    contactModel.DistrictId = _district !== null ? _district.districtId : null;
    contactModel.WardId = _ward !== null ? _ward.wardId : null;
    contactModel.Address = this.createVendorForm.get('Address').value;

    contactModel.Active = true;
    contactModel.CreatedById = this.auth.UserId;
    contactModel.CreatedDate = new Date();
    contactModel.UpdatedById = null;
    contactModel.UpdatedDate = null;
    return contactModel;
  };

  mapListContact(): Array<ContactModel> {
    let listContact: Array<ContactModel> = [];
    this.listContact.forEach(_contact => {
      let newContact: ContactModel = new ContactModel();
      newContact.ContactId = this.emptyGuid;
      newContact.ObjectId = this.emptyGuid;
      newContact.ObjectType = "VEN_CON";
      newContact.FirstName = _contact.FullName;
      newContact.LastName = "";
      newContact.Gender = _contact.GenderDisplay;
      newContact.Phone = _contact.Phone;
      newContact.Email = _contact.Email;
      newContact.Role = _contact.Role;
      newContact.Active = true;
      newContact.CreatedDate = new Date();
      newContact.CreatedById = this.auth.UserId;
      listContact.push(newContact);
    });
    return listContact;
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
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


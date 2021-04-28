import { Component, OnInit, ElementRef, Inject, ViewChild, HostListener, Renderer2, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import * as $ from 'jquery';

import { TranslateService } from '@ngx-translate/core';
import { ConfirmationService } from 'primeng/api';
import { MessageService } from 'primeng/api';
import { CustomerService } from '../../services/customer.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { EmailConfigService } from '../../../admin/services/email-config.service';

import { CustomerModel } from "../../models/customer.model";
import { ContactModel } from "../../../shared/models/contact.model";
import { SendEmailModel } from '../../../admin/models/sendEmail.model';
import { BankService } from '../../../shared/services/bank.service';
import { DynamicDialogRef } from 'primeng';

class leadReferenceCustomer {
  customerCode: string;
  customerStatus: string;
  customerId: string;
  customerName: string;
  customerType: number;
  personInChargeId: string;
  phone: string;
  address: string;
  email: string;
  workEmail: string;
  investmentFundId: string;
}

class Category {
  public categoryId: string;
  public categoryCode: string;
  public categoryName: string;
  public categoryTypeId: string;
  public isDefault: boolean;
}

class Province {
  public provinceId: string;
  public provinceCode: string;
  public provinceName: string;
  public provinceType: string;
}

class District {
  public districtId: string;
  public districtName: string;
  public districtCode: string;
  public districtType: string;
  public provinceId: string;
}

class Ward {
  public wardId: string;
  public wardName: string;
  public wardCode: string;
  public wardType: string;
  public districtId: string;
}

class Employee {
  public employeeId: string;
  public employeeName: string;
  public employeeCode: string;
}

class leadModel {
  public leadId: string;
  public personInChargeId: string;
}

interface BankAccount {
  bankAccountId: string,
  objectId: string,
  objectType: string,
  bankName: string, //Tên ngân hàng
  accountNumber: string,  //Số tài khoản
  accountName: string,  //Chủ tài khoản
  branchName: string, //Chi nhánh
  bankDetail: string,  //Diễn giải
  createdById: string,
  createdDate: Date
}

class contactModel {
  public contactId: string;
  public objectId: string;
  public companyName: string;
  public firstName: string;
  public lastName: string;
  public phone: string;
  public gender: string;
  public workPhone: string;
  public otherPhone: string;
  public email: string;
  public workEmail: string;
  public otherEmail: string;
  public provinceId: string;
  public districtId: string;
  public wardId: string;
}
class GeographicalArea {
  geographicalAreaId: string;
  geographicalAreaCode: string;
  geographicalAreaName: string;
}
interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  customer: leadReferenceCustomer
}

@Component({
  selector: 'app-add-customer-dialog',
  templateUrl: './add-customer-dialog.component.html',
  styleUrls: ['./add-customer-dialog.component.css']
})
export class AddCustomerDialogComponent implements OnInit {
  emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
  loading: boolean = false;
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  //list action in page
  actionAdd: boolean = true;
  awaitResult: boolean = false;
  /*End*/
  isManager: boolean = localStorage.getItem('IsManager') === "true" ? true : false;
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;

  /* master data */
  listBusinessLocal: Array<Category> = []; //trong nước - nước ngoài
  listBusinessScale: Array<Category> = []; //quy mô
  listCustomerGroup: Array<Category> = []; //nhom khach hang
  listEnterPriseType: Array<Category> = []; //loai hinh
  listMainBusiness: Array<Category> = []; //nganh nghe thu nhap chinh
  listPosition: Array<Category> = []; //chuc vu

  listProvince: Array<Province> = []; //master data
  listDistrict: Array<District> = []; //master data
  listWard: Array<Ward> = []; //master data
  listArea: Array<GeographicalArea> = [];

  listProvinceCompanyCustomer: Array<Province> = [];
  listDistrictCompanyCustomer: Array<District> = [];
  listWardCompanyCustomer: Array<Ward> = [];

  listProvincePersonalCustomer: Array<Province> = [];
  listDistrictPersonalCustomer: Array<District> = [];
  listWardPersonalCustomer: Array<Ward> = [];

  listProvinceHouseHoldCustomer: Array<Province> = [];
  listDistrictHouseHoldCustomer: Array<District> = [];
  listWardHouseHoldCustomer: Array<Ward> = [];
  listEmployee: Array<Employee> = [];
  listDefaultAccount: Array<any> = [];
  listTermsPayment: Array<any> = [];
  listCustomerCode: Array<string> = [];
  listCustomeTax: Array<string> = [];
  listPhoneContactCode: Array<any> = [];
  listEmailContactCode: Array<string> = [];
  /* end */

  /* form */
  customerType: number = 1;
  isInland: number = 1;
  isInlandLH: number = 1;
  listGenders = [{ code: 'NAM', name: 'Nam' }, { code: 'NU', name: 'Nữ' }];
  companyCustomerForm: FormGroup;
  contactCustomerForm: FormGroup;
  personalCustomerForm: FormGroup;
  /* contact table */
  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  listContact: Array<ContactModel> = [];
  rows: number = 10;
  selectedContact: ContactModel;

  /* create by lead */
  leadModel: leadModel;
  contactModel: contactModel;

  isCheckedLead: boolean = false;
  leadId: string = null;
  chooseCusVenEmp: string[] = ["CUS"]
  isCustomerCodeLikeTaxCode: boolean = true;
  bankAccount: BankAccount = {
    bankAccountId: null,
    objectId: '',
    objectType: '',
    accountNumber: '',
    bankName: '',
    bankDetail: '',
    branchName: '',
    accountName: '',
    createdById: this.auth.UserId,
    createdDate: new Date()
  }
  /* create valiable add class scroll */
  fixed: boolean = false;
  isInvalidForm: boolean = false;
  isInvalidFormCN: boolean = false;
  isOpenNotifiError: boolean = false;
  isOpenNotifiErrorCN: boolean = false;
  withFiexd: string = "";

  @ViewChild('toggleButton') toggleButton: ElementRef;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;

  listCustomer: Array<any> = [];
  filteredCustomer: Array<any> = [];

  validEmailCustomer: boolean = true;
  validPhoneCustomer: boolean = true;

  constructor(
    public messageService: MessageService,
    private getPermission: GetPermission,
    private translate: TranslateService,
    private bankService: BankService,
    private customerService: CustomerService,
    private confirmationService: ConfirmationService,
    private route: ActivatedRoute,
    private router: Router,
    public builder: FormBuilder,
    private renderer: Renderer2,
    private el: ElementRef,
    private emailConfigService: EmailConfigService,
    private cdRef: ChangeDetectorRef,
    public ref: DynamicDialogRef,
  ) {
    translate.setDefaultLang('vi');
    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (this.saveAndCreate) {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target) &&
            !this.saveAndCreate.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
            this.isOpenNotifiErrorCN = false;
          }
        } else {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
            this.isOpenNotifiErrorCN = false;
          }
        }
      }
    });
  }

  ngAfterViewChecked() {
    this.cdRef.detectChanges();
  }

  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width() + 30;
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  async ngOnInit() {
    //Check permission
    let resource = "crm/customer/create";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      setTimeout(() => {
        this.showToast('popup', 'warn', 'Thông báo', 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ');
        setTimeout(() => {
          this.router.navigate(['/home']);
        }, 1000);
      }, 0);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      } else {
        this.initTable();
        this.initForm();
        this.getMasterData();
      }
    }
  }

  initTable() {
    this.columns = [
      { field: 'FullName', header: 'Người liên hệ', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'GenderDisplay', header: 'Giới tính', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'Role', header: 'Chức vụ', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'Phone', header: 'Số điện thoại', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'Email', header: 'Email', width: '50px', textAlign: 'left', color: '#f44336' }
    ];
    this.selectedColumns = this.columns;
  }

  initForm() {
    this.companyCustomerForm = new FormGroup({
      "CustomerGroup": new FormControl(null),
      "CustomerCode": new FormControl("", [Validators.required, checkDuplicateCode(this.listCustomerCode), checkBlankString()]),
      "provinceControl": new FormControl(""),
      "districtControl": new FormControl(""),
      "wardControl": new FormControl(""),
      "addressControl": new FormControl(""),
      "CustomerTax": new FormControl(""),
      "CompanyName": new FormControl("", [Validators.required, checkBlankString()]),
      "Email": new FormControl("", [Validators.pattern(this.emailPattern)]),
      "Phone": new FormControl("", [Validators.required, Validators.pattern(this.getPhonePattern())]),
      "BusinessLicenseDate": new FormControl(""),
      "Local": new FormControl(1),
      "CustomerCodeIsTaxCode": new FormControl(true),
      "SaleEmployee": new FormControl(null, [Validators.required]),
      "SupportEmployee": new FormControl(null),
      "TotalIncomeLastYear": new FormControl(''),
      "EnterpriseType": new FormControl(null),
      "TotalEmployeeInsurance": new FormControl(null),
      "BusinessScale": new FormControl(null),
      "TotalCapital": new FormControl(''),
      "Note": new FormControl(''),
      "isInlandControl": new FormControl('1'),
      "isCustomerCodeLikeTaxCodeControl": new FormControl(''),
      "chooseVendor": new FormControl(''),
      "chooseCustomer": new FormControl(''),
      "chooseEmployee": new FormControl(''),
      "BankName": new FormControl(''),
      "BankCode": new FormControl(''),
      "BankUserName": new FormControl(''),
      "BankAdress": new FormControl(''),
      "MoneyLimit": new FormControl(''),
      "TermsPayment": new FormControl(''),
      "DefaultAccount": new FormControl(''),
      'areaControl': new FormControl(null),

    });
    this.contactCustomerForm = new FormGroup({
      "FirstName": new FormControl("", [Validators.required, checkBlankString()]),
      "LastName": new FormControl("", [Validators.required, checkBlankString()]),
      "Email": new FormControl("", [Validators.pattern(this.emailPattern), checkDuplicateCode(this.listEmailContactCode)]),
      "Position": new FormControl(""),
      "DateOfBirth": new FormControl(null),
      "Phone": new FormControl("", [Validators.required, Validators.pattern(this.getPhonePattern()), checkDuplicateCode(this.listPhoneContactCode)]),
      "OtherInfor": new FormControl(""),
      "Gender": new FormControl(this.listGenders[0]),
      "isInlandLHControl": new FormControl(''),
      "provinceControl": new FormControl(""),
      "districtControl": new FormControl(""),
      "wardControl": new FormControl(""),
      "addressControl": new FormControl(""),
    });

    this.personalCustomerForm = new FormGroup({
      "CustomerGroup": new FormControl(null),
      "CustomerCode": new FormControl("", [checkDuplicateCode(this.listCustomerCode)]),
      "CustomerTax": new FormControl(""),
      "FirstName": new FormControl("", [Validators.required, checkBlankString()]),
      "LastName": new FormControl("", [Validators.required, checkBlankString()]),
      "Gender": new FormControl(this.listGenders[0]),
      "SaleEmployee": new FormControl(null, [Validators.required]),
      "SupportEmployee": new FormControl(null),
      "Note": new FormControl(''),
      "PersonalPhone": new FormControl('', [Validators.required, Validators.pattern(this.getPhonePattern())]),
      "PersonalPhoneWork": new FormControl('', [Validators.required, Validators.pattern(this.getPhonePattern())]),
      "PersonalPhoneOther": new FormControl('', [Validators.required, Validators.pattern(this.getPhonePattern())]),
      "PersonalEmail": new FormControl('', [Validators.pattern(this.emailPattern)]),
      "PersonalEmailWork": new FormControl('', [Validators.pattern(this.emailPattern)]),
      "PersonalEmailOther": new FormControl('', [Validators.pattern(this.emailPattern)]),
      "addressCompanyControl": new FormControl(''),
      "chooseVendor": new FormControl(''),
      "chooseCustomer": new FormControl(''),
      "chooseEmployee": new FormControl(''),
      "BankName": new FormControl(''),
      "BankCode": new FormControl(''),
      "BankUserName": new FormControl(''),
      "BankAdress": new FormControl(''),
      "MoneyLimit": new FormControl(''),
      "TermsPayment": new FormControl(''),
      "isInlandControl": new FormControl('1'),
      "DefaultAccount": new FormControl(''),
      "provinceControl": new FormControl(""),
      "districtControl": new FormControl(""),
      "wardControl": new FormControl(""),
      "addressControl": new FormControl(""),
      "IdentityId": new FormControl(""),
      "CompanyName": new FormControl(""),
      "Position": new FormControl(""),
      'areaControl': new FormControl(null),
    });

    this.leadModel = new leadModel();
    this.leadModel.leadId = null;
  }

  showToast(key: string, severity: string, summary: string, detail: string) {
    this.messageService.add({ key: key, severity: severity, summary: summary, detail: detail });
  }

  setDefaultValue() {
    //customer group
    let defaultCustomerGroup = this.listCustomerGroup.find(group => group.isDefault == true);
    if (defaultCustomerGroup !== undefined) {
      this.companyCustomerForm.get('CustomerGroup').setValue(defaultCustomerGroup);
      this.personalCustomerForm.get('CustomerGroup').setValue(defaultCustomerGroup);
    } else {
      this.companyCustomerForm.get('CustomerGroup').setValue(this.listCustomerGroup[0]);
      this.personalCustomerForm.get('CustomerGroup').setValue(this.listCustomerGroup[0]);
    }
    //sale employee and support employee
    let defaultEmployee = this.listEmployee.find(emp => emp.employeeId == this.employeeId);
    if (defaultEmployee !== undefined) {
      this.companyCustomerForm.get('SaleEmployee').setValue(defaultEmployee);
      this.companyCustomerForm.get('SupportEmployee').setValue(defaultEmployee);
      this.personalCustomerForm.get('SaleEmployee').setValue(defaultEmployee);
      this.personalCustomerForm.get('SupportEmployee').setValue(defaultEmployee);
    } else {
      this.companyCustomerForm.get('SaleEmployee').setValue(null);
      this.companyCustomerForm.get('SupportEmployee').setValue(null);
      this.personalCustomerForm.get('SaleEmployee').setValue(null);
      this.personalCustomerForm.get('SupportEmployee').setValue(null);
    }
  }

  resetAllForm() {
    this.isCheckedLead = false;
    this.leadId = null;
    this.companyCustomerForm.reset();
    this.resetCustomerForm();
    this.listContact = [];
    this.contactCustomerForm.reset();
    this.resetCompanyCustomerForm();
    this.personalCustomerForm.reset();
    this.resetPersonalCustomerForm();
    this.setDefaultValue();
  }

  resetCustomerForm() {
    this.companyCustomerForm.patchValue({
      "CustomerGroup": null,
      "CustomerCode": '',
      "CustomerTax": '',
      "CompanyName": '',
      "Email": '',
      "Phone": '',
      "BusinessLicenseDate": '',
      "Local": 1,
      "CustomerCodeIsTaxCode": true,
      "SaleEmployee": null,
      "SupportEmployee": null,
      "TotalIncomeLastYear": '',
      "EnterpriseType": null,
      "TotalEmployeeInsurance": null,
      "BusinessScale": null,
      "TotalCapital": null,
      "Note": '',
      "isCustomerCodeLikeTaxCodeControl": null,
      "chooseVendor": null,
      "chooseCustomer": null,
      "chooseEmployee": null,
      "isInlandControl": "1",
      "provinceControl": null,
      "districtControl": null,
      "wardControl": null,
      "addressControl": null,
      "DefaultAccount": null,
      "TermsPayment": null,
      "MoneyLimit": null,
      "BankCode": null,
      "Bankname": null,
      "BankUserName": null,
      "BankAdress": null,
    });
  }

  resetCompanyCustomerForm() {
    this.contactCustomerForm.reset();
    this.contactCustomerForm.patchValue({
      "FirstName": "",
      "LastName": "",
      "Email": "",
      "Position": "",
      "DateOfBirth": null,
      "Phone": "",
      "OtherInfor": "",
      "Gender": this.listGenders[0],
      "isInlandLHControl": 1,
      "provinceControl": null,
      "districtControl": null,
      "wardControl": null,
      "addressControl": null,
    });
  }

  resetPersonalCustomerForm() {
    this.personalCustomerForm.patchValue({
      "CustomerGroup": '',
      "CustomerCode": '',
      "CustomerTax": '',
      "FirstName": '',
      "LastName": '',
      "Gender": this.listGenders[0],
      "SaleEmployee": null,
      "SupportEmployee": null,
      "Note": '',
      "PersonalPhone": '',
      "PersonalEmail": '',
      "provinceControl": null,
      "districtControl": null,
      "wardControl": null,
      "addressControl": null,
      "DefaultAccount": null,
      "TermsPayment": null,
      "isInlandControl": "1",
      "MoneyLimit": null,
      "BankCode": null,
      "Bankname": null,
      "BankUserName": null,
      "BankAdress": null,
      "chooseVendor": null,
      "chooseCustomer": null,
      "chooseEmployee": null,
      "IdentityId": null,
      "CompanyName": null,
      "Position": null,
      "addressCompanyControl": null,
      "PersonalPhoneWork": null,
      "PersonalPhoneOther": null,
      "PersonalEmailWork": null,
      "PersonalEmailOther": null,
    });
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.customerService.createCustomerMasterDataAsync(this.employeeId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listBusinessLocal = result.listBusinessLocal;
      this.listBusinessScale = result.listBusinessScale;
      this.listCustomerGroup = result.listCustomerGroup;
      this.listDistrict = result.listDistrictModel;
      this.listEmployee = result.listEmployeeModel;
      this.listEnterPriseType = result.listEnterPriseType;
      this.listMainBusiness = result.listMainBusiness;
      this.listPosition = result.listPosition;
      this.listProvince = result.listProvinceModel;
      this.listWard = result.listWardModel;
      this.listCustomerCode = result.listCustomerCode;
      this.listCustomeTax = result.listCustomerTax;
      this.listArea = result.listArea;
      this.listCustomer = result.listCustomer;
      this.listProvinceCompanyCustomer = this.listProvincePersonalCustomer = this.listProvinceHouseHoldCustomer = [...this.listProvince.sort((a, b) => a.provinceName.localeCompare(b.provinceName))];
      this.setDefaultValue();
      this.companyCustomerForm.get('CustomerCode').setValidators([Validators.required, checkDuplicateCode(this.listCustomerCode), checkBlankString()]);
      // this.companyCustomerForm.get('CustomerTax').setValidators([Validators.required, checkBlankString(), checkDuplicateCode(this.listCustomeTax)]);
      this.personalCustomerForm.get('CustomerCode').setValidators([checkDuplicateCode(this.listCustomerCode)]);
      this.companyCustomerForm.updateValueAndValidity();
      this.personalCustomerForm.updateValueAndValidity();
    } else {
      this.showToast('popup', 'error', 'Thông báo', 'Lấy dữ liệu thất bại')
    }
  }

  addCustomerContactWhenSave() {
    if (this.contactCustomerForm.valid) {
      let contactModel: ContactModel = this.mappingFormToContactModel();
      this.listContact = [...this.listContact, contactModel];
      this.listPhoneContactCode = this.listContact.map(c => c.Phone);
      this.listEmailContactCode = this.listContact.map(c => c.Email);
      this.contactCustomerForm.get('Phone').setValidators([Validators.required, Validators.pattern(this.getPhonePattern()), checkDuplicateCode(this.listPhoneContactCode)]);
      this.contactCustomerForm.get('Email').setValidators([Validators.pattern(this.emailPattern), checkDuplicateCode(this.listEmailContactCode)]);
      this.contactCustomerForm.updateValueAndValidity();
      this.resetCompanyCustomerForm();
    }
  }

  addCustomerContact() {
    if (!this.contactCustomerForm.valid) {
      Object.keys(this.contactCustomerForm.controls).forEach(key => {
        if (!this.contactCustomerForm.controls[key].valid) {
          this.contactCustomerForm.controls[key].markAsTouched();
        }
      });
      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {
      let contactModel: ContactModel = this.mappingFormToContactModel();
      this.listContact = [...this.listContact, contactModel];
      this.listPhoneContactCode = this.listContact.map(c => c.Phone);
      this.listEmailContactCode = this.listContact.map(c => c.Email);
      this.contactCustomerForm.get('Phone').setValidators([Validators.required, Validators.pattern(this.getPhonePattern()), checkDuplicateCode(this.listPhoneContactCode)]);
      this.contactCustomerForm.get('Email').setValidators([Validators.pattern(this.emailPattern), checkDuplicateCode(this.listEmailContactCode)]);
      this.contactCustomerForm.updateValueAndValidity();
      this.resetCompanyCustomerForm();
    }
  }

  onDeleteContact(rowData: ContactModel) {
    this.listContact = this.listContact.filter(e => e != rowData);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  toggleNotifiErrorCN() {
    this.isOpenNotifiErrorCN = !this.isOpenNotifiErrorCN;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  async onSaveCustomer(type: string) {
    this.loading = true;
    let customerType = Number(this.customerType);
    switch (customerType) {
      case 1:
        //khách hàng doanh nghiệp
        if (!this.companyCustomerForm.valid) {
          Object.keys(this.companyCustomerForm.controls).forEach(key => {
            if (!this.companyCustomerForm.controls[key].valid) {
              this.companyCustomerForm.controls[key].markAsTouched();
            }
          });
          let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
          if (target) {
            $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
            target.focus();
          }

          this.isInvalidForm = true;
          this.isOpenNotifiError = true;  //Hiển thị message lỗi
          this.loading = false;
        }
        else if (!this.validEmailCustomer || !this.validPhoneCustomer) {
          this.showMessageErr();
          this.loading = false;
        }
        else {
          let contactModelOnSave = new ContactModel();
          contactModelOnSave.ContactId = this.emptyGuid;
          contactModelOnSave.ObjectId = this.leadId == null ? this.emptyGuid : this.leadId;
          contactModelOnSave.Email = this.companyCustomerForm.get('Email').value;
          contactModelOnSave.Phone = this.companyCustomerForm.get('Phone').value;
          contactModelOnSave.CreatedDate = new Date();
          //chỉ check khi chưa đổ data
          let companyCustomerModel: CustomerModel = this.mappingCompanyCustomerFormToModel();
          let companyCustomerContactModel: ContactModel = this.mappingCompanyCustomerContacFormToModel();
          if (this.listContact) {
            if (this.listContact.length == 0) {
              this.addCustomerContactWhenSave();
            }
          }
          let companyContactList: Array<ContactModel> = this.listContact;
          if (this.isCheckedLead == true) {
            this.loading = true;
          }
          let result: any = await this.customerService.createCustomerAsync(companyCustomerModel, companyCustomerContactModel, companyContactList, this.auth.userId, false, false);
          if (result.statusCode === 200) {
            this.listCustomer = result.listCustomer;
            //gui email sau khi tao khach hang
            this.sendEmail(result);
            let customerId = result.customerId;
            let contactId = result.contactId;

            if (this.companyCustomerForm.get('BankName').value || this.companyCustomerForm.get('BankCode').value || this.companyCustomerForm.get('BankUserName').value || this.companyCustomerForm.get('BankAdress').value) {
              this.bankAccount.bankName = this.companyCustomerForm.get('BankName').value ? this.companyCustomerForm.get('BankName').value.trim() : null;
              this.bankAccount.accountNumber = this.companyCustomerForm.get('BankCode').value ? this.companyCustomerForm.get('BankCode').value.trim() : null;
              this.bankAccount.accountName = this.companyCustomerForm.get('BankUserName').value ? this.companyCustomerForm.get('BankUserName').value.trim() : null;
              this.bankAccount.branchName = this.companyCustomerForm.get('BankAdress').value ? this.companyCustomerForm.get('BankAdress').value.trim() : null;
              this.bankAccount.objectId = result.customerId;
              this.bankAccount.objectType = "CUS";

              this.bankService.createBank(this.bankAccount).subscribe(responseBank => {
                //this.loading = false;
                let resultBank = <any>responseBank;

                if (resultBank.statusCode === 202 || resultBank.statusCode === 200) {
                  switch (type) {
                    case "save":
                      let res: ResultDialog = {
                        status: true,
                        customer: new leadReferenceCustomer()
                      }
                      res.customer.address = companyCustomerContactModel.Address;
                      res.customer.customerCode = companyCustomerModel.CustomerCode;
                      res.customer.customerId = customerId;
                      res.customer.customerName = companyCustomerModel.CustomerName;
                      res.customer.customerType = companyCustomerModel.CustomerType;
                      res.customer.email = companyCustomerContactModel.Email;
                      res.customer.phone = companyCustomerContactModel.Phone;
                      res.customer.personInChargeId = companyCustomerModel.PersonInChargeId;
                      this.ref.close(res);
                      break;
                    case "save_new":
                      if (result.duplicateContact == true) {
                        this.showToast('popup', 'warn', 'Thông báo', 'Người liên hệ đã tồn tại trong hệ thống');
                      }
                      this.showToast('popup', 'success', 'Thông báo', 'Tạo khách hàng thành công');
                      this.resetAllForm();
                      this.setDefaultValue();
                      this.listContact = [];
                      this.isInvalidForm = false;
                      break;
                    default:
                      break;
                  }
                } else {
                  this.showToast('popup', 'error', 'Thông báo', resultBank.messageCode);
                }

                this.loading = false;
              });
            }
            else {
              switch (type) {
                case "save":
                  let res: ResultDialog = {
                    status: true,
                    customer: new leadReferenceCustomer()
                  }
                  res.customer.address = companyCustomerContactModel.Address;
                  res.customer.customerCode = companyCustomerModel.CustomerCode;
                  res.customer.customerId = customerId;
                  res.customer.customerName = companyCustomerModel.CustomerName;
                  res.customer.customerType = companyCustomerModel.CustomerType;
                  res.customer.email = companyCustomerContactModel.Email;
                  res.customer.phone = companyCustomerContactModel.Phone;
                  res.customer.personInChargeId = companyCustomerModel.PersonInChargeId;
                  this.ref.close(res);
                  break;
                case "save_new":
                  if (result.duplicateContact == true) {
                    this.showToast('popup', 'warn', 'Thông báo', 'Người liên hệ đã tồn tại trong hệ thống');
                  }
                  this.showToast('popup', 'success', 'Thông báo', 'Tạo khách hàng thành công');
                  this.resetAllForm();
                  this.setDefaultValue();
                  this.listContact = [];
                  this.isInvalidForm = false;
                  break;
                default:
                  break;
              }
              this.loading = false;
            }
          } else {
            this.showToast('popup', 'error', 'Thông báo', result.messageCode);
            this.loading = false;
          }
        }
        break;
      case 2:
        //khách hàng cá nhân
        if (!this.personalCustomerForm.valid) {
          Object.keys(this.personalCustomerForm.controls).forEach(key => {
            if (!this.personalCustomerForm.controls[key].valid) {
              this.personalCustomerForm.controls[key].markAsTouched();
              this.isInvalidFormCN = true;
              this.isOpenNotifiErrorCN = true;  //Hiển thị message lỗi
            }
          });
          let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
          if (target) {
            $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
            target.focus();
          }
          this.loading = false;
        }
        else if (!this.validEmailCustomer || !this.validPhoneCustomer) {
          this.showMessageErr();
          this.loading = false;
        }
        else {
          let contactModelOnSave = new ContactModel();
          contactModelOnSave.ContactId = this.emptyGuid;
          contactModelOnSave.ObjectId = this.leadId == null ? this.emptyGuid : this.leadId;
          contactModelOnSave.Email = this.personalCustomerForm.get('PersonalEmail').value;
          contactModelOnSave.Phone = this.personalCustomerForm.get('PersonalPhone').value;
          //chỉ check khi chưa đổ data
          let companyCustomerModel: CustomerModel = this.mappingPersonalCustomerFormToModel();
          let companyCustomerContactModel: ContactModel = this.mappingPersonalCustomerContacFormToModel();
          let companyContactList: Array<ContactModel> = [];
          if (this.isCheckedLead == true) {
            this.loading = true;
          }

          let result: any = await this.customerService.createCustomerAsync(companyCustomerModel, companyCustomerContactModel, companyContactList, this.auth.userId, false, false);
          if (result.statusCode === 200) {
            this.listCustomer = result.listCustomer;
            this.sendEmail(result);
            let customerId = result.customerId;
            let contactId = result.contactId;

            if (this.personalCustomerForm.get('BankName').value || this.personalCustomerForm.get('BankCode').value || this.personalCustomerForm.get('BankUserName').value || this.personalCustomerForm.get('BankAdress').value) {
              this.bankAccount.bankName = this.personalCustomerForm.get('BankName').value ? this.personalCustomerForm.get('BankName').value.trim() : null;
              this.bankAccount.accountNumber = this.personalCustomerForm.get('BankCode').value ? this.personalCustomerForm.get('BankCode').value.trim() : null;
              this.bankAccount.accountName = this.personalCustomerForm.get('BankUserName').value ? this.personalCustomerForm.get('BankUserName').value.trim() : null;
              this.bankAccount.branchName = this.personalCustomerForm.get('BankAdress').value ? this.personalCustomerForm.get('BankAdress').value.trim() : null;
              this.bankAccount.objectId = result.customerId;
              this.bankAccount.objectType = "CUS";

              this.bankService.createBank(this.bankAccount).subscribe(responseBank => {
                let resultBank = <any>responseBank;

                if (resultBank.statusCode === 202 || resultBank.statusCode === 200) {
                  switch (type) {
                    case "save":
                      let res: ResultDialog = {
                        status: true,
                        customer: new leadReferenceCustomer()
                      }
                      res.customer.address = companyCustomerContactModel.Address;
                      res.customer.customerCode = companyCustomerModel.CustomerCode;
                      res.customer.customerId = customerId;
                      res.customer.customerName = companyCustomerModel.CustomerName;
                      res.customer.customerType = companyCustomerModel.CustomerType;
                      res.customer.email = companyCustomerContactModel.Email;
                      res.customer.phone = companyCustomerContactModel.Phone;
                      res.customer.personInChargeId = companyCustomerModel.PersonInChargeId;
                      this.ref.close(res);
                      break;
                    case "save_new":
                      this.showToast('popup', 'success', 'Thông báo', 'Tạo khách hàng thành công');
                      this.resetAllForm();
                      this.isInvalidFormCN = false;
                      break;
                    default:
                      break;
                  }
                } else {
                  this.showToast('popup', 'error', 'Thông báo', resultBank.messageCode);
                }
                this.loading = false;
              });
            }
            else {
              switch (type) {
                case "save":
                  let res: ResultDialog = {
                    status: true,
                    customer: new leadReferenceCustomer()
                  }
                  res.customer.address = companyCustomerContactModel.Address;
                  res.customer.customerCode = companyCustomerModel.CustomerCode;
                  res.customer.customerId = customerId;
                  res.customer.customerName = companyCustomerModel.CustomerName;
                  res.customer.customerType = companyCustomerModel.CustomerType;
                  res.customer.email = companyCustomerContactModel.Email;
                  res.customer.phone = companyCustomerContactModel.Phone;
                  res.customer.personInChargeId = companyCustomerModel.PersonInChargeId;
                  this.ref.close(res);
                  break;
                case "save_new":
                  this.showToast('popup', 'success', 'Thông báo', 'Tạo khách hàng thành công');
                  this.resetAllForm();
                  this.isInvalidFormCN = false;
                  break;
                default:
                  break;
              }
              this.loading = false;
            }
          } else {
            this.showToast('popup', 'error', 'Thông báo', result.messageCode);
            this.loading = false;
          }
        }
        break;
      default:
        break;
    }
  }

  sendEmail(result: any) {
    //gui email
    let sendMailModel: SendEmailModel = result.sendEmailEntityModel;
    this.emailConfigService.sendEmail(5, sendMailModel).subscribe(reponse => {
    });
  }

  mappingCompanyCustomerFormToModel(): CustomerModel {
    let customerModel = new CustomerModel();
    customerModel.CustomerId = this.emptyGuid;
    customerModel.LeadId = this.leadId;
    customerModel.CustomerGroupId = this.companyCustomerForm.get('CustomerGroup').value == null ? null : this.companyCustomerForm.get('CustomerGroup').value.categoryId;
    customerModel.CustomerCode = this.companyCustomerForm.get('CustomerCode').value;
    let customerName = this.companyCustomerForm.get('CompanyName').value;
    customerModel.CustomerName = typeof (customerName) == "string" ? customerName : (customerName.customerName);
    customerModel.StatusId = this.emptyGuid;
    customerModel.CustomerServiceLevelId = null;
    let saleEmployee = this.companyCustomerForm.get('SaleEmployee').value;
    if (saleEmployee == null) {
      customerModel.PersonInChargeId = null;
    } else {
      customerModel.PersonInChargeId = this.companyCustomerForm.get('SaleEmployee').value.employeeId;  //nhan vien ban hang
    }
    let supportEmployee = this.companyCustomerForm.get('SupportEmployee').value;
    if (supportEmployee == null) {
      customerModel.CustomerCareStaff = null;
    } else {
      customerModel.CustomerCareStaff = this.companyCustomerForm.get('SupportEmployee').value.employeeId; //nhan vien cham soc khach hang
    }
    customerModel.CustomerType = Number(this.customerType);
    customerModel.PaymentId = null;
    customerModel.FieldId = null;
    customerModel.ScaleId = null;
    customerModel.MaximumDebtDays = null;
    customerModel.MaximumDebtValue = null;
    customerModel.TotalSaleValue = 0;
    customerModel.TotalReceivable = 0;
    customerModel.NearestDateTransaction = null;
    customerModel.TotalCapital = null;
    customerModel.BusinessRegistrationDate = null;
    customerModel.EnterpriseType = null;
    customerModel.TotalEmployeeParticipateSocialInsurance = null;
    customerModel.TotalRevenueLastYear = null;
    customerModel.BusinessType = null;
    customerModel.BusinessScale = null;
    customerModel.CreatedById = this.auth.UserId;
    customerModel.Active = true;
    customerModel.CreatedDate = new Date();

    return customerModel;
  }

  mappingCompanyCustomerContacFormToModel(): ContactModel {
    let contactModel = new ContactModel;
    contactModel.ContactId = this.emptyGuid;
    contactModel.ObjectId = this.emptyGuid;
    contactModel.Phone = this.companyCustomerForm.get('Phone').value;
    contactModel.Email = this.companyCustomerForm.get('Email').value;
    contactModel.Note = this.companyCustomerForm.get('Note').value;
    contactModel.GeographicalAreaId = this.companyCustomerForm.get('areaControl').value ? this.companyCustomerForm.get('areaControl').value.geographicalAreaId : null;
    contactModel.TaxCode = this.companyCustomerForm.get('CustomerTax').value;
    contactModel.ProvinceId = this.companyCustomerForm.get('provinceControl').value?.provinceId;
    contactModel.DistrictId = this.companyCustomerForm.get('districtControl').value?.districtId;
    contactModel.WardId = this.companyCustomerForm.get('wardControl').value?.wardId;
    contactModel.Address = this.companyCustomerForm.get('addressControl').value;
    contactModel.OptionPosition = 'CUS'; //this.chooseCusVenEmp == null || this.chooseCusVenEmp == [] ? null : this.chooseCusVenEmp.toString();
    contactModel.BankCode = this.companyCustomerForm.get('BankCode').value;
    contactModel.BankName = this.companyCustomerForm.get('BankName').value;
    contactModel.TermsPayment = this.companyCustomerForm.get('TermsPayment').value;
    contactModel.DefaultAccount = this.companyCustomerForm.get('DefaultAccount').value;
    contactModel.MoneyLimit = this.companyCustomerForm.get('MoneyLimit').value;

    contactModel.CreatedDate = new Date();
    contactModel.Active = true;

    return contactModel;
  }

  mappingPersonalCustomerFormToModel(): CustomerModel {
    let customerModel = new CustomerModel();
    customerModel.CustomerId = this.emptyGuid;
    customerModel.LeadId = this.leadId;
    customerModel.CustomerGroupId = this.personalCustomerForm.get('CustomerGroup').value == null ? null : this.personalCustomerForm.get('CustomerGroup').value.categoryId;
    customerModel.CustomerCode = this.personalCustomerForm.get('CustomerCode').value;
    customerModel.CustomerName = this.personalCustomerForm.get('FirstName').value.trim() + " " + this.personalCustomerForm.get('LastName').value.trim();
    customerModel.StatusId = this.emptyGuid;
    customerModel.CustomerServiceLevelId = null;
    let saleEmployee = this.personalCustomerForm.get('SaleEmployee').value;
    if (saleEmployee == null) {
      customerModel.PersonInChargeId = null;
    } else {
      customerModel.PersonInChargeId = this.personalCustomerForm.get('SaleEmployee').value.employeeId;  //nhan vien ban hang
    }
    let supportEmployee = this.personalCustomerForm.get('SupportEmployee').value;
    if (supportEmployee == null) {
      customerModel.CustomerCareStaff = null;
    } else {
      customerModel.CustomerCareStaff = this.personalCustomerForm.get('SupportEmployee').value.employeeId; //nhan vien cham soc khach hang
    }
    customerModel.CustomerType = Number(this.customerType);
    customerModel.PaymentId = null;
    customerModel.FieldId = null;
    customerModel.ScaleId = null;
    customerModel.MaximumDebtDays = null;
    customerModel.MaximumDebtValue = null;
    customerModel.TotalSaleValue = 0;
    customerModel.TotalReceivable = 0;
    customerModel.NearestDateTransaction = null;
    customerModel.TotalCapital = null;
    customerModel.BusinessRegistrationDate = null;
    customerModel.EnterpriseType = null;
    customerModel.TotalEmployeeParticipateSocialInsurance = null;
    customerModel.TotalRevenueLastYear = null;
    customerModel.BusinessType = null;
    customerModel.BusinessScale = null;
    customerModel.CreatedById = this.auth.UserId;
    customerModel.Active = true;
    customerModel.CreatedDate = new Date();

    return customerModel;
  }

  mappingPersonalCustomerContacFormToModel(): ContactModel {
    let contactModel = new ContactModel;
    contactModel.ContactId = this.emptyGuid;
    contactModel.ObjectId = this.emptyGuid;
    contactModel.FirstName = this.personalCustomerForm.get('FirstName').value;
    contactModel.LastName = this.personalCustomerForm.get('LastName').value;
    contactModel.Phone = this.personalCustomerForm.get('PersonalPhone').value;
    contactModel.Email = this.personalCustomerForm.get('PersonalEmail').value;
    contactModel.Note = this.personalCustomerForm.get('Note').value;
    contactModel.Gender = this.personalCustomerForm.get('Gender').value.code;
    contactModel.CreatedDate = new Date();
    contactModel.Active = true;

    contactModel.IdentityID = this.personalCustomerForm.get('IdentityId').value;
    contactModel.ProvinceId = (this.personalCustomerForm.get('provinceControl').value === "" || this.personalCustomerForm.get('provinceControl').value === null || this.personalCustomerForm.get('provinceControl').value === undefined) ? null : this.personalCustomerForm.get('provinceControl').value.provinceId;
    contactModel.DistrictId = (this.personalCustomerForm.get('districtControl').value === "" || this.personalCustomerForm.get('districtControl').value === null || this.personalCustomerForm.get('districtControl').value === undefined) ? null : this.personalCustomerForm.get('districtControl').value.districtId;
    contactModel.WardId = (this.personalCustomerForm.get('wardControl').value === "" || this.personalCustomerForm.get('wardControl').value === null || this.personalCustomerForm.get('wardControl').value === undefined) ? null : this.personalCustomerForm.get('wardControl').value.wardId;
    contactModel.Address = this.personalCustomerForm.get('addressControl').value;
    contactModel.OptionPosition = 'CUS'; //this.chooseCusVenEmp == null || this.chooseCusVenEmp == [] ? null : this.chooseCusVenEmp.toString();
    contactModel.GeographicalAreaId = this.personalCustomerForm.get('areaControl').value ? this.personalCustomerForm.get('areaControl').value.geographicalAreaId : null;
    // contactModel.BankCode = this.personalCustomerForm.get('BankCode').value;
    // contactModel.BankName = this.personalCustomerForm.get('BankName').value;
    contactModel.TermsPayment = this.personalCustomerForm.get('TermsPayment').value;
    contactModel.DefaultAccount = this.personalCustomerForm.get('DefaultAccount').value;
    contactModel.MoneyLimit = this.personalCustomerForm.get('MoneyLimit').value;
    contactModel.CompanyAddress = this.personalCustomerForm.get('addressCompanyControl').value;
    contactModel.CompanyName = this.personalCustomerForm.get('CompanyName').value;
    contactModel.CustomerPosition = (this.personalCustomerForm.get('Position').value === "" || this.personalCustomerForm.get('Position').value === null || this.personalCustomerForm.get('Position').value === undefined) ? null : this.personalCustomerForm.get('Position').value.categoryId;
    contactModel.WorkPhone = this.personalCustomerForm.get('PersonalPhoneWork').value;
    contactModel.WorkEmail = this.personalCustomerForm.get('PersonalEmailWork').value;
    contactModel.OtherPhone = this.personalCustomerForm.get('PersonalPhoneOther').value;
    contactModel.OtherEmail = this.personalCustomerForm.get('PersonalEmailOther').value;
    return contactModel;
  }


  mappingFormToContactModel(): ContactModel {
    let contactModel = new ContactModel;
    contactModel.FirstName = this.contactCustomerForm.get('FirstName').value;
    contactModel.LastName = this.contactCustomerForm.get('LastName').value;
    (contactModel as any).FullName = contactModel.FirstName + " " + contactModel.LastName;
    contactModel.Email = this.contactCustomerForm.get('Email').value;
    contactModel.Role = this.contactCustomerForm.get('Position').value;
    contactModel.DateOfBirth = this.contactCustomerForm.get('DateOfBirth').value;
    contactModel.Phone = this.contactCustomerForm.get('Phone').value;
    contactModel.Other = this.contactCustomerForm.get('OtherInfor').value;
    contactModel.Gender = this.contactCustomerForm.get('Gender').value.code;
    contactModel.GenderDisplay = this.contactCustomerForm.get('Gender').value.name;
    contactModel.ProvinceId = this.contactCustomerForm.get('provinceControl').value ? this.contactCustomerForm.get('provinceControl').value.provinceId : null;
    contactModel.DistrictId = this.contactCustomerForm.get('districtControl').value ? this.contactCustomerForm.get('districtControl').value.districtId : null;
    contactModel.WardId = this.contactCustomerForm.get('wardControl').value ? this.contactCustomerForm.get('wardControl').value.wardId : null;
    contactModel.Address = this.contactCustomerForm.get('addressControl').value;

    return contactModel;
  }

  resetLeadId() {
    this.isCheckedLead = false; // reset isCreatedByLead
    this.leadId = null;
  }

  async onCheckDuplicateCustomer(checkBy: string, type: string, formControl: FormControl, customerType: any) {
    if (formControl.valid) {
      switch (checkBy) {
        case "checkByEmail":
          switch (type) {
            case "Email":
              let contactModelCheckEmail = new ContactModel();
              contactModelCheckEmail.Email = formControl.value;
              this.loading = true;
              let resultCheckEmail: any = await this.customerService.checkDuplicateCustomerAllType(contactModelCheckEmail, false, false, this.employeeId, customerType);
              this.loading = false;
              if (resultCheckEmail.statusCode === 200) {
                if (resultCheckEmail.isDuplicateLead == true) {
                  this.resetLeadId();
                  let leadModel: leadModel = resultCheckEmail.duplicateLeadModel;
                  let leadContactModel: contactModel = resultCheckEmail.duplicateLeadContactModel;
                  this.showConfirm("onChange", '', "lead", leadModel, leadContactModel, '', '', '', new contactModel());
                } else if (resultCheckEmail.isDuplicateCustomer) {
                  this.resetLeadId();
                  let emailValue = formControl.value;
                  let duplicateModel: contactModel = resultCheckEmail.duplicateCustomerContactModel;
                  this.showConfirm("onChange", '', "customer", new leadModel(), new contactModel(), "email", emailValue, '', duplicateModel);
                }
              } else {
                this.showToast('popup', 'error', 'Thông báo', resultCheckEmail.messageCode);
              }
              break;
            case "EmailOther":
              let contactModelCheckEmailOther = new ContactModel();
              contactModelCheckEmailOther.OtherEmail = formControl.value;
              this.loading = true;
              let resultCheckEmailOther: any = await this.customerService.checkDuplicateCustomerAllType(contactModelCheckEmailOther, false, false, this.employeeId, customerType);
              this.loading = false;
              if (resultCheckEmailOther.statusCode === 200) {
                if (resultCheckEmailOther.isDuplicateLead == true) {
                  this.resetLeadId();
                  let leadModel: leadModel = resultCheckEmailOther.duplicateLeadModel;
                  let leadContactModel: contactModel = resultCheckEmailOther.duplicateLeadContactModel;
                  this.showConfirm("onChange", '', "lead", leadModel, leadContactModel, '', '', '', new contactModel());
                } else if (resultCheckEmailOther.isDuplicateCustomer) {
                  this.resetLeadId();
                  let emailValue = formControl.value;
                  let duplicateModel: contactModel = resultCheckEmailOther.duplicateCustomerContactModel;
                  this.showConfirm("onChange", '', "customer", new leadModel(), new contactModel(), "email", emailValue, '', duplicateModel);
                }
              } else {
                this.showToast('popup', 'error', 'Thông báo', resultCheckEmailOther.messageCode);
              }
              break;
            case "EmailWork":
              let contactModelCheckEmailWork = new ContactModel();
              contactModelCheckEmailWork.WorkEmail = formControl.value;
              this.loading = true;
              let resultCheckEmailWork: any = await this.customerService.checkDuplicateCustomerAllType(contactModelCheckEmailWork, false, false, this.employeeId, customerType);
              this.loading = false;
              if (resultCheckEmailWork.statusCode === 200) {
                if (resultCheckEmailWork.isDuplicateLead == true) {
                  this.resetLeadId();
                  let leadModel: leadModel = resultCheckEmailWork.duplicateLeadModel;
                  let leadContactModel: contactModel = resultCheckEmailWork.duplicateLeadContactModel;
                  this.showConfirm("onChange", '', "lead", leadModel, leadContactModel, '', '', '', new contactModel());
                } else if (resultCheckEmailWork.isDuplicateCustomer) {
                  this.resetLeadId();
                  let emailValue = formControl.value;
                  let duplicateModel: contactModel = resultCheckEmailWork.duplicateCustomerContactModel;
                  this.showConfirm("onChange", '', "customer", new leadModel(), new contactModel(), "email", emailValue, '', duplicateModel);
                }
              } else {
                this.showToast('popup', 'error', 'Thông báo', resultCheckEmailWork.messageCode);
              }
              break;
            default:
              break;
          }
          break;
        case "checkByPhone":
          switch (type) {
            case "Phone":
              let contactModelCheckPhone = new ContactModel();
              contactModelCheckPhone.Phone = formControl.value;
              this.loading = true;
              let resultCheckPhone: any = await this.customerService.checkDuplicateCustomerAllType(contactModelCheckPhone, false, false, this.employeeId, customerType);
              this.loading = false;
              if (resultCheckPhone.statusCode === 200) {
                if (resultCheckPhone.isDuplicateLead == true) {
                  this.resetLeadId();
                  let leadModel: leadModel = resultCheckPhone.duplicateLeadModel;
                  let leadContactModel: contactModel = resultCheckPhone.duplicateLeadContactModel;
                  this.showConfirm("onChange", '', "lead", leadModel, leadContactModel, '', '', '', new contactModel());
                } else if (resultCheckPhone.isDuplicateCustomer) {
                  this.resetLeadId();
                  let phonelValue = formControl.value;
                  let duplicateModel: contactModel = resultCheckPhone.duplicateCustomerContactModel;
                  this.showConfirm("onChange", '', "customer", new leadModel(), new contactModel(), "phone", '', phonelValue, duplicateModel);
                }
              } else {
                this.showToast('popup', 'error', 'Thông báo', resultCheckPhone.messageCode);
              }
              break;
            case "PhoneOther":
              let contactModelCheckPhoneOther = new ContactModel();
              contactModelCheckPhoneOther.OtherPhone = formControl.value;
              this.loading = true;
              let resultCheckPhoneOther: any = await this.customerService.checkDuplicateCustomerAllType(contactModelCheckPhoneOther, false, false, this.employeeId, customerType);
              this.loading = false;
              if (resultCheckPhoneOther.statusCode === 200) {
                if (resultCheckPhoneOther.isDuplicateLead == true) {
                  this.resetLeadId();
                  let leadModel: leadModel = resultCheckPhoneOther.duplicateLeadModel;
                  let leadContactModel: contactModel = resultCheckPhoneOther.duplicateLeadContactModel;
                  this.showConfirm("onChange", '', "lead", leadModel, leadContactModel, '', '', '', new contactModel());
                } else if (resultCheckPhoneOther.isDuplicateCustomer) {
                  this.resetLeadId();
                  let phonelValue = formControl.value;
                  let duplicateModel: contactModel = resultCheckPhoneOther.duplicateCustomerContactModel;
                  this.showConfirm("onChange", '', "customer", new leadModel(), new contactModel(), "phone", '', phonelValue, duplicateModel);
                }
              } else {
                this.showToast('popup', 'error', 'Thông báo', resultCheckPhoneOther.messageCode);
              }
              break;
            case "PhoneWork":
              let contactModelCheckPhoneWork = new ContactModel();
              contactModelCheckPhoneWork.WorkPhone = formControl.value;
              this.loading = true;
              let resultCheckPhoneWork: any = await this.customerService.checkDuplicateCustomerAllType(contactModelCheckPhoneWork, false, false, this.employeeId, customerType);
              this.loading = false;
              if (resultCheckPhoneWork.statusCode === 200) {
                if (resultCheckPhoneWork.isDuplicateLead == true) {
                  this.resetLeadId();
                  let leadModel: leadModel = resultCheckPhoneWork.duplicateLeadModel;
                  let leadContactModel: contactModel = resultCheckPhoneWork.duplicateLeadContactModel;
                  this.showConfirm("onChange", '', "lead", leadModel, leadContactModel, '', '', '', new contactModel());
                } else if (resultCheckPhoneWork.isDuplicateCustomer) {
                  this.resetLeadId();
                  let phonelValue = formControl.value;
                  let duplicateModel: contactModel = resultCheckPhoneWork.duplicateCustomerContactModel;
                  this.showConfirm("onChange", '', "customer", new leadModel(), new contactModel(), "phone", '', phonelValue, duplicateModel);
                }
              } else {
                this.showToast('popup', 'error', 'Thông báo', resultCheckPhoneWork.messageCode);
              }
              break;
            default:
              break;
          }
          break;
        default:
          break;
      }
    }
  }

  async showConfirm(event: string, typeOnSave: string, duplicateBy: string, leadModel: leadModel, leadContactModel: contactModel, duplicateCustomerTyppe: string, emailValue: string, phoneValue: string, contactCustomerModel: contactModel) {
    switch (event) {
      case "onChange":
        switch (duplicateBy) {
          case "lead":
            this.confirmationService.confirm({
              message: `Đã tồn tại một khách hàng tiềm năng trong hệ thống. Bạn có muốn chuyển khách hàng tiềm năng ${leadContactModel.firstName} ${leadContactModel.lastName} thành khách hàng không?`,
              accept: () => {
                this.isCheckedLead = true; //create customer by lead
                this.leadId = leadModel.leadId;
                this.patchLeadContactModelToForm(leadContactModel, leadModel);
              }
            });
            break;
          case "customer":
            switch (duplicateCustomerTyppe) {
              case "email":
                this.confirmationService.confirm({
                  message: `Đã tồn tại một khách hàng trong hệ thống. Bạn có muốn cập nhật thông tin khách hàng có email ${emailValue} này không?`,
                  accept: () => {
                    this.router.navigate(['/customer/detail', { customerId: contactCustomerModel.objectId, contactId: contactCustomerModel.contactId }]);
                  }
                });
                break;
              case "phone":
                this.confirmationService.confirm({
                  message: `Đã tồn tại một khách hàng trong hệ thống. Bạn có muốn cập nhật thông tin khách hàng có số điện thoại ${phoneValue} này không?`,
                  accept: () => {
                    this.router.navigate(['/customer/detail', { customerId: contactCustomerModel.objectId, contactId: contactCustomerModel.contactId }]);
                  }
                });
                break;
              default:
                break;
            }
            break;
          default:
            break;
        }
        break;
      case "onSave":
        switch (duplicateBy) {
          case "lead":
            this.confirmationService.confirm({
              message: `Đã tồn tại một khách hàng tiềm năng trong hệ thống. Bạn có muốn chuyển khách hàng tiềm năng ${leadContactModel.firstName} ${leadContactModel.lastName} thành khách hàng không?`,
              accept: () => {
                this.isCheckedLead = true; //create customer by lead
                this.leadId = leadModel.leadId;
                this.patchLeadContactModelToForm(leadContactModel, leadModel);
              }
            });
            break;
          case "customer":
            this.confirmationService.confirm({
              message: `Đã tồn tại một khách hàng trong hệ thống. Bạn có muốn cập nhật thông tin khách hàng này không?`,
              accept: () => {
                this.router.navigate(['/customer/detail', { customerId: contactCustomerModel.objectId, contactId: contactCustomerModel.contactId }]);
              }
            });
            break;
          default:
            break;
        }
        break;
      default:
        break;
    }
  }

  patchLeadContactModelToForm(leadContactModel: contactModel, leadModel: leadModel) {
    switch (Number(this.customerType)) {
      case 1:
        //form company customer
        this.companyCustomerForm.get('CompanyName').setValue(leadContactModel.companyName);
        this.companyCustomerForm.get('Email').setValue(leadContactModel.email);
        this.companyCustomerForm.get('Phone').setValue(leadContactModel.phone);

        if (leadModel.personInChargeId == null) {
          this.companyCustomerForm.get('SupportEmployee').setValue(null);
        } else {
          let supportEmployee = this.listEmployee.find(e => e.employeeId == leadModel.personInChargeId);
          if (supportEmployee != null) {
            this.companyCustomerForm.get('SupportEmployee').setValue(supportEmployee);
          } else {
            this.companyCustomerForm.get('SupportEmployee').setValue(null);
          }
        }
        let companyName = leadContactModel.firstName + " " + leadContactModel.lastName;
        this.companyCustomerForm.get('CompanyName').setValue(companyName);
        break;
      case 2:
        //form personal customer
        this.personalCustomerForm.get('FirstName').setValue(leadContactModel.firstName);
        this.personalCustomerForm.get('LastName').setValue(leadContactModel.lastName);
        this.personalCustomerForm.get('Gender').setValue(this.listGenders.find(e => e.code == leadContactModel.gender));
        if (leadModel.personInChargeId == null) {
          this.personalCustomerForm.get('SupportEmployee').setValue(null);
        } else {
          let supportEmployee = this.listEmployee.find(e => e.employeeId == leadModel.personInChargeId);
          if (supportEmployee != null) {
            this.personalCustomerForm.get('SupportEmployee').setValue(supportEmployee);
          } else {
            this.personalCustomerForm.get('SupportEmployee').setValue(null);
          }
        }
        this.personalCustomerForm.get('PersonalEmail').setValue(leadContactModel.email);
        this.personalCustomerForm.get('PersonalPhone').setValue(leadContactModel.phone);
      default:
        break;
    }
  }

  PhonePattern(event: any) {
    // const pattern = /^[0-9]$/;
    // const inputChar = String.fromCharCode(event.charCode);
    // if (!pattern.test(inputChar)) {
    //   event.preventDefault();
    // }
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  customerCodePattern(event: any) {
    const pattern = /^[a-zA-Z0-9-]$/;
    const inputChar = String.fromCharCode(event.charCode);
    if (!pattern.test(inputChar)) {
      event.preventDefault();
    }
  }

  onCancel() {
    let res: ResultDialog = {
      status: false,
      customer: null
    }
    this.ref.close(res);
  }

  changeProvince(e) {
    if (e.value == null) {
      this.listDistrict = [];
      this.listWard = [];
    }
    else this.listDistrict = e.value.districtList;
  }
  changeDistrict(e) {
    if (e.value == null) this.listWard = [];
    else this.listWard = e.value.wardList;
  }

  changeProvinceContact(e) {
    if (e.value == null) {
      this.listDistrictPersonalCustomer = [];
      this.listWardPersonalCustomer = [];
    }
    else this.listDistrictPersonalCustomer = e.value.districtList;
  }
  changeDistrictContact(e) {
    if (e.value == null) this.listWardPersonalCustomer = [];
    else this.listWardPersonalCustomer = e.value.wardList;
  }
  changeMKH() {
    if (this.isCustomerCodeLikeTaxCode) {
      this.companyCustomerForm.get('CustomerTax').setValue(this.companyCustomerForm.get('CustomerCode').value);
    }
  }
  // changeMST() {
  //   if (this.isCustomerCodeLikeTaxCode) {
  //     this.companyCustomerForm.get('CustomerCode').setValue(this.companyCustomerForm.get('CustomerTax').value);
  //   }
  //   // this.getMasterData();
  // }
  isInlandOnChange() {
    if (Number(this.isInland) == 1) {
      // this.companyCustomerForm.get('provinceControl').setValidators([Validators.required]);
      // this.companyCustomerForm.get('provinceControl').updateValueAndValidity();
      // this.companyCustomerForm.get('districtControl').setValidators([Validators.required]);
      // this.companyCustomerForm.get('districtControl').updateValueAndValidity();
      // this.companyCustomerForm.get('wardControl').setValidators([Validators.required]);
      // this.companyCustomerForm.get('wardControl').updateValueAndValidity();

      // this.personalCustomerForm.get('provinceControl').setValidators([Validators.required]);
      // this.personalCustomerForm.get('provinceControl').updateValueAndValidity();
      // this.personalCustomerForm.get('districtControl').setValidators([Validators.required]);
      // this.personalCustomerForm.get('districtControl').updateValueAndValidity();
      // this.personalCustomerForm.get('wardControl').setValidators([Validators.required]);
      // this.personalCustomerForm.get('wardControl').updateValueAndValidity();
    }
    else {
      this.companyCustomerForm.get('provinceControl').clearValidators();
      this.companyCustomerForm.get('provinceControl').updateValueAndValidity();
      this.companyCustomerForm.get('districtControl').clearValidators();
      this.companyCustomerForm.get('districtControl').updateValueAndValidity();
      this.companyCustomerForm.get('wardControl').clearValidators();
      this.companyCustomerForm.get('wardControl').updateValueAndValidity();

      this.personalCustomerForm.get('provinceControl').clearValidators();
      this.personalCustomerForm.get('provinceControl').updateValueAndValidity();
      this.personalCustomerForm.get('districtControl').clearValidators();
      this.personalCustomerForm.get('districtControl').updateValueAndValidity();
      this.personalCustomerForm.get('wardControl').clearValidators();
      this.personalCustomerForm.get('wardControl').updateValueAndValidity();
    }

    if (this.isInlandLH == 1) {
      // this.contactCustomerForm.get('provinceControl').setValidators([Validators.required]);
      // this.contactCustomerForm.get('provinceControl').updateValueAndValidity();
      // this.contactCustomerForm.get('districtControl').setValidators([Validators.required]);
      // this.contactCustomerForm.get('districtControl').updateValueAndValidity();
      // this.contactCustomerForm.get('wardControl').setValidators([Validators.required]);
      // this.contactCustomerForm.get('wardControl').updateValueAndValidity();
    }
    else {
      this.contactCustomerForm.get('provinceControl').clearValidators();
      this.contactCustomerForm.get('provinceControl').updateValueAndValidity();
      this.contactCustomerForm.get('districtControl').clearValidators();
      this.contactCustomerForm.get('districtControl').updateValueAndValidity();
      this.contactCustomerForm.get('wardControl').clearValidators();
      this.contactCustomerForm.get('wardControl').updateValueAndValidity();
    }
  }

  onCheckInvail(phone) {
    // if(phone != null && phone != undefined && phone != ""){
    if (phone.valid) {
      this.personalCustomerForm.get('PersonalPhoneOther').setValidators([Validators.pattern(this.getPhonePattern())]);
      this.personalCustomerForm.get('PersonalPhoneOther').updateValueAndValidity();
      this.personalCustomerForm.get('PersonalPhoneWork').setValidators([Validators.pattern(this.getPhonePattern())]);
      this.personalCustomerForm.get('PersonalPhoneWork').updateValueAndValidity();
    }
    else {
      this.personalCustomerForm.get('PersonalPhoneOther').setValidators([Validators.required, Validators.pattern(this.getPhonePattern())]);
      this.personalCustomerForm.get('PersonalPhoneOther').updateValueAndValidity();
      this.personalCustomerForm.get('PersonalPhoneWork').setValidators([Validators.required, Validators.pattern(this.getPhonePattern())]);
      this.personalCustomerForm.get('PersonalPhoneWork').updateValueAndValidity();
    }
  }
  /* end */

  filterCustomerName(event) {
    let filtered: any[] = [];
    let query = event.query;
    for (let i = 0; i < this.listCustomer.length; i++) {
      let customer = this.listCustomer[i];
      if (customer.customerName.toLowerCase().indexOf(query.toLowerCase()) != -1) {
        filtered.push(customer);
      }
    }

    this.filteredCustomer = filtered;
  }

  goToDetail(customerId: string) {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/customer/detail', { customerId: customerId }]));
    window.open(url, '_blank');
  }

  checkDuplicateInforCustomer(checkType: number, formControl: FormControl) {
    if (formControl.valid) {
      let email = '';
      let phone = '';

      //Nếu là khách hàng doanh nghiệp
      if (Number(this.customerType) == 1) {
        email = this.companyCustomerForm.get('Email').value;
        phone = this.companyCustomerForm.get('Phone').value;
      }
      //Nếu là khách hàng cá nhân
      else if (Number(this.customerType) == 2) {
        email = this.personalCustomerForm.get('PersonalEmail').value;
        phone = this.personalCustomerForm.get('PersonalPhone').value;
      }

      this.customerService.checkDuplicateInforCustomer(null, Number(checkType), email, phone).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          if (result.valid == false) {
            if (Number(checkType) == 1) {
              this.validEmailCustomer = result.valid;
            }
            else if (Number(checkType) == 2) {
              this.validPhoneCustomer = result.valid;
            }
            this.showMessageErr();
          }
          else {
            if (Number(checkType) == 1) {
              this.validEmailCustomer = result.valid;
            }
            else if (Number(checkType) == 2) {
              this.validPhoneCustomer = result.valid;
            }
          }
        }
        else {
          this.showToast('popup', 'error', 'Thông báo', result.messageCode);
        }
      });
    }
  }

  showMessageErr() {
    //Nếu là Email
    if (!this.validEmailCustomer) {
      this.showToast('popup', 'error', 'Thông báo', 'Email khách hàng đã tồn tại trên hệ thống');
    }
    //Nếu là Số điện thoại
    if (!this.validPhoneCustomer) {
      this.showToast('popup', 'error', 'Thông báo', 'Số điện thoại khách hàng đã tồn tại trên hệ thống');
    }
  }

}

function checkDuplicateCode(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() !== "") {
        let duplicateCode = array.find(e => e === control.value.trim());
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
      if (typeof (control.value) === "string") {
        if (control.value.trim() === "") {
          return { 'blankString': true };
        }
      }
    }
    return null;
  }
}




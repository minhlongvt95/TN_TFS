import { Component, ElementRef, OnInit, ViewChild, ChangeDetectorRef, AfterViewInit, AfterViewChecked } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { } from 'primeng/api';
import { Router, ActivatedRoute } from '@angular/router';
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
  areaId: string;
  customerGroupId: string;
}

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  customer: leadReferenceCustomer
}

class investFund {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class category {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  isManager: boolean;
  positionId: string;
  organizationId: string;
}

class GeographicalArea {
  geographicalAreaId: string;
  geographicalAreaCode: string;
  geographicalAreaName: string;
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
@Component({
  selector: 'app-add-protential-customer-dialog',
  templateUrl: './add-protential-customer-dialog.component.html',
  styleUrls: ['./add-protential-customer-dialog.component.css']
})
export class AddProtentialCustomerDialogComponent implements OnInit, AfterViewChecked {

  emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  auth = JSON.parse(localStorage.getItem("auth"));
  currentEmployeeId = this.auth.EmployeeId;
  awaitResult: boolean = false;
  //list action in page
  actionAdd: boolean = true;
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  listGenders = [{ code: 'NAM', name: 'Nam' }, { code: 'NU', name: 'Nữ' }];
  isInland: number = 1;
  isInlandLH: number = 1;
  listProvincePersonalCustomer: Array<Province> = [];
  listDistrictPersonalCustomer: Array<District> = [];
  listWardPersonalCustomer: Array<Ward> = [];
  listPhoneContactCode: Array<any> = [];
  listEmailContactCode: Array<string> = [];
  listProvince: Array<Province> = []; //master data
  listDistrict: Array<District> = []; //master data
  listWard: Array<Ward> = []; //master data
  listGroupCustomer: Array<category> = [];
  index: number = 0;
  indexContactNow: number;
  isContact: boolean = true;
  //form
  customerType: number = 1;
  potentialCustomerForm: FormGroup; //form khach hang doanh nghiep
  potentialPersonalCustomerForm: FormGroup; //form khach hang ca nhan
  contactCustomerForm: FormGroup; // Thông tin liên he
  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  listContact: Array<ContactModel> = [];
  rows: number = 10;
  selectedContact: ContactModel;
  //master data
  listPersonalInChange: Array<Employee> = [];
  listEmployeeTakeCare: Array<Employee> = [];
  listInvestFund: Array<investFund> = [];
  listArea: Array<GeographicalArea> = [];
  /* create valiable add class scroll */
  fixed: boolean = false;
  isInvalidForm: boolean = false;
  isInvalidPersonalForm: boolean = false;
  isOpenNotifiError: boolean = false;
  isOpenNotifiErrorPersonal: boolean = false;
  withFiexd: string = "";

  waitRouting: boolean = false;

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
    private route: ActivatedRoute,
    private router: Router,
    private el: ElementRef,
    private customerService: CustomerService,
    private confirmationService: ConfirmationService,
    public ref: DynamicDialogRef,
    public changeRef: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    //Check permission
    let resource = "crm/customer/potential-customer-create";
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
    this.potentialCustomerForm = new FormGroup({
      'PotentialCustomerName': new FormControl('', [Validators.required, forbiddenSpaceText]), //ten doanh nghiep
      'Phone': new FormControl('', [Validators.pattern(this.getPhonePattern())]),
      'Email': new FormControl('', [Validators.pattern(this.emailPattern)]),
      'Address': new FormControl(''),
      'SocialLink': new FormControl(''),
      'InvestFund': new FormControl(null, [Validators.required]), //Nguon tiem nang
      'Pic': new FormControl(null, [Validators.required]), //nguoi phu trach
      'CustomerGroup': new FormControl(null), //nhom khach hang
      'Area': new FormControl(null),
      'Note': new FormControl(''),
      'EmployeeTakeCare': new FormControl(null),
      'ContactDate': new FormControl(null),
      'SalesUpdate': new FormControl(null),
      'EvaluateCompany': new FormControl(null),
      'SalesUpdateAfterMeeting': new FormControl(null)
    });

    this.contactCustomerForm = new FormGroup({
      "FirstName": new FormControl("", [Validators.required, checkBlankString()]),
      "LastName": new FormControl(""),
      "Email": new FormControl("", [Validators.pattern(this.emailPattern), checkDuplicateCode(this.listEmailContactCode)]),
      "Position": new FormControl(""),
      "DateOfBirth": new FormControl(null),
      "Phone": new FormControl("", [Validators.required, Validators.pattern(this.getPhonePattern()), checkDuplicateCode(this.listPhoneContactCode)]),
      "OtherInfor": new FormControl(""),
      "Gender": new FormControl(this.listGenders[0]),
      "isInlandLHControl": new FormControl(''),
      "GenderControl": new FormControl('NAM'),
      "provinceControl": new FormControl(""),
      "districtControl": new FormControl(""),
      "wardControl": new FormControl(""),
      "addressControl": new FormControl(""),
      'LinkFace': new FormControl(""),
      'EvaluateContactPeople': new FormControl("")
    });

    this.potentialPersonalCustomerForm = new FormGroup({
      'FirstName': new FormControl('', [Validators.required, forbiddenSpaceText]),
      'LastName': new FormControl('', [Validators.required, forbiddenSpaceText]),
      'Phone': new FormControl('', [Validators.pattern(this.getPhonePattern())]),
      'Email': new FormControl('', [Validators.pattern(this.emailPattern)]),
      'Address': new FormControl(''),
      'SocialLink': new FormControl(''),
      'InvestFund': new FormControl(null, [Validators.required]), //Nguon tiem nang
      'Pic': new FormControl(null, [Validators.required]), //nguoi phu trach
      'CustomerGroup': new FormControl(null), //nhom khach hang
      'Area': new FormControl(null),
      'Note': new FormControl(''),
    });
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


  async getMasterData() {
    this.loading = true;
    let result: any = await this.customerService.getDataCreatePotentialCustomer(this.currentEmployeeId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listPersonalInChange = result.listEmployeeModel;
      this.listGroupCustomer = result.listGroupCustomer;
      this.listInvestFund = result.listInvestFund;
      this.listArea = result.listArea;
      this.listProvince = result.listProvinceEntityModel;
      this.listWard = result.listWardEntityModel;
      this.listDistrict = result.listDistrictEntityModel;
      this.listCustomer = result.listCustomer;
      this.listEmployeeTakeCare = result.listEmployeeTakeCare;
      if (this.listEmployeeTakeCare) {
        this.listEmployeeTakeCare.forEach(item => {
          item.employeeName = item.employeeCode + '-' + item.employeeName;
        });
      }
      this.listProvincePersonalCustomer = [...this.listProvince.sort((a, b) => a.provinceName.localeCompare(b.provinceName))];
      //hiển thị người phụ trách theo định dạng mã - tên
      this.listPersonalInChange.forEach(item => item.employeeName = `${item.employeeCode} - ${item.employeeName}`);
      this.patchDefaultFormValue();
    } else {
      this.showToast('error', 'Thông báo', 'Lấy dữ liệu thất bại')
    }
  }

  patchDefaultFormValue() {
    let employeeId = this.auth.EmployeeId;
    let employee = this.listPersonalInChange.find(e => e.employeeId == employeeId);
    if (employee) {
      this.potentialCustomerForm.get('Pic').setValue(employee);
      this.potentialPersonalCustomerForm.get('Pic').setValue(employee);
    }
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  cancel() {
    let result: ResultDialog = {
      status: false,
      customer: null
    }
    this.ref.close(result);
  }

  PhonePattern(event: any) {
    // const pattern = /^[0-9]$/;
    // const inputChar = String.fromCharCode(event.charCode);
    // if (!pattern.test(inputChar)) {
    //   event.preventDefault();
    // }
  }

  changeCustomerType(event: any) {
    this.resetNotification();
    switch (Number(this.customerType)) {
      case 1:
        //khách hàng doanh nghiệp;
        // if (!this.potentialCustomerForm.valid) {
        //   Object.keys(this.potentialCustomerForm.controls).forEach(key => {
        //     if (!this.potentialCustomerForm.controls[key].valid) {
        //       this.potentialCustomerForm.controls[key].markAsTouched();
        //     }
        //   });
        //   let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
        //   if (target) {
        //     $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        //     target.focus();
        //   }

        //   this.isInvalidForm = true;
        //   this.isOpenNotifiError = true;  //Hiển thị message lỗi
        //   return;
        // }
        break
      case 2:
        //khách hàng cá nhân
        // if (!this.potentialPersonalCustomerForm.valid) {
        //   Object.keys(this.potentialPersonalCustomerForm.controls).forEach(key => {
        //     if (!this.potentialPersonalCustomerForm.controls[key].valid) {
        //       this.potentialPersonalCustomerForm.controls[key].markAsTouched();
        //     }
        //   });
        //   let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
        //   if (target) {
        //     $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        //     target.focus();
        //   }

        //   this.isInvalidPersonalForm = true;
        //   this.isOpenNotifiErrorPersonal = true;  //Hiển thị message lỗi
        //   return;
        // }
        break

      default:
        break;
    }
  }

  async createPotentialCustomer(type: string) {
    this.resetNotification();
    let customerType = Number(this.customerType);
    switch (customerType) {
      case 1:
        //khách hàng doanh nghiệp;
        this.handlePotentialCustomer(type);
        break
      case 2:
        //khách hàng cá nhân
        this.handlePotentialPersonalCustomer(type);
        break
    }
  }

  async handlePotentialCustomer(type) {
    if (!this.potentialCustomerForm.valid) {
      Object.keys(this.potentialCustomerForm.controls).forEach(key => {
        if (!this.potentialCustomerForm.controls[key].valid) {
          this.potentialCustomerForm.controls[key].markAsTouched();
        }
      });
      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }

      this.isInvalidForm = true;
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      return;
    }
    else if (!this.validEmailCustomer || !this.validPhoneCustomer) {
      this.showMessageErr();
      return;
    }

    let customer: CustomerModel = this.mappingPotentialCustomerFormToModel();
    let contact: ContactModel = this.mappingPotentialCustomerFormToContactModel();
    if (this.listContact) {
      if (this.listContact.length == 0) {
        this.addContactWhenSave();
      }
    }
    let companyContactList: Array<ContactModel> = this.listContact;
    companyContactList.forEach(item => {
      if (item.DateOfBirth) {
        item.DateOfBirth = convertToUTCTime(item.DateOfBirth);
      }
    });
    this.loading = true;
    let result: any = await this.customerService.createCustomerAsync(customer, contact, companyContactList, this.auth.userId, false, true);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listCustomer = result.listCustomer;

      switch (type) {
        case "save":
          let res: ResultDialog = {
            status: true,  //Hủy
            customer: new leadReferenceCustomer()
          }
          res.customer.address = contact.Address;
          res.customer.customerCode = customer.CustomerCode;
          res.customer.customerId = result.customerId;
          res.customer.customerName = customer.CustomerName;
          res.customer.customerType = customer.CustomerType;
          res.customer.email = contact.Email;
          res.customer.phone = contact.Phone;
          res.customer.personInChargeId = customer.PersonInChargeId;
          res.customer.investmentFundId = customer.InvestmentFundId;
          res.customer.areaId = contact.AreaId;
          res.customer.customerGroupId = customer.CustomerGroupId;
          res.customer.customerStatus = 'MOI';
          this.ref.close(res);
          break;
        case "save_new":
          if (result.duplicateContact == true) {
            this.showToast('warn', 'Thông báo', 'Người liên hệ đã tồn tại trong hệ thống');
          }
          this.showToast('success', 'Thông báo', 'Tạo khách hàng tiềm năng thành công');
          this.resetCustomerForm();
          this.patchDefaultFormValue();
          this.isInvalidForm = false;
          break;
        default:
          break;
      }
      this.loading = false;
    } else {
      this.showToast('error', 'Thông báo', result.messageCode);
      this.loading = false;
    }
  }

  async handlePotentialPersonalCustomer(type) {
    if (!this.potentialPersonalCustomerForm.valid) {
      Object.keys(this.potentialPersonalCustomerForm.controls).forEach(key => {
        if (!this.potentialPersonalCustomerForm.controls[key].valid) {
          this.potentialPersonalCustomerForm.controls[key].markAsTouched();
        }
      });
      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }

      this.isInvalidPersonalForm = true;
      this.isOpenNotifiErrorPersonal = true;  //Hiển thị message lỗi
      return;
    }
    else if (!this.validEmailCustomer || !this.validPhoneCustomer) {
      this.showMessageErr();
      return;
    }

    let customer: CustomerModel = this.mappingPotentialPersonalCustomerFormToModel();
    let contact: ContactModel = this.mappingPotentialPersonalCustomerFormToContactModel();
    this.loading = true;
    let result: any = await this.customerService.createCustomerAsync(customer, contact, [], this.auth.userId, false, true);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listCustomer = result.listCustomer;
      switch (type) { 
        case "save":
          let res: ResultDialog = {
            status: true,  //Hủy
            customer: new leadReferenceCustomer()
          }
          res.customer.address = contact.Address;
          res.customer.customerCode = customer.CustomerCode;
          res.customer.customerId = result.customerId;
          res.customer.customerName = customer.CustomerName;
          res.customer.customerType = customer.CustomerType;
          res.customer.email = contact.Email;
          res.customer.phone = contact.Phone;
          res.customer.investmentFundId = customer.InvestmentFundId;
          res.customer.areaId = contact.AreaId;
          res.customer.customerGroupId = customer.CustomerGroupId;
          res.customer.customerStatus = 'MOI';
          this.ref.close(res);
          break;
        case "save_new":
          if (result.duplicateContact == true) {
            this.showToast('warn', 'Thông báo', 'Người liên hệ đã tồn tại trong hệ thống');
          }
          this.showToast('success', 'Thông báo', 'Tạo khách hàng tiềm năng thành công');
          this.resetPersonalCustomerForm();
          this.isInvalidForm = false;
          break;
        default:
          break;
      }
      this.loading = false;
    } else {
      this.showToast('error', 'Thông báo', result.messageCode);
      this.loading = false;
    }

  }

  mappingPotentialCustomerFormToModel(): CustomerModel {
    let _potentialCustomerName = this.potentialCustomerForm.get('PotentialCustomerName').value;
    let _pic = this.potentialCustomerForm.get('Pic').value || null;
    let _investFund = this.potentialCustomerForm.get('InvestFund').value || null;
    let _customerGroup = this.potentialCustomerForm.get('CustomerGroup').value || null;
    let _empTakeCare = this.potentialCustomerForm.get('EmployeeTakeCare').value || null;
    let _contactDate = this.potentialCustomerForm.get('ContactDate').value || null;
    let _evaluateCompany = this.potentialCustomerForm.get('EvaluateCompany').value || null;
    let _salesUpdate = this.potentialCustomerForm.get('SalesUpdate').value || null;
    let _salesUpdateAfterMeeting = this.potentialCustomerForm.get('SalesUpdateAfterMeeting').value || null;

    let customer = new CustomerModel();
    customer.CustomerId = this.emptyGuid;
    customer.LeadId = null;
    customer.CustomerCode = '';
    customer.CustomerGroupId = _customerGroup ? _customerGroup.categoryId : null;
    customer.CustomerName = typeof (_potentialCustomerName) == "string" ? (_potentialCustomerName ? _potentialCustomerName.trim() : '') : _potentialCustomerName.customerName;
    customer.StatusId = this.emptyGuid;
    customer.CustomerServiceLevelId = null;
    customer.EmployeeTakeCareId = _empTakeCare ? _empTakeCare.employeeId : null;
    customer.InvestmentFundId = _investFund ? _investFund.categoryId : null;
    customer.PersonInChargeId = _pic ? _pic.employeeId : null;
    customer.CustomerCareStaff = null;
    customer.CustomerType = Number(this.customerType);
    customer.PaymentId = null;
    customer.FieldId = null;
    customer.ScaleId = null;
    customer.MaximumDebtDays = null;
    customer.MaximumDebtValue = null;
    customer.TotalSaleValue = 0;
    customer.TotalReceivable = 0;
    customer.NearestDateTransaction = null;
    customer.TotalCapital = null;
    customer.BusinessRegistrationDate = null;
    customer.EnterpriseType = null;
    customer.TotalEmployeeParticipateSocialInsurance = null;
    customer.TotalRevenueLastYear = null;
    customer.BusinessType = null;
    customer.BusinessScale = null;
    customer.CreatedById = this.auth.UserId;
    customer.Active = true;
    customer.ContactDate = _contactDate ? convertToUTCTime(_contactDate) : null;
    customer.EvaluateCompany = _evaluateCompany ? _evaluateCompany.trim() : null;
    customer.SalesUpdate = _salesUpdate ? _salesUpdate.trim() : null;
    customer.SalesUpdateAfterMeeting = _salesUpdateAfterMeeting ? _salesUpdateAfterMeeting.trim() : null;

    customer.CreatedDate = new Date();
    customer.CustomerCareStaff = null;
    customer.TotalCapital = 0;

    return customer;
  }

  mappingPotentialCustomerFormToContactModel(): ContactModel {
    let _potentialCustomerName = this.potentialCustomerForm.get('PotentialCustomerName').value;
    let _phone = this.potentialCustomerForm.get('Phone').value || '';
    let _email = this.potentialCustomerForm.get('Email').value || '';
    let _socialLink = this.potentialCustomerForm.get('SocialLink').value || '';
    let _address = this.potentialCustomerForm.get('Address').value || '';
    let _area = this.potentialCustomerForm.get('Area').value;
    let _note = this.potentialCustomerForm.get('Note').value || null;
    let contactModel = new ContactModel;

    contactModel.ContactId = this.emptyGuid;
    contactModel.ObjectId = this.emptyGuid;
    contactModel.FirstName = typeof (_potentialCustomerName) == "string" ? (_potentialCustomerName ? _potentialCustomerName.trim() : '') : _potentialCustomerName.customerName;
    contactModel.LastName = '';
    contactModel.Phone = _phone;
    contactModel.Email = _email;
    contactModel.WorkEmail = '';
    contactModel.Note = _note;
    contactModel.Gender = '';
    contactModel.SocialUrl = _socialLink;
    contactModel.Address = _address;
    contactModel.CreatedDate = new Date();
    contactModel.Active = true;
    contactModel.GeographicalAreaId = _area ? _area.geographicalAreaId : null;

    contactModel.CompanyAddress = '';
    contactModel.CompanyName = '';
    contactModel.DistrictId = null;
    contactModel.ProvinceId = null;
    contactModel.WardId = null;
    contactModel.IdentityID = '';
    contactModel.MaritalStatusId = null;
    contactModel.ObjectType = '';
    contactModel.OptionPosition = '';

    return contactModel;
  }

  mappingPotentialPersonalCustomerFormToModel(): CustomerModel {
    let _firstName = this.potentialPersonalCustomerForm.get('FirstName').value;
    let _lastName = this.potentialPersonalCustomerForm.get('LastName').value;
    let _fullName = _firstName + " " + _lastName;
    let _pic = this.potentialPersonalCustomerForm.get('Pic').value || null;
    let _customerGroup = this.potentialPersonalCustomerForm.get('CustomerGroup').value || null;
    let _investFund = this.potentialPersonalCustomerForm.get('InvestFund').value || null;
    let customer = new CustomerModel();
    customer.CustomerId = this.emptyGuid;
    customer.LeadId = null;
    customer.CustomerCode = '';
    customer.CustomerGroupId = _customerGroup ? _customerGroup.categoryId : null;
    customer.CustomerName = _fullName ? _fullName.trim() : '';
    customer.StatusId = this.emptyGuid;
    customer.CustomerServiceLevelId = null;
    customer.InvestmentFundId = _investFund ? _investFund.categoryId : null;
    customer.PersonInChargeId = _pic ? _pic.employeeId : null;
    customer.CustomerCareStaff = null;
    customer.CustomerType = Number(this.customerType);
    customer.PaymentId = null;
    customer.FieldId = null;
    customer.ScaleId = null;
    customer.MaximumDebtDays = null;
    customer.MaximumDebtValue = null;
    customer.TotalSaleValue = 0;
    customer.TotalReceivable = 0;
    customer.NearestDateTransaction = null;
    customer.TotalCapital = null;
    customer.BusinessRegistrationDate = null;
    customer.EnterpriseType = null;
    customer.TotalEmployeeParticipateSocialInsurance = null;
    customer.TotalRevenueLastYear = null;
    customer.BusinessType = null;
    customer.BusinessScale = null;
    customer.CreatedById = this.auth.UserId;
    customer.Active = true;

    customer.CreatedDate = new Date();
    customer.CustomerCareStaff = null;
    customer.TotalCapital = 0;

    return customer;
  }

  mappingPotentialPersonalCustomerFormToContactModel(): ContactModel {
    let _firstName = this.potentialPersonalCustomerForm.get('FirstName').value;
    let _lastName = this.potentialPersonalCustomerForm.get('LastName').value;
    let _phone = this.potentialPersonalCustomerForm.get('Phone').value || '';
    let _email = this.potentialPersonalCustomerForm.get('Email').value || '';
    let _socialLink = this.potentialPersonalCustomerForm.get('SocialLink').value || '';
    let _address = this.potentialPersonalCustomerForm.get('Address').value || '';
    let _area = this.potentialPersonalCustomerForm.get('Area').value;

    let _note = this.potentialPersonalCustomerForm.get('Note').value || null;
    let contactModel = new ContactModel;

    contactModel.ContactId = this.emptyGuid;
    contactModel.ObjectId = this.emptyGuid;
    contactModel.FirstName = _firstName ? _firstName.trim() : '';
    contactModel.LastName = _lastName ? _lastName.trim() : '';
    contactModel.Phone = _phone;
    contactModel.Email = _email;
    contactModel.Note = _note;
    contactModel.Gender = '';
    contactModel.SocialUrl = _socialLink;
    contactModel.Address = _address;
    contactModel.CreatedDate = new Date();
    contactModel.Active = true;
    contactModel.GeographicalAreaId = _area ? _area.geographicalAreaId : null;

    contactModel.CompanyAddress = '';
    contactModel.CompanyName = '';
    contactModel.DistrictId = null;
    contactModel.ProvinceId = null;
    contactModel.WardId = null;
    contactModel.IdentityID = '';
    contactModel.MaritalStatusId = null;
    contactModel.ObjectType = '';
    contactModel.OptionPosition = '';


    return contactModel;
  }

  resetCustomerForm() {
    this.potentialCustomerForm.reset();
    this.potentialCustomerForm.patchValue({
      'PotentialCustomerName': '',
      'Phone': '',
      'Email': '',
      'SocialLink': '',
      'CustomerGroup': null,
      'InvestFund': null,
      'Pic': null,
      'Note': '',
      'Address': ''
    });
    this.listContact = [];
  }

  resetPersonalCustomerForm() {
    this.potentialPersonalCustomerForm.reset();
    this.potentialPersonalCustomerForm.patchValue({
      'FirstName': '',
      'LastName': '',
      'Phone': '',
      'Email': '',
      'SocialLink': '',
      'CustomerGroup': null,
      'InvestFund': null,
      'Pic': null,
      'Note': '',
      'Address': ''
    });
    this.listContact = [];
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  resetNotification() {
    this.isInvalidForm = false;
    this.isInvalidPersonalForm = false;
    this.isOpenNotifiError = false;
    this.isOpenNotifiErrorPersonal = false;
  }

  isInlandOnChange() {
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

  addContactWhenSave() {
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
  updateCustomerContact() {
    this.listContact.forEach(item => {
      if (item.Index == this.indexContactNow) {

        let _linkFace = this.contactCustomerForm.get('LinkFace').value || null;
        let _evaluateContactPeople = this.contactCustomerForm.get('EvaluateContactPeople').value || null;
        item.FirstName = this.contactCustomerForm.get('FirstName').value;
        item.LastName = this.contactCustomerForm.get('LastName').value;
        (item as any).FullName = item.FirstName; //+ " " + item.LastName;
        item.Email = this.contactCustomerForm.get('Email').value;
        item.Role = this.contactCustomerForm.get('Position').value;
        item.DateOfBirth = this.contactCustomerForm.get('DateOfBirth').value;
        item.Phone = this.contactCustomerForm.get('Phone').value;
        item.Other = this.contactCustomerForm.get('OtherInfor').value;
        item.Gender = this.contactCustomerForm.get('GenderControl').value;
        item.GenderDisplay = item.Gender == 'NAM' ? 'Nam' : 'Nữ';
        item.ProvinceId = this.contactCustomerForm.get('provinceControl').value ? this.contactCustomerForm.get('provinceControl').value.provinceId : null;
        item.DistrictId = this.contactCustomerForm.get('districtControl').value ? this.contactCustomerForm.get('districtControl').value.districtId : null;
        item.WardId = this.contactCustomerForm.get('wardControl').value ? this.contactCustomerForm.get('wardControl').value.wardId : null;
        item.Address = this.contactCustomerForm.get('addressControl').value;
        item.LinkFace = _linkFace ? _linkFace.trim() : null;
        item.EvaluateContactPeople = _evaluateContactPeople ? _evaluateContactPeople.trim() : null;
        item.IsInlandLH = this.isInlandLH;
      }
    });
    this.isContact = true;
    this.resetCompanyCustomerForm();
  }

  cancelContact() {
    this.isContact = true;
    this.resetCompanyCustomerForm();
  }

  onDeleteContact(rowData: ContactModel) {
    this.listContact = this.listContact.filter(e => e != rowData);
  }

  editContact(rowData: ContactModel) {
    this.isContact = false;
    this.isInlandLH = rowData.IsInlandLH;
    this.indexContactNow = rowData.Index;

    this.contactCustomerForm.get('FirstName').setValue(rowData.FirstName);
    this.contactCustomerForm.get('LastName').setValue(rowData.LastName);
    this.contactCustomerForm.get('Email').setValue(rowData.Email);
    this.contactCustomerForm.get('Position').setValue(rowData.Role);
    this.contactCustomerForm.get('DateOfBirth').setValue(rowData.DateOfBirth);
    this.contactCustomerForm.get('Phone').setValue(rowData.Phone);
    this.contactCustomerForm.get('OtherInfor').setValue(rowData.Other);
    var gender = this.listGenders.find(c => c.code == rowData.Gender);
    this.contactCustomerForm.get('Gender').setValue(gender);
    this.contactCustomerForm.get('GenderControl').setValue(rowData.Gender);

    if (rowData.IsInlandLH == 1) {
      if (rowData.ProvinceId) {
        let province: any = this.listProvince.find(c => c.provinceId == rowData.ProvinceId);
        this.contactCustomerForm.get('provinceControl').setValue(province);
        this.listDistrictPersonalCustomer = province.districtList;
        if (rowData.DistrictId) {
          let district: any = province.districtList.find(c => c.districtId == rowData.DistrictId);
          this.contactCustomerForm.get('districtControl').setValue(district);
          this.listWardPersonalCustomer = district.wardList;
          if (rowData.WardId) {
            let ward = district.wardList.find(c => c.wardId == rowData.WardId);
            this.contactCustomerForm.get('wardControl').setValue(ward);
          }
        }
      }
    }
    this.contactCustomerForm.get('addressControl').setValue(rowData.Address);
    this.contactCustomerForm.get('LinkFace').setValue(rowData.LinkFace);
    this.contactCustomerForm.get('EvaluateContactPeople').setValue(rowData.EvaluateContactPeople);
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

  mappingFormToContactModel(): ContactModel {
    this.index++;
    let _linkFace = this.contactCustomerForm.get('LinkFace').value || null;
    let _evaluateContactPeople = this.contactCustomerForm.get('EvaluateContactPeople').value || null;

    let contactModel = new ContactModel;
    contactModel.Index = this.index;
    contactModel.FirstName = this.contactCustomerForm.get('FirstName').value;
    contactModel.LastName = this.contactCustomerForm.get('LastName').value;
    (contactModel as any).FullName = contactModel.FirstName; // + " " + contactModel.LastName;
    contactModel.Email = this.contactCustomerForm.get('Email').value;
    contactModel.Role = this.contactCustomerForm.get('Position').value;
    contactModel.DateOfBirth = this.contactCustomerForm.get('DateOfBirth').value;
    contactModel.Phone = this.contactCustomerForm.get('Phone').value;
    contactModel.Other = this.contactCustomerForm.get('OtherInfor').value;
    contactModel.Gender = this.contactCustomerForm.get('GenderControl').value;
    contactModel.GenderDisplay = contactModel.Gender == 'NAM' ? 'Nam' : 'Nữ';
    contactModel.ProvinceId = this.contactCustomerForm.get('provinceControl').value ? this.contactCustomerForm.get('provinceControl').value.provinceId : null;
    contactModel.DistrictId = this.contactCustomerForm.get('districtControl').value ? this.contactCustomerForm.get('districtControl').value.districtId : null;
    contactModel.WardId = this.contactCustomerForm.get('wardControl').value ? this.contactCustomerForm.get('wardControl').value.wardId : null;
    contactModel.Address = this.contactCustomerForm.get('addressControl').value;

    contactModel.LinkFace = _linkFace ? _linkFace.trim() : null;
    contactModel.EvaluateContactPeople = _evaluateContactPeople ? _evaluateContactPeople.trim() : null;
    return contactModel;
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
      "GenderControl": "NAM",
      "isInlandLHControl": 1,
      "provinceControl": null,
      "districtControl": null,
      "wardControl": null,
      "addressControl": null,
    });
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
    let url = this.router.serializeUrl(this.router.createUrlTree(['/customer/potential-customer-detail', { customerId: customerId }]));
    window.open(url, '_blank');
  }

  checkDuplicateInforCustomer(checkType: number, formControl: FormControl) {
    if (formControl.valid) {
      let email = '';
      let phone = '';

      //Nếu là khách hàng doanh nghiệp
      if (Number(this.customerType) == 1) {
        email = this.potentialCustomerForm.get('Email').value;
        phone = this.potentialCustomerForm.get('Phone').value;
      }
      //Nếu là khách hàng cá nhân
      else if (Number(this.customerType) == 2) {
        email = this.potentialPersonalCustomerForm.get('Email').value;
        phone = this.potentialPersonalCustomerForm.get('Phone').value;
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
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      });
    }
  }

  showMessageErr() {
    //Nếu là Email
    if (!this.validEmailCustomer) {
      this.showToast('error', 'Thông báo', 'Email khách hàng đã tồn tại trên hệ thống');
    }
    //Nếu là Số điện thoại
    if (!this.validPhoneCustomer) {
      this.showToast('error', 'Thông báo', 'Số điện thoại khách hàng đã tồn tại trên hệ thống');
    }
  }

  ngAfterViewChecked() {
    this.changeRef.detectChanges();
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
      if (control.value.trim() === "") {
        return { 'blankString': true };
      }
    }
    return null;
  }
}

function forbiddenSpaceText(control: FormControl) {
  if (typeof (control.value) === "string") {
    let text = control.value;
    if (text && text.trim() == "") {
      return {
        forbiddenSpaceText: {
          parsedDomain: text
        }
      }
    }
  }

  return null;
}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

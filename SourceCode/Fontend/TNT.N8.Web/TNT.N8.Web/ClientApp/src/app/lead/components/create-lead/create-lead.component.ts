import { Component, OnInit, ElementRef, HostListener, ViewChild, Renderer2 } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import * as $ from 'jquery';

import { LeadModel } from "../../models/lead.model";
import { ContactModel, contactModel } from "../../../shared/models/contact.model";
import { NoteModel } from '../../../shared/models/note.model';
import { SendEmailModel } from '../../../admin/models/sendEmail.model';

import { MessageService, ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { NoteService } from '../../../shared/services/note.service';
import { TranslateService } from '@ngx-translate/core';
import { LeadService } from "../../services/lead.service";
import { EmailConfigService } from '../../../admin/services/email-config.service';
import { GetPermission } from '../../../shared/permission/get-permission';

import { LeadDetailDialogComponent } from '../lead-detail-dialog/lead-detail-dialog.component';
import { CreatContactLeadDialogComponent } from './../creat-contact-lead-dialog/creat-contact-lead-dialog.component';

import { LeadDetailModel } from './../../models/leadDetail.Model';
import { QuoteService } from '../../../customer/services/quote.service';
import { ThrowStmt } from '@angular/compiler';
import { AddProtentialCustomerDialogComponent } from '../../../customer/components/add-protential-customer-dialog/add-protential-customer-dialog.component';
import { AddCustomerDialogComponent } from '../../../customer/components/add-customer-dialog/add-customer-dialog.component';

/* #region  MODELS */
/* MODELS */
class customerContactModel {
  customerId: string;
  customerFullName: string;
  email: string;
  phone: string;
  address: string;
}

class genderModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class interestedGroupModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class leadTypeModel {
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

class potentialModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class leadGroup {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class businessType {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class investFund {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

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

class probability {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

interface ResultDetailDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  leadDetailModel: LeadDetailModel,
}

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  customer: leadReferenceCustomer
}

class leadContactConfigDataModel {
  isEdit: boolean; //tạo mới: false; chỉnh sửa: true
  contact: contactModel
}

class resultContactDialog {
  status: boolean;
  contact: contactModel;
}

interface Customer {
  customerId: string;
  sheetName: string;
  customerName: string;
  sheetNameName: string;
  customerEmail: string;
  customerPhone: string;
  fullAddress: string;
  customerGroupId: string;
  maximumDebtDays: number;
  maximumDebtValue: number;
  personInChargeId: string;
}

class GeographicalArea {
  geographicalAreaId: string;
  geographicalAreaCode: string;
  geographicalAreaName: string;
}

/* END MODELS */
/* #endregion */

@Component({
  selector: 'app-create-lead',
  templateUrl: './create-lead.component.html',
  styleUrls: ['./create-lead.component.css'],
  providers: [AddProtentialCustomerDialogComponent]
})
export class CreateLeadComponent implements OnInit {
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;

  auth = JSON.parse(localStorage.getItem("auth"));
  currentEmployeeId = this.auth.EmployeeId;
  awaitResult: boolean = false;
  //routing varriables
  customerId: string = null;
  //list action in page
  actionAdd: boolean = true;
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  //master data
  listCustomerContact: Array<customerContactModel> = [];
  listEmailLead: Array<string> = [];
  listGender: Array<genderModel> = [];
  listInterestedGroup: Array<interestedGroupModel> = [];
  listLeadType: Array<leadTypeModel> = [];
  listLeadGroup: Array<leadGroup> = [];
  listPersonalInChange: Array<Employee> = [];
  listPersonalInChangeCus: Array<Employee> = [];
  listPhoneLead: Array<string> = [];
  listPotential: Array<potentialModel> = [];
  listArea: Array<GeographicalArea> = [];
  //new
  listBusinessType: Array<businessType> = [];
  listInvestFund: Array<investFund> = [];
  listProbability: Array<probability> = [];
  listLeadReferenceCustomer: Array<leadReferenceCustomer> = [];
  listCurrentReferenceCustomer: Array<leadReferenceCustomer> = [];
  //form
  createLeadForm: FormGroup;
  //create company
  /* create valiable add class scroll */
  fixed: boolean = false;
  isKCL: boolean = false;
  withFiexd: string = "";
  cols: any[];
  selectedColumns: any[];
  colsContacts: any[];
  selectedColumnsContact: any[];
  actionEdit: boolean = true;

  //table
  listContact: Array<contactModel> = [];
  isOpenNotifiError: boolean = false;
  isInvalidForm: boolean = false;
  isExist: boolean = true;
  //popup
  leadDetail: LeadDetailModel = new LeadDetailModel();
  listLeadDetail: Array<LeadDetailModel> = [];
  listLeadDetailModelOrderBy: Array<LeadDetailModel> = [];

  unitMoneyLabel: string = 'VND';
  amountDiscount: number;
  amountVat: number;

  selectedObjectType: string = 'cus';
  listCustomer: Array<leadReferenceCustomer> = [];  //list khách hàng
  listCurrentCustomer: Array<leadReferenceCustomer> = []; //list Khách hàng tiềm năng

  @ViewChild('toggleButton') toggleButton: ElementRef;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  /*Khởi tại constructor*/
  constructor(
    private translate: TranslateService,
    private leadService: LeadService,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private router: Router,
    private noteService: NoteService,
    public builder: FormBuilder,
    private el: ElementRef,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private renderer: Renderer2,
  ) {
    this.translate.setDefaultLang('vi');
    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (!this.toggleButton.nativeElement.contains(e.target) &&
          !this.notifi.nativeElement.contains(e.target) &&
          !this.save.nativeElement.contains(e.target) &&
          !this.saveAndCreate.nativeElement.contains(e.target)) {
          this.isOpenNotifiError = false;
        }
      }
    });
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  ngOnInit() {
    //Check permission
    let resource = "crm/lead/create-lead";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }

      this.route.params.subscribe(params => {
        this.customerId = params['customerId'];
      });

      this.initForm();
      this.setTable();
      this.getMasterData();
    }
  }

  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  initForm() {
    this.createLeadForm = new FormGroup({
      //cot thu 1
      'LeadName': new FormControl('', [Validators.required, forbiddenSpaceText]), //ten co hoi
      'LeadType': new FormControl(null),
      'Email': new FormControl('', [Validators.pattern(this.emailPattern)]),
      'Phone': new FormControl('', [Validators.pattern(this.getPhonePattern()), Validators.required]),
      'DetailAddress': new FormControl(''),
      'Pic': new FormControl(null, []), //nguoi phu trach
      //cot thu 2
      'Group': new FormControl(null), //nhom khach hang
      'Interested': new FormControl([], [Validators.required]),//nhu cau san pham, dich vu
      'DetailInterested': new FormControl(''), //chi tiet nhu cau
      'Potential': new FormControl(null, [Validators.required]),
      //new
      'BusinessType': new FormControl(null), //loai hinh doanh nghiep
      'InvestFund': new FormControl(null, [Validators.required]), //Nguon tiem nang
      'ExpectedSale': new FormControl('0'),
      'Probability': new FormControl(null, [Validators.required]), //xac suat
      'RefCustomer': new FormControl(null),// khach hang
      'Area': new FormControl(null),
    });
  }

  resetForm() {
    this.createLeadForm.reset();
    this.createLeadForm.patchValue({
      'LeadName': '',
      'LeadType': null,
      'RefCustomer': null,
      'Email': '',
      'Phone': '',
      'DetailAddress': '',
      'Pic': null,
      'Group': null,
      'Interested': [],
      'DetailInterested': '',
      'BusinessType': null,
      'InvestFund': null,
      'ExpectedSale': '0',
      'Probability': null,
      'Area': null
    });
  }

  resetDetailTable() {
    this.listLeadDetail = [];
  }

  resetContactTalbe() {
    this.listContact = [];
  }

  setTable() {
    this.cols = [
      { field: 'Move', header: '#', width: '70px', textAlign: 'center', color: '#f44336' },
      { field: 'ProductCode', header: 'Mã sản phẩm', width: '70px', textAlign: 'left', color: '#f44336' },
      { field: 'ExplainStr', header: 'Tên sản phẩm', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'NameVendor', header: 'Nhà cung cấp', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'Quantity', header: 'Số lượng', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'ProductNameUnit', header: 'Đơn vị tính', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'UnitPrice', header: 'Đơn giá', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'NameMoneyUnit', header: 'Đơn vị tiền', width: '30px', textAlign: 'left', color: '#f44336' },
      { field: 'ExchangeRate', header: 'Tỷ giá', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'Vat', header: 'Thuế GTGT (%)', width: '30px', textAlign: 'right', color: '#f44336' },
      { field: 'DiscountValue', header: 'Chiết khấu', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'SumAmount', header: 'Thành tiền (VND)', width: '60px', textAlign: 'right', color: '#f44336' },
      { field: 'Delete', header: 'Thao tác', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumns = this.cols.filter(e => e.field == "Move" || e.field == "ProductCode" || e.field == "ExplainStr" || e.field == "Quantity"
      || e.field == "UnitPrice" || e.field == "SumAmount" || e.field == "Delete");

    this.colsContacts = [
      { field: 'firstName', header: 'Tên liên hệ', textAlign: 'left', color: '#f44336' },
      { field: 'genderDisplay', header: 'Giới tính', textAlign: 'center', color: '#f44336' },
      { field: 'dateOfBirth', header: 'Ngày sinh', textAlign: 'right', color: '#f44336' },
      { field: 'role', header: 'Vị trí công việc', textAlign: 'left', color: '#f44336' },
      { field: 'relationShip', header: 'Mối liên hệ', textAlign: 'left', color: '#f44336' },
      { field: 'phone', header: 'Số di động', textAlign: 'right', color: '#f44336' },
      { field: 'email', header: 'Email', textAlign: 'left', color: '#f44336' },
      { field: 'note', header: 'Ghi chú', textAlign: 'left', color: '#f44336' },
      { field: 'Delete', header: 'Thao tác', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumnsContact = this.colsContacts.filter(e => e.field == "firstName" || e.field == "genderDisplay" || e.field == "dateOfBirth"
      || e.field == "role" || e.field == "phone" || e.field == "Delete");
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  /*Function lấy ra các master data*/
  async getMasterData() {
    this.loading = true;
    let result: any = await this.leadService.getDataCreateLead(this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listCustomerContact = result.listCustomerContact;
      this.listEmailLead = result.listEmailLead;
      this.listGender = result.listGender;
      this.listInterestedGroup = result.listInterestedGroup;
      this.listLeadType = result.listLeadType;
      this.listLeadGroup = result.listLeadGroup;
      this.listPersonalInChange = result.listPersonalInChange;
      this.listArea = result.listArea;
      this.listPersonalInChange.forEach(item => {
        item.employeeName = item.employeeCode + ' - ' + item.employeeName;
      });
      this.listPersonalInChangeCus = this.listPersonalInChange;
      this.listPhoneLead = result.listPhoneLead;
      this.listPotential = result.listPotential;
      //new
      this.listBusinessType = result.listBusinessType;
      this.listInvestFund = result.listInvestFund;
      this.listProbability = result.listProbability;
      this.listLeadReferenceCustomer = result.listLeadReferenceCustomer;
      this.createLeadForm.updateValueAndValidity();

      let customer = this.listLeadReferenceCustomer.find(e => e.customerId == this.customerId);
      //set default value after get master data
      this.setDefaultValue();
      if (this.listPersonalInChange.length == 1) {
        this.setDefaultPic();
      }
      if (customer) {
        if (customer.customerStatus == 'HDO') {
          this.selectedObjectType = 'cus'
          this.changeObjectType('cus');
        }
        else {
          this.selectedObjectType = 'lea'
          this.changeObjectType('lea')
        }
      }
      else {
        this.selectedObjectType = 'cus'
        this.changeObjectType('cus');
      }
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
    }
  }



  /*Event chuyển loại khách hàng (Khách hàng hoặc Khách hàng tiềm năng)*/
  async changeObjectType(objecType: any) {
    let customer = this.listLeadReferenceCustomer.find(e => e.customerId == this.customerId);
    if (objecType == 'cus') {
      var customerType = 'HDO';
      let result: any = await this.leadService.getListCustomerByType(this.auth.UserId, customerType);
      if (result.statusCode === 200) {
        this.listCustomer = result.listCustomerByType;
        this.listCustomer.forEach(x => {
          x.customerName = x.customerCode + ' - ' + x.customerName;
        });
        this.listCurrentCustomer = this.listCustomer;
      }
    } else if (objecType == 'lea') {
      var customerType = 'MOI';
      let result: any = await this.leadService.getListCustomerByType(this.auth.UserId, customerType);
      if (result.statusCode === 200) {
        this.listCustomer = result.listCustomerByType;
        this.listCustomer.forEach(x => {
          x.customerName = x.customerCode + ' - ' + x.customerName;
        });
        this.listCurrentCustomer = this.listCustomer;
      }
    }
    if (this.customerId) {
      let customer = this.listLeadReferenceCustomer.find(e => e.customerId == this.customerId);
      if (customer) {
        switch (Number(customer.customerType)) {
          case 1:
            let _refCustomer1 = this.listLeadType.find(e => e.categoryCode == "KCL") || null;
            this.createLeadForm.get('LeadType').setValue(_refCustomer1);
            switch (_refCustomer1.categoryCode) {
              case "KPL":
                //khach hang ca nhan
                this.listCurrentCustomer = this.listCustomer.filter(e => e.customerType === 2);
                this.createLeadForm.get('RefCustomer').reset();
                // this.isTypeCustomer = true;
                break;
              case "KCL":
                //khach hang doanh nghiep
                this.listCurrentCustomer = this.listCustomer.filter(e => e.customerType === 1);
                this.createLeadForm.get('RefCustomer').reset();
                // this.isTypeCustomer = true;
                break;
              default:
                break;
            }
            let _customer1 = this.listCurrentCustomer.find(e => e.customerId == this.customerId) || null;
            this.createLeadForm.get('LeadName').setValue('Bán hàng cho ' + _customer1.customerName);
            this.createLeadForm.get('RefCustomer').setValue(_customer1);
            this.createLeadForm.get('DetailAddress').setValue(_customer1.address);
            this.createLeadForm.get('Email').setValue(_customer1.workEmail);
            this.createLeadForm.get('Phone').setValue(_customer1.phone);
            let pic1 = this.listPersonalInChange.find(x => x.employeeId == _customer1.personInChargeId);
            this.createLeadForm.get('Pic').setValue(pic1);
            let investFund1 = this.listInvestFund.find(x => x.categoryId == _customer1.investmentFundId);
            this.createLeadForm.get('InvestFund').setValue(investFund1);
            break;
          case 2:
            let _refCustomer2 = this.listLeadType.find(e => e.categoryCode == "KPL") || null;
            this.createLeadForm.get('LeadType').setValue(_refCustomer2);
            switch (_refCustomer2.categoryCode) {
              case "KPL":
                //khach hang ca nhan
                this.listCurrentCustomer = this.listCustomer.filter(e => e.customerType === 2);
                this.createLeadForm.get('RefCustomer').reset();
                // this.isTypeCustomer = true;
                break;
              case "KCL":
                //khach hang doanh nghiep
                this.listCurrentCustomer = this.listCustomer.filter(e => e.customerType === 1);
                this.createLeadForm.get('RefCustomer').reset();
                // this.isTypeCustomer = true;
                break;
              default:
                break;
            }
            let _customer2 = this.listCurrentCustomer.find(e => e.customerId == this.customerId) || null;
            this.createLeadForm.get('LeadName').setValue('Bán hàng cho ' + _customer2.customerName);
            this.createLeadForm.get('RefCustomer').setValue(_customer2);
            this.createLeadForm.get('DetailAddress').setValue(_customer2.address);
            this.createLeadForm.get('Email').setValue(_customer2.email);
            this.createLeadForm.get('Phone').setValue(_customer2.phone);
            let pic2 = this.listPersonalInChange.find(x => x.employeeId == _customer2.personInChargeId);
            this.createLeadForm.get('Pic').setValue(pic2);
            let investFund2 = this.listInvestFund.find(x => x.categoryId == _customer2.investmentFundId);
            this.createLeadForm.get('InvestFund').setValue(investFund2);
            break;
          default:
            break;
        }
      }
    } else {
      this.createLeadForm.reset();
    }
  }

  goToCustomerDetail(event: any) {
    var customer = this.createLeadForm.get('RefCustomer').value;
    if (this.selectedObjectType == 'cus') {
      var url = this.router.serializeUrl(
        this.router.createUrlTree(['/customer/detail', { customerId: customer.customerId }])
      );
      window.open(url, '_blank');
    }
    else if (this.selectedObjectType == 'lea') {
      var url = this.router.serializeUrl(
        this.router.createUrlTree(['/customer/potential-customer-detail', { customerId: customer.customerId }])
      );
      window.open(url, '_blank');
    }
  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case -1: {
        result = result;
        break;
      }
      case 0: {
        result = Math.round(result);
        break;
      }
      case 1: {
        result = Math.round(number * 10) / 10;
        break;
      }
      case 2: {
        result = Math.round(number * 100) / 100;
        break;
      }
      case 3: {
        result = Math.round(number * 1000) / 1000;
        break;
      }
      case 4: {
        result = Math.round(number * 10000) / 10000;
        break;
      }
      default: {
        result = result;
        break;
      }
    }
    return result;
  }

  setDefaultPic() {
    let personalInChange = this.listPersonalInChange[0];
    if (personalInChange) {
      this.createLeadForm.get('Pic').setValue(personalInChange);
    }
  }

  setDefaultValue() {
    let highPotential = this.listPotential.find(c => c.categoryCode == 'TBI');
    if (highPotential) {
      this.createLeadForm.get('Potential').setValue(highPotential);
    }
    //gán khách hàng nếu cơ hội được tạo từ routing từ khách hàng
    if (this.customerId) {
      let customer = this.listLeadReferenceCustomer.find(e => e.customerId == this.customerId);
      if (customer) {
        switch (Number(customer.customerType)) {
          case 1:
            let _refCustomer1 = this.listLeadType.find(e => e.categoryCode == "KCL") || null;
            this.createLeadForm.get('LeadType').setValue(_refCustomer1);
            this.listCurrentReferenceCustomer = this.listLeadReferenceCustomer.filter(e => e.customerType === 1);
            let _customer1 = this.listLeadReferenceCustomer.find(e => e.customerId == this.customerId) || null;
            this.createLeadForm.get('RefCustomer').setValue(_customer1);
            break;
          case 2:
            let _refCustomer2 = this.listLeadType.find(e => e.categoryCode == "KPL") || null;
            this.createLeadForm.get('LeadType').setValue(_refCustomer2);
            this.listCurrentReferenceCustomer = this.listLeadReferenceCustomer.filter(e => e.customerType === 1);
            let _customer2 = this.listLeadReferenceCustomer.find(e => e.customerId == this.customerId) || null;
            this.createLeadForm.controls['RefCustomer'].setValue(_customer2);
            break;
          default:
            break;
        }
      }
    }
  }

  checkDuplicateCustomer(type: string, formControl: FormControl | AbstractControl): boolean {
    if (formControl.invalid) return false;
    let isDuplicate: boolean = false;
    let value = formControl.value;
    switch (type) {
      case 'email':
        let duplicateEmailCustomer: customerContactModel = this.listCustomerContact.filter(e => e.email != null && e.email != "").find(e => e.email.trim() == value);
        if (duplicateEmailCustomer) {
          isDuplicate = true;
          this.confirmationService.confirm({
            message: `Đã tồn tại khách hàng có email này trong hệ thống. Bạn có muốn cập nhật thông tin khách hàng này không?`,
            accept: () => {
              this.router.navigate(['/customer/detail', { customerId: duplicateEmailCustomer.customerId, contactId: this.emptyGuid }]);
            }
          });
        }
        break;
      case 'phone':
        let duplicatePhoneCustomer: customerContactModel = this.listCustomerContact.filter(e => e.phone != null).find(e => e.phone.trim() == value);
        if (duplicatePhoneCustomer) {
          isDuplicate = true;
          this.confirmationService.confirm({
            message: `Đã tồn tại khách hàng có số điện thoại này trong hệ thống. Bạn có muốn cập nhật thông tin khách hàng này không?`,
            accept: () => {
              this.router.navigate(['/customer/detail', { customerId: duplicatePhoneCustomer.customerId, contactId: this.emptyGuid }]);
            }
          });
        }
        break;
      default:
        break;
    }
    return isDuplicate;
  }

  mapFormToLeadModel(): LeadModel {
    let lead = new LeadModel();
    lead.LeadId = this.emptyGuid;
    let _detailInterested = this.createLeadForm.get('DetailInterested').value;
    let _potential: potentialModel = this.createLeadForm.get('Potential').value;
    let _interested: interestedGroupModel = null; //chuyen qua bang mapping giua lead va interested group [LeadInterestedGroupMapping]
    let _pic: Employee = this.createLeadForm.get('Pic').value;
    let _leadType: leadTypeModel = this.createLeadForm.get('LeadType').value;
    let _group: leadGroup = this.createLeadForm.get('Group').value;
    let _customer: leadReferenceCustomer = this.createLeadForm.get('RefCustomer').value;
    let _businessType: businessType = this.createLeadForm.get('BusinessType').value;
    let _investFund: investFund = this.createLeadForm.get('InvestFund').value;
    let _probability: probability = this.createLeadForm.get('Probability').value;
    let _expectedSale = this.createLeadForm.get('ExpectedSale').value;
    let _area = this.createLeadForm.get('Area').value;

    lead.RequirementDetail = _detailInterested;
    lead.PotentialId = _potential ? _potential.categoryId : null;
    lead.InterestedGroupId = _interested ? _interested.categoryId : null;
    lead.PersonInChargeId = _pic ? _pic.employeeId : null;
    lead.LeadTypeId = _leadType ? _leadType.categoryId : null;
    lead.LeadGroupId = _group ? _group.categoryId : null;
    lead.CustomerId = _customer ? _customer.customerId : null;
    lead.BusinessTypeId = _businessType ? _businessType.categoryId : null;
    lead.InvestmentFundId = _investFund ? _investFund.categoryId : null;
    lead.ProbabilityId = _probability ? _probability.categoryId : null;
    lead.ExpectedSale = ParseStringToFloat(_expectedSale);
    lead.StatusId = this.emptyGuid;
    lead.CreatedById = this.auth.UserId;
    lead.CreatedDate = new Date();
    lead.UpdatedById = null;
    lead.UpdatedDate = null;
    lead.Active = true;
    lead.Role = null;
    lead.WaitingForApproval = false;
    lead.GeographicalAreaId = _area ? _area.geographicalAreaId : null;
    return lead;
  }

  mapFormToLeadContactModel(): ContactModel {
    let _leadName = this.createLeadForm.get('LeadName').value;
    let _phone = this.createLeadForm.get('Phone').value;
    let _email = this.createLeadForm.get('Email').value;
    let _detailAddress = this.createLeadForm.get('DetailAddress').value;


    let contact = new ContactModel();
    contact.ContactId = this.emptyGuid;
    contact.ObjectId = this.emptyGuid;
    contact.ObjectType = "LEA";
    contact.FirstName = _leadName;
    contact.LastName = '';
    contact.Gender = null; //bo gioi tinh cua lead
    contact.DateOfBirth = null;
    contact.Phone = _phone
    contact.WorkPhone = null;
    contact.OtherPhone = null;
    contact.Email = _email
    contact.WorkEmail = null;
    contact.OtherEmail = null;
    contact.IdentityID = null;
    contact.AvatarUrl = null;
    contact.Address = _detailAddress
    contact.CreatedById = this.auth.UserId;
    contact.CreatedDate = new Date();
    contact.UpdatedById = null;
    contact.UpdatedDate = null;
    contact.Active = true;
    // contact.GeographicalAreaId = _area ? _area.geographicalAreaId : null;
    return contact;
  }

  getListInterestedId(): Array<string> {
    let result = [];
    let _interested: Array<interestedGroupModel> = this.createLeadForm.get('Interested').value;
    let _listInterestedgroup = _interested.map(e => e.categoryId);
    result = _listInterestedgroup ? _listInterestedgroup : [];
    return result;
  }

  getListContactFromTable(): Array<ContactModel> {
    let result = new Array<ContactModel>();

    this.listContact.forEach(_contact => {
      let newContact = new ContactModel();
      //data
      newContact.FirstName = _contact.firstName;
      newContact.LastName = _contact.lastName;
      newContact.Gender = _contact.gender;
      newContact.DateOfBirth = _contact.dateOfBirth;
      newContact.Role = _contact.role;
      newContact.RelationShip = _contact.relationShip;
      newContact.Phone = _contact.phone;
      newContact.Email = _contact.email;
      newContact.Note = _contact.note;
      //default value
      newContact.ContactId = this.emptyGuid;
      newContact.ObjectId = this.emptyGuid;
      newContact.ObjectType = "LEA_CON";
      newContact.Active = true;
      newContact.CreatedById = this.auth.UserId;
      newContact.CreatedDate = new Date();

      result = [newContact, ...result];
    });

    return result;
  }

  cancel() {
    this.confirmationService.confirm({
      message: `Các thay đổi sẽ không được lưu lại. Hành động này không thể được hoàn tác, bạn có chắc chắn muốn huỷ?`,
      accept: () => {
        this.router.navigate(['/lead/list']);
      }
    });
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  async CreateLead(type: boolean) {
    //type = true: luu va tao moi
    //type = false: luu
    if (!this.createLeadForm.valid) {
      Object.keys(this.createLeadForm.controls).forEach(key => {
        if (!this.createLeadForm.controls[key].valid) {
          this.createLeadForm.controls[key].markAsTouched();
        }
      });
      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
      this.isInvalidForm = true;
      this.isOpenNotifiError = true;
    } else {
      let leadModel: LeadModel = this.mapFormToLeadModel();
      let leadContactModel: ContactModel = this.mapFormToLeadContactModel();
      let listInterestedId: Array<string> = this.getListInterestedId();
      let listContact: Array<ContactModel> = this.getListContactFromTable();
      this.loading = true;
      this.awaitResult = true;
      let result: any = await this.leadService.createLeadAsync(leadModel, leadContactModel, false, '', listInterestedId, listContact, this.listLeadDetail);

      if (result.statusCode === 200) {
        //create new note
        let note: NoteModel = new NoteModel();
        note.Type = 'NEW';
        if (result.picName) {
          note.Description = 'Mức độ tiềm năng - <b>' + result.potential + '</b>, trạng thái - <b>' + result.statusName + '</b>, người phụ trách - <b>' + result.picName;
        } else {
          note.Description = 'Mức độ tiềm năng - <b>' + result.potential + '</b>, trạng thái - <b>' + result.statusName + '</b>, chưa có người phụ trách';
        }
        this.noteService.createNote(note, result.leadId, null, this.auth.UserId).subscribe(response => {
          var _noteresult = <any>response;
          if (_noteresult.statusCode !== 200) {
            this.clearToast();
            this.showToast('error', 'Thông báo', _noteresult.messageCode);

            this.isInvalidForm = false;
            this.isOpenNotifiError = false;
          }
        });

        if (type === false) {
          this.showToast('success', 'Thông báo', 'Tạo Cơ hội thành công');
          setTimeout(() => {
            this.router.navigate(['/lead/detail', { 'leadId': result.leadId }]);
          }, 2000);
        } else {
          this.resetForm();
          this.resetDetailTable();
          this.resetContactTalbe();
          this.clearToast();
          this.showToast('success', 'Thông báo', 'Tạo Cơ hội thành công');
          this.awaitResult = false;
        }
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
      this.loading = false;
    }
  }

  // isTypeCustomer: boolean = false;
  changeTypeLead(event: any) {
    if (event.value === null) {
      this.listCurrentCustomer = this.listCustomer;
      return false;
    };
    let currentType: leadTypeModel = event.value;
    switch (currentType.categoryCode) {
      case "KPL":
        //khach hang ca nhan
        this.listCurrentCustomer = this.listCustomer.filter(e => e.customerType === 2);
        this.createLeadForm.get('RefCustomer').reset();
        // this.isTypeCustomer = true;
        break;
      case "KCL":
        //khach hang doanh nghiep
        this.listCurrentCustomer = this.listCustomer.filter(e => e.customerType === 1);
        this.createLeadForm.get('RefCustomer').reset();
        // this.isTypeCustomer = true;
        break;
      default:
        break;
    }
  }

  changeCustomer(event: any) {
    //Nếu bỏ chọn Khách hàng
    if (event == null) {
      this.createLeadForm.controls['Email'].setValue(null);
      this.createLeadForm.controls['Phone'].setValue(null);
      this.createLeadForm.controls['DetailAddress'].setValue(null);
      this.getListPersonInChange(this.emptyGuid, this.auth.UserId);
      // this.listPersonalInChangeCus = [];
      return false;
    }
    if (event.customerType == 1) {
      this.createLeadForm.controls['Email'].setValue(event.workEmail ? event.workEmail : null);
    } else if (event.customerType == 2) {
      this.createLeadForm.controls['Email'].setValue(event.email ? event.email : null);
    }
    this.createLeadForm.controls['Phone'].setValue(event.phone);
    let address = event.address + ' ' + event.addressWard;
    this.createLeadForm.controls['DetailAddress'].setValue(address);

    //nếu khách hàng không có người phụ trách thì kết quả = có người phụ trách là nhân viên đang đăng nhập
    this.getListPersonInChange(event.personInChargeId, this.auth.UserId)
  }

  getListPersonInChange(empId: any, userId: any) {
    let personInChargeId = empId;
    if (personInChargeId) {
      this.leadService.getEmployeeByPersonInCharge(personInChargeId, userId).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.listPersonalInChangeCus = result.listEmployee || [];
          let emp = this.listPersonalInChangeCus.find(e => e.employeeId == personInChargeId);
          this.createLeadForm.controls['Pic'].setValue(emp);
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
    else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Khách hàng này không có người phụ trách' };
      this.showMessage(msg);
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  actionDelete = true;
  /*Thêm sản phẩm dịch vụ*/
  addCustomerOrderDetail() {
    let customerGroup = this.createLeadForm.get('Group').value;
    let customerGroupId = null;
    if (customerGroup) {
      customerGroupId = customerGroup.categoryId;
    }

    let ref = this.dialogService.open(LeadDetailDialogComponent, {
      data: {
        isCreate: true,
        customerGroupId: customerGroupId,
        dateOrder: convertToUTCTime(new Date())
      },
      header: 'Thêm sản phẩm dịch vụ',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultDetailDialog) => {
      if (result) {
        if (result.status) {
          this.leadDetail = result.leadDetailModel;

          //set orderNumber cho sản phẩm/dịch vụ mới thêm
          this.leadDetail.OrderNumber = this.listLeadDetail.length + 1;

          this.listLeadDetail.push(this.leadDetail);

          // this.listLeadDetailModelOrderBy = [];
          // for (let i = this.listLeadDetail.length - 1; i >= 0; i--) {
          //   this.listLeadDetailModelOrderBy.push(this.listLeadDetail[i]);
          // }
          // this.listLeadDetail = [];
          // this.listLeadDetail = this.listLeadDetailModelOrderBy;
          // this.leadDetail = new LeadDetailModel;
        }
      }
    });
  }

  /*Xóa một sản phẩm dịch vụ*/
  deleteItem(dataRow) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listLeadDetail = this.listLeadDetail.filter(e => e != dataRow)
        //Đánh lại số OrderNumber
        this.listLeadDetail.forEach((item, index) => {
          item.OrderNumber = index + 1;
        });
      }
    });
  }

  deleteContact(dataRow) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listContact = this.listContact.filter(e => e != dataRow)
      }
    });
  }

  /*Sửa một sản phẩm dịch vụ*/
  onRowSelect(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
    if (this.actionEdit) {
      var index = this.listLeadDetail.indexOf(dataRow);
      var OldArray = this.listLeadDetail[index];

      let titlePopup = '';
      if (OldArray.OrderDetailType == 0) {
        titlePopup = 'Sửa sản phẩm dịch vụ';
      } else if (OldArray.OrderDetailType == 1) {
        titlePopup = 'Sửa chi phí phát sinh';
      }

      let customerGroup = this.createLeadForm.get('Group').value;
      let customerGroupId = null;
      if (customerGroup) {
        customerGroupId = customerGroup.categoryId;
      }

      let ref = this.dialogService.open(LeadDetailDialogComponent, {
        data: {
          isCreate: false,
          leadDetailModel: OldArray,
          customerGroupId: customerGroupId,
          dateOrder: convertToUTCTime(new Date())
        },
        header: titlePopup,
        width: '70%',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "280px",
          "max-height": "600px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: ResultDetailDialog) => {
        if (result) {
          if (result.status) {
            this.listLeadDetail[index] = result.leadDetailModel;
            //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
            this.listLeadDetail.sort((a, b) =>
              (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));
          }
        }
      });
    }
  }

  addLeadContact() {
    let leadContactConfigData = new leadContactConfigDataModel();
    leadContactConfigData.isEdit = false; //tao moi lien he
    leadContactConfigData.contact = null;

    let ref = this.dialogService.open(CreatContactLeadDialogComponent, {
      data: {
        leadContactConfigData: leadContactConfigData
      },
      header: 'Thêm liên hệ',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px"
      }
    });

    ref.onClose.subscribe((result: resultContactDialog) => {
      if (result) {
        if (result.status === true) {
          this.listContact = [result.contact, ...this.listContact];
        }
      }
    });
  }

  editContact(rowData: contactModel) {
    let configData = new leadContactConfigDataModel();
    configData.isEdit = true;
    configData.contact = rowData;

    let ref = this.dialogService.open(CreatContactLeadDialogComponent, {
      data: {
        leadContactConfigData: configData
      },
      header: 'Thêm liên hệ',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px"
      }
    });

    ref.onClose.subscribe((result: resultContactDialog) => {
      if (result) {
        if (result.status === true) {
          //update
          let editItemIndex = this.listContact.findIndex(e => e === rowData);
          this.listContact[editItemIndex] = result.contact;
        }
      }
    });
  }

  /* Chuyển item lên một cấp */
  moveUp(data: LeadDetailModel) {
    let currentOrderNumber = data.OrderNumber;
    let preOrderNumber = currentOrderNumber - 1;
    let pre_data = this.listLeadDetail.find(x => x.OrderNumber == preOrderNumber);

    //Đổi số OrderNumber của 2 item
    pre_data.OrderNumber = currentOrderNumber;
    data.OrderNumber = preOrderNumber;

    //Xóa 2 item
    this.listLeadDetail = this.listLeadDetail.filter(x =>
      x.OrderNumber != preOrderNumber && x.OrderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listLeadDetail = [...this.listLeadDetail, pre_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listLeadDetail.sort((a, b) =>
      (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));
  }

  /* Chuyển item xuống một cấp */
  moveDown(data: LeadDetailModel) {
    let currentOrderNumber = data.OrderNumber;
    let nextOrderNumber = currentOrderNumber + 1;
    let next_data = this.listLeadDetail.find(x => x.OrderNumber == nextOrderNumber);

    //Đổi số OrderNumber của 2 item
    next_data.OrderNumber = currentOrderNumber;
    data.OrderNumber = nextOrderNumber;

    //Xóa 2 item
    this.listLeadDetail = this.listLeadDetail.filter(x =>
      x.OrderNumber != nextOrderNumber && x.OrderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listLeadDetail = [...this.listLeadDetail, next_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listLeadDetail.sort((a, b) =>
      (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));
  }


  openAddPotentialCusDialog() {
    let ref = this.dialogService.open(AddProtentialCustomerDialogComponent, {
      data: {
        isCreate: true,
        orderDate: new Date()
      },
      header: 'Thêm khách hàng tiềm năng',
      width: '60%',
      baseZIndex: 1030,
      closable: false,
      contentStyle: {
        "min-height": "300px",
        "max-height": "700px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultDialog) => {
      if (result) {
        if (result.status) {
          let customer: leadReferenceCustomer = result.customer;
          this.listCurrentCustomer = [customer, ...this.listCurrentCustomer];
          switch (customer.customerType) {
            case 1:
              this.createLeadForm.get('LeadType').setValue(this.listLeadType.find(x => x.categoryCode == 'KCL'));
              break;
            case 2:
              this.createLeadForm.get('LeadType').setValue(this.listLeadType.find(x => x.categoryCode == 'KPL'));
              break;
          }
          this.createLeadForm.get('RefCustomer').setValue(this.listCurrentCustomer.find(x => x.customerCode == customer.customerCode));
          this.createLeadForm.get('Email').setValue(customer.email);
          this.createLeadForm.get('Phone').setValue(customer.phone);
          this.createLeadForm.get('Pic').setValue(this.listPersonalInChangeCus.find(x => x.employeeId == customer.personInChargeId));
          this.createLeadForm.get('DetailAddress').setValue(customer.address);
          this.createLeadForm.get('InvestFund').setValue(this.listInvestFund.find(x => x.categoryId == customer.investmentFundId));
          this.createLeadForm.get('Area').setValue(this.listArea.find(x => x.geographicalAreaId == customer.areaId));
          this.createLeadForm.get('Group').setValue(this.listLeadGroup.find(x => x.categoryId == customer.customerGroupId));

          this.listCurrentCustomer = [result.customer, ...this.listCurrentCustomer]
          this.showToast('success', 'Thông báo', 'Tạo khách hàng tiềm năng thành công');
        }
      }
    });
  }

  openAddCustomerDialog() {
    let ref = this.dialogService.open(AddCustomerDialogComponent, {
      data: {
        isCreate: true,
        orderDate: new Date()
      },
      header: 'Thêm khách hàng',
      width: '60%',
      baseZIndex: 1030,
      closeOnEscape: false,
      closable: false,
      contentStyle: {
        "min-height": "300px",
        "max-height": "700px",
        "overflow": "auto !important",
      }
    });

    ref.onClose.subscribe((result: ResultDialog) => {
      if (result) {
        if (result.status) {
          let customer: leadReferenceCustomer = result.customer;
          this.listCurrentCustomer = [customer, ...this.listCurrentCustomer];
          switch (customer.customerType) {
            case 1:
              this.createLeadForm.get('LeadType').setValue(this.listLeadType.find(x => x.categoryCode == 'KCL'));
              break;
            case 2:
              this.createLeadForm.get('LeadType').setValue(this.listLeadType.find(x => x.categoryCode == 'KPL'));
              break;
          }
          this.createLeadForm.get('RefCustomer').setValue(this.listCurrentCustomer.find(x => x.customerCode == customer.customerCode));
          this.createLeadForm.get('Email').setValue(customer.email);
          this.createLeadForm.get('Phone').setValue(customer.phone);
          this.createLeadForm.get('Pic').setValue(this.listPersonalInChangeCus.find(x => x.employeeId == customer.personInChargeId));
          this.createLeadForm.get('DetailAddress').setValue(customer.address);
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo khách hàng thành công' };
          this.showMessage(msg);
        }
      }
    });
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function checkCreateCompanyValidation(checkValue: boolean): ValidatorFn {
  return (): { [key: string]: boolean } => {
    if (checkValue) {
      return { 'checkCreateCompanyValidation': true };
    }
    return null;
  }
}

function checkDuplicateEmailLead(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() !== "") {
        let duplicateEmailLead = array.find(e => e === control.value.trim());
        if (duplicateEmailLead !== undefined) {
          return { 'duplicateEmailLead': true };
        }
      }
    }
    return null;
  }
}

function checkDuplicatePhonelLead(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() !== "") {
        let duplicatePhoneLead = array.find(e => e === control.value.trim());
        if (duplicatePhoneLead !== undefined) {
          return { 'duplicatePhoneLead': true };
        }
      }
    }
    return null;
  }
}

function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
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

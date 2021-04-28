import { Time } from '@angular/common';
import { Component, ElementRef, OnInit, Renderer2 } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationService, DynamicDialogConfig, DynamicDialogRef, MessageService } from 'primeng';
import { GetPermission } from '../../../shared/permission/get-permission';
import { CustomerService } from '../../services/customer.service';
import { LeadModel } from "../../../lead/models/lead.model";
import { ContactModel, contactModel } from "../../../shared/models/contact.model";
import * as $ from 'jquery';

class personalInChange {
  employeeCode: string;
  employeeId: string;
  employeeName: string;
}

class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefault: boolean;
}

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
}

class customerResponseModel {
  customerId: string;
  customerCode: string;
  customerGroupId: string;
  customerName: string;
  leadId: string;
  statusId: string;
  customerServiceLevelId: string;
  customerServiceLevelName: string;
  personInChargeId: string;
  customerCareStaff: string;
  customerType: number;
  paymentId: string;
  fieldId: string;
  scaleId: string;
  totalSaleValue: number;
  totalReceivable: number;
  nearestDateTransaction: Date;
  maximumDebtValue: number;
  maximumDebtDays: number;
  mainBusinessSector: string;
  businessRegistrationDate: Date;
  enterpriseType: string;
  businessType: string;
  isGraduated: boolean;
  totalEmployeeParticipateSocialInsurance: number;
  businessScale: string;
  totalCapital: number;
  totalRevenueLastYear: number;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: string;
  active: boolean;
  isApproval: boolean;
  approvalStep: number;
  investmentFundId: string;
  allowSendEmail: boolean;
  allowCall: boolean;
  isConverted: boolean;
  careStateId: string;
  employeeTakeCareId: string;
  contactDate: Date;
  salesUpdate: string;
  evaluateCompany: string;
  salesUpdateAfterMeeting: string;

  constructor() {
    this.customerName = '';
    this.isConverted = true;
  }
}


class contactResponseModel {
  contactId: string;
  objectId: string;
  objectType: string;
  firstName: string;
  lastName: string;
  contactName: string; //full name
  gender: string;
  genderDisplay: string;
  dateOfBirth: Date;
  phone: string;
  workPhone: string;
  otherPhone: string;
  email: string;
  workEmail: string;
  otherEmail: string;
  identityID: string;
  avatarUrl: string;
  address: string;
  countryId: string;
  provinceId: string;
  districtId: string;
  wardId: string;
  maritalStatusId: string;
  postCode: string;
  websiteUrl: string;
  socialUrl: string;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  active: Boolean;
  note: string;
  role: string;
  taxCode: string;
  job: string;
  agency: string;
  birthplace: string;
  identityIDDateOfIssue: Date;
  identityIDPlaceOfIssue: string;
  identityIDDateOfParticipation: Date;
  workPermitNumber: string;
  visaNumber: string;
  visaDateOfIssue: Date;
  visaExpirationDate: Date;
  socialInsuranceNumber: string;
  socialInsuranceDateOfIssue: Date;
  socialInsuranceDateOfParticipation: Date;
  healthInsuranceNumber: string;
  healthInsuranceDateOfIssue: Date;
  healthInsuranceDateOfParticipation: Date;
  workHourOfStart: Time;
  workHourOfEnd: Time;
  typePaid: string;
  other: string;
  companyName: string;
  companyAddress: string;
  customerPosition: string;
  bankName: string;
  bankCode: string;
  moneyLimit: number;
  termsPayment: string;
  defaultAccount: string;
  optionPosition: string;
  relationShip: string;
  potentialCustomerPosition: string;
  latitude: number;
  longitude: number;
  geographicalAreaId: string;
  linkFace: string;

  constructor() {
    this.contactName = '';
    this.role = '';
    this.phone = '';
    this.workPhone = '';
    this.email = '';
    this.workEmail = '';
    this.address = '';
  }
}


@Component({
  selector: 'app-potential-conversion-dialog',
  templateUrl: './potential-conversion-dialog.component.html',
  styleUrls: ['./potential-conversion-dialog.component.css']
})
export class PotentialConversionDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  converdialog_isCreatelead: boolean = false;
  convertToCustomerForm: FormGroup;
  converdialog_isCreateCustomer: boolean = true;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;

  customerId: string = this.emptyGuid;

  /*Biến lưu giá trị trả về*/
  listPersonalInChange: Array<personalInChange> = [];
  potentialCustomerModel: customerResponseModel = new customerResponseModel();
  potentialCustomerContactModel: contactResponseModel;
  /*end*/

  constructor(
    public messageService: MessageService,
    private getPermission: GetPermission,
    private translate: TranslateService,
    private customerService: CustomerService,
    private confirmationService: ConfirmationService,
    private route: ActivatedRoute,
    private router: Router,
    public builder: FormBuilder,
    private renderer: Renderer2,
    private el: ElementRef,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,

  ) { }

  ngOnInit(): void {
    this.customerId = this.config.data.customerId;
    this.initForm();
    this.getMasterData();
  }

  initForm() {
    /* form chuyển đổi */
    this.convertToCustomerForm = new FormGroup({
      'Pic': new FormControl(null, [Validators.required]), //người phụ trách
      'IsCreatedLead': new FormControl(true),  //Có tạo cơ hội đi kèm không? true: có, false: không
      'LeadName': new FormControl(null, [Validators.required])  //Tên cơ hội
    });
  }

  async getMasterData(){
    this.loading = true;
    let [result]: any = await Promise.all([
      this.customerService.getDataDetailPotentialCustomer(this.customerId, this.auth.UserId),
    ]);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listPersonalInChange = result.listPersonalInChange || [];
      this.potentialCustomerContactModel = result.potentialCustomerContactModel;
      this.potentialCustomerModel = result.potentialCustomerModel;
    }
  }

  async save() {
    let isCreateCustomer: boolean = false;
    let isCreateLead: boolean = false;
    if (this.converdialog_isCreateCustomer) {
      //tạo khách hàng
      isCreateCustomer = true;
    }
    // if (this.converdialog_isCreatelead) {
    //   //tạo cơ hội
    //   isCreateLead = true;
    // }

    //nếu chuyển thành khách hàng thì validation form tạo khách hàng
    if (isCreateCustomer) {
      if (!this.convertToCustomerForm.valid) {
        console.log('false')
        Object.keys(this.convertToCustomerForm.controls).forEach(key => {
          if (!this.convertToCustomerForm.controls[key].valid) {
            this.convertToCustomerForm.controls[key].markAsTouched();
          }
        });
        let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
        if (target) {
          $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
          target.focus();
        }
        return;
      }
    }

    //map form tao khach hang
    let pic: personalInChange = this.convertToCustomerForm.get('Pic').value;
    let picId = pic ? pic.employeeId : null;
    let LeadName = this.convertToCustomerForm.get('LeadName').value;

    this.loading = true;
    let result: any = await this.customerService.convertPotentialCustomer(this.customerId, isCreateCustomer,
      isCreateLead, picId, LeadName);
    this.loading = false;
    if (result.statusCode === 200) {
      let res: ResultDialog = {
        status: true,
      }
      this.ref.close(res);
    } else {
      this.showToast('error', 'Thông báo', result.messageCode);
    }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ key: 'popup' ,severity: severity, summary: summary, detail: detail });
  }

  hangeCreateCustomerCheckBox(event: any) {
  }

  changeCreateCustomerCheckBox(event: any) {

  }
  changeCreateLeadDialog(event: any) {
    let value: boolean = event.checked;

    if (value)
    {
      this.convertToCustomerForm.get('LeadName').setValidators([Validators.required]);
      this.convertToCustomerForm.get('LeadName').updateValueAndValidity();
    }
    else
    {
      this.convertToCustomerForm.get('LeadName').setValidators(null);
      this.convertToCustomerForm.get('LeadName').updateValueAndValidity();
    }
  }

  cancel(){
    let res: ResultDialog = {
      status: false,
    }
    this.ref.close(res);
  }
}

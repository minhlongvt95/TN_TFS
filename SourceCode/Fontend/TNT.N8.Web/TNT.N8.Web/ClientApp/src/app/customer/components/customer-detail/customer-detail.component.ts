import { Component, OnInit, ElementRef, ViewChild, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import * as $ from 'jquery';
import { TranslateService } from '@ngx-translate/core';
import { CategoryService } from "../../../shared/services/category.service";
import { CustomerService } from "../../services/customer.service";
import { ContactService } from "../../../shared/services/contact.service";
import { WardService } from '../../../shared/services/ward.service';
import { ProvinceService } from '../../../shared/services/province.service';
import { DistrictService } from '../../../shared/services/district.service';
import { ImageUploadService } from '../../../shared/services/imageupload.service';

import { CustomerModel } from "../../models/customer.model";
import { ContactModel } from "../../../shared/models/contact.model";
import { NoteModel } from '../../../shared/models/note.model';
import { NoteService } from '../../../shared/services/note.service';
import { BankService } from '../../../shared/services/bank.service';
import { EmployeeService } from "../../../employee/services/employee.service";
import { CustomerCareService } from '../../services/customer-care.service';
import { SendSmsDialogComponent } from '../../../shared/components/send-sms-dialog/send-sms-dialog.component';
import { SendEmailDialogComponent } from '../../../shared/components/send-email-dialog/send-email-dialog.component';
import { ContactpopupComponent } from '../../../shared/components/contactpopup/contactpopup.component';
import { BankpopupComponent } from '../../../shared/components/bankpopup/bankpopup.component';
import { GetPermission } from '../../../shared/permission/get-permission';
import { AddQuestionAnswerDialogComponent } from '../add-question-answer-dialog/add-question-answer-dialog.component';
import { TemplateQuickEmailComponent } from '../template-quick-email/template-quick-email.component';
import { TemplateQuickSmsComponent } from '../template-quick-sms/template-quick-sms.component';
import { TemplateQuickGiftComponent } from '../template-quick-gift/template-quick-gift.component';
import { MeetingDialogComponent } from '../../components/meeting-dialog/meeting-dialog.component';

import { MessageService } from 'primeng/api';
import { MenuItem } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { ConfirmationService } from 'primeng/api';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { SendEmailModel } from '../../../admin/models/sendEmail.model';
import { DialogService } from 'primeng/dynamicdialog';
import { EmailConfigService } from '../../../admin/services/email-config.service';
import { GoogleService } from '../../../shared/services/google.service';

import { WarningComponent } from '../../../shared/toast/warning/warning.component';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

declare var google: any;

interface Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefault: boolean;
}

interface Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
}

interface Area {
  areaId: string;
  areaName: string;
}

interface Province {
  provinceId: string;
  provinceName: string;
}

interface District {
  districtId: string;
  districtName: string;
  provinceId: string;
}

interface Ward {
  wardId: string;
  wardName: string;
  districtId: string;
}

interface Note {
  active: boolean;
  createdById: string;
  createdDate: Date;
  description: string;
  noteDocList: Array<NoteDocument>;
  noteId: string;
  noteTitle: string;
  objectId: string;
  objectType: string;
  responsibleAvatar: string;
  responsibleName: string;
  type: string;
  updatedById: string;
  updatedDate: Date;
}

interface NoteDocument {
  active: boolean;
  base64Url: string;
  createdById: string;
  createdDate: Date;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  noteDocumentId: string;
  noteId: string;
  updatedById: string;
  updatedDate: Date;
}

interface Country {
  countryId: string;
  countryName: string;
  countryCode: string;
}

interface CustomerAdditionalInformation {
  customerAdditionalInformationId: string,
  question: string,
  answer: string,
  customerId: string
}

interface OtherContact {
  contactId: string,
  objectId: string,
  objectType: string,
  firstName: string,
  lastName: string,
  contactName: string,
  gender: string,
  genderName: string,
  dateOfBirth: Date,
  address: string,
  role: string, //Chức vụ
  phone: string,
  email: string,
  other: string,  //Thông tin khác
}

interface PersonalCustomerContact {
  ContactId: string,
  Email: string,
  WorkEmail: string,
  OtherEmail: string,
  Phone: string,
  WorkPhone: string,
  OtherPhone: string,
  AreaId: string,
  ProvinceId: string,
  DistrictId: string,
  WardId: string,
  Address: string,
  Other: string,
  Latitude: number,
  Longitude: number
}

interface OrderOfCustomer {
  orderId: string,
  orderCode: string,
  seller: string,
  sellerName: string,
  amount: string,
  statusId: string,
  statusName: string,
  createdDate: string,
  orderDate: string
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

interface CustomerCareForWeek {
  customerCareId: string,
  employeeCharge: string,
  title: string,
  type: number,
  feedBackStatus: number,
  background: string,
  subtitle: string,
  activeDate: Date
}

interface CustomerCareInfor {
  employeeName: string,
  employeePosition: string,
  employeeCharge: string,
  week1: Array<CustomerCareForWeek>,
  week2: Array<CustomerCareForWeek>,
  week3: Array<CustomerCareForWeek>,
  week4: Array<CustomerCareForWeek>,
  week5: Array<CustomerCareForWeek>
}

interface PreviewCustomerCare {
  effecttiveFromDate: Date,
  effecttiveToDate: Date,
  sendDate: Date,
  statusName: string,
  previewEmailContent: string,
  previewEmailName: string,
  previewEmailTitle: string,
  previewSmsPhone: string,
  previewSmsContent: string
}

interface CustomerCareFeedBack {
  customerCareFeedBackId: string,
  feedBackFromDate: Date,
  feedBackToDate: Date,
  feedBackType: string,
  feedBackCode: string,
  feedBackContent: string,
  customerId: string,
  customerCareId: string,
}

interface DataDialogCustomerFeedBack {
  name: string,
  fromDate: Date,
  toDate: Date,
  typeName: string,
  feedBackCode: string,
  feedbackContent: string
}

interface CustomerMeetingInfor {
  employeeName: string,
  employeePosition: string,
  employeeId: string,
  week1: Array<CustomerMeetingForWeek>,
  week2: Array<CustomerMeetingForWeek>,
  week3: Array<CustomerMeetingForWeek>,
  week4: Array<CustomerMeetingForWeek>,
  week5: Array<CustomerMeetingForWeek>
}

interface CustomerMeetingForWeek {
  customerMeetingId: string,
  employeeId: string,
  title: string,
  subTitle: string,
  background: string,
  startDate: Date,
  startHours: Date
}

interface DateInMonth {
  startDateWeek1: Date,
  endDateWeek1: Date,
  startDateWeek2: Date,
  endDateWeek2: Date,
  startDateWeek3: Date,
  endDateWeek3: Date,
  startDateWeek4: Date,
  endDateWeek4: Date,
  startDateWeek5: Date,
  endDateWeek5: Date,
}

@Component({
  selector: 'app-customer-detail',
  templateUrl: './customer-detail.component.html',
  styleUrls: ['./customer-detail.component.css'],
  providers: [GoogleService]
})
export class CustomerDetailComponent implements OnInit {
  /*Khai báo biến*/
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  auth: any = JSON.parse(localStorage.getItem("auth"));

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionSMS: boolean = true;
  actionEmail: boolean = true;
  actionImport: boolean = true;
  /*END*/

  defaultNumberType = this.getDefaultNumberType();
  loading: boolean = false;
  listAction: MenuItem[];
  listCustomerGroup: Array<Category> = [];
  listCustomerStatus: Array<Category> = [];
  listPersonInCharge: Array<Employee> = [];
  listParticipants: Array<Employee> = [];
  customerTypeString: string = '';
  listStatusCustomerCare: Array<Category> = []; // List trạng thái chăm sóc khách hàng
  listBusinessSize: Array<Category> = []; //List Quy mô doanh nghiệp
  listBusinessType: Array<Category> = []; //List Lĩnh vực kinh doanh
  listPaymentMethod: Array<Category> = []; //List Phương thức thanh toán
  listTypeOfBusiness: Array<Category> = []; //list Loại hình doanh nghiệp
  listCareStaff: Array<Employee> = []; //List nhân viên chăm sóc khách hàng
  listBusinessCareer: Array<Category> = []; //List Ngành nghề kinh doanh chính
  listLocalTypeBusiness: Array<Category> = []; //List Loại doanh nghiệp
  listCustomerPosition: Array<Category> = []; //List Chức vụ của Khách hàng
  listArea: Array<Area> = [];
  listProvince: Array<Province> = [];
  listDistrict: Array<District> = [];
  listCurrentDistrict: Array<District> = [];
  listWard: Array<Ward> = [];
  listCurrentWard: Array<Ward> = [];
  noteContent: string = '';
  customerNameLabel: string = '';
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  noteDocumentModel: NoteDocumentModel = new NoteDocumentModel();
  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  noteHistory: Array<Note> = [];
  listGenders = [{ categoryCode: 'NAM', categoryName: 'Nam' }, { categoryCode: 'NU', categoryName: 'Nữ' }];
  listCountry: Array<Country> = [];
  listMaritalStatus: Array<Category> = [];
  customerId: string = '';
  noteId: string = null;
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  selectedCustomerList: Array<any> = [];
  countCustomer: number;
  defaultTab: number = 0;
  customerModel: CustomerModel = new CustomerModel();
  contactModel: ContactModel = new ContactModel();
  currentYear: number = (new Date()).getFullYear();
  listCustomerAdditionalInformation: Array<CustomerAdditionalInformation> = [];
  listSelectedCusAddInf: Array<CustomerAdditionalInformation> = [];
  listCusContact: Array<OtherContact> = []; //Danh sách người liên hệ của khách hàng doanh nghiệp
  listOrderOfCustomer: Array<OrderOfCustomer> = []; //Danh sách đơn hàng của khách hàng
  listBankAccount: Array<BankAccount> = [];
  listCustomerCareInfor: Array<CustomerCareInfor> = [];
  month: string = '';
  year: number = (new Date()).getFullYear();
  startDateWeek1: Date = null;
  endDateWeek1: Date = null;
  startDateWeek2: Date = null;
  endDateWeek2: Date = null;
  startDateWeek3: Date = null;
  endDateWeek3: Date = null;
  startDateWeek4: Date = null;
  endDateWeek4: Date = null;
  startDateWeek5: Date = null;
  endDateWeek5: Date = null;
  previewEmail: boolean = false;
  previewSMS: boolean = false;
  previewCustomerCare: PreviewCustomerCare = {
    effecttiveFromDate: new Date(),
    effecttiveToDate: new Date(),
    sendDate: new Date(),
    statusName: '',
    previewEmailName: '',
    previewEmailTitle: '',
    previewEmailContent: '',
    previewSmsContent: '',
    previewSmsPhone: '',
  }
  feedback: boolean = false;
  dataDialogCustomerFeedBack: DataDialogCustomerFeedBack = {
    name: '',
    fromDate: new Date(),
    toDate: new Date(),
    typeName: '',
    feedBackCode: '',
    feedbackContent: ''
  }
  listFeedBackCode: Array<Category> = [];
  customerCareFeedBack: CustomerCareFeedBack = {
    customerCareFeedBackId: this.emptyGuid,
    feedBackFromDate: new Date(),
    feedBackToDate: new Date(),
    feedBackType: '',
    feedBackCode: '',
    feedBackContent: '',
    customerId: '',
    customerCareId: '',
  }
  monthMeeting: string = '';
  yearMeeting: number = (new Date()).getFullYear();
  startDateWeek1Meeting: Date = null;
  endDateWeek1Meeting: Date = null;
  startDateWeek2Meeting: Date = null;
  endDateWeek2Meeting: Date = null;
  startDateWeek3Meeting: Date = null;
  endDateWeek3Meeting: Date = null;
  startDateWeek4Meeting: Date = null;
  endDateWeek4Meeting: Date = null;
  startDateWeek5Meeting: Date = null;
  endDateWeek5Meeting: Date = null;
  customerCodeOld: string = "";
  customerMeetingInfor: CustomerMeetingInfor = {
    employeeId: '',
    employeeName: '',
    employeePosition: '',
    week1: [],
    week2: [],
    week3: [],
    week4: [],
    week5: []
  }
  isExist: boolean = true;
  isApprovalWain: boolean = false;
  isSendApproval: boolean = false;
  isApprovalNew: boolean = false;
  isApprovalDD: boolean = false;
  listCustomerCode: any = []
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;

  @ViewChild('fileUpload') fileUpload: FileUpload;

  /*Form thông tin cơ bản*/
  customerBaseForm: FormGroup;

  cusFirstNameControl: FormControl;
  cusCodeControl: FormControl;
  cusLastNameControl: FormControl;
  cusGroupControl: FormControl;
  cusStatusControl: FormControl;
  cusStatusCareControl: FormControl;
  cusPersonInChargeControl: FormControl;
  /*End*/


  /*Form thông tin chung KH Doanh nghiệp*/
  customerInforForm: FormGroup;

  cusPhoneControl: FormControl;
  cusBusinessSizeControl: FormControl;
  cusBusinessTypeControl: FormControl;
  cusTaxControl: FormControl;
  cusEmailControl: FormControl;
  cusPaymentMethodControl: FormControl;
  cusMaxDebtDayControl: FormControl;
  cusMaxDebtValueControl: FormControl;
  cusLinkControl: FormControl;
  cusTypeOfBusinessControl: FormControl;
  cusTotalEmpSocialInsuranceControl: FormControl;
  cusCareStaffControl: FormControl;
  cusBusinessRegistrationDateControl: FormControl;
  cusTotalCapitalControl: FormControl;
  cusTotalRevenueLastYearControl: FormControl;
  cusMainBusinessSectorControl: FormControl;
  cusLocalTypeBusinessControl: FormControl;
  cusAreaControl: FormControl;
  cusProvinceControl: FormControl;
  cusDistrictControl: FormControl;
  cusWardControl: FormControl;
  cusAddressControl: FormControl;
  /*End*/

  /*Form thông tin chung KH Cá nhân*/
  customerPersonalInforForm: FormGroup;

  cusPerGenderControl: FormControl;
  cusPerCountryControl: FormControl;
  cusPerTaxcodeControl: FormControl;
  cusPerBirthdayControl: FormControl;
  cusPerIdentityControl: FormControl;
  cusPerPaymentMethodControl: FormControl;
  cusPerBirthplaceControl: FormControl;
  cusPerMaritalStatusControl: FormControl;
  cusPerMaxDebtDayControl: FormControl;
  cusPerMaxDebtValueControl: FormControl;
  cusPerJobControl: FormControl;
  cusPerAgencyControl: FormControl;
  cusPerCareStaffControl: FormControl;
  cusPerCompanyNameControl: FormControl;
  cusPerCompanyAddressControl: FormControl;
  cusPerPositionControl: FormControl;
  /*End*/

  /*Form thông tin liên hệ KH Cá nhân*/
  customerPersonalContactForm: FormGroup;

  cusPerContPhoneControl: FormControl;
  cusPerContWorkPhoneControl: FormControl;
  cusPerContOtherPhoneControl: FormControl;
  cusPerContEmailControl: FormControl;
  cusPerContWorkEmailControl: FormControl;
  cusPerContOtherEmailControl: FormControl;
  cusPerContAreaControl: FormControl;
  cusPerContProvinceControl: FormControl;
  cusPerContDistrictControl: FormControl;
  cusPerContWardControl: FormControl;
  cusPerContAddressControl: FormControl;
  cusPerContOtherControl: FormControl;
  /*End*/

  /*Form thông tin liên hệ KH Cá nhân*/
  feedbackForm: FormGroup;

  feedbackCodeControl: FormControl;
  feedbackContentControl: FormControl;
  /*End*/

  /*Table*/
  colsFile: any;
  colsCustomerAdditionalInformation: any;
  colsCusContact: any;
  colsCusLead: any;
  colsCustomerOrderHistory: any;
  colsBankAccount: any;
  /*End*/

  /* create valiable add class scroll */
  fixed: boolean = false;
  withFiexd: string = "";

  //map
  options: any = {};
  overlays: any[] = [];
  map: google.maps.Map;
  zoom: number = 16;

  point: number = 0; //điểm tích lũy
  payPoint: number = 0; //điểm đã thanh toán

  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };

  validEmailCustomer: boolean = true;

  validPhoneCustomer: boolean = true;

  listCustomerLead: Array<any> = [];

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private customerService: CustomerService,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    private wardService: WardService,
    private districtService: DistrictService,
    private provinceService: ProvinceService,
    private employeeService: EmployeeService,
    private customerCareService: CustomerCareService,
    private contactService: ContactService,
    private bankService: BankService,
    private router: Router,
    private fb: FormBuilder,
    private imageService: ImageUploadService,
    private noteService: NoteService,
    private el: ElementRef,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private emailConfigService: EmailConfigService,
    private googleService: GoogleService,
    private dialogService: DialogService,
    private snackBar: MatSnackBar) {
    this.translate.setDefaultLang('vi');
  }

  ngOnInit() {
    this.setForm();
    //Check permission
    let resource = "crm/customer/detail";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
      this.snackBar.openFromComponent(WarningComponent, { data: 'Bạn không có quyền truy cập', ... this.warningConfig });
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      if (listCurrentActionResource.indexOf("sms") == -1) {
        this.actionSMS = false;
      }
      if (listCurrentActionResource.indexOf("email") == -1) {
        this.actionEmail = false;
      }
      if (listCurrentActionResource.indexOf("import") == -1) {
        this.actionImport = false;
      }

      this.route.params.subscribe(params => {
        this.customerId = params['customerId'];
        this.defaultTab = params['defaultTab'] != undefined ? params['defaultTab'] : 0;
      });

      this.getMasterData();
    }
  }

  setMap(event) {
    this.map = event.map;
    this.patchGeoCodingForMap(this.contactModel.Latitude, this.contactModel.Longitude, this.customerNameLabel);
  }


  getMasterData() {
    this.loading = true;
    this.customerService.getCustomerById(this.customerId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        // danh sách cơ hội
        this.listCustomerLead = result.listCustomerLead;

        //Nhóm khách hàng
        this.listCustomerGroup = result.listCustomerGroup;

        //Trạng thái khách hàng
        this.listCustomerStatus = result.listCustomerStatus;

        //Trạng thái chăm sóc khách hàng
        this.listStatusCustomerCare = result.listStatusCustomerCare;

        //List nhân viên phụ trách
        this.listPersonInCharge = result.listPersonInCharge;
        this.listPersonInCharge.forEach(item => {
          item.employeeName = item.employeeCode + ' - ' + item.employeeName;
        });

        this.listBusinessSize = result.listBusinessSize;
        this.listBusinessType = result.listBusinessType;
        this.listPaymentMethod = result.listPaymentMethod;
        this.listTypeOfBusiness = result.listTypeOfBusiness;
        this.listBusinessCareer = result.listBusinessCareer;
        this.listLocalTypeBusiness = result.listLocalTypeBusiness;
        this.listCustomerPosition = result.listCustomerPosition;
        this.listArea = result.listArea;
        this.listProvince = result.listProvince;
        this.listDistrict = result.listDistrict;
        this.listWard = result.listWard;
        this.listCountry = result.countryList;
        this.listMaritalStatus = result.listMaritalStatus;
        this.isSendApproval = result.isSendApproval;
        this.isApprovalNew = result.isApprovalNew;
        this.isApprovalDD = result.isApprovalDD;
        this.listParticipants = result.listParticipants;
        this.listParticipants.forEach(item => {
          item.employeeName = item.employeeCode + ' - ' + item.employeeName;
        });

        //Thông tin khác (Câu hỏi/Câu trả lời)
        this.listCustomerAdditionalInformation = result.listCustomerAdditionalInformation;

        //Danh sách người liên hệ (KHDN)
        this.listCusContact = result.listCusContact;

        //Danh sách đơn hàng của khách hàng
        this.listOrderOfCustomer = result.listOrderOfCustomer;

        //Danh sách thông tin thanh toán của khách hàng
        this.listBankAccount = result.listBankAccount;

        //Danh sách thông tin CSKH
        this.listCustomerCareInfor = result.listCustomerCareInfor;

        //Lịch hẹn
        this.customerMeetingInfor = result.customerMeetingInfor;

        this.listCareStaff = result.listCareStaff;
        this.listCareStaff.forEach(item => {
          item.employeeName = item.employeeCode + ' - ' + item.employeeName;
        });

        this.mapDataResponse(result.customer, result.contact);

        /*Reshow Time Line */
        this.noteHistory = result.listNote;
        this.handleNoteContent();

        // this.getListCustomerValidate();
        this.listCustomerCode = result.customerCode;
        this.customerBaseForm.get('cusCodeControl').setValidators([Validators.required, checkDuplicateCustomeCode(this.listCustomerCode, this.customerCodeOld)]);
        this.customerBaseForm.updateValueAndValidity();

      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  approveOrReject(isApprove) {
    let listSelectCus = [];
    listSelectCus.push(this.customerModel)
    let listIdCus: Array<string> = listSelectCus.map(e => e.CustomerId);
    if (isApprove !== null) {
      if (isApprove) {
        let count = listSelectCus.filter(f => f.PersonInChargeId.trim() == "");
        if (count.length != 0) {
          if (listSelectCus.length == 1) {
            this.showToast('warn', 'Thông báo', "Khách hàng " + listSelectCus[0].customerName + " chưa được gán người phụ trách, không thể chuyển định danh");
          }
          else {
            this.showToast('warn', 'Thông báo', "Danh sách có khách hàng chưa được gán người phụ trách, không thể chuyển định danh");
          }
        }
        else {
          this.confirmationService.confirm({
            message: 'Bạn có chắc chắn muốn định danh khách hàng này?',
            accept: () => {
              this.loading = true;
              this.customerService.approvalOrRejectCustomer(listIdCus, isApprove, true, this.auth.UserId).subscribe(response => {
                this.loading = false;
                let result = <any>response;
                if (result.statusCode === 202 || result.statusCode === 200) {
                  // this.listLead = this.listLead.filter(e => !leadListId.includes(e.leadId));
                  this.getMasterData();
                  this.showToast('success', 'Thông báo', result.messageCode);
                } else {
                  this.showToast('error', 'Thông báo', result.messageCode);
                }
              }, error => { this.loading = false; });
            }
          });
        }
      }
      else {
        this.confirmationService.confirm({
          message: 'Bạn có chắc chắn muốn từ chối danh sách này?',
          accept: () => {
            this.loading = true;
            this.customerService.approvalOrRejectCustomer(listIdCus, isApprove, true, this.auth.UserId).subscribe(response => {
              this.loading = false;
              let result = <any>response;
              if (result.statusCode === 202 || result.statusCode === 200) {
                this.getMasterData();
                // this.listLead = this.listLead.filter(e => !leadListId.includes(e.leadId));
                this.showToast('success', 'Thông báo', result.messageCode);
              } else {
                this.showToast('error', 'Thông báo', result.messageCode);
              }
            }, error => { this.loading = false; });
          }
        });
      }
    }
    else {
      this.confirmationService.confirm({
        message: 'Bạn có chắc chắn muốn phê duyệt danh sách này?',
        accept: () => {
          this.loading = true;
          this.customerService.approvalOrRejectCustomer(listIdCus, true, false, this.auth.UserId).subscribe(response => {
            this.loading = false;
            let result = <any>response;
            if (result.statusCode === 202 || result.statusCode === 200) {
              // this.listLead = this.listLead.filter(e => !leadListId.includes(e.leadId));
              this.getMasterData();
              this.showToast('success', 'Thông báo', result.messageCode);
            } else {
              this.showToast('error', 'Thông báo', result.messageCode);
            }
          }, error => { this.loading = false; });
        }
      });
    }
  }

  statusCustomer() {

  }

  // async getListCustomerValidate() {
  //   this.loading = true;
  //   let result: any = await this.customerService.createCustomerMasterDataAsync(this.employeeId);
  //   this.loading = false;
  //   if (result.statusCode === 200) {
  //     this.listCustomerCode = result.listCustomerCode;
  //     this.customerBaseForm.get('cusCodeControl').setValidators([Validators.required, checkDuplicateCustomeCode(this.listCustomerCode, this.customerCodeOld)]);
  //     this.customerBaseForm.updateValueAndValidity();
  //   }
  // }

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

  setForm() {
    this.listAction = [
      {
        label: 'Tạo cơ hội', icon: 'pi pi-plus-circle', command: () => {
          if (this.actionAdd) {
            this.goToLead();
          } else {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền Tạo đơn hàng' };
            this.showMessage(msg);
          }
        }
      },
      {
        label: 'Tạo đơn hàng', icon: 'pi pi-plus-circle', command: () => {
          if (this.actionAdd) {
            this.goToOrderCreate();
          } else {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền Tạo đơn hàng' };
            this.showMessage(msg);
          }
        }
      },
      {
        label: 'Tạo phiếu thu', icon: 'pi pi-plus-circle', command: () => {
          if (this.actionAdd) {
            this.goToCashReceiptsCreate();
          } else {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền Tạo phiếu thu' };
            this.showMessage(msg);
          }
        }
      },
      {
        label: 'Tạo báo có', icon: 'pi pi-plus-circle', command: () => {
          if (this.actionAdd) {
            this.goToBankReceiptsCreate();
          } else {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền Tạo báo có' };
            this.showMessage(msg);
          }
        }
      },
      {
        label: 'Tạo báo giá', icon: 'pi pi-plus-circle', command: () => {
          if (this.actionAdd) {
            if (this.cusPersonInChargeControl.value === null || this.cusPersonInChargeControl.value === undefined) {
              let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Khách hàng chưa được gán người phụ trách!' };
              this.showMessage(mgs);
            } else {
              this.goToQuoteCreate();
            }
          } else {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền Tạo báo giá' };
            this.showMessage(msg);
          }
        }
      },
      {
        label: 'Chi tiết công nợ', icon: 'pi pi-plus-circle', command: () => {
          this.goToReceivableCustomerDetail();
        }
      },
      {
        label: 'Gửi Email', icon: 'pi pi-plus', command: () => {
          if (this.actionEmail) {
            this.sendEmail();
          } else {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền Gửi Email' };
            this.showMessage(msg);
          }
        }
      },
      {
        label: 'Gửi SMS', icon: 'pi pi-plus', command: () => {
          if (this.actionSMS) {
            this.sendSMS();
          } else {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền Gửi SMS' };
            this.showMessage(msg);
          }
        }
      },
    ];

    /*Table*/
    this.colsFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'documentSize', header: 'Kích thước tài liệu', width: '50%', textAlign: 'left' },
    ];

    //Thông tin khác (Bộ Câu hỏi/Câu trả lời)
    this.colsCustomerAdditionalInformation = [
      { field: 'question', header: 'Câu hỏi', width: '45%', textAlign: 'left' },
      { field: 'answer', header: 'Câu trả lời', width: '45%', textAlign: 'left' },
    ];

    //Thông tin liên hệ
    this.colsCusContact = [
      { field: 'contactName', header: 'Họ và Tên', width: '15%', textAlign: 'left' },
      { field: 'genderName', header: 'Giới tính', width: '7%', textAlign: 'left' },
      { field: 'dateOfBirth', header: 'Ngày sinh', width: '7%', textAlign: 'left' },
      { field: 'role', header: 'Chức vụ', width: '15%', textAlign: 'left' },
      { field: 'phone', header: 'Số điện thoại', width: '10%', textAlign: 'left' },
      { field: 'email', header: 'Email', width: '15%', textAlign: 'left' },
      { field: 'address', header: 'Địa chỉ', width: '20%', textAlign: 'left' },
      { field: 'other', header: 'Thông tin khác', width: '11%', textAlign: 'left' },
    ];

    //Cơ hội
    this.colsCusLead = [
      { field: 'leadCode', header: 'Mã cơ hội', width: '150px', textAlign: 'left' },
      { field: 'fullName', header: 'Tên cơ hội', width: '200px', textAlign: 'left' },
      { field: 'personInChargeFullName', header: 'Người phụ trách', width: '200px', textAlign: 'left' },
      { field: 'requirementDetail', header: 'Chi tiết yêu cầu', width: '250px', textAlign: 'left' },
      { field: 'expectedSale', header: 'Doanh thu mong đợi', width: '150px', textAlign: 'right' },
      { field: 'statusName', header: 'Tình trạng', width: '100px', textAlign: 'left' },
    ];

    //Lịch sử mua hàng
    this.colsCustomerOrderHistory = [
      { field: 'orderCode', header: 'Mã đơn hàng', width: '20%', textAlign: 'left' },
      { field: 'orderDate', header: 'Ngày đặt hàng', width: '10%', textAlign: 'right' },
      { field: 'statusName', header: 'Trạng thái', width: '15%', textAlign: 'left' },
      { field: 'amount', header: 'Giá trị', width: '25%', textAlign: 'right' },
      { field: 'sellerName', header: 'Nhân viên bán hàng', width: '30%', textAlign: 'left' },
    ];

    //Thông tin thanh toán
    this.colsBankAccount = [
      { field: 'bankName', header: 'Tên ngân hàng', width: '25%', textAlign: 'left' },
      { field: 'accountNumber', header: 'Số tài khoản', width: '20%', textAlign: 'left' },
      { field: 'accountName', header: 'Chủ tài khoản', width: '20%', textAlign: 'left' },
      { field: 'branchName', header: 'Chi nhánh', width: '25%', textAlign: 'left' },
    ];
    /*End*/

    this.cusFirstNameControl = new FormControl(null, [Validators.required, Validators.maxLength(100), forbiddenSpaceText]);
    this.cusCodeControl = new FormControl(null, [Validators.required, checkDuplicateCode(this.listCustomerCode), forbiddenSpaceText]);
    this.cusLastNameControl = new FormControl(null, [Validators.maxLength(50)]);
    this.cusGroupControl = new FormControl(null);
    this.cusStatusControl = new FormControl(null);
    this.cusStatusCareControl = new FormControl(null);
    this.cusPersonInChargeControl = new FormControl(null, [Validators.required]);

    this.customerBaseForm = new FormGroup({
      cusFirstNameControl: this.cusFirstNameControl,
      cusCodeControl: this.cusCodeControl,
      cusLastNameControl: this.cusLastNameControl,
      cusGroupControl: this.cusGroupControl,
      cusStatusControl: this.cusStatusControl,
      cusStatusCareControl: this.cusStatusCareControl,
      cusPersonInChargeControl: this.cusPersonInChargeControl,
    });

    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
    this.cusPhoneControl = new FormControl('', [Validators.required, Validators.pattern(this.getPhonePattern()), Validators.maxLength(50)]);
    this.cusBusinessSizeControl = new FormControl(null);
    this.cusBusinessTypeControl = new FormControl(null);
    this.cusTaxControl = new FormControl(null, [Validators.pattern('[a-zA-Z0-9-]+$'), Validators.maxLength(50)]);
    this.cusEmailControl = new FormControl('', [Validators.pattern(emailPattern), Validators.maxLength(100)]);
    this.cusPaymentMethodControl = new FormControl(null);
    this.cusMaxDebtDayControl = new FormControl(null);
    this.cusMaxDebtValueControl = new FormControl(null);
    this.cusLinkControl = new FormControl(null);
    this.cusTypeOfBusinessControl = new FormControl(null);
    this.cusTotalEmpSocialInsuranceControl = new FormControl(null);
    this.cusCareStaffControl = new FormControl(null, [Validators.required]);
    this.cusBusinessRegistrationDateControl = new FormControl(null);
    this.cusTotalCapitalControl = new FormControl(null);
    this.cusTotalRevenueLastYearControl = new FormControl(null);
    this.cusMainBusinessSectorControl = new FormControl(null);
    this.cusLocalTypeBusinessControl = new FormControl(null);
    this.cusAreaControl = new FormControl(null);
    this.cusProvinceControl = new FormControl(null);
    this.cusDistrictControl = new FormControl(null);
    this.cusWardControl = new FormControl(null);
    this.cusAddressControl = new FormControl(null, [Validators.maxLength(250)]);

    this.customerInforForm = new FormGroup({
      cusPhoneControl: this.cusPhoneControl,
      cusBusinessSizeControl: this.cusBusinessSizeControl,
      cusBusinessTypeControl: this.cusBusinessTypeControl,
      cusTaxControl: this.cusTaxControl,
      cusEmailControl: this.cusEmailControl,
      cusPaymentMethodControl: this.cusPaymentMethodControl,
      cusMaxDebtDayControl: this.cusMaxDebtDayControl,
      cusMaxDebtValueControl: this.cusMaxDebtValueControl,
      cusLinkControl: this.cusLinkControl,
      cusTypeOfBusinessControl: this.cusTypeOfBusinessControl,
      cusTotalEmpSocialInsuranceControl: this.cusTotalEmpSocialInsuranceControl,
      cusCareStaffControl: this.cusCareStaffControl,
      cusBusinessRegistrationDateControl: this.cusBusinessRegistrationDateControl,
      cusTotalCapitalControl: this.cusTotalCapitalControl,
      cusTotalRevenueLastYearControl: this.cusTotalRevenueLastYearControl,
      cusMainBusinessSectorControl: this.cusMainBusinessSectorControl,
      cusLocalTypeBusinessControl: this.cusLocalTypeBusinessControl,
      cusAreaControl: this.cusAreaControl,
      cusProvinceControl: this.cusProvinceControl,
      cusDistrictControl: this.cusDistrictControl,
      cusWardControl: this.cusWardControl,
      cusAddressControl: this.cusAddressControl
    });

    /*FORM KHÁCH HÀNG CÁ NHÂN*/
    let prc = '[a-zA-Z0-9-]+$';
    let prc_1 = '[a-zA-Z0-9]+$';
    this.cusPerGenderControl = new FormControl(null);
    this.cusPerCountryControl = new FormControl(null);
    this.cusPerTaxcodeControl = new FormControl(null, [Validators.maxLength(50), Validators.pattern(prc)]);
    this.cusPerBirthdayControl = new FormControl(null);
    this.cusPerIdentityControl = new FormControl(null, [Validators.maxLength(20), Validators.pattern(prc_1)]);
    this.cusPerPaymentMethodControl = new FormControl(null);
    this.cusPerBirthplaceControl = new FormControl(null, [Validators.maxLength(100)]);
    this.cusPerMaritalStatusControl = new FormControl(null);
    this.cusPerMaxDebtDayControl = new FormControl(null);
    this.cusPerMaxDebtValueControl = new FormControl(null);
    this.cusPerJobControl = new FormControl(null, [Validators.maxLength(100)]);
    this.cusPerAgencyControl = new FormControl(null, [Validators.maxLength(100)]);
    this.cusPerCareStaffControl = new FormControl(null, [Validators.required]);
    this.cusPerCompanyNameControl = new FormControl(null, [Validators.maxLength(500)]);
    this.cusPerCompanyAddressControl = new FormControl(null, [Validators.maxLength(250)]);
    this.cusPerPositionControl = new FormControl(null);

    this.customerPersonalInforForm = new FormGroup({
      cusPerGenderControl: this.cusPerGenderControl,
      cusPerCountryControl: this.cusPerCountryControl,
      cusPerTaxcodeControl: this.cusPerTaxcodeControl,
      cusPerBirthdayControl: this.cusPerBirthdayControl,
      cusPerIdentityControl: this.cusPerIdentityControl,
      cusPerPaymentMethodControl: this.cusPerPaymentMethodControl,
      cusPerBirthplaceControl: this.cusPerBirthplaceControl,
      cusPerMaritalStatusControl: this.cusPerMaritalStatusControl,
      cusPerMaxDebtDayControl: this.cusPerMaxDebtDayControl,
      cusPerMaxDebtValueControl: this.cusPerMaxDebtValueControl,
      cusPerJobControl: this.cusPerJobControl,
      cusPerAgencyControl: this.cusPerAgencyControl,
      cusPerCareStaffControl: this.cusPerCareStaffControl,
      cusPerCompanyNameControl: this.cusPerCompanyNameControl,
      cusPerCompanyAddressControl: this.cusPerCompanyAddressControl,
      cusPerPositionControl: this.cusPerPositionControl
    });
    /*END*/

    /*FORM THÔNG TIN LIÊN HỆ KHÁCH HÀNG CÁ NHÂN*/
    this.cusPerContPhoneControl = new FormControl(null, [Validators.pattern(this.getPhonePattern()), Validators.maxLength(50), Validators.required]);
    this.cusPerContWorkPhoneControl = new FormControl(null, [Validators.pattern(this.getPhonePattern()), Validators.maxLength(50)]);
    this.cusPerContOtherPhoneControl = new FormControl(null, [Validators.pattern(this.getPhonePattern()), Validators.maxLength(50)]);
    this.cusPerContEmailControl = new FormControl(null, [Validators.pattern(emailPattern), Validators.maxLength(100)]);
    this.cusPerContWorkEmailControl = new FormControl(null, [Validators.pattern(emailPattern), Validators.maxLength(100)]);
    this.cusPerContOtherEmailControl = new FormControl(null, [Validators.pattern(emailPattern), Validators.maxLength(100)]);
    this.cusPerContAreaControl = new FormControl(null);
    this.cusPerContProvinceControl = new FormControl(null);
    this.cusPerContDistrictControl = new FormControl(null);
    this.cusPerContWardControl = new FormControl(null);
    this.cusPerContAddressControl = new FormControl(null, [Validators.maxLength(250)]);
    this.cusPerContOtherControl = new FormControl(null, [Validators.maxLength(500)]);

    this.customerPersonalContactForm = this.fb.group({
      'cusPerContPhoneControl': this.cusPerContPhoneControl,
      'cusPerContWorkPhoneControl': this.cusPerContWorkPhoneControl,
      'cusPerContOtherPhoneControl': this.cusPerContOtherPhoneControl,
      'cusPerContEmailControl': this.cusPerContEmailControl,
      'cusPerContWorkEmailControl': this.cusPerContWorkEmailControl,
      'cusPerContOtherEmailControl': this.cusPerContOtherEmailControl,
      'cusPerContAreaControl': this.cusPerContAreaControl,
      'cusPerContProvinceControl': this.cusPerContProvinceControl,
      'cusPerContDistrictControl': this.cusPerContDistrictControl,
      'cusPerContWardControl': this.cusPerContWardControl,
      'cusPerContAddressControl': this.cusPerContAddressControl,
      'cusPerContOtherControl': this.cusPerContOtherControl
    });
    /*END*/

    /*FORM FEED BACK*/
    this.feedbackCodeControl = new FormControl(null, [Validators.required]);
    this.feedbackContentControl = new FormControl(null, [Validators.required]);

    this.feedbackForm = new FormGroup({
      feedbackCodeControl: this.feedbackCodeControl,
      feedbackContentControl: this.feedbackContentControl
    });
    /*END*/

    this.month = ((new Date()).getMonth() + 1).toString().length == 1 ? ('0' + ((new Date()).getMonth() + 1).toString()) : ((new Date()).getMonth() + 1).toString();
    this.getDateByTime(parseFloat(this.month), this.year, 'cus_care');

    this.monthMeeting = ((new Date()).getMonth() + 1).toString().length == 1 ? ('0' + ((new Date()).getMonth() + 1).toString()) : ((new Date()).getMonth() + 1).toString();
    this.getDateByTime(parseFloat(this.monthMeeting), this.yearMeeting, 'cus_meeting');

  }

  mapDataResponse(customer: any, contact: any) {
    //Điểm tích lũy
    this.point = customer.point;

    this.countCustomer = customer.countCustomerInfo;
    this.customerCodeOld = customer.customerCode;
    this.customerModel = <CustomerModel>({
      CustomerId: this.customerId,
      CustomerCode: customer.customerCode,
      CustomerGroupId: customer.customerGroupId,
      CustomerName: customer.customerName,
      CustomerCareStaff: customer.customerCareStaff,
      StatusId: customer.statusId,
      CustomerServiceLevelId: customer.customerServiceLevelId,
      CustomerServiceLevelName: customer.customerServiceLevelName,
      PersonInChargeId: customer.personInChargeId,
      CustomerType: customer.customerType,
      PaymentId: customer.paymentId,
      MaximumDebtValue: customer.maximumDebtValue,
      MaximumDebtDays: customer.maximumDebtDays,
      MainBusinessSector: customer.mainBusinessSector,
      FieldId: customer.fieldId,
      ScaleId: customer.scaleId,
      TotalSaleValue: customer.totalSaleValue,
      TotalReceivable: customer.totalReceivable,
      NearestDateTransaction: customer.nearestDateTransaction,
      BusinessRegistrationDate: customer.businessRegistrationDate,
      EnterpriseType: customer.enterpriseType,
      BusinessScale: customer.businessScale,
      BusinessType: customer.businessType,
      TotalEmployeeParticipateSocialInsurance: customer.totalEmployeeParticipateSocialInsurance,
      TotalCapital: customer.totalCapital,
      TotalRevenueLastYear: customer.totalRevenueLastYear,
      CreatedById: customer.createdById,
      CreatedDate: customer.createdDate,
      UpdatedById: customer.updatedById,
      UpdatedDate: customer.updatedDate,
      Active: customer.active,
      IsGraduated: customer.isGraduated,
      IsApproval: customer.isApproval,
      ApprovalStep: customer.approvalStep,
      StatusCareId: customer.statusCareId
    });

    //Thông tin chi tiết khách hàng
    this.contactModel = <ContactModel>({
      ContactId: contact.contactId,
      ObjectId: contact.objectId,
      ObjectType: contact.objectType,
      FirstName: contact.firstName != null ? contact.firstName.trim() : "",
      LastName: contact.lastName != null ? contact.lastName.trim() : "",
      Phone: contact.phone != null ? contact.phone.trim() : "",
      Email: contact.email != null ? contact.email.trim() : "",
      Address: contact.address != null ? contact.address.trim() : "",
      Gender: contact.gender,
      DateOfBirth: contact.dateOfBirth != null ? new Date(contact.dateOfBirth) : null,
      WorkPhone: contact.workPhone,
      OtherPhone: contact.otherPhone,
      WorkEmail: contact.workEmail,
      OtherEmail: contact.otherEmail,
      IdentityID: contact.identityId,
      AvatarUrl: contact.avatarUrl,
      CountryId: contact.countryId,
      DistrictId: contact.districtId,
      ProvinceId: contact.provinceId,
      WebsiteUrl: contact.websiteUrl,
      WardId: contact.wardId,
      MaritalStatusId: contact.maritalStatusId,
      PostCode: contact.postCode,
      Note: contact.note,
      Role: contact.role,
      TaxCode: contact.taxCode,
      Job: contact.job,
      Agency: contact.agency,
      CompanyName: contact.companyName,
      CompanyAddress: contact.companyAddress,
      CustomerPosition: contact.customerPosition,
      Birthplace: contact.birthplace,
      CreatedById: contact.createdById,
      CreatedDate: contact.createdDate,
      UpdatedById: contact.updatedById,
      UpdatedDate: contact.updatedDate,
      Active: contact.active,
      Other: contact.other,
      Longitude: contact.longitude,
      Latitude: contact.latitude,
      AreaId: contact.areaId,
    });

    if (this.isSendApproval && this.customerModel.IsApproval) {
      this.isApprovalWain = true;
    }
    else { this.isApprovalWain = false }

    //customerType: Khách hàng doanh nghiệp, khách hàng cá nhân, hộ kinh doanh
    switch (this.customerModel.CustomerType) {
      case 1:
        this.customerTypeString = "Khách hàng doanh nghiệp";
        break;

      case 2:
        this.customerTypeString = "Khách hàng cá nhân";
        break;

      case 3:
        this.customerTypeString = "Hộ kinh doanh";
        break;

      default:
        break;
    }

    //Full Name
    this.customerNameLabel = this.customerModel.CustomerName;

    //Mã khách hàng
    this.cusCodeControl.setValue(this.customerModel.CustomerCode);

    //Họ và Tên đệm
    this.cusFirstNameControl.setValue(this.contactModel.FirstName);

    //Tên
    this.cusLastNameControl.setValue(this.contactModel.LastName);

    //Nhóm khách hàng
    let toSelectedCusGroup = this.listCustomerGroup.find(x => x.categoryId == this.customerModel.CustomerGroupId);
    this.cusGroupControl.setValue(toSelectedCusGroup);

    //Trạng thái khách hàng
    let toSelectedCusStatus = this.listCustomerStatus.find(x => x.categoryId == this.customerModel.StatusId);
    this.cusStatusControl.setValue(toSelectedCusStatus);

    //Trạng thái chăm sóc khách hàng
    let toSelectedCusStatusCare = this.listStatusCustomerCare.find(x => x.categoryId == this.customerModel.StatusCareId);
    if(toSelectedCusStatusCare){
      this.cusStatusCareControl.setValue(toSelectedCusStatusCare);
    }else{
      this.cusStatusCareControl.setValue(null);
    }

    //Nhân viên phụ trách
    let toSelectedPersonInCharge = this.listPersonInCharge.find(x => x.employeeId == this.customerModel.PersonInChargeId);
    this.cusPersonInChargeControl.setValue(toSelectedPersonInCharge);


    /*Khách hàng doanh nghiệp*/

    if (this.customerModel.CustomerType == 1) {
      //Số điện thoại
      this.cusPhoneControl.setValue(this.contactModel.Phone);

      //Lĩnh vực kinh doanh
      let toSelectedBusinessType = this.listBusinessType.find(x => x.categoryId == this.customerModel.FieldId);
      if (toSelectedBusinessType) {
        this.cusBusinessTypeControl.setValue(toSelectedBusinessType);
      }

      //Mã số thuế
      this.cusTaxControl.setValue(this.contactModel.TaxCode);

      //Email
      this.cusEmailControl.setValue(this.contactModel.Email);

      //Quy Mô
      let toSelectedBussineeSize = this.listBusinessSize.find(x => x.categoryId == this.customerModel.BusinessScale);
      if (toSelectedBussineeSize) {
        this.cusBusinessSizeControl.setValue(toSelectedBussineeSize);
      }

      //Phương thức thanh toán
      let toSelectedPaymentMethod = this.listPaymentMethod.find(x => x.categoryId == this.customerModel.PaymentId);
      if (toSelectedPaymentMethod) {
        this.cusPaymentMethodControl.setValue(toSelectedPaymentMethod);
      }

      //Số ngày được nợ
      this.cusMaxDebtDayControl.setValue(this.customerModel.MaximumDebtDays);

      //Số nợ tối đa
      this.cusMaxDebtValueControl.setValue(this.customerModel.MaximumDebtValue);

      //Link
      this.cusLinkControl.setValue(this.contactModel.WebsiteUrl);

      //Loại hình doanh nghiệp
      let toSelectedTypeOfBusiness = this.listTypeOfBusiness.find(x => x.categoryId == this.customerModel.EnterpriseType);
      if (toSelectedTypeOfBusiness) {
        this.cusTypeOfBusinessControl.setValue(toSelectedTypeOfBusiness);
      }

      //Số lao động tham gia bảo hiểm
      this.cusTotalEmpSocialInsuranceControl.setValue(this.customerModel.TotalEmployeeParticipateSocialInsurance);

      //Nhân viên chăm sóc khách hàng
      let toSelectedCareStaff = this.listCareStaff.find(x => x.employeeId == this.customerModel.CustomerCareStaff);
      if (toSelectedCareStaff) {
        this.cusCareStaffControl.setValue(toSelectedCareStaff);
      }

      //Ngày cấp
      let businessRegistrationDate = this.customerModel.BusinessRegistrationDate;
      if (businessRegistrationDate) {
        this.cusBusinessRegistrationDateControl.setValue(new Date(businessRegistrationDate))
      }

      //Tổng nguồn vốn
      this.cusTotalCapitalControl.setValue(this.customerModel.TotalCapital);

      //Tổng doanh thu năm trước
      this.cusTotalRevenueLastYearControl.setValue(this.customerModel.TotalRevenueLastYear);

      //Ngành nghề kinh doanh chính
      let toSelectedMainBusinessSector = this.listBusinessCareer.find(x => x.categoryId == this.customerModel.MainBusinessSector);
      if (toSelectedMainBusinessSector) {
        this.cusMainBusinessSectorControl.setValue(toSelectedMainBusinessSector);
      }

      //Loại doanh nghiệp
      let toSelectLocalTypeBusiness = this.listLocalTypeBusiness.find(x => x.categoryId == this.customerModel.BusinessType);
      if (toSelectLocalTypeBusiness) {
        this.cusLocalTypeBusinessControl.setValue(toSelectLocalTypeBusiness);
      }

      //Khu vuc
      let toSelectedArea: Area = this.listArea.find(x => x.areaId == this.contactModel.AreaId);
      if (toSelectedArea) {
        this.cusAreaControl.setValue(toSelectedArea);
      }

      //Tỉnh
      let toSelectedProvince: Province = this.listProvince.find(x => x.provinceId == this.contactModel.ProvinceId);
      if (toSelectedProvince) {
        this.cusProvinceControl.setValue(toSelectedProvince);
        this.listCurrentDistrict = this.listDistrict.filter(x => x.provinceId == toSelectedProvince.provinceId);
      }

      //Quận/Huyện
      let toSelectedDistrict = this.listCurrentDistrict.find(x => x.districtId == this.contactModel.DistrictId);
      if (toSelectedDistrict) {
        this.cusDistrictControl.setValue(toSelectedDistrict);
        this.listCurrentWard = this.listWard.filter(x => x.districtId == toSelectedDistrict.districtId);
      }

      //Phường/Xã
      let toSelectedWard = this.listCurrentWard.find(x => x.wardId == this.contactModel.WardId);
      if (toSelectedWard) {
        this.cusWardControl.setValue(toSelectedWard);
      }

      //Địa chỉ
      this.cusAddressControl.setValue(this.contactModel.Address);
    }

    /*End*/

    /*Khách hàng Cá nhân*/

    if (this.customerModel.CustomerType != 1) {
      //Giới tính
      let toSelectedGender = this.listGenders.find(x => x.categoryCode == this.contactModel.Gender);
      if (toSelectedGender) {
        this.cusPerGenderControl.setValue(toSelectedGender);
      }

      //Quốc tịch
      let toSelectedCountry = this.listCountry.find(x => x.countryId == this.contactModel.CountryId);
      if (toSelectedCountry) {
        this.cusPerCountryControl.setValue(toSelectedCountry);
      }

      //Mã số thuế
      this.cusPerTaxcodeControl.setValue(this.contactModel.TaxCode);

      //Ngày sinh
      this.cusPerBirthdayControl.setValue(this.contactModel.DateOfBirth);

      //Số địn danh cá nhân
      this.cusPerIdentityControl.setValue(this.contactModel.IdentityID);

      //Phương thức thanh toán
      let toSelectedPerPaymentMethod = this.listPaymentMethod.find(x => x.categoryId == this.customerModel.PaymentId);
      if (toSelectedPerPaymentMethod) {
        this.cusPerPaymentMethodControl.setValue(toSelectedPerPaymentMethod);
      }

      //Nơi sinh
      this.cusPerBirthplaceControl.setValue(this.contactModel.Birthplace);

      //Tình trạng hôn nhân
      let toSelectedMaritalStatus = this.listMaritalStatus.find(x => x.categoryId == this.contactModel.MaritalStatusId);
      if (toSelectedMaritalStatus) {
        this.cusPerMaritalStatusControl.setValue(toSelectedMaritalStatus);
      }

      //Số ngày được nợ
      this.cusPerMaxDebtDayControl.setValue(this.customerModel.MaximumDebtDays);

      //Số nợ tối đa
      this.cusPerMaxDebtValueControl.setValue(this.customerModel.MaximumDebtValue);

      //Nghề nghiệp
      this.cusPerJobControl.setValue(this.contactModel.Job);

      //Cơ quan
      this.cusPerAgencyControl.setValue(this.contactModel.Agency);

      //Nhân viên chăm sóc khách hàng
      let toSelectedPerCareStaff = this.listCareStaff.find(x => x.employeeId == this.customerModel.CustomerCareStaff);
      if (toSelectedPerCareStaff) {
        this.cusPerCareStaffControl.setValue(toSelectedPerCareStaff);
      }

      //Tên công ty
      this.cusPerCompanyNameControl.setValue(this.contactModel.CompanyName);

      //Địa chỉ công ty
      this.cusPerCompanyAddressControl.setValue(this.contactModel.CompanyAddress);

      //Chức vụ
      let toSelectedPosition = this.listCustomerPosition.find(x => x.categoryId == this.contactModel.CustomerPosition);
      if (toSelectedPosition) {
        this.cusPerPositionControl.setValue(toSelectedPosition);
      }

      /*Thông tin liên hệ*/

      if (this.contactModel.Phone) {
        this.cusPerContPhoneControl.setValue(this.contactModel.Phone.trim());
      }

      if (this.contactModel.WorkPhone) {
        this.cusPerContWorkPhoneControl.setValue(this.contactModel.WorkPhone.trim());
      }

      if (this.contactModel.OtherPhone) {
        this.cusPerContOtherPhoneControl.setValue(this.contactModel.OtherPhone.trim());
      }

      if (this.contactModel.Email) {
        this.cusPerContEmailControl.setValue(this.contactModel.Email.trim());
      }

      if (this.contactModel.WorkEmail) {
        this.cusPerContWorkEmailControl.setValue(this.contactModel.WorkEmail.trim());
      }

      if (this.contactModel.OtherEmail) {
        this.cusPerContOtherEmailControl.setValue(this.contactModel.OtherEmail.trim());
      }

      //Khu vực
      let toSelectedArea: Area = this.listArea.find(x => x.areaId == this.contactModel.AreaId);
      if (toSelectedArea) {
        this.cusPerContAreaControl.setValue(toSelectedArea);
      }

      //Tỉnh
      let toSelectedProvince: Province = this.listProvince.find(x => x.provinceId == this.contactModel.ProvinceId);
      if (toSelectedProvince) {
        this.cusPerContProvinceControl.setValue(toSelectedProvince);
        this.listCurrentDistrict = this.listDistrict.filter(x => x.provinceId == toSelectedProvince.provinceId);
      }

      //Quận/Huyện
      let toSelectedDistrict = this.listCurrentDistrict.find(x => x.districtId == this.contactModel.DistrictId);
      if (toSelectedDistrict) {
        this.cusPerContDistrictControl.setValue(toSelectedDistrict);
        this.listCurrentWard = this.listWard.filter(x => x.districtId == toSelectedDistrict.districtId);
      }

      //Phường/Xã
      let toSelectedWard = this.listCurrentWard.find(x => x.wardId == this.contactModel.WardId);
      if (toSelectedWard) {
        this.cusPerContWardControl.setValue(toSelectedWard);
      }

      //Địa chỉ
      if (this.contactModel.Address) {
        this.cusPerContAddressControl.setValue(this.contactModel.Address.trim());
      }

      //Thông tin khác
      if (this.contactModel.Other) {
        this.cusPerContOtherControl.setValue(this.contactModel.Other.trim());
      }

      /*End*/
    }

    //patch vị trí khách hàng trên bản đồ
    this.patchGeoCodingForMap(contact.latitude, contact.longitude, this.customerNameLabel);
    /*End*/
  }

  /*Đến trang tạo cơ hội*/
  goToLead() {
    this.router.navigate(['/lead/create-lead', {
      customerId: this.customerId
    }]);
  }

  /*Đến trang tạo đơn hàng*/
  goToOrderCreate() {
    this.router.navigate(['/order/create', { customerId: this.customerId }]);
  }

  /*Đến trang tạo phiếu thu*/
  goToCashReceiptsCreate() {
    this.router.navigate(['/accounting/cash-receipts-create', { customerId: this.customerId }]);
  }

  /*Đến trang tạo báo có*/
  goToBankReceiptsCreate() {
    this.router.navigate(['/accounting/bank-receipts-create', { customerId: this.customerId }]);
  }

  /*Đến trang tạo báo giá*/
  goToQuoteCreate() {
    this.router.navigate(['/customer/quote-create', { customerId: this.customerId }]);
  }

  /*Đến trang chi tiết công nợ khách hàng*/
  goToReceivableCustomerDetail() {
    this.router.navigate(['/accounting/receivable-customer-detail', { id: this.customerId }]);
  }

  /*Gửi Email*/
  sendEmail() {
    this.selectedCustomerList = [];
    this.selectedCustomerList.push(this.customerId);

    let ref = this.dialogService.open(SendEmailDialogComponent, {
      data: {
        leadIdList: this.selectedCustomerList
      },
      header: 'Gửi email',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          if (result.message == "success") {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Gửi email thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        }
      }
    });
  }

  /*Gửi SMS*/
  sendSMS() {
    this.selectedCustomerList = [];
    this.selectedCustomerList.push(this.customerId);

    let ref = this.dialogService.open(SendSmsDialogComponent, {
      data: {
        leadIdList: this.selectedCustomerList
      },
      header: 'Gửi SMS',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          if (result.message == "success") {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Gửi SMS thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        }
      }
    });
  }

  /*Event thay đổi Tỉnh/Thành phố*/
  async changeProvince() {
    let toSelectedProvince: Province = this.cusProvinceControl.value;
    this.cusDistrictControl.setValue(null);
    this.cusWardControl.setValue(null);
    if (toSelectedProvince) {
      this.loading = true;
      let result: any = await this.districtService.getAllDistrictByProvinceIdAsync(toSelectedProvince.provinceId);
      this.loading = false;
      if (result.statusCode === 200) {
        this.listCurrentDistrict = result.listDistrict;
      }
    } else {
      this.listCurrentDistrict = [];
      this.listCurrentWard = [];
    }
  }

  /*Event thay đổi Quận/Huyện*/
  async changeDistrict() {
    let toSelectedDistrict: District = this.cusDistrictControl.value;
    this.cusWardControl.setValue(null);
    if (toSelectedDistrict) {
      this.loading = true;
      let result: any = await this.wardService.getAllWardByDistrictIdAsync(toSelectedDistrict.districtId);
      this.loading = false;
      if (result.statusCode === 200) {
        this.listCurrentWard = result.listWard;
      }
    } else {
      this.listCurrentWard = [];
    }
  }

  /*Event thay đổi nội dung ghi chú*/
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  /*Event Thêm các file được chọn vào list file*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= 10000000) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedFiles.push(file);
          }
        }
      }
    }
  }

  /*Event Khi click xóa từng file*/
  removeFile(event) {
    let index = this.uploadedFiles.indexOf(event.file);
    this.uploadedFiles.splice(index, 1);
  }

  /*Event Khi click xóa toàn bộ file*/
  clearAllFile() {
    this.uploadedFiles = [];
  }

  /*Hủy sửa ghi chú*/
  cancelNote() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn hủy ghi chú này?',
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.listUpdateNoteDocument = [];
        this.isEditNote = false;
      }
    });
  }

  /*Lưu file và ghi chú vào Db*/
  async saveNote() {
    this.loading = true;
    this.listNoteDocumentModel = [];

    /*Upload file mới nếu có*/
    if (this.uploadedFiles.length > 0) {
      await this.uploadFilesAsync(this.uploadedFiles);

      for (var x = 0; x < this.uploadedFiles.length; ++x) {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = this.uploadedFiles[x].name;
        noteDocument.DocumentSize = this.uploadedFiles[x].size.toString();

        this.listNoteDocumentModel.push(noteDocument);
      }
    }

    let noteModel = new NoteModel();

    if (!this.noteId) {
      /*Tạo mới ghi chú*/

      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.customerId;
      noteModel.ObjectType = 'CUS';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/

      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.customerId;
      noteModel.ObjectType = 'CUS';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();

      //Thêm file cũ đã lưu nếu có
      this.listUpdateNoteDocument.forEach(item => {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = item.documentName;
        noteDocument.DocumentSize = item.documentSize;

        this.listNoteDocumentModel.push(noteDocument);
      });
    }

    this.noteService.createNoteForCustomerDetail(noteModel, this.listNoteDocumentModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.listUpdateNoteDocument = [];
        this.noteId = null;
        this.isEditNote = false;

        /*Reshow Time Line */
        this.noteHistory = result.listNote;
        this.handleNoteContent();

        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu ghi chú thành công" };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  async uploadFilesAsync(files: File[]) {
    await this.imageService.uploadFileAsync(files);
  }

  // Upload file to server
  uploadFiles(files: File[]) {
    this.imageService.uploadFile(files).subscribe(response => { });
  }

  /*Xử lý và hiển thị lại nội dung ghi chú*/
  handleNoteContent() {

    this.noteHistory.forEach(element => {
      setTimeout(() => {
        let count = 0;
        if (element.description == null) {
          element.description = "";
        }

        let des = $.parseHTML(element.description);
        let newTextContent = '';
        for (let i = 0; i < des.length; i++) {
          count += des[i].textContent.length;
          newTextContent += des[i].textContent;
        }

        if (count > 250) {
          newTextContent = newTextContent.substr(0, 250) + '<b>...</b>';
          $('#' + element.noteId).find('.short-content').append($.parseHTML(newTextContent));
        } else {
          $('#' + element.noteId).find('.short-content').append($.parseHTML(element.description));
        }

        // $('#' + element.noteId).find('.note-title').append($.parseHTML(element.noteTitle));
        $('#' + element.noteId).find('.full-content').append($.parseHTML(element.description));
      }, 1000);
    });
  }
  /*End*/

  /*Event Mở rộng/Thu gọn nội dung của ghi chú*/
  toggle_note_label: string = 'Mở rộng';
  trigger_node(nodeid: string, event) {
    // noteContent
    let shortcontent_ = $('#' + nodeid).find('.short-content');
    let fullcontent_ = $('#' + nodeid).find('.full-content');
    if (shortcontent_.css("display") === "none") {
      fullcontent_.css("display", "none");
      shortcontent_.css("display", "block");
    } else {
      fullcontent_.css("display", "block");
      shortcontent_.css("display", "none");
    }
    // noteFile
    let shortcontent_file = $('#' + nodeid).find('.short-content-file');
    let fullcontent_file = $('#' + nodeid).find('.full-content-file');
    let continue_ = $('#' + nodeid).find('.continue')
    if (shortcontent_file.css("display") === "none") {
      continue_.css("display", "block");
      fullcontent_file.css("display", "none");
      shortcontent_file.css("display", "block");
    } else {
      continue_.css("display", "none");
      fullcontent_file.css("display", "block");
      shortcontent_file.css("display", "none");
    }
    let curr = $(event.target);

    if (curr.attr('class').indexOf('pi-chevron-right') != -1) {
      this.toggle_note_label = 'Thu gọn';
      curr.removeClass('pi-chevron-right');
      curr.addClass('pi-chevron-down');
    } else {
      this.toggle_note_label = 'Mở rộng';
      curr.removeClass('pi-chevron-down');
      curr.addClass('pi-chevron-right');
    }
  }
  /*End */

  /*Kiểm tra noteText > 250 ký tự hoặc noteDocument > 3 thì ẩn đi một phần nội dung*/
  tooLong(note): boolean {
    if (note.noteDocList.length > 3) return true;
    var des = $.parseHTML(note.description);
    var count = 0;
    for (var i = 0; i < des.length; i++) {
      count += des[i].textContent.length;
      if (count > 250) return true;
    }
    return false;
  }

  openItem(name, url) {
    this.imageService.downloadFile(name, url).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(file);
      } else {
        var fileURL = URL.createObjectURL(file);
        if (fileType.indexOf('image') !== -1) {
          window.open(fileURL);
        } else {
          var anchor = document.createElement("a");
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    }, error => { });
  }

  /*Event Sửa ghi chú*/
  onClickEditNote(noteId: string, noteDes: string) {
    this.noteContent = noteDes;
    this.noteId = noteId;
    this.listUpdateNoteDocument = this.noteHistory.find(x => x.noteId == this.noteId).noteDocList;
    this.isEditNote = true;
  }
  /*End*/

  /*Event Xóa ghi chú*/
  onClickDeleteNote(noteId: string) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa ghi chú này?',
      accept: () => {
        this.loading = true;
        this.noteService.disableNote(noteId).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            let note = this.noteHistory.find(x => x.noteId == noteId);
            let index = this.noteHistory.lastIndexOf(note);
            this.noteHistory.splice(index, 1);

            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa ghi chú thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }
  /*End*/

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: NoteDocument) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listUpdateNoteDocument.indexOf(file);
        this.listUpdateNoteDocument.splice(index, 1);
      }
    });
  }

  /*Event khi download 1 file đã lưu trên server*/
  downloadFile(fileInfor: NoteDocument) {
    this.imageService.downloadFile(fileInfor.documentName, fileInfor.documentUrl).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(file);
      } else {
        var fileURL = URL.createObjectURL(file);
        if (fileType.indexOf('image') !== -1) {
          window.open(fileURL);
        } else {
          var anchor = document.createElement("a");
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    });
  }

  sendApprovalCustomer() {
    this.customerService.sendApprovalCustomer(this.customerModel.CustomerId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.getMasterData();
        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Gửi phê duyệt thành công' };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Lưu thông tin khách hàng*/
  async saveCustomer() {
    let isValid = this.checkValidator();

    if (!this.validEmailCustomer || !this.validPhoneCustomer)
    {
      this.showMessageErr();
    }
    else if (isValid)
    {
      /*Form chung*/

      let fullName = '';
      let firstName = this.cusFirstNameControl.value != null ? this.cusFirstNameControl.value.trim() : "";
      let lastName = this.cusLastNameControl.value != null ? this.cusLastNameControl.value.trim() : "";
      fullName = (firstName + " " + lastName).trim();

      this.customerModel.CustomerName = fullName;
      this.customerModel.CustomerCode = this.cusCodeControl.value != null ? this.cusCodeControl.value.trim() : "";

      this.contactModel.FirstName = firstName;
      this.contactModel.LastName = lastName;

      if (this.cusGroupControl.value) {
        let group: Category = this.cusGroupControl.value;
        this.customerModel.CustomerGroupId = group.categoryId;
      }

      if (this.cusStatusControl.value) {
        let status: Category = this.cusStatusControl.value;
        this.customerModel.StatusId = status.categoryId;
      }

      if (this.cusStatusCareControl.value) {
        let status: Category = this.cusStatusCareControl.value;
        this.customerModel.StatusCareId = status.categoryId;
      }

      if (this.cusPersonInChargeControl.value) {
        let personInCharge: Employee = this.cusPersonInChargeControl.value;
        this.customerModel.PersonInChargeId = personInCharge.employeeId;
      }
      /*End Form chung*/

      if (this.customerModel.CustomerType == 1) {
        /*Khách hàng doanh nghiệp*/

        this.contactModel.Phone = this.cusPhoneControl.value;

        if (this.cusBusinessTypeControl.value) {
          let field: Category = this.cusBusinessTypeControl.value;
          this.customerModel.FieldId = field.categoryId;
        } else {
          this.customerModel.FieldId = null;
        }

        this.contactModel.TaxCode = this.cusTaxControl.value != null ? this.cusTaxControl.value.trim() : null;

        this.contactModel.Email = this.cusEmailControl.value.trim();

        if (this.cusBusinessSizeControl.value) {
          let businessScale: Category = this.cusBusinessSizeControl.value;
          this.customerModel.BusinessScale = businessScale.categoryId;
        } else {
          this.customerModel.BusinessScale = null;
        }

        if (this.cusPaymentMethodControl.value) {
          let payment: Category = this.cusPaymentMethodControl.value;
          this.customerModel.PaymentId = payment.categoryId;
        } else {
          this.customerModel.PaymentId = null;
        }

        this.customerModel.MaximumDebtDays = this.cusMaxDebtDayControl.value != null ? parseFloat(this.cusMaxDebtDayControl.value.replace(/,/g, '')) : 0;
        this.customerModel.MaximumDebtValue = this.cusMaxDebtValueControl.value != null ? parseFloat(this.cusMaxDebtValueControl.value.replace(/,/g, '')) : 0;

        this.contactModel.WebsiteUrl = this.cusLinkControl.value != null ? this.cusLinkControl.value.trim() : null;

        if (this.cusTypeOfBusinessControl.value) {
          let enterpriseType: Category = this.cusTypeOfBusinessControl.value;
          this.customerModel.EnterpriseType = enterpriseType.categoryId;
        } else {
          this.customerModel.EnterpriseType = null;
        }

        this.customerModel.TotalEmployeeParticipateSocialInsurance = this.cusTotalEmpSocialInsuranceControl.value != null ? parseFloat(this.cusTotalEmpSocialInsuranceControl.value.replace(/,/g, '')) : 0;

        if (this.cusCareStaffControl.value) {
          let careStaff: Employee = this.cusCareStaffControl.value;
          this.customerModel.CustomerCareStaff = careStaff.employeeId;
        } else {
          this.customerModel.CustomerCareStaff = null;
        }

        if (this.cusBusinessRegistrationDateControl.value) {
          this.customerModel.BusinessRegistrationDate = convertToUTCTime(this.cusBusinessRegistrationDateControl.value);
        } else {
          this.customerModel.BusinessRegistrationDate = null;
        }

        this.customerModel.TotalCapital = this.cusTotalCapitalControl.value != null ?
          parseFloat(this.cusTotalCapitalControl.value.replace(/,/g, '')) : null;

        this.customerModel.TotalRevenueLastYear = this.cusTotalRevenueLastYearControl.value != null ?
          parseFloat(this.cusTotalRevenueLastYearControl.value.replace(/,/g, '')) : null;

        if (this.cusMainBusinessSectorControl.value) {
          let MainBusinessSector: Category = this.cusMainBusinessSectorControl.value;
          this.customerModel.MainBusinessSector = MainBusinessSector.categoryId;
        } else {
          this.customerModel.MainBusinessSector = null;
        }

        if (this.cusLocalTypeBusinessControl.value) {
          let businessType: Category = this.cusLocalTypeBusinessControl.value;
          this.customerModel.BusinessType = businessType.categoryId;
        } else {
          this.customerModel.BusinessType = null;
        }

        if (this.cusAreaControl.value) {
          let area: Area = this.cusAreaControl.value;
          this.contactModel.AreaId = area.areaId;
        } else {
          this.contactModel.AreaId = null;
        }

        if (this.cusProvinceControl.value) {
          let province: Province = this.cusProvinceControl.value;
          this.contactModel.ProvinceId = province.provinceId;
        } else {
          this.contactModel.ProvinceId = null;
        }

        if (this.cusDistrictControl.value) {
          let district: District = this.cusDistrictControl.value;
          this.contactModel.DistrictId = district.districtId;
        } else {
          this.contactModel.DistrictId = null;
        }

        if (this.cusWardControl.value) {
          let Ward: Ward = this.cusWardControl.value;
          this.contactModel.WardId = Ward.wardId;
        } else {
          this.contactModel.WardId = null;
        }

        this.contactModel.Address = this.cusAddressControl.value != null ?
          this.cusAddressControl.value.trim() : "";
      } else {
        /*Khách hàng cá nhân*/
        this.saveContact();

        if (this.cusPerGenderControl.value) {
          let gender = this.cusPerGenderControl.value.categoryCode;
          this.contactModel.Gender = gender;
        } else {
          this.contactModel.Gender = null;
        }

        if (this.cusPerCountryControl.value) {
          let country: Country = this.cusPerCountryControl.value;
          this.contactModel.CountryId = country.countryId;
        } else {
          this.contactModel.CountryId = null;
        }

        this.contactModel.TaxCode = this.cusPerTaxcodeControl.value != null ?
          this.cusPerTaxcodeControl.value.trim() : null;

        this.contactModel.DateOfBirth = this.cusPerBirthdayControl.value != null ?
          convertToUTCTime(this.cusPerBirthdayControl.value) : null;

        this.contactModel.IdentityID = this.cusPerIdentityControl.value != null ?
          this.cusPerIdentityControl.value.trim() : null;

        if (this.cusPerPaymentMethodControl.value) {
          let paymentMethod: Category = this.cusPerPaymentMethodControl.value;
          this.customerModel.PaymentId = paymentMethod.categoryId;
        } else {
          this.customerModel.PaymentId = null;
        }

        this.contactModel.Birthplace = this.cusPerBirthplaceControl.value != null ?
          this.cusPerBirthplaceControl.value.trim() : "";

        if (this.cusPerMaritalStatusControl.value) {
          let maritalStatus: Category = this.cusPerMaritalStatusControl.value;
          this.contactModel.MaritalStatusId = maritalStatus.categoryId;
        } else {
          this.contactModel.MaritalStatusId = null;
        }

        this.customerModel.MaximumDebtDays = this.cusPerMaxDebtDayControl.value != null ?
          parseFloat(this.cusPerMaxDebtDayControl.value.replace(/,/g, '')) : 0;
        this.customerModel.MaximumDebtValue = this.cusPerMaxDebtValueControl.value != null ?
          parseFloat(this.cusPerMaxDebtValueControl.value.replace(/,/g, '')) : 0;

        this.contactModel.Job = this.cusPerJobControl.value != null ?
          this.cusPerJobControl.value.trim() : "";

        this.contactModel.Agency = this.cusPerAgencyControl.value != null ?
          this.cusPerAgencyControl.value.trim() : "";

        if (this.cusPerCareStaffControl.value) {
          let careStaff: Employee = this.cusPerCareStaffControl.value;
          this.customerModel.CustomerCareStaff = careStaff.employeeId;
        } else {
          this.customerModel.CustomerCareStaff = null;
        }

        this.contactModel.CompanyName = this.cusPerCompanyNameControl.value != null ?
          this.cusPerCompanyNameControl.value.trim() : "";

        this.contactModel.CompanyAddress = this.cusPerCompanyAddressControl.value != null ?
          this.cusPerCompanyAddressControl.value.trim() : "";

        if (this.cusPerPositionControl.value) {
          let customerPosition: Category = this.cusPerPositionControl.value;
          this.contactModel.CustomerPosition = customerPosition.categoryId;
        } else {
          this.contactModel.CustomerPosition = null;
        }
      }

      this.loading = true;
      //cập nhật google map
      await this.getCustomerGeoCoding();
      let lat = this.overlays.length > 0 ? this.overlays[0].position.lat() : null;
      let lng = this.overlays.length > 0 ? this.overlays[0].position.lng() : null;
      this.contactModel.Longitude = lng;
      this.contactModel.Latitude = lat;

      this.customerService.updateCustomerById(this.customerModel, this.contactModel).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: "Cập nhập khách hàng thành công" };
          this.showMessage(msg);
          //send email after edit
          let sendMailModel: SendEmailModel = result.sendEmailEntityModel;
          this.emailConfigService.sendEmail(6, sendMailModel).subscribe(reponse => {
          });
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  /*Cập nhật liên hệ KHCN*/
  async saveContact() {
    if (!this.customerPersonalContactForm.valid) {
      Object.keys(this.customerPersonalContactForm.controls).forEach(key => {
        if (this.customerPersonalContactForm.controls[key].valid == false) {
          this.customerPersonalContactForm.controls[key].markAsTouched();
        }
      });
    } else {
      let contact: PersonalCustomerContact = {
        ContactId: this.contactModel.ContactId,
        Email: this.contactModel.Email,
        WorkEmail: this.contactModel.WorkEmail,
        OtherEmail: this.contactModel.OtherEmail,
        Phone: this.contactModel.Phone,
        WorkPhone: this.contactModel.WorkPhone,
        OtherPhone: this.contactModel.OtherPhone,
        AreaId: this.contactModel.AreaId,
        ProvinceId: this.contactModel.ProvinceId,
        DistrictId: this.contactModel.DistrictId,
        WardId: this.contactModel.WardId,
        Address: this.contactModel.Address,
        Other: this.contactModel.Other,
        Latitude: null,
        Longitude: null,
      };

      if (this.cusPerContPhoneControl.value) {
        contact.Phone = this.cusPerContPhoneControl.value.trim();
      } else {
        contact.Phone = null;
      }

      if (this.cusPerContWorkPhoneControl.value) {
        contact.WorkPhone = this.cusPerContWorkPhoneControl.value.trim();
      } else {
        contact.WorkPhone = null;
      }

      if (this.cusPerContOtherPhoneControl.value) {
        contact.OtherPhone = this.cusPerContOtherPhoneControl.value.trim();
      } else {
        contact.OtherPhone = null;
      }

      if (this.cusPerContEmailControl.value) {
        contact.Email = this.cusPerContEmailControl.value.trim();
      } else {
        contact.Email = null;
      }

      if (this.cusPerContWorkEmailControl.value) {
        contact.WorkEmail = this.cusPerContWorkEmailControl.value.trim();
      } else {
        contact.WorkEmail = null;
      }

      if (this.cusPerContOtherEmailControl.value) {
        contact.OtherEmail = this.cusPerContOtherEmailControl.value.trim();
      } else {
        contact.OtherEmail = null;
      }

      let toSelectedArea: Area = this.cusPerContAreaControl.value;
      if (toSelectedArea) {
        contact.AreaId = toSelectedArea.areaId;
      } else {
        contact.AreaId = null;
      }

      let toSelectedProvince: Province = this.cusPerContProvinceControl.value;
      if (toSelectedProvince) {
        contact.ProvinceId = toSelectedProvince.provinceId;
      } else {
        contact.ProvinceId = null;
      }

      let toSelectedDistrict: District = this.cusPerContDistrictControl.value;
      if (toSelectedDistrict) {
        contact.DistrictId = toSelectedDistrict.districtId;
      } else {
        contact.DistrictId = null;
      }

      let toSelectedWard: Ward = this.cusPerContWardControl.value;
      if (toSelectedWard) {
        contact.WardId = toSelectedWard.wardId;
      } else {
        contact.WardId = null;
      }

      if (this.cusPerContAddressControl.value) {
        contact.Address = this.cusPerContAddressControl.value.trim();
      } else {
        contact.Address = null;
      }

      if (this.cusPerContOtherControl.value) {
        contact.Other = this.cusPerContOtherControl.value.trim();
      } else {
        contact.Other = null;
      }

      //nối địa chỉ khách hàng cá nhân
      let AreaCusPer: Area = this.cusPerContAreaControl.value;
      let proviceCusPer: Province = this.cusPerContProvinceControl.value;
      let districtCusPer: District = this.cusPerContDistrictControl.value;
      let wardCusPer: Ward = this.cusPerContWardControl.value;
      let detailAddressCusPer = this.cusPerContAddressControl.value;
      let proviceCusPerName = proviceCusPer ? proviceCusPer.provinceName : "";
      let districtCusPerName = districtCusPer ? districtCusPer.districtName : "";
      let wardCusPerName = wardCusPer ? wardCusPer.wardName : "";
      let address = `${detailAddressCusPer},${wardCusPerName},${districtCusPerName},${proviceCusPerName}`;

      this.loading = true;
      await this.getCustomerGeoCodingUpdateContact(address, this.customerNameLabel);
      let lat = this.overlays.length > 0 ? this.overlays[0].position.lat() : null;
      let lng = this.overlays.length > 0 ? this.overlays[0].position.lng() : null;
      contact.Latitude = lat;
      contact.Longitude = lng;

      this.contactService.updatePersonalCustomerContact(contact).subscribe(response => {
        this.loading = false;
        let result: any = response;

        if (result.statusCode == 200) {
          this.contactModel.Phone = contact.Phone; //Gán lại để trong Tab Tiến trình CSKH -> Gửi SMS lấy được số điện thoại mới
          this.contactModel.Email = contact.Email; //Gán lại để trong Tab Tiến trình CSKH -> Gửi Email lấy được email mới
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật thành công' };
          this.showMessage(msg);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  removeCustomerMeeting(week: CustomerMeetingForWeek){
    if(week){
      this.confirmationService.confirm({
        message: 'Bạn có chắc chắn muốn xóa lịch hẹn?',
        accept: () => {
          this.loading = true;
          this.customerCareService.removeCustomerMeeting(week.customerMeetingId, this.auth.UserId).subscribe(response => {
            this.loading = false;
            let result = <any>response;
            if (result.statusCode === 200) {
              this.getHistoryCustomerMeeting(parseFloat(this.monthMeeting), this.yearMeeting);
              let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Xóa lịch hẹn thành công' };
              this.showMessage(mgs);
            } else {
              let mgs = { severity: 'error', summary: 'Thông báo', detail: 'Xóa lịch hẹn thất bại' };
              this.showMessage(mgs);
            }
          }, error => { this.loading = false; });
        }
      });
    }
  }

  checkValidator(): boolean {
    let result: boolean = true;

    if (this.customerModel.CustomerType == 1) {
      if (!this.customerBaseForm.valid) {
        Object.keys(this.customerBaseForm.controls).forEach(key => {
          if (this.customerBaseForm.controls[key].valid == false) {
            this.customerBaseForm.controls[key].markAsTouched();
          }
        });

        result = false;
      }

      if (!this.customerInforForm.valid) {
        Object.keys(this.customerInforForm.controls).forEach(key => {
          if (this.customerInforForm.controls[key].valid == false) {
            this.customerInforForm.controls[key].markAsTouched();
          }
        });

        result = false;
      }
    } else {
      if (!this.customerBaseForm.valid) {
        Object.keys(this.customerBaseForm.controls).forEach(key => {
          if (this.customerBaseForm.controls[key].valid == false) {
            this.customerBaseForm.controls[key].markAsTouched();
          }
        });

        result = false;
      }

      if (!this.customerPersonalInforForm.valid) {
        Object.keys(this.customerPersonalInforForm.controls).forEach(key => {
          if (this.customerPersonalInforForm.controls[key].valid == false) {
            this.customerPersonalInforForm.controls[key].markAsTouched();
          }
        });

        result = false;
      }

      if (!this.customerPersonalContactForm.valid) {
        Object.keys(this.customerPersonalContactForm.controls).forEach(key => {
          if (this.customerPersonalContactForm.controls[key].valid == false) {
            this.customerPersonalContactForm.controls[key].markAsTouched();
          }
        });

        result = false;
      }
    }

    return result;
  }

  del_customer() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa khách hàng này?',
      accept: () => {
        this.customerService.changeCustomerStatusToDelete(this.customerId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.router.navigate(['/customer/list']);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }

  /*Thêm bộ câu hỏi từ db*/
  AddQAList() {
    this.loading = true;
    this.customerService.createListQuestion(this.customerId, this.auth.UserId).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listCustomerAdditionalInformation = result.listCustomerAdditionalInformation;
        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo mới thành công' };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Tạo một bộ câu hỏi/câu trả lời mới ở tab Thông tin khác*/
  createAddition() {
    let ref = this.dialogService.open(AddQuestionAnswerDialogComponent, {
      data: {
        customerAdditionalInformationId: null,
        customerId: this.customerId
      },
      header: 'Thêm thông tin',
      width: '450px',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "190px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.listCustomerAdditionalInformation = result.listCustomerAdditionalInformation;
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo mới thành công' };
          this.showMessage(msg);
        }
      }
    });
  }

  /*Sửa câu hỏi/câu trả lời ở tab Thông tin khác*/
  editAddition(rowData: CustomerAdditionalInformation) {
    let ref = this.dialogService.open(AddQuestionAnswerDialogComponent, {
      data: {
        customerAdditionalInformationId: rowData.customerAdditionalInformationId,
        customerId: this.customerId,
        question: rowData.question,
        answer: rowData.answer
      },
      header: 'Cập nhật thông tin',
      width: '450px',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "190px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.listCustomerAdditionalInformation = result.listCustomerAdditionalInformation;
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật thành công' };
          this.showMessage(msg);
        }
      }
    });
  }

  /*Xóa câu hỏi/câu trả lời ở tab Thông tin khác*/
  deleteAddition(rowData: CustomerAdditionalInformation) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa thông tin này?',
      accept: () => {
        this.customerService.deleteCustomerAdditional(rowData.customerAdditionalInformationId, rowData.customerId, this.auth.UserId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode == 200) {
            this.listCustomerAdditionalInformation = result.listCustomerAdditionalInformation;
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }

  /*Xóa list câu hỏi/câu trả lời được chọn ở tab Thông tin khác*/
  deleteSelectedQAList() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa các thông tin được chọn này?',
      accept: () => {
        let listCusAddInfId = this.listSelectedCusAddInf.map(x => x.customerAdditionalInformationId);
        this.customerService.deleteListCustomerAdditional(listCusAddInfId, this.customerId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode == 200) {
            this.listSelectedCusAddInf.length = 0;
            this.listCustomerAdditionalInformation = result.listCustomerAdditionalInformation;
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }

  /*Thêm người liên hệ KHDN*/
  AddCusContact() {
    let cusContact: OtherContact = {
      contactId: null,
      objectId: this.customerId,
      objectType: "CUS_CON",
      firstName: null,
      lastName: null,
      contactName: null,
      gender: null,
      genderName: null,
      dateOfBirth: null,
      address: null,
      email: null,
      other: null,
      phone: null,
      role: null
    };

    let ref = this.dialogService.open(ContactpopupComponent, {
      data: {
        contact: cusContact,
      },
      header: 'Thêm liên hệ',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "190px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.listCusContact = result.listContact;
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo mới thành công' };
          this.showMessage(msg);
        }
      }
    });
  }

  /*Sửa người liên hệ KHDN*/
  editCusContact(rowData: OtherContact) {
    let cusContact: OtherContact = {
      contactId: rowData.contactId,
      objectId: this.customerId,
      objectType: "CUS_CON",
      firstName: rowData.firstName,
      lastName: rowData.lastName,
      contactName: rowData.contactName,
      gender: rowData.gender,
      genderName: rowData.genderName,
      dateOfBirth: rowData.dateOfBirth,
      address: rowData.address,
      email: rowData.email,
      other: rowData.other,
      phone: rowData.phone,
      role: rowData.role
    };

    let ref = this.dialogService.open(ContactpopupComponent, {
      data: {
        contact: cusContact,
      },
      header: 'Cập nhật liên hệ',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "190px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.listCusContact = result.listContact;
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật thành công' };
          this.showMessage(msg);
        }
      }
    });
  }

  /*Xóa người liên hệ KHDN*/
  deleteCusContact(rowData: OtherContact) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa liên hệ này?',
      accept: () => {
        this.contactService.deleteContactById(rowData.contactId, rowData.objectId, rowData.objectType)
          .subscribe(response => {
            let result: any = response;

            if (result.statusCode == 200) {
              this.listCusContact = result.listContact;
              let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa thành công' };
              this.showMessage(msg);
            } else {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          });
      }
    });
  }

  /*Event thay đổi Tỉnh/Thành phố của KHCN*/
  async changeCusPerProvince() {
    let toSelectedProvince: Province = this.cusPerContProvinceControl.value;
    this.cusPerContDistrictControl.setValue(null);
    this.cusPerContWardControl.setValue(null);
    if (toSelectedProvince) {
      this.loading = true;
      let result: any = await this.districtService.getAllDistrictByProvinceIdAsync(toSelectedProvince.provinceId);
      this.loading = false;
      if (result.statusCode === 200) {
        this.listCurrentDistrict = result.listDistrict;
      }
    } else {
      this.listCurrentDistrict = [];
      this.listCurrentWard = [];
    }
  }

  /*Event thay đổi Quận/Huyện của KHCN*/
  async changeCusPerDistrict() {
    let toSelectedDistrict: District = this.cusPerContDistrictControl.value;
    this.cusPerContWardControl.setValue(null);
    if (toSelectedDistrict) {
      this.loading = true;
      let result: any = await this.wardService.getAllWardByDistrictIdAsync(toSelectedDistrict.districtId);
      this.loading = false;
      if (result.statusCode === 200) {
        this.listCurrentWard = result.listWard;
      }
    } else {
      this.listCurrentWard = [];
    }
  }

  changeCustomerName() {
    let firstName = this.cusFirstNameControl.value != null ? this.cusFirstNameControl.value.trim() : "";
    let lastName = this.cusLastNameControl.value != null ? this.cusLastNameControl.value.trim() : "";
    this.customerNameLabel = (firstName + " " + lastName).trim();
  }

  /*Event Thêm tài khoản thanh toán*/
  AddBankAccount() {
    let bankAccount: BankAccount = {
      bankAccountId: null,
      objectId: this.customerId,
      objectType: "CUS",
      bankName: "", //Tên ngân hàng
      accountNumber: "",  //Số tài khoản
      accountName: "",  //Chủ tài khoản
      branchName: "", //Chi nhánh
      bankDetail: "",  //Diễn giải
      createdById: this.emptyGuid,
      createdDate: new Date()
    };

    let ref = this.dialogService.open(BankpopupComponent, {
      data: {
        bankAccount: bankAccount,
      },
      header: 'Thêm thông tin thanh toán',
      width: '55%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "190px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.listBankAccount = result.listBankAccount;
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo mới thành công' };
          this.showMessage(msg);
        }
      }
    });
  }

  /*Event Sửa tài khoản thanh toán*/
  editBankAccount(rowData: BankAccount) {
    let bankAccount: BankAccount = {
      bankAccountId: rowData.bankAccountId,
      objectId: this.customerId,
      objectType: rowData.objectType,
      bankName: rowData.bankName, //Tên ngân hàng
      accountNumber: rowData.accountNumber,  //Số tài khoản
      accountName: rowData.accountName,  //Chủ tài khoản
      branchName: rowData.bankName, //Chi nhánh
      bankDetail: rowData.bankDetail,  //Diễn giải
      createdById: rowData.createdById,
      createdDate: rowData.createdDate
    };

    let ref = this.dialogService.open(BankpopupComponent, {
      data: {
        bankAccount: bankAccount,
      },
      header: 'Cập nhật thông tin thanh toán',
      width: '55%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "190px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.listBankAccount = result.listBankAccount;
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật thành công' };
          this.showMessage(msg);
        }
      }
    });
  }

  /*Event Xóa tài khoản thanh toán*/
  deleteBankAccount(rowData: BankAccount) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa thông tin thanh toán này?',
      accept: () => {
        this.bankService.deleteBankById(rowData.bankAccountId, rowData.objectId, rowData.objectType)
          .subscribe(response => {
            let result: any = response;

            if (result.statusCode == 200) {
              this.listBankAccount = result.listBankAccount;
              let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa thành công' };
              this.showMessage(msg);
            } else {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          });
      }
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  goToOrderDetail(orderId) {
    this.router.navigate(['/order/order-detail', { customerOrderID: orderId }]);
  }

  /*Chọn tháng trước*/
  preMonth() {
    this.isExist = false;
    //Chuyển tháng từ string -> number
    let current_month = parseFloat(this.month);

    if (current_month != 1) {
      //Nếu không phải tháng 1 thì trừ đi 1
      current_month = current_month - 1;
    } else {
      //Nếu là tháng thì chuyển thành tháng 12
      current_month = 12;
      //Giảm năm đi 1
      this.year = this.year - 1;
      this.yearMeeting = this.yearMeeting - 1;
    }
    //Chuyển lại thành string
    this.month = current_month.toString().length == 1 ? ('0' + current_month.toString()) : current_month.toString();
    this.monthMeeting = current_month.toString().length == 1 ? ('0' + current_month.toString()) : current_month.toString();

    //Lấy lại các ngày trong tháng
    this.getDateByTime(parseFloat(this.month), this.year, 'cus_care');
    this.getDateByTime(parseFloat(this.monthMeeting), this.yearMeeting, 'cus_meeting');

    //Lấy dữ liệu
    this.getHistoryCustomerCare(parseFloat(this.month), this.year);
    this.getHistoryCustomerMeeting(parseFloat(this.monthMeeting), this.yearMeeting);
  }

  /*Chọn tháng tiếp theo*/
  nextMonth() {
    //Chuyển tháng từ string -> number
    let current_month = parseFloat(this.month);

    //Kiểm tra nếu là tháng hiện tại và năm hiện tại thì không next tiếp
    if (current_month != ((new Date).getMonth() + 1) || this.year != (new Date).getFullYear()) {
      this.isExist = false;
      if (current_month != 12) {
        //Nếu không phải tháng 12 thì cộng thêm 1
        current_month = current_month + 1;
      } else {
        //Nếu là tháng 12 thì chuyển thành tháng 1
        current_month = 1;
        //Tăng năm thêm 1
        this.year = this.year + 1;
        this.yearMeeting = this.yearMeeting + 1;
      }
      //Chuyển lại thành string
      this.month = current_month.toString().length == 1 ? ('0' + current_month.toString()) : current_month.toString();
      this.monthMeeting = current_month.toString().length == 1 ? ('0' + current_month.toString()) : current_month.toString();

      //Lấy lại các ngày trong tháng
      this.getDateByTime(parseFloat(this.month), this.year, 'cus_care');
      this.getDateByTime(parseFloat(this.monthMeeting), this.yearMeeting, 'cus_meeting');

      //Lấy dữ liệu
      this.getHistoryCustomerCare(parseFloat(this.month), this.year);
      this.getHistoryCustomerMeeting(parseFloat(this.monthMeeting), this.yearMeeting);

    }
  }

  getHistoryCustomerCare(month: number, year: number) {
    this.loading = true;
    this.customerService.getHistoryCustomerCare(month, year, this.customerId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listCustomerCareInfor = result.listCustomerCareInfor;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  send_quick_email() {
    let ref = this.dialogService.open(TemplateQuickEmailComponent, {
      data: {
        sendTo: this.contactModel.Email,
        customerId: this.customerId
      },
      header: 'Gửi Email',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "190px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.month = ((new Date).getMonth() + 1).toString();
          this.year = (new Date).getFullYear();
          this.getHistoryCustomerCare((new Date).getMonth() + 1, this.year);

          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Gửi email thành công' };
          this.showMessage(msg);
        }
      }
    });
  }

  send_quick_sms() {
    let ref = this.dialogService.open(TemplateQuickSmsComponent, {
      data: {
        sendTo: this.contactModel.Phone,
        customerId: this.customerId
      },
      header: 'Gửi SMS',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "190px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.month = ((new Date).getMonth() + 1).toString();
          this.year = (new Date).getFullYear();
          this.getHistoryCustomerCare((new Date).getMonth() + 1, this.year);

          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Gửi SMS thành công' };
          this.showMessage(msg);
        }
      }
    });
  }

  send_quick_gift() {
    let ref = this.dialogService.open(TemplateQuickGiftComponent, {
      data: {
        customerType: this.customerModel.CustomerType,
        customerId: this.customerId
      },
      header: 'Tặng quà',
      width: '700px',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.month = ((new Date).getMonth() + 1).toString();
          this.year = (new Date).getFullYear();
          this.getHistoryCustomerCare((new Date).getMonth() + 1, this.year);

          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo Chương trình CSKH thành công' };
          this.showMessage(msg);
        }
      }
    });
  }

  showPreview(week: CustomerCareForWeek) {
    switch (week.type) {
      case 1:
        this.loading = true;
        this.customerService.getDataPreviewCustomerCare('SMS', this.customerId, week.customerCareId).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            this.previewCustomerCare.effecttiveFromDate = result.effecttiveFromDate;
            this.previewCustomerCare.effecttiveToDate = result.effecttiveToDate;
            this.previewCustomerCare.sendDate = result.sendDate;
            this.previewCustomerCare.statusName = result.statusName;
            this.previewCustomerCare.previewSmsContent = result.previewSmsContent;
            this.previewSMS = true; //Mở popup
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
        break;

      case 2:
        this.loading = true;
        this.customerService.getDataPreviewCustomerCare('Email', this.customerId, week.customerCareId).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            this.previewCustomerCare.effecttiveFromDate = result.effecttiveFromDate;
            this.previewCustomerCare.effecttiveToDate = result.effecttiveToDate;
            this.previewCustomerCare.sendDate = result.sendDate;
            this.previewCustomerCare.statusName = result.statusName;
            this.previewCustomerCare.previewEmailTitle = result.previewEmailTitle;
            this.previewCustomerCare.previewEmailContent = result.previewEmailContent;
            this.previewEmail = true; //Mở popup
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
        break;

      case 3:
        this.loading = true;
        this.customerCareFeedBack.customerCareId = week.customerCareId;
        this.customerService.getDataCustomerCareFeedBack(week.customerCareId, this.customerId).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            this.dataDialogCustomerFeedBack.name = result.name;
            this.dataDialogCustomerFeedBack.fromDate = result.fromDate;
            this.dataDialogCustomerFeedBack.toDate = result.toDate;
            this.dataDialogCustomerFeedBack.typeName = result.typeName;
            this.dataDialogCustomerFeedBack.feedBackCode = result.feedBackCode;
            this.dataDialogCustomerFeedBack.feedbackContent = result.feedBackContent;
            this.listFeedBackCode = result.listFeedBackCode;

            let toSelectedFeedBackCode = this.listFeedBackCode.find(x => x.categoryId == result.feedBackCode);
            if (toSelectedFeedBackCode) {
              this.feedbackCodeControl.setValue(toSelectedFeedBackCode);
            }

            this.feedbackContentControl.setValue(result.feedBackContent);

            this.feedback = true; //Mở popup
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
        break;

      case 4:
        this.loading = true;
        this.customerCareFeedBack.customerCareId = week.customerCareId;
        this.customerService.getDataCustomerCareFeedBack(week.customerCareId, this.customerId).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            this.dataDialogCustomerFeedBack.name = result.name;
            this.dataDialogCustomerFeedBack.fromDate = result.fromDate;
            this.dataDialogCustomerFeedBack.toDate = result.toDate;
            this.dataDialogCustomerFeedBack.typeName = result.typeName;
            this.dataDialogCustomerFeedBack.feedBackCode = result.feedBackCode;
            this.dataDialogCustomerFeedBack.feedbackContent = result.feedBackContent;
            this.listFeedBackCode = result.listFeedBackCode;

            let toSelectedFeedBackCode = this.listFeedBackCode.find(x => x.categoryId == result.feedBackCode);
            if (toSelectedFeedBackCode) {
              this.feedbackCodeControl.setValue(toSelectedFeedBackCode);
            }

            this.feedbackContentControl.setValue(result.feedBackContent);

            this.feedback = true; //Mở popup
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
        break;

      default:
        break;
    }
  }

  awaitSaveFeedBack: boolean = false;
  saveFeedBack() {
    if (!this.feedbackForm.valid) {
      Object.keys(this.feedbackForm.controls).forEach(key => {
        if (this.feedbackForm.controls[key].valid == false) {
          this.feedbackForm.controls[key].markAsTouched();
        }
      });
    } else {
      let feedBackCode: Category = this.feedbackCodeControl.value;
      this.customerCareFeedBack.feedBackCode = feedBackCode.categoryId;
      this.customerCareFeedBack.feedBackContent = this.feedbackContentControl.value;
      this.customerCareFeedBack.customerId = this.customerId;

      this.loading = true;
      this.awaitSaveFeedBack = true;
      this.customerService.saveCustomerCareFeedBack(this.customerCareFeedBack).subscribe(response => {
        let result1: any = response;

        if (result1.statusCode == 200) {
          let month = parseFloat(this.month);
          this.customerService.getHistoryCustomerCare(month, this.year, this.customerId).subscribe(response => {
            let result: any = response;
            this.loading = false;

            if (result.statusCode == 200) {
              this.listCustomerCareInfor = result.listCustomerCareInfor;
            } else {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          });

          this.closeFeedBack();
        } else {
          this.loading = false;
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result1.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  closePreview(type: string) {
    switch (type) {
      case "Email":
        this.previewEmail = false;
        break;

      case "SMS":
        this.previewSMS = false;
        break;
    }

    this.previewCustomerCare = {
      effecttiveFromDate: new Date(),
      effecttiveToDate: new Date(),
      sendDate: new Date(),
      statusName: '',
      previewEmailName: '',
      previewEmailTitle: '',
      previewEmailContent: '',
      previewSmsContent: '',
      previewSmsPhone: '',
    };
  }

  closeFeedBack() {
    this.feedback = false;
    this.dataDialogCustomerFeedBack = {
      name: '',
      fromDate: new Date(),
      toDate: new Date(),
      typeName: '',
      feedBackCode: '',
      feedbackContent: ''
    };
    this.customerCareFeedBack = {
      customerCareFeedBackId: this.emptyGuid,
      feedBackFromDate: new Date(),
      feedBackToDate: new Date(),
      feedBackType: '',
      feedBackCode: '',
      feedBackContent: '',
      customerId: '',
      customerCareId: '',
    }
    this.feedbackForm.reset();
    this.feedbackCodeControl.reset();
    this.feedbackContentControl.reset();
    this.awaitSaveFeedBack = false;
  }

  customerCodePattern(event: any) {
    const pattern = /^[a-zA-Z0-9-]$/;
    const inputChar = String.fromCharCode(event.charCode);
    if (!pattern.test(inputChar)) {
      event.preventDefault();
    }
  }

  /*Chọn tháng trước*/
  preMonthMeeting() {
    //Chuyển tháng từ string -> number
    let current_month = parseFloat(this.monthMeeting);

    if (current_month != 1) {
      //Nếu không phải tháng 1 thì trừ đi 1
      current_month = current_month - 1;
    } else {
      //Nếu là tháng thì chuyển thành tháng 12
      current_month = 12;
      //Giảm năm đi 1
      this.yearMeeting = this.yearMeeting - 1;
    }
    //Chuyển lại thành string
    this.monthMeeting = current_month.toString().length == 1 ? ('0' + current_month.toString()) : current_month.toString();

    //Lấy lại các ngày trong tháng
    this.getDateByTime(parseFloat(this.monthMeeting), this.yearMeeting, 'cus_meeting');

    //Lấy dữ liệu
    this.getHistoryCustomerMeeting(parseFloat(this.monthMeeting), this.yearMeeting);
  }

  /*Chọn tháng tiếp theo*/
  nextMonthMeeting() {
    //Chuyển tháng từ string -> number
    let current_month = parseFloat(this.monthMeeting);

    if (current_month != 12) {
      //Nếu không phải tháng 12 thì cộng thêm 1
      current_month = current_month + 1;
    } else {
      //Nếu là tháng 12 thì chuyển thành tháng 1
      current_month = 1;
      //Tăng năm thêm 1
      this.yearMeeting = this.yearMeeting + 1;
    }
    //Chuyển lại thành string
    this.monthMeeting = current_month.toString().length == 1 ? ('0' + current_month.toString()) : current_month.toString();

    //Lấy lại các ngày trong tháng
    this.getDateByTime(parseFloat(this.monthMeeting), this.yearMeeting, 'cus_meeting');

    //Lấy dữ liệu
    this.getHistoryCustomerMeeting(parseFloat(this.monthMeeting), this.yearMeeting);
  }

  getHistoryCustomerMeeting(month: number, year: number) {
    this.loading = true;
    this.customerService.getHistoryCustomerMeeting(month, year, this.customerId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.customerMeetingInfor = result.customerMeetingInfor;
        if (this.customerMeetingInfor.week1.length != 0 || this.customerMeetingInfor.week2.length != 0 || this.customerMeetingInfor.week3.length != 0
          || this.customerMeetingInfor.week4.length != 0 || this.customerMeetingInfor.week5.length != 0) {
          this.isExist = true;
        }
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  add_meeting() {
    let ref = this.dialogService.open(MeetingDialogComponent, {
      data: {
        customerId: this.customerId,
        customerMeetingId: null,
        listParticipants: this.listParticipants
      },
      header: 'Tạo lịch hẹn',
      width: '700px',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.getHistoryCustomerMeeting(parseFloat(this.monthMeeting), this.yearMeeting);
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo lịch hẹn thành công' };
          this.showMessage(msg);
        }
      }
    });
  }

  showPreviewMeeting(week: CustomerMeetingForWeek) {
    let ref = this.dialogService.open(MeetingDialogComponent, {
      data: {
        customerId: this.customerId,
        customerMeetingId: week.customerMeetingId,
        listParticipants: this.listParticipants
      },
      header: 'Cập nhật lịch hẹn',
      width: '700px',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.getHistoryCustomerMeeting(parseFloat(this.monthMeeting), this.yearMeeting);
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật lịch hẹn thành công' };
          this.showMessage(msg);
        }
      }
    });
  }

  getDateByTime(month: number, year: number, mode: string) {
    switch (mode) {
      case 'cus_care':
        this.startDateWeek1 = new Date();
        this.startDateWeek1.setDate(1);
        this.startDateWeek1.setMonth(month - 1);
        this.startDateWeek1.setFullYear(year);
        let dateInMonth = this.getDateInMonth(this.startDateWeek1);
        this.endDateWeek1 = dateInMonth.endDateWeek1;
        this.startDateWeek2 = dateInMonth.startDateWeek2;
        this.endDateWeek2 = dateInMonth.endDateWeek2;
        this.startDateWeek3 = dateInMonth.startDateWeek3;
        this.endDateWeek3 = dateInMonth.endDateWeek3;
        this.startDateWeek4 = dateInMonth.startDateWeek4;
        this.endDateWeek4 = dateInMonth.endDateWeek4;
        this.startDateWeek5 = dateInMonth.startDateWeek5;
        this.endDateWeek5 = dateInMonth.endDateWeek5;
        break;
      case 'cus_meeting':
        this.startDateWeek1Meeting = new Date();
        this.startDateWeek1Meeting.setDate(1);
        this.startDateWeek1Meeting.setMonth(month - 1);
        this.startDateWeek1Meeting.setFullYear(year);
        let dateInMonthMeeting = this.getDateInMonth(this.startDateWeek1Meeting);
        this.endDateWeek1Meeting = dateInMonthMeeting.endDateWeek1;
        this.startDateWeek2Meeting = dateInMonthMeeting.startDateWeek2;
        this.endDateWeek2Meeting = dateInMonthMeeting.endDateWeek2;
        this.startDateWeek3Meeting = dateInMonthMeeting.startDateWeek3;
        this.endDateWeek3Meeting = dateInMonthMeeting.endDateWeek3;
        this.startDateWeek4Meeting = dateInMonthMeeting.startDateWeek4;
        this.endDateWeek4Meeting = dateInMonthMeeting.endDateWeek4;
        this.startDateWeek5Meeting = dateInMonthMeeting.startDateWeek5;
        this.endDateWeek5Meeting = dateInMonthMeeting.endDateWeek5;
        break;
    }
  }

  getDateInMonth(date: Date): DateInMonth {
    let result: Date = new Date();
    let day = date.getDay() + 1;  //Ngày trong tuần (Chủ nhật = 1)
    let dateInMonth: DateInMonth = {
      startDateWeek1: new Date(),
      endDateWeek1: new Date(),
      startDateWeek2: new Date(),
      endDateWeek2: new Date(),
      startDateWeek3: new Date(),
      endDateWeek3: new Date(),
      startDateWeek4: new Date(),
      endDateWeek4: new Date(),
      startDateWeek5: new Date(),
      endDateWeek5: new Date(),
    }

    switch (day) {
      case 1:
        result = date;
        break;
      case 2:
        let timeMore1 = 6 * 24 * 60 * 60 * 1000;
        result.setTime(date.getTime() + timeMore1);
        break;
      case 3:
        let timeMore2 = 5 * 24 * 60 * 60 * 1000;
        result.setTime(date.getTime() + timeMore2);
        break;
      case 4:
        let timeMore3 = 4 * 24 * 60 * 60 * 1000;
        result.setTime(date.getTime() + timeMore3);
        break;
      case 5:
        let timeMore4 = 3 * 24 * 60 * 60 * 1000;
        result.setTime(date.getTime() + timeMore4);
        break;
      case 6:
        let timeMore5 = 2 * 24 * 60 * 60 * 1000;
        result.setTime(date.getTime() + timeMore5);
        break;
      case 7:
        let timeMore6 = 1 * 24 * 60 * 60 * 1000;
        result.setTime(date.getTime() + timeMore6);
        break;
    }

    //Tính ngày đầu tuần thứ 2
    let last_date_week1_number = result.getDate();
    let first_date_week2_number = last_date_week1_number + 1;
    let temp1 = new Date(result);
    temp1.setDate(first_date_week2_number);
    dateInMonth.startDateWeek2 = temp1;
    //Tính ngày cuối tuần thứ 2
    let last_date_week2_number = first_date_week2_number + 6;
    let temp2 = new Date(dateInMonth.startDateWeek2);
    temp2.setDate(last_date_week2_number);
    dateInMonth.endDateWeek2 = temp2;

    //Tính ngày đầu tuần thứ 3
    let first_date_week3_number = last_date_week2_number + 1;
    let temp3 = new Date(dateInMonth.endDateWeek2);
    temp3.setDate(first_date_week3_number);
    dateInMonth.startDateWeek3 = temp3;
    //Tính ngày cuối tuần thứ 3
    let last_date_week3_number = first_date_week3_number + 6;
    let temp4 = new Date(dateInMonth.startDateWeek3);
    temp4.setDate(last_date_week3_number);
    dateInMonth.endDateWeek3 = temp4;

    //Tính ngày đầu tuần thứ 4
    let first_date_week4_number = last_date_week3_number + 1;
    let temp5 = new Date(dateInMonth.endDateWeek3);
    temp5.setDate(first_date_week4_number);
    dateInMonth.startDateWeek4 = temp5;
    //Tính ngày cuối tuần thứ 4
    let last_date_week4_number = first_date_week4_number + 6;
    let temp6 = new Date(dateInMonth.startDateWeek4);
    temp6.setDate(last_date_week4_number);
    dateInMonth.endDateWeek4 = temp6;

    //Kiểm tra xem ngày cuối của tuần thứ 4 có phải ngày cuối cùng của tháng hay không?
    let current_month = result.getMonth();
    let current_year = result.getFullYear();
    let first_date_of_next_month = new Date();
    first_date_of_next_month.setDate(1);
    first_date_of_next_month.setMonth(current_month + 1);
    if (current_month == 11) {
      current_year = current_year + 1;
    }
    first_date_of_next_month.setFullYear(current_year);
    let last_date_of_month = new Date(first_date_of_next_month);
    let time_number = first_date_of_next_month.getTime();
    last_date_of_month.setTime(time_number - 24 * 60 * 60 * 1000);

    if (dateInMonth.endDateWeek4.getDate() != last_date_of_month.getDate()) {
      //Ngày cuối của tuần thứ 4 không phải ngày cuối của tháng nên sẽ có tuần thứ 5
      //Tính ngày đầu tuần thứ 5
      let first_date_week5_number = last_date_week4_number + 1;
      let temp6 = new Date(dateInMonth.endDateWeek4);
      temp6.setDate(first_date_week5_number);
      dateInMonth.startDateWeek5 = temp6;
      //Ngày cuối tuần thứ 5 chắc chắn là ngày cuối tháng
      dateInMonth.endDateWeek5 = last_date_of_month;
    } else {
      dateInMonth.startDateWeek5 = null;
      dateInMonth.endDateWeek5 = null;
    }

    dateInMonth.endDateWeek1 = result;

    return dateInMonth;
  }

  patchGeoCodingForMap(lat: number, long: number, customerName: string) {
    if (!lat && !long) return;
    this.overlays = [
      new google.maps.Marker({ position: { lat: lat, lng: long }, title: customerName }),
    ];

    let bounds = new google.maps.LatLngBounds();
    this.overlays.forEach(marker => {
      bounds.extend(marker.getPosition());
    });

    if (this.map) {
      setTimeout(() => { // map will need some time to load
        this.map.setCenter(bounds.getCenter());
        this.map.setZoom(this.zoom);
      }, 1);
    }
  }

  async getCustomerGeoCoding() {
    let address = '';
    let customerName = this.customerNameLabel;

    let customerType = Number(this.customerModel.CustomerType);
    switch (customerType) {
      case 1:
        //nối địa chỉ khách hàng doanh nghiệp
        let area: Area = this.cusAreaControl.value;
        let provice: Province = this.cusProvinceControl.value;
        let district: District = this.cusDistrictControl.value;
        let ward: Ward = this.cusWardControl.value;
        let detailAddress = this.cusAddressControl.value;

        let proviceName = provice ? provice.provinceName : "";
        let districtName = district ? district.districtName : "";
        let wardName = ward ? ward.wardName : "";

        address = `${detailAddress},${wardName},${districtName},${proviceName}`;
        break
      case 2:
        //nối địa chỉ khách hàng cá nhân
        let areaCusPer: Area = this.cusPerContAreaControl.value;
        let proviceCusPer: Province = this.cusPerContProvinceControl.value;
        let districtCusPer: District = this.cusPerContDistrictControl.value;
        let wardCusPer: Ward = this.cusPerContWardControl.value;
        let detailAddressCusPer = this.cusPerContAddressControl.value;

        let proviceCusPerName = proviceCusPer ? proviceCusPer.provinceName : "";
        let districtCusPerName = districtCusPer ? districtCusPer.districtName : "";
        let wardCusPerName = wardCusPer ? wardCusPer.wardName : "";

        address = `${detailAddressCusPer},${wardCusPerName},${districtCusPerName},${proviceCusPerName}`;
        break
    }
    //reset map position
    this.overlays = [];

    if (!address) return;

    try {
      let geoCodingResponse = await this.googleService.getGeoCoding(address);
      let listMatchPosition = geoCodingResponse.data.results;
      //lấy giá trị đầu tiên
      let position = listMatchPosition[0];
      this.overlays = [
        new google.maps.Marker({ position: { lat: position.geometry.lat, lng: position.geometry.lng }, title: customerName }),
      ];

      let bounds = new google.maps.LatLngBounds();
      this.overlays.forEach(marker => {
        bounds.extend(marker.getPosition());
      });

      if (this.map) {
        setTimeout(() => { // map will need some time to load
          this.map.setCenter(bounds.getCenter());
          this.map.setZoom(this.zoom);
        }, 1);
      }
    } catch (error) {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy vị trí' };
      this.showMessage(msg);
    }
  }

  async getCustomerGeoCodingUpdateContact(address, customerName) {
    if (!address) {
      this.overlays = [];
      return;
    }

    try {
      let geoCodingResponse = await this.googleService.getGeoCoding(address);
      let listMatchPosition = geoCodingResponse.data.results;
      //lấy giá trị đầu tiên
      let position = listMatchPosition[0];
      this.overlays = [
        new google.maps.Marker({ position: { lat: position.geometry.lat, lng: position.geometry.lng }, title: customerName }),
      ];

      let bounds = new google.maps.LatLngBounds();
      this.overlays.forEach(marker => {
        bounds.extend(marker.getPosition());
      });

      if (this.map) {
        setTimeout(() => { // map will need some time to load
          this.map.setCenter(bounds.getCenter());
          this.map.setZoom(this.zoom);
        }, 1000);
      }
    } catch (error) {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy vị trí' };
      this.showMessage(msg);
    }
  }

  checkDuplicateInforCustomer(checkType: number, formControl: FormControl) {
    if (formControl.valid) {
      let email = '';
      let phone = '';

      //Nếu là khách hàng doanh nghiệp
      if (Number(this.customerModel.CustomerType) == 1)
      {
        email = this.cusEmailControl.value;
        phone = this.cusPhoneControl.value;
      }
      //Nếu là khách hàng cá nhân
      else if (Number(this.customerModel.CustomerType) == 2)
      {
        email = this.cusPerContEmailControl.value;
        phone = this.cusPerContPhoneControl.value;
      }

      this.customerService.checkDuplicateInforCustomer(this.customerId, Number(checkType), email, phone).subscribe(response => {
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
}

function checkDuplicateCode(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null) {
      if (array.indexOf(control.value.toLowerCase()) !== -1 && control.value.toLowerCase() !== "") {
        return { 'checkDuplicateCode': true };
      }
      return null;
    }
  }
}
function checkDuplicateCustomeCode(array: Array<any>, customerCode: any): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined && customerCode !== null && customerCode !== undefined) {
      if (customerCode.toLowerCase() !== control.value.toLowerCase()) {
        if (control.value.trim() !== "") {
          let duplicateCode = array.find(e => e === control.value.trim());
          if (duplicateCode !== undefined) {
            return { 'duplicateCode': true };
          }
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

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

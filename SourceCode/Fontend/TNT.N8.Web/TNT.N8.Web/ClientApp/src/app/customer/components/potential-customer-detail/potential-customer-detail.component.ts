import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener, AfterViewInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import * as $ from 'jquery';
import { TranslateService } from '@ngx-translate/core';
import { CustomerService } from '../../services/customer.service';
import { Time } from "@angular/common";
import { ForderConfigurationService } from '../../../admin/components/folder-configuration/services/folder-configuration.service';
import { LeadService } from "../../../lead/services/lead.service";
import { ContactpopupComponent } from '../../../shared/components/contactpopup/contactpopup.component';
import { PontentialCustomerQuoteComponent } from '../pontential-customer-quote/pontential-customer-quote.component';
import { AddQuestionAnswerDialogComponent } from '../add-question-answer-dialog/add-question-answer-dialog.component';
import { TemplateQuickEmailComponent } from '../template-quick-email/template-quick-email.component';
import { TemplateQuickSmsComponent } from '../template-quick-sms/template-quick-sms.component';
import { TemplateQuickGiftComponent } from '../template-quick-gift/template-quick-gift.component';
import { MeetingDialogComponent } from '../../components/meeting-dialog/meeting-dialog.component';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { NoteService } from '../../../shared/services/note.service';

import { LeadModel } from "../../../lead/models/lead.model";
import { ContactModel, contactModel } from "../../../shared/models/contact.model";
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { NoteModel } from '../../../shared/models/note.model';

import { GoogleService } from '../../../shared/services/google.service'

import { HttpClient } from '@angular/common/http';

import { QuickCreateProductComponent } from '../../../shared/components/quick-create-product/quick-create-product.component';
import { CustomerCareService } from '../../services/customer-care.service';
import { PotentialConversionDialogComponent } from '../potential-conversion-dialog/potential-conversion-dialog.component';

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
}

declare var google: any;

class customerContactModel {
  customerId: string;
  customerFullName: string;
  email: string;
  phone: string;
  address: string;
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

class careState {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class leadReferenceCustomer {
  customerCode: string;
  customerId: string;
  customerName: string;
  customerType: number;
  personInChargeId: string;
}

class probability {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
}

class DateInMonth {
  startDateWeek1: Date;
  endDateWeek1: Date;
  startDateWeek2: Date;
  endDateWeek2: Date;
  startDateWeek3: Date;
  endDateWeek3: Date;
  startDateWeek4: Date;
  endDateWeek4: Date;
  startDateWeek5: Date;
  endDateWeek5: Date;
}

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

class investFundModel {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

class personalInChange {
  employeeCode: string;
  employeeId: string;
  employeeName: string;
}

class genderModel {
  label: string;
  value: string;
}

class linkOfDocumentRequest {
  LinkOfDocumentId: string;
  LinkName: string;
  LinkValue: string;
  ObjectId: string;
  ObjectType: string;
  Active: boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: string;
  CreatedByName: string;
  IsNewLink: boolean; // phân biệt link mới hoặc link cũ
}

class linkOfDocumentResponse {
  linkOfDocumentId: string;
  linkName: string;
  linkValue: string;
  objectId: string;
  objectType: string;
  active: boolean;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: string;
  createdByName: string;
  typeDocument: string; //"DOC": tài liệu; "LINK": liên kết
  name: string;
  isNewLink: boolean; // phân biệt link mới hoặc link cũ
}

class customerRequestModel {
  CustomerId: string;
  ContactId: string;
  CustomerCode: string;
  CustomerGroupId: string;
  CustomerName: string;
  LeadId: string;
  StatusId: string;
  CustomerServiceLevelId: string;
  CustomerServiceLevelName: string;
  PersonInChargeId: string;
  CustomerCareStaff: string;
  CustomerType: number;
  PaymentId: string;
  FieldId: string;
  ScaleId: string;
  TotalSaleValue: number;
  TotalReceivable: number;
  NearestDateTransaction: Date;
  MaximumDebtValue: number;
  MaximumDebtDays: number;
  MainBusinessSector: string;
  BusinessRegistrationDate: Date;
  EnterpriseType: string;
  BusinessType: string;
  IsGraduated: boolean;
  TotalEmployeeParticipateSocialInsurance: number;
  BusinessScale: string;
  TotalCapital: number;
  TotalRevenueLastYear: number;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: string;
  Active: boolean;
  IsApproval: boolean;
  ApprovalStep: number;
  InvestmentFundId: string;
  AllowSendEmail: boolean;
  AllowCall: boolean;
  CareStateId: string;
  EmployeeTakeCareId: string;
  ContactDate: Date;
  SalesUpdate: string;
  EvaluateCompany: string;
  NoteCompany: string;
  SalesUpdateAfterMeeting: string;
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
  noteCompany: string;
  salesUpdateAfterMeeting: string;

  constructor() {
    this.customerName = '';
    this.isConverted = true;
  }
}

class contactRequestModel {
  ContactId: string;
  ObjectId: string;
  ObjectType: string;
  FirstName: string;
  LastName: string;
  Gender: string;
  GenderDisplay: string;
  DateOfBirth: Date;
  Phone: string;
  WorkPhone: string;
  OtherPhone: string;
  Email: string;
  WorkEmail: string;
  OtherEmail: string;
  IdentityID: string;
  AvatarUrl: string;
  Address: string;
  CountryId: string;
  ProvinceId: string;
  DistrictId: string;
  WardId: string;
  MaritalStatusId: string;
  PostCode: string;
  WebsiteUrl: string;
  SocialUrl: string;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: Boolean;
  Note: string;
  Role: string;
  TaxCode: string;
  Job: string;
  Agency: string;
  Birthplace: string;
  IdentityIDDateOfIssue: Date;
  IdentityIDPlaceOfIssue: string;
  IdentityIDDateOfParticipation: Date;
  WorkPermitNumber: string;
  VisaNumber: string;
  VisaDateOfIssue: Date;
  VisaExpirationDate: Date;
  SocialInsuranceNumber: string;
  SocialInsuranceDateOfIssue: Date;
  SocialInsuranceDateOfParticipation: Date;
  HealthInsuranceNumber: string;
  HealthInsuranceDateOfIssue: Date;
  HealthInsuranceDateOfParticipation: Date;
  WorkHourOfStart: Time;
  WorkHourOfEnd: Time;
  TypePaid: string;
  Other: string;
  CompanyName: string;
  CompanyAddress: string;
  CustomerPosition: string;
  BankName: string;
  BankCode: string;
  MoneyLimit: number;
  TermsPayment: string;
  DefaultAccount: string;
  OptionPosition: string;
  RelationShip: string;
  PotentialCustomerPosition: string;
  Latitude: number;
  Longitude: number;
  GeographicalAreaId: string;
  LinkFace: string;
  evaluateContactPeople: string;
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
  evaluateContactPeople: string;

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

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdByName: string;
  createdDate: Date;
  dumpId: any;
  typeDocument: string; //"DOC": tài liệu; "LINK": liên kết
  linkName: string;
  linkValue: string;
  name: string;
  isNewLink: boolean; // phân biệt link mới hoặc link cũ
  linkOfDocumentId: string;
}

class potentialCustomerProductRequest {
  PotentialCustomerProductId: string;
  ProductId: string;
  CustomerId: string;
  IsInTheSystem: boolean;
  ProductName: string;
  ProductUnit: string;
  ProductFixedPrice: number;
  ProductUnitPrice: number;
  Active: boolean;
  ProductNote: string;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
}

class potentialCustomerProductResponse {
  potentialCustomerProductId: string;
  productId: string;
  customerId: string;
  prodocutCode: string;
  isInTheSystem: boolean;
  productName: string;
  productUnit: string;
  productFixedPrice: number;
  productUnitPrice: number;
  active: boolean;
  productNote: string;
  createdById: string;
  createdDate: Date;
  UpdatedById: string;
  updatedDate: Date;
  selectedProduct: productFixedPrice;
}

class productFixedPrice {
  productId: string;
  productName: string;
  productCode: string;
  fixedPrice: number;
}

class quote {
  public quoteId: string;
  public quoteCode: string;
  public quoteDate: Date;
  public effectiveQuoteDate: string;
  public dueQuoteDate: Date;
  public amount: number;
  public quoteStatusName: string;
  public note: string;
}

class lead {
  public leadId: string;
  public leadCode: string;
  public statusName: string;
  public requirementDetail: string;
  public expectedSale: string;
  public personInChargeFullName: string;
  public leadCodeName: string;
}

class CustomerCareInfor {
  employeeName: string;
  employeePosition: string;
  employeeCharge: string;
  week1: Array<CustomerCareForWeek>;
  week2: Array<CustomerCareForWeek>;
  week3: Array<CustomerCareForWeek>;
  week4: Array<CustomerCareForWeek>;
  week5: Array<CustomerCareForWeek>;
}

class CustomerCareForWeek {
  customerCareId: string;
  employeeCharge: string;
  title: string;
  type: number;
  feedBackStatus: number;
  background: string;
  subtitle: string;
  activeDate: Date;
}

class CustomerMeetingInfor {
  employeeName: string;
  employeePosition: string;
  employeeId: string;
  week1: Array<CustomerMeetingForWeek>;
  week2: Array<CustomerMeetingForWeek>;
  week3: Array<CustomerMeetingForWeek>;
  week4: Array<CustomerMeetingForWeek>;
  week5: Array<CustomerMeetingForWeek>;
}

class CustomerMeetingForWeek {
  customerMeetingId: string;
  employeeId: string;
  title: string;
  subTitle: string;
  background: string;
  startDate: Date;
  startHours: Date;
}

class PreviewCustomerCare {
  effecttiveFromDate: Date;
  effecttiveToDate: Date;
  sendDate: Date;
  statusName: string;
  previewEmailContent: string;
  previewEmailName: string;
  previewEmailTitle: string;
  previewSmsPhone: string;
  previewSmsContent: string;
}

class CustomerCareFeedBack {
  customerCareFeedBackId: string;
  feedBackFromDate: Date;
  feedBackToDate: Date;
  feedBackType: string;
  feedBackCode: string;
  feedBackContent: string;
  customerId: string;
  customerCareId: string;
}

class DataDialogCustomerFeedBack {
  name: string;
  fromDate: Date;
  toDate: Date;
  typeName: string;
  feedBackCode: string;
  feedbackContent: string;
}

class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefault: boolean;
}

class NoteDocument {
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

class Note {
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

class GeographicalArea {
  geographicalAreaId: string;
  geographicalAreaCode: string;
  geographicalAreaName: string;
}

class StatusSupport {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  children: Array<Category>;
  isComplete: boolean;
  isCurrent: boolean;
  isActive: boolean;

  constructor() {
    this.children = [];
    this.isCurrent = false;
    this.isComplete = false;
    this.isActive = false;
  }
}

@Component({
  selector: 'app-potential-customer-detail',
  templateUrl: './potential-customer-detail.component.html',
  styleUrls: ['./potential-customer-detail.component.css'],
  providers: [
    GoogleService, LeadService, DynamicDialogRef, DynamicDialogConfig
  ]
})
export class PotentialCustomerDetailComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  @ViewChild('fileUpload') fileUpload: FileUpload;
  @ViewChild('fileNoteUpload') fileNoteUpload: FileUpload;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  /*Khai báo biến*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  isAdmin: any = JSON.parse(localStorage.getItem("IsAdmin"));
  userFullName: any = localStorage.getItem("UserFullName");
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;

  /*  permission */
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  actionDownload: boolean = true;
  actionImport: boolean = true;
  activeState: boolean[] = [false, false, false, false, false, false];

  /* html var */
  fixed: boolean = false;
  withFiexd: string = "";
  withFiexdCol: string = "";
  withColCN: number = 0;
  withCol: number = 0;
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
      var colT = 0;
      if (this.withColCN != width) {
        colT = this.withColCN - width;
        this.withColCN = width;
        this.withCol = $('#parentTH').width();
      }
      this.withFiexdCol = (this.withCol) + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
      this.withCol = $('#parentTH').width();
      this.withColCN = $('#parent').width();
      this.withFiexdCol = "";
    }
  }

  /* Valid Form */
  customerType: number = 1;
  isInvalidForm: boolean = false;
  isInvalidPersonalForm: boolean = false;
  display: boolean = false;
  isOpenNotifiErrorPersonal: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('gmap') gmapElement: any;
  /* End */
  customerId: string = null;
  /*  declare form */
  potentialCustomerForm: FormGroup;
  potentialPersonalCustomerForm: FormGroup;
  addLinkForm: FormGroup;
  /*Form thông tin liên hệ KH Cá nhân*/
  feedbackForm: FormGroup;
  feedbackCodeControl: FormControl;
  feedbackContentControl: FormControl;
  /* master data */
  listGender: Array<genderModel> = [{ label: "Ông", value: "NAM" }, { label: "Bà", value: "NU" },]
  listPersonalInChange: Array<personalInChange> = [];
  listEmpTakeCare: Array<personalInChange> = [];
  listInvestFund: Array<investFundModel> = [];
  potentialCustomerModel: customerResponseModel = new customerResponseModel();
  potentialCustomerContactModel: contactResponseModel;
  listFileByPotentialCustomer: Array<FileInFolder> = [];
  listLinkOfDocument: Array<linkOfDocumentResponse> = [];
  listPotentialCustomerProduct: Array<potentialCustomerProductResponse> = [];
  listProductMasterdata: Array<productFixedPrice> = [];
  listQuoteByPotentialCustomer: Array<quote> = [];
  ListLeadByPotentialCustomer: Array<lead> = [];
  listCustomerCareInfor: Array<CustomerCareInfor> = [];
  listContact: Array<contactResponseModel> = [];
  listArea: Array<GeographicalArea> = [];
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

  //master data
  listCustomerContact: Array<customerContactModel> = [];
  listEmailLead: Array<string> = [];
  //  listGender: Array<genderModel> = [];
  listInterestedGroup: Array<interestedGroupModel> = [];
  listLeadType: Array<leadTypeModel> = [];
  listLeadGroup: Array<leadGroup> = [];
  //  listPersonalInChange: Array<Employee> = [];
  listPersonalInChangeCus: Array<Employee> = [];
  listPhoneLead: Array<string> = [];
  listPotential: Array<potentialModel> = [];
  //new
  listBusinessType: Array<businessType> = [];
  listCareState: Array<careState> = [];
  // listInvestFund: Array<investFund> = [];
  listProbability: Array<probability> = [];
  listLeadReferenceCustomer: Array<leadReferenceCustomer> = [];
  listCurrentReferenceCustomer: Array<leadReferenceCustomer> = [];
  listCusGroup: Array<Category> = [];

  //Trạng thái phụ
  listStatusSupport: Array<Category> = [];
  listTempStatusSupport: Array<Category> = [];
  selectedTempStatusSupport: Category = null;
  listFormatStatusSupport: Array<StatusSupport> = [];
  isConvert: boolean = false;

  /* dialog varriable */
  displayAttachFile: boolean = false;
  displayAttachLink: boolean = false;

  /* file section */
  colsDocument: any = [];
  colsProduct: any = [];
  colsQuote: any = [];
  colsLead: any = [];
  colsContact: any = [];
  listDocument: Array<FileInFolder> = [];
  uploadedFiles: any[] = [];
  listDocumentIdNeedRemove: Array<string> = [];
  listParticipants: Array<Employee> = [];

  /* convert dialog forms */
  converdialog_isCreateCustomer: boolean = true;
  converdialog_isCreatelead: boolean = false;

  /* customer care section */
  month: string = '';
  year: number = (new Date()).getFullYear();
  isExist: boolean = true;

  //note
  colsFile: any;
  noteContent: string = '';
  noteId: string = null;
  isEditNote: boolean = false;
  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  noteHistory: Array<Note> = [];
  //dialog
  potentialCustomerFullName: string = '';
  //map
  options: any = {};
  overlays: any[] = [];
  map: google.maps.Map;
  zoom: number = 16;

  validEmailCustomer: boolean = true;
  validPhoneCustomer: boolean = true;

  isLockPersonIncharge: boolean = false;

  constructor(
    public messageService: MessageService,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private router: Router,
    private el: ElementRef,
    private customerService: CustomerService,
    private cusotmerCareService: CustomerCareService,
    private cdr: ChangeDetectorRef,
    private folderService: ForderConfigurationService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private leadService: LeadService,
    private imageService: ImageUploadService,
    private noteService: NoteService,
    private googleService: GoogleService,
  ) { }

  ngAfterViewInit() {
    this.cdr.detectChanges();
  }

  ngAfterContentChecked() {
    this.cdr.detectChanges();
  }

  setMap(event) {
    this.map = event.map;
  }

  ngOnInit(): void {
    /* Check permission */
    let resource = "crm/customer/potential-customer-detail";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
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

      this.route.params.subscribe(params => {
        this.customerId = params['customerId']
        this.initForm();
        this.initTable();
        this.getMasterData();
      });
    }
  }

  initForm() {
    //khách hàng doanh nghiệp
    this.potentialCustomerForm = new FormGroup({
      /* left content */
      'Gender': new FormControl(null), //gioi tinh
      'CustomerName': new FormControl('', [Validators.required, forbiddenSpaceText]), //ten khach hang tiem nang
      'PotentialCustomerPosition': new FormControl(''), //phong ban
      'Phone': new FormControl('', [Validators.pattern(this.getPhonePattern())]), //so dien thoai
      'OtherPhone': new FormControl('', [Validators.pattern(this.getPhonePattern())]), //so dien thoai khac
      'TaxCode': new FormControl(''),
      'WorkEmail': new FormControl('', [Validators.pattern(this.emailPattern)]),  //email co quan
      'AllowSendEmail': new FormControl(false),
      /* right content */
      'Email': new FormControl('', [Validators.pattern(this.emailPattern)]),  //email
      'WorkPhone': new FormControl('', [Validators.pattern(this.getPhonePattern())]), //so dien thoai co quan
      'Role': new FormControl(''),
      'InvestFund': new FormControl(null, [Validators.required]), //Nguon tiem nang
      'SocialLink': new FormControl(''), //link facebook
      'AllowCall': new FormControl(false),
      'Address': new FormControl(''),
      'Pic': new FormControl(null, [Validators.required]), //nguoi phu trach
      'CareState': new FormControl(null), //Trạng thái chăm sóc
      'Area': new FormControl(null), //Khu vực
      'CusGroup': new FormControl(null), //Nhóm khách hàng
      'EmployeeTakeCare': new FormControl(null),
      'ContactDate': new FormControl(null),
      'SalesUpdate': new FormControl(null),
      'EvaluateCompany': new FormControl(null),
      'NoteCompany': new FormControl(null),
      'SalesUpdateAfterMeeting': new FormControl(null),
      'Potential': new FormControl(null),
    });
    //khách hàng cá nhân
    this.potentialPersonalCustomerForm = new FormGroup({
      /* left content */
      'Gender': new FormControl(null), //gioi tinh
      'FirstName': new FormControl('', [Validators.required, forbiddenSpaceText]), //ten khach hang tiem nang
      'LastName': new FormControl('', [Validators.required, forbiddenSpaceText]), //ten khach hang tiem nang
      'PotentialCustomerPosition': new FormControl(''), //phong ban
      'Phone': new FormControl('', [Validators.pattern(this.getPhonePattern())]), //so dien thoai
      'OtherPhone': new FormControl('', [Validators.pattern(this.getPhonePattern())]), //so dien thoai khac
      'TaxCode': new FormControl(''),
      'WorkEmail': new FormControl('', [Validators.pattern(this.emailPattern)]),  //email co quan
      'AllowSendEmail': new FormControl(false),
      /* right content */
      'Email': new FormControl('', [Validators.pattern(this.emailPattern)]),  //email
      'WorkPhone': new FormControl('', [Validators.pattern(this.getPhonePattern())]), //so dien thoai co quan
      'Role': new FormControl(''),
      'InvestFund': new FormControl(null, [Validators.required]), //Nguon tiem nang
      'SocialLink': new FormControl(''), //link facebook
      'AllowCall': new FormControl(false),
      'Address': new FormControl(''),
      'Pic': new FormControl(null, [Validators.required]), //nguoi phu trach
      'CareState': new FormControl(null), //Trạng thái chăm sóc
      'Area': new FormControl(null), //Khu vực
      'CusGroup': new FormControl(null), //Nhóm khách hàng
      'NoteCompany': new FormControl(null),
      'Potential': new FormControl(null),
    });

    this.addLinkForm = new FormGroup({
      'NameOfLink': new FormControl('', [Validators.required]),
      'Link': new FormControl('', [Validators.required]),
    });

    /*FORM FEED BACK*/
    this.feedbackCodeControl = new FormControl(null, [Validators.required]);
    this.feedbackContentControl = new FormControl(null, [Validators.required]);

    this.feedbackForm = new FormGroup({
      feedbackCodeControl: this.feedbackCodeControl,
      feedbackContentControl: this.feedbackContentControl
    });
    /*END*/
  }

  resetLinkForm() {
    this.addLinkForm.reset();
    this.addLinkForm.patchValue({
      'NameOfLink': '',
      'Link': ''
    });
  }

  initTable() {
    /*Table*/
    this.colsFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'documentSize', header: 'Kích thước tài liệu', width: '50%', textAlign: 'left' },
    ];
    this.colsDocument = [
      { field: 'name', header: 'Tên tài liệu', width: '25%', textAlign: 'left', type: 'string' },
      { field: 'createdByName', header: 'Người đính kèm', width: '25%', textAlign: 'left', type: 'string' },
      { field: 'createdDate', header: 'Ngày đính kèm', width: '25%', textAlign: 'left', type: 'date' },
      { field: 'size', header: 'Dung lượng', width: '25%', textAlign: 'right', type: 'number' },
    ];

    this.colsProduct = [
      { field: 'productCode', header: 'Mã hàng hóa', width: '20%', textAlign: 'left', type: 'string' },
      { field: 'productName', header: 'Tên hàng hóa', width: '20%', textAlign: 'left', type: 'string' },
      { field: 'productUnit', header: 'Đơn vị tính', width: '20%', textAlign: 'left', type: 'string' },
      { field: 'productFixedPrice', header: 'Giá niêm yết', width: '20%', textAlign: 'right', type: 'number' },
      { field: 'productUnitPrice', header: 'Đơn giá', width: '10%', textAlign: 'right', type: 'number' },
      { field: 'productNote', header: 'Ghi chú', width: '10%', textAlign: 'left', type: 'string' },
    ];

    this.colsQuote = [
      { field: 'quoteCode', header: 'Số báo giá', width: '20%', textAlign: 'left', type: 'string' },
      { field: 'quoteDate', header: 'Ngày báo giá', width: '20%', textAlign: 'left', type: 'string' },
      { field: 'dueQuoteDate', header: 'Hiệu lực đến ngày', width: '20%', textAlign: 'right', type: 'string' },
      { field: 'amount', header: 'Tổng tiền', width: '20%', textAlign: 'right', type: 'string' },
      { field: 'quoteStatusName', header: 'Tình trạng', width: '10%', textAlign: 'center', type: 'string' },
      { field: 'quoteName', header: 'Mô tả', width: '10%', textAlign: 'left', type: 'string' },
    ];

    this.colsLead = [
      { field: 'leadCode', header: 'Mã Cơ Hội', width: '20%', textAlign: 'left', type: 'string' },
      { field: 'fullName', header: 'Tên cơ hội', width: '10%', textAlign: 'left', type: 'string' },
      { field: 'personInChargeFullName', header: 'Người phụ trách', width: '20%', textAlign: 'left', type: 'string' },
      { field: 'requirementDetail', header: 'Chi tiết yêu cầu', width: '10%', textAlign: 'left', type: 'string' },
      { field: 'expectedSale', header: 'Doanh thu mong đợi', width: '20%', textAlign: 'right', type: 'string' },
      { field: 'statusName', header: 'Tình trạng', width: '10%', textAlign: 'center', type: 'string' },
    ];

    this.colsContact = [
      { field: 'contactName', header: 'Họ và tên', width: '150px', textAlign: 'left', type: 'string' },
      { field: 'role', header: 'Chức danh', width: '100px', textAlign: 'left', type: 'string' },
      { field: 'evaluateContactPeople', header: 'Đánh giá thông tin người liên hệ', width: '300px', textAlign: 'left', type: 'string' },
      { field: 'dateOfBirth', header: 'Ngày sinh', width: '150px', textAlign: 'right', type: 'string' },
      { field: 'phone', header: 'ĐT di động', width: '150px', textAlign: 'right', type: 'string' },
      // { field: 'workPhone', header: 'ĐT cơ quan', width: '100px', textAlign: 'right', type: 'string' },
      { field: 'linkFace', header: 'Link facebook', width: '300px', textAlign: 'left', type: 'string' },
      // { field: 'workEmail', header: 'Email cơ quan', width: '150px', textAlign: 'left', type: 'string' },
      { field: 'email', header: 'Email cá nhân', width: '150px', textAlign: 'left', type: 'string' },
      { field: 'address', header: 'Địa chỉ', width: '200px', textAlign: 'left', type: 'string' },
    ];
  }

  async getMasterData() {
    this.loading = true;
    let [result, resultLead]: any = await Promise.all([
      this.customerService.getDataDetailPotentialCustomer(this.customerId, this.auth.UserId),
      this.leadService.getDataCreateLead(this.auth.UserId)
    ]);
    this.loading = false;
    if (result.statusCode === 200 && resultLead.statusCode) {
      this.listInvestFund = result.listInvestFund || [];
      this.listPersonalInChange = result.listPersonalInChange || [];
      this.potentialCustomerContactModel = result.potentialCustomerContactModel;
      this.potentialCustomerModel = result.potentialCustomerModel;
      this.listFileByPotentialCustomer = result.listFileByPotentialCustomer || [];
      this.listLinkOfDocument = result.listLinkOfDocument || [];
      this.listProductMasterdata = result.listProduct || []; //danh sach san pham dich vu cho drop down list
      this.listPotentialCustomerProduct = result.listPotentialCustomerProduct || [];
      this.listQuoteByPotentialCustomer = result.listQuoteByPotentialCustomer || [];
      this.ListLeadByPotentialCustomer = result.listLeadByPotentialCustomer || [];
      this.listContact = result.listContact || [];
      this.listParticipants = result.listParticipants;
      this.listCareState = resultLead.listCareState;
      this.listArea = result.listArea;
      this.listCusGroup = result.listCusGroup;
      this.listEmpTakeCare = result.listEmpTakeCare;

      //Build trạng thái phụ
      this.listStatusSupport = result.listStatusSupport;
      this.listTempStatusSupport = this.listStatusSupport.filter(x => x.categoryCode.indexOf('B') != -1);
      this.buildListStatusSupport(result.statusSupportId);

      this.listParticipants.forEach(item => {
        item.employeeName = item.employeeCode + ' - ' + item.employeeName;
      });

      this.customerType = this.potentialCustomerModel.customerType;
      if (this.potentialCustomerModel.customerType == 1) {
        //khách hàng doanh nghiệp
        this.patchDefaultFormValue();
      }
      if (this.potentialCustomerModel.customerType == 2) {
        this.patchDefaultPersonalFormValue();
      }

      //Danh sách thông tin CSKH
      this.listCustomerCareInfor = result.listCustomerCareInfor || [];
      //Lịch hẹn
      this.customerMeetingInfor = result.customerMeetingInfor;

      this.month = ((new Date()).getMonth() + 1).toString().length == 1 ? ('0' + ((new Date()).getMonth() + 1).toString()) : ((new Date()).getMonth() + 1).toString();
      this.getDateByTime(parseFloat(this.month), this.year, 'cus_care');

      this.monthMeeting = ((new Date()).getMonth() + 1).toString().length == 1 ? ('0' + ((new Date()).getMonth() + 1).toString()) : ((new Date()).getMonth() + 1).toString();
      this.getDateByTime(parseFloat(this.monthMeeting), this.yearMeeting, 'cus_meeting');

      /*Reshow Time Line */
      this.noteHistory = result.listNote;
      this.handleNoteContent();

      this.listContact.forEach(e =>{
        e.contactName = `${e.firstName} ${e.lastName}`;
        e.dateOfBirth = e.dateOfBirth != null ? new Date(e.dateOfBirth) : e.dateOfBirth;
      });

      this.patchDefaultDocumentFile();
      this.patchDefaultDocumentLink();
      this.patchDefaultPotentialCustomerProduct();
      this.patchDefaultPotentialCustomerQuote();

      //patch data to convert form
      this.listCustomerContact = resultLead.listCustomerContact;
      this.listEmailLead = resultLead.listEmailLead;
      // this.listGender = resultLead.listGender;
      this.listInterestedGroup = resultLead.listInterestedGroup;
      this.listLeadType = resultLead.listLeadType;
      this.listLeadGroup = resultLead.listLeadGroup;

      this.listPersonalInChange.forEach(item => {
        item.employeeName = item.employeeCode + ' - ' + item.employeeName;
      });
      this.listPersonalInChangeCus = this.listPersonalInChange;
      this.listPhoneLead = resultLead.listPhoneLead;
      this.listPotential = resultLead.listPotential;
      //new
      this.listBusinessType = resultLead.listBusinessType;
      // this.listInvestFund = resultLead.listInvestFund;
      this.listProbability = resultLead.listProbability;
      this.listLeadReferenceCustomer = resultLead.listLeadReferenceCustomer;
      this.listLeadReferenceCustomer.forEach(item => {
        item.customerName = item.customerCode + ' - ' + item.customerName;
      });

      this.setDefaultValue();
      if (this.listPersonalInChange.length == 1) {
        // this.setDefaultPic();
      }
    } else {
      this.showToast('error', 'Thông báo', 'Lấy dữ liệu thất bại')
    }
  }

  buildListStatusSupport(statusSupportId: string) {
    //Format list status support
    this.listFormatStatusSupport = [];

    this.listStatusSupport.forEach(item => {
      if (item.categoryCode.indexOf('B') == -1) {
        let _item = new StatusSupport();

        _item.categoryId = item.categoryId;
        _item.categoryCode = item.categoryCode;
        _item.categoryName = item.categoryName;

        this.listFormatStatusSupport.push(_item);
      }
      else {
        if (!this.listFormatStatusSupport.find(x => x.categoryCode == 'TEMP')) {
          let _item = new StatusSupport();
          _item.categoryCode = 'TEMP';
          _item.children = this.listTempStatusSupport;
          this.listFormatStatusSupport.push(_item);
        }
      }
    });

    //Tìm trạng thái phụ hiện tại (nếu có)
    let statusSupportCode = this.listStatusSupport.find(x => x.categoryId == statusSupportId)?.categoryCode;

    //Nếu trạng thái hiện tại là Không phù hợp thì giá trị mặc định của droplist là Không phù hợp
    if (statusSupportCode == "B2") {
      this.selectedTempStatusSupport = this.listTempStatusSupport.find(x => x.categoryCode == statusSupportCode);
    }
    //Nếu chưa có trạng thái phụ hoặc trạng thái phụ là Phù hợp thì giá trị mặc định của droplist là Phù hợp
    else {
      this.selectedTempStatusSupport = this.listTempStatusSupport.find(x => x.categoryCode == "B1");
    }

    //Nếu có trạng thái phụ
    if (statusSupportId) {
      if (statusSupportCode == "A") {
        this.listFormatStatusSupport.forEach(item => {
          if (item.categoryCode == "A") {
            item.isActive = true;
            item.isCurrent = true;
          }
        });
      }
      //Không phù hợp
      else if (statusSupportCode == "B2") {
        this.listFormatStatusSupport.forEach(item => {
          if (item.categoryCode == "A" || item.categoryCode == "TEMP") {
            if (item.categoryCode == "TEMP") {
              item.isActive = true;
              item.isCurrent = true;
            }
            else {
              item.isComplete = true;
            }
          }
        });
      }
      //Phù hợp
      else if (statusSupportCode == "B1") {
        this.listFormatStatusSupport.forEach(item => {
          if (item.categoryCode == "A" || item.categoryCode == "TEMP") {
            if (item.categoryCode == "TEMP") {
              item.isActive = true;
              item.isCurrent = true;
            }
            else {
              item.isComplete = true;
            }
          }
        });
      }
      else if (statusSupportCode == "C") {
        this.listFormatStatusSupport.forEach(item => {
          if (item.categoryCode == "A" || item.categoryCode == "TEMP" || item.categoryCode == "C") {
            if (item.categoryCode == "C") {
              item.isActive = true;
              item.isCurrent = true;
            }
            else {
              item.isComplete = true;
            }
          }
        });
      }
      else if (statusSupportCode == "D") {
        this.listFormatStatusSupport.forEach(item => {
          if (item.categoryCode == "A" || item.categoryCode == "TEMP" || item.categoryCode == "C" || item.categoryCode == "D") {
            if (item.categoryCode == "D") {
              item.isActive = true;
              item.isCurrent = true;
            }
            else {
              item.isComplete = true;
            }
          }
        });
      }
      else if (statusSupportCode == "E") {
        this.listFormatStatusSupport.forEach(item => {
          if (item.categoryCode == "E") {
            item.isActive = true;
            item.isCurrent = true;
          }
          else {
            item.isComplete = true;
          }
        });

        //Ẩn nút Chuyển trạng thái
        this.isConvert = true;
      }
    }
    //Nếu chưa có trạng thái phụ
    else {
      //Mặc định là mới tạo
      this.listFormatStatusSupport.forEach(item => {
        if (item.categoryCode == "A") {
          item.isActive = true;
          item.isCurrent = true;
        }
      });
    }
  }

  openAddLinkDialog() {
    this.resetLinkForm();
    this.displayAttachLink = true;
  }

  openAddFileDialog() {
    this.displayAttachFile = true;
  }

  closeAddFileDialog() {
    this.displayAttachFile = false;
  }

  closeAddLinkDialog() {
    this.displayAttachLink = false;
  }

  addFileToTable() {
    if (this.uploadedFiles) {
      this.uploadedFiles.forEach((file, index) => {
        file.createdById = this.auth.UserId;
        file.createdByName = this.userFullName;
        file.createdDate = new Date();
        file.dumpId = index;
        file.typeDocument = "DOC";
        file.fileInFolderId = this.emptyGuid;
        this.listDocument = [...this.listDocument, file];
      });
    }

    //upload file
    this.displayAttachFile = false;
    this.fileUpload.files = []; //primeNg control model
  }

  addLinkToTable() {
    if (!this.addLinkForm.valid) {
      Object.keys(this.addLinkForm.controls).forEach(key => {
        if (!this.addLinkForm.controls[key].valid) {
          this.addLinkForm.controls[key].markAsTouched();
        }
      });
      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
      return;
    }

    let linkName = this.addLinkForm.get('NameOfLink').value || '';
    let link = this.addLinkForm.get('Link').value || '';

    let item = new FileInFolder();
    item.linkOfDocumentId = this.emptyGuid;
    item.linkName = linkName ? linkName.trim() : "";
    item.linkValue = link ? link.trim() : "";
    item.typeDocument = "LINK";

    item.name = item.linkName;
    item.createdById = this.auth.UserId;
    item.createdByName = this.userFullName;
    item.createdDate = new Date();
    item.isNewLink = true;
    this.listDocument = [...this.listDocument, item];

    this.displayAttachLink = false;
  }

  addNewProduct() {
    let item = new potentialCustomerProductResponse();
    item.potentialCustomerProductId = null;
    item.productId = null;
    item.customerId = null;
    item.prodocutCode = '';
    item.isInTheSystem = false;
    item.productName = '';
    item.productUnit = '';
    item.productFixedPrice = null;
    item.productUnitPrice = null;
    item.productNote = '';

    this.listPotentialCustomerProduct = [...this.listPotentialCustomerProduct, item];
  }

  addContact() {
    let item = new contactResponseModel();
    this.listContact = [...this.listContact, item];
  }

  patchDefaultFormValue() {
    let gender = this.listGender.find(e => e.value == this.potentialCustomerContactModel.gender);
    this.potentialCustomerForm.get('Gender').patchValue(gender ? gender : null); //giới tính
    let customerName = this.potentialCustomerModel.customerName;
    this.potentialCustomerForm.get('CustomerName').patchValue(customerName ? customerName : ''); //tên
    let potentialCustomerPosition = this.potentialCustomerContactModel.potentialCustomerPosition;
    this.potentialCustomerForm.get('PotentialCustomerPosition').patchValue(potentialCustomerPosition ? potentialCustomerPosition : ''); //phòng ban
    let phone = this.potentialCustomerContactModel.phone;
    this.potentialCustomerForm.get('Phone').patchValue(phone ? phone : ''); //so dien thoai
    let otherPhone = this.potentialCustomerContactModel.otherPhone;
    this.potentialCustomerForm.get('OtherPhone').patchValue(otherPhone ? otherPhone : ''); //so dien thoai khac
    let taxCode = this.potentialCustomerContactModel.taxCode;
    this.potentialCustomerForm.get('TaxCode').patchValue(taxCode ? taxCode : ''); //ma so thue
    let workEmail = this.potentialCustomerContactModel.workEmail;
    this.potentialCustomerForm.get('WorkEmail').patchValue(workEmail ? workEmail : ''); //email khac
    let allowSendEmail = this.potentialCustomerModel.allowSendEmail;
    this.potentialCustomerForm.get('AllowSendEmail').patchValue(allowSendEmail ? allowSendEmail : false); //cho phep gui email
    let email = this.potentialCustomerContactModel.email;
    this.potentialCustomerForm.get('Email').patchValue(email ? email : ''); //email
    let workPhone = this.potentialCustomerContactModel.workPhone;
    this.potentialCustomerForm.get('WorkPhone').patchValue(workPhone ? workPhone : ''); //so dien thoai lam viec
    let role = this.potentialCustomerContactModel.role;
    this.potentialCustomerForm.get('Role').patchValue(role ? role : ''); //chuc danh
    let investmentFund = this.listInvestFund.find(e => e.categoryId == this.potentialCustomerModel.investmentFundId);
    this.potentialCustomerForm.get('InvestFund').patchValue(investmentFund ? investmentFund : null); //nguon tiem nang
    let socialUrl = this.potentialCustomerContactModel.socialUrl;
    this.potentialCustomerForm.get('SocialLink').patchValue(socialUrl ? socialUrl : ''); //link facebook
    let allowCall = this.potentialCustomerModel.allowCall;
    this.potentialCustomerForm.get('AllowCall').patchValue(allowCall ? allowCall : false); //cho phep goi
    let address = this.potentialCustomerContactModel.address;
    this.potentialCustomerForm.get('Address').patchValue(address ? address : ''); //dia chi
    let personalInChange = this.listPersonalInChange.find(e => e.employeeId == this.potentialCustomerModel.personInChargeId);
    this.potentialCustomerForm.get('Pic').patchValue(personalInChange ? personalInChange : null); //nguoi phu trach
    let careState = this.listCareState.find(c => c.categoryId == this.potentialCustomerModel.careStateId);
    this.potentialCustomerForm.get('CareState').patchValue(careState ? careState : null); //Trạng thái chăm sóc
    let area = this.listArea.find(c => c.geographicalAreaId == this.potentialCustomerContactModel.geographicalAreaId);
    this.potentialCustomerForm.get('Area').patchValue(area ? area : null); //khu vực
    let cusGroup = this.listCusGroup.find(c => c.categoryId == this.potentialCustomerModel.customerGroupId);
    this.potentialCustomerForm.get('CusGroup').patchValue(cusGroup ? cusGroup : null); //khu vực
    let empTakeCare =  this.listEmpTakeCare.find(c => c.employeeId == this.potentialCustomerModel.employeeTakeCareId);
    this.potentialCustomerForm.get('EmployeeTakeCare').patchValue(empTakeCare ? empTakeCare : null); //Nhân viên take care
    this.potentialCustomerForm.get('ContactDate').patchValue(this.potentialCustomerModel.contactDate ? new Date(this.potentialCustomerModel.contactDate) : null); //Ngày liên hệ
    this.potentialCustomerForm.get('EvaluateCompany').patchValue(this.potentialCustomerModel.evaluateCompany ? this.potentialCustomerModel.evaluateCompany : null); //evaluateCompany
    this.potentialCustomerForm.get('SalesUpdate').patchValue(this.potentialCustomerModel.salesUpdate ? this.potentialCustomerModel.salesUpdate : null); //sale update
    this.potentialCustomerForm.get('SalesUpdateAfterMeeting').patchValue(this.potentialCustomerModel.salesUpdateAfterMeeting ? this.potentialCustomerModel.salesUpdateAfterMeeting : null); //sale update
    let note = this.potentialCustomerContactModel.note
    this.potentialCustomerForm.get('NoteCompany').patchValue(note ? note : null); // ghi chú
    //tạo marker cho bản đồ
    // this.patchGeoCodingForMap(this.potentialCustomerContactModel.latitude, this.potentialCustomerContactModel.longitude, this.potentialCustomerModel.customerName);
  }

  patchDefaultPersonalFormValue() {
    let gender = this.listGender.find(e => e.value == this.potentialCustomerContactModel.gender);
    this.potentialPersonalCustomerForm.get('Gender').patchValue(gender ? gender : null); //giới tính
    let firstName = this.potentialCustomerContactModel.firstName;
    let lastName = this.potentialCustomerContactModel.lastName;
    this.potentialPersonalCustomerForm.get('FirstName').patchValue(firstName ? firstName : ''); //tên
    this.potentialPersonalCustomerForm.get('LastName').patchValue(lastName ? lastName : ''); //tên
    let potentialCustomerPosition = this.potentialCustomerContactModel.potentialCustomerPosition;
    this.potentialPersonalCustomerForm.get('PotentialCustomerPosition').patchValue(potentialCustomerPosition ? potentialCustomerPosition : ''); //phòng ban
    let phone = this.potentialCustomerContactModel.phone;
    this.potentialPersonalCustomerForm.get('Phone').patchValue(phone ? phone : ''); //so dien thoai
    let otherPhone = this.potentialCustomerContactModel.otherPhone;
    this.potentialPersonalCustomerForm.get('OtherPhone').patchValue(otherPhone ? otherPhone : ''); //so dien thoai khac
    let taxCode = this.potentialCustomerContactModel.taxCode;
    this.potentialPersonalCustomerForm.get('TaxCode').patchValue(taxCode ? taxCode : ''); //ma so thue
    let workEmail = this.potentialCustomerContactModel.workEmail;
    this.potentialPersonalCustomerForm.get('WorkEmail').patchValue(workEmail ? workEmail : ''); //email khac
    let allowSendEmail = this.potentialCustomerModel.allowSendEmail;
    this.potentialPersonalCustomerForm.get('AllowSendEmail').patchValue(allowSendEmail ? allowSendEmail : false); //cho phep gui email
    let email = this.potentialCustomerContactModel.email;
    this.potentialPersonalCustomerForm.get('Email').patchValue(email ? email : ''); //email
    let workPhone = this.potentialCustomerContactModel.workPhone;
    this.potentialPersonalCustomerForm.get('WorkPhone').patchValue(workPhone ? workPhone : ''); //so dien thoai lam viec
    let role = this.potentialCustomerContactModel.role;
    this.potentialPersonalCustomerForm.get('Role').patchValue(role ? role : ''); //chuc danh
    let investmentFund = this.listInvestFund.find(e => e.categoryId == this.potentialCustomerModel.investmentFundId);
    this.potentialPersonalCustomerForm.get('InvestFund').patchValue(investmentFund ? investmentFund : null); //nguon tiem nang
    let socialUrl = this.potentialCustomerContactModel.socialUrl;
    this.potentialPersonalCustomerForm.get('SocialLink').patchValue(socialUrl ? socialUrl : ''); //link facebook
    let allowCall = this.potentialCustomerModel.allowCall;
    this.potentialPersonalCustomerForm.get('AllowCall').patchValue(allowCall ? allowCall : false); //cho phep goi
    let address = this.potentialCustomerContactModel.address;
    this.potentialPersonalCustomerForm.get('Address').patchValue(address ? address : ''); //dia chi
    let personalInChange = this.listPersonalInChange.find(e => e.employeeId == this.potentialCustomerModel.personInChargeId);
    this.potentialPersonalCustomerForm.get('Pic').patchValue(personalInChange ? personalInChange : null); //nguoi phu trach
    let careState = this.listCareState.find(c => c.categoryId == this.potentialCustomerModel.careStateId);
    this.potentialPersonalCustomerForm.get('CareState').patchValue(careState ? careState : null); //Trạng thái chăm sóc
    let area = this.listArea.find(c => c.geographicalAreaId == this.potentialCustomerContactModel.geographicalAreaId);
    this.potentialPersonalCustomerForm.get('Area').patchValue(area ? area : null); //khu vực
    let cusGroup = this.listCusGroup.find(c => c.categoryId == this.potentialCustomerModel.customerGroupId);
    this.potentialPersonalCustomerForm.get('CusGroup').patchValue(cusGroup ? cusGroup : null); // nhóm khách hàng
    let note = this.potentialCustomerContactModel.note
    this.potentialPersonalCustomerForm.get('NoteCompany').patchValue(note ? note : null); // ghi chú
    //tạo marker cho bản đồ
    // this.patchGeoCodingForMap(this.potentialCustomerContactModel.latitude, this.potentialCustomerContactModel.longitude, this.potentialCustomerModel.customerName);
  }

  patchDefaultDocumentFile() {
    /* patch file to table */
    this.listFileByPotentialCustomer.forEach(file => {
      (file as any).name = file.fileName;
      this.listDocument = [...this.listDocument, file];
    });
  }

  patchDefaultDocumentLink() {
    this.listLinkOfDocument.forEach(link => {
      link.name = link.linkName;
      link.typeDocument = "LINK";
      link.isNewLink = false;
      (this.listDocument as any) = [...this.listDocument, link];
    })
  }

  patchDefaultPotentialCustomerProduct() {
    this.listPotentialCustomerProduct.forEach(customerProduct => {
      if (customerProduct.productId) {
        let _product = this.listProductMasterdata.find(e => e.productId == customerProduct.productId);
        customerProduct.selectedProduct = _product ? _product : null;
      }
    });
  }

  patchDefaultPotentialCustomerQuote() {
    this.listQuoteByPotentialCustomer.forEach(quote => {
      let temp: Date = new Date(quote.quoteDate);
      temp ? temp.setDate(temp.getDate() + Number(quote.effectiveQuoteDate) || 0) : null;
      quote.dueQuoteDate = temp;
    });
  }

  setDefaultValue() {
    //gán khách hàng nếu cơ hội được tạo từ routing từ khách hàng
    if (this.customerId) {
      let customer = this.listLeadReferenceCustomer.find(e => e.customerId == this.customerId);
      if (customer) {
        switch (Number(customer.customerType)) {
          case 1:

            this.listCurrentReferenceCustomer = this.listLeadReferenceCustomer.filter(e => e.customerType === 1);
            break;
          case 2:
            this.listCurrentReferenceCustomer = this.listLeadReferenceCustomer.filter(e => e.customerType === 1);
            break;
          default:
            break;
        }
      }
    }
  }

  changeProduct(event: any, rowData: potentialCustomerProductResponse) {
    let index = this.listPotentialCustomerProduct.findIndex(x => x == rowData);
    let selectedProduct: productFixedPrice = event.value;
    if (selectedProduct) {
      //thay đổi sản phẩm
      rowData.productName = selectedProduct.productName;
      rowData.productFixedPrice = selectedProduct.fixedPrice;
      rowData.isInTheSystem = true;
    } else {
      //xóa sản phẩm
      rowData.productName = '';
      rowData.productFixedPrice = 0;
      rowData.isInTheSystem = false;
    }
  }

  deleteProduct(rowData: potentialCustomerProductResponse) {
    this.listPotentialCustomerProduct = this.listPotentialCustomerProduct.filter(e => e != rowData);
  }

  editContact(rowData: contactResponseModel){

  }

  deleteContact(rowData: contactResponseModel) {
    this.listContact = this.listContact.filter(e => e != rowData);
  }

  async updatePotentialCustomer() {
    this.resetNotification();
    let customerType = Number(this.customerType);
    switch (customerType) {
      case 1:
        //khách hàng doanh nghiệp;
        this.handlePotentialCustomer();
        break
      case 2:
        //khách hàng cá nhân
        this.handlePotentialPersonalCustomer();
        break
    }
  }

  openConvertDialog() {
    // this.resetConvertDialog();
    // this.isShowConvertDialog = true;
    let ref = this.dialogService.open(PotentialConversionDialogComponent, {
      data: {
        customerId: this.customerId
      },
      header: 'Chuyển đổi khách hàng tiềm năng',
      width: '30%',
      baseZIndex: 1030,
      closable: false,
      contentStyle: {
        "min-height": "300px",
        "max-height": "700px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe(async (result: ResultDialog) => {
      if (result) {
        if (result.status) {
          this.getMasterData();
          this.showToast('success', 'Thông báo', 'Chuyển đổi khách khách hàng tiềm năng thành công');
        }
      }
    });
  }

  async handlePotentialCustomer() {
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

    //reset if valid form
    this.isInvalidForm = false;
    this.isOpenNotifiError = false;

    this.loading = true;
    await this.getCustomerGeoCoding();
    let customer: customerRequestModel = this.mappingPotentialCustomerFormToModel();
    let contact: contactRequestModel = this.mappingPotentialCustomerFormToContactModel();
    let listDocumentLink: Array<linkOfDocumentRequest> = this.getListOfDocumentRequest();
    let listCustomerProduct = this.getListProduct();
    let listContact = this.getListContact();

    await this.uploadFlileToServer();
    let result: any = await this.customerService.updatePotentialCustomer(customer, contact, this.listDocumentIdNeedRemove, listDocumentLink, listCustomerProduct, listContact, this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listDocument = [];
      this.listFileByPotentialCustomer = result.listFileByPotentialCustomer || [];
      this.listLinkOfDocument = result.listLinkOfDocument || [];
      this.patchDefaultDocumentFile();
      this.patchDefaultDocumentLink();

      let listNote = result.listNote;
      this.reRenderNote(listNote);

      this.listEmpTakeCare = result.listEmpTakeCare;
      this.listPersonalInChange = result.listPersonalInChange;
      this.listPersonalInChange.forEach(item => {
        item.employeeName = item.employeeCode + ' - ' + item.employeeName;
      });
      this.listPersonalInChangeCus = this.listPersonalInChange;

      this.showToast('success', 'Thông báo', 'Chỉnh sửa khách hàng tiềm năng thành công');

      this.listDocument.forEach(item => item.isNewLink = false); //đánh dấu là đã thêm vào dòng thời gian
      this.listDocumentIdNeedRemove = [];
    } else {
      this.showToast('error', 'Thông báo', result.messageCode);
    }
  }

  async uploadFlileToServer() {
    if (this.uploadedFiles.length == 0) return;
    let listFileUploadModel: Array<FileUploadModel> = [];
    this.uploadedFiles.forEach(item => {
      let fileUpload: FileUploadModel = new FileUploadModel();
      fileUpload.FileInFolder = new FileInFolder();
      fileUpload.FileInFolder.active = true;
      let index = item.name.lastIndexOf(".");
      let name = item.name.substring(0, index);
      fileUpload.FileInFolder.fileName = name;
      fileUpload.FileInFolder.fileExtension = item.name.substring(index, item.name.length - index);
      fileUpload.FileInFolder.size = item.size;
      fileUpload.FileInFolder.objectId = this.potentialCustomerModel.customerId;
      fileUpload.FileInFolder.objectType = 'QLKHTN';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    let result: any = await this.folderService.uploadFileeByFolderTypeAsync("QLKHTN", listFileUploadModel, this.potentialCustomerModel.customerId);
    this.uploadedFiles = [];
  }

  async handlePotentialPersonalCustomer() {
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

    //reset if valid form
    this.isInvalidPersonalForm = false;
    this.isOpenNotifiErrorPersonal = false;

    this.loading = true;
    await this.getCustomerGeoCoding();
    let customer: customerRequestModel = this.mappingPotentialPersonalCustomerFormToModel();
    let contact: contactRequestModel = this.mappingPotentialPersonalCustomerFormToContactModel();
    let listDocumentLink: Array<linkOfDocumentRequest> = this.getListOfDocumentRequest();
    let listCustomerProduct = this.getListProduct();
    let listContact = this.getListContact();

    await this.uploadFlileToServer();
    let result: any = await this.customerService.updatePotentialCustomer(customer, contact, this.listDocumentIdNeedRemove, listDocumentLink, listCustomerProduct, listContact, this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      let listNote = result.listNote;
      this.reRenderNote(listNote);

      this.listPersonalInChange = result.listPersonalInChange;
      this.listPersonalInChange.forEach(item => {
        item.employeeName = item.employeeCode + ' - ' + item.employeeName;
      });
      this.listPersonalInChangeCus = this.listPersonalInChange;
      
      this.showToast('success', 'Thông báo', 'Chỉnh sửa khách hàng tiềm năng thành công');
      this.listDocumentIdNeedRemove = [];
      this.listDocument.forEach(item => item.isNewLink = false); //đánh dấu là đã thêm vào dòng thời gian
    } else {
      this.showToast('error', 'Thông báo', result.messageCode);
    }
  }

  mappingPotentialCustomerFormToModel(): customerRequestModel {
    let _potentialCustomerName = this.potentialCustomerForm.get('CustomerName').value;
    let _pic = this.potentialCustomerForm.get('Pic').value || null;
    let _investFund = this.potentialCustomerForm.get('InvestFund').value || null;
    let _allowSendEmail = this.potentialCustomerForm.get('AllowSendEmail').value || false;
    let _allowCall = this.potentialCustomerForm.get('AllowCall').value || false;
    let _careState = this.potentialCustomerForm.get('CareState').value || null;
    let _cusGroup = this.potentialCustomerForm.get('CusGroup').value || null;
    let _empTakeCare = this.potentialCustomerForm.get('EmployeeTakeCare').value || null;
    let _contactDate = this.potentialCustomerForm.get('ContactDate').value || null;
    let _evaluateCompany = this.potentialCustomerForm.get('EvaluateCompany').value || null;
    let _noteCompany = this.potentialCustomerForm.get('NoteCompany').value || null;
    let _salesUpdate = this.potentialCustomerForm.get('SalesUpdate').value || null;
    let _salesUpdateAfterMeeting = this.potentialCustomerForm.get('SalesUpdateAfterMeeting').value || null;

    let customer = new customerRequestModel();
    customer.CustomerId = this.customerId;
    // customer.ContactId = this.emptyGuid;
    customer.CustomerName = _potentialCustomerName ? _potentialCustomerName.trim() : '';
    customer.PersonInChargeId = _pic ? _pic.employeeId : null;
    customer.InvestmentFundId = _investFund ? _investFund.categoryId : null;
    customer.AllowSendEmail = _allowSendEmail;
    customer.AllowCall = _allowCall;
    customer.CareStateId = _careState ? _careState.categoryId : null;
    customer.CustomerGroupId = _cusGroup ? _cusGroup.categoryId : null;
    customer.EmployeeTakeCareId = _empTakeCare ? _empTakeCare.employeeId : null;

    customer.CustomerCode = '';
    customer.LeadId = null;
    customer.StatusId = this.emptyGuid;
    customer.CustomerServiceLevelId = null;
    customer.CustomerCareStaff = null;
    customer.CustomerType = 0; //0: khách hàng tiềm năng, 1: khách hàng doanh nghiệp, 2: khách hàng cá nhân
    customer.PaymentId = null;
    customer.FieldId = null;
    customer.ScaleId = null;
    customer.MaximumDebtDays = null;
    customer.MaximumDebtValue = null;
    customer.TotalSaleValue = 0;
    customer.TotalReceivable = 0;
    customer.NearestDateTransaction = null;
    customer.BusinessRegistrationDate = null;
    customer.EnterpriseType = null;
    customer.TotalEmployeeParticipateSocialInsurance = null;
    customer.TotalRevenueLastYear = null;
    customer.BusinessType = null;
    customer.BusinessScale = null;
    customer.TotalCapital = 0;
    customer.CreatedById = this.auth.UserId;
    customer.Active = true;
    customer.CreatedDate = new Date();
    customer.ContactDate = _contactDate ? convertToUTCTime(_contactDate) : null;
    customer.EvaluateCompany = _evaluateCompany ? _evaluateCompany.trim() : null;
    customer.NoteCompany = _noteCompany ? _noteCompany.trim() : null;
    customer.SalesUpdate = _salesUpdate ? _salesUpdate.trim() : null;
    customer.SalesUpdateAfterMeeting = _salesUpdateAfterMeeting ? _salesUpdateAfterMeeting.trim() : null;

    return customer;
  }

  mappingPotentialPersonalCustomerFormToModel(): customerRequestModel {
    let _firstName = this.potentialPersonalCustomerForm.get('FirstName').value;
    let _lastName = this.potentialPersonalCustomerForm.get('LastName').value;
    let _fullName = _firstName + " " + _lastName;
    let _pic = this.potentialPersonalCustomerForm.get('Pic').value || null;
    let _investFund = this.potentialPersonalCustomerForm.get('InvestFund').value || null;
    let _careState = this.potentialPersonalCustomerForm.get('CareState').value || null;
    let _cusGroup = this.potentialPersonalCustomerForm.get('CusGroup').value || null;
    let _allowSendEmail = this.potentialPersonalCustomerForm.get('AllowSendEmail').value || false;
    let _allowCall = this.potentialPersonalCustomerForm.get('AllowCall').value || false;

    let customer = new customerRequestModel();
    customer.CustomerId = this.customerId;
    // customer.ContactId = this.emptyGuid;
    customer.CustomerName = _fullName ? _fullName.trim() : '';
    customer.PersonInChargeId = _pic ? _pic.employeeId : null;
    customer.InvestmentFundId = _investFund ? _investFund.categoryId : null;
    customer.AllowSendEmail = _allowSendEmail;
    customer.AllowCall = _allowCall;
    customer.CareStateId = _careState ? _careState.categoryId : null;

    customer.CustomerGroupId = _cusGroup ? _cusGroup.categoryId : null;
    customer.CustomerCode = '';
    customer.LeadId = null;
    customer.StatusId = this.emptyGuid;
    customer.CustomerServiceLevelId = null;
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
    customer.BusinessRegistrationDate = null;
    customer.EnterpriseType = null;
    customer.TotalEmployeeParticipateSocialInsurance = null;
    customer.TotalRevenueLastYear = null;
    customer.BusinessType = null;
    customer.BusinessScale = null;
    customer.TotalCapital = 0;
    customer.CreatedById = this.auth.UserId;
    customer.Active = true;
    customer.CreatedDate = new Date();
    return customer;
  }

  mappingPotentialCustomerFormToContactModel(): contactRequestModel {
    let _potentialCustomerName = this.potentialCustomerForm.get('CustomerName').value;
    let _potentialCustomerPosition = this.potentialCustomerForm.get('PotentialCustomerPosition').value;
    let _phone = this.potentialCustomerForm.get('Phone').value || '';
    let _otherphone = this.potentialCustomerForm.get('OtherPhone').value || '';
    let _taxCode = this.potentialCustomerForm.get('TaxCode').value || '';
    let _workEmail = this.potentialCustomerForm.get('WorkEmail').value || '';
    let _gender: genderModel = this.potentialCustomerForm.get('Gender').value || '';
    let _area = this.potentialCustomerForm.get('Area').value;
    let _note = this.potentialCustomerForm.get('NoteCompany').value;

    let _email = this.potentialCustomerForm.get('Email').value || '';
    let _workPhone = this.potentialCustomerForm.get('WorkPhone').value || '';
    let _role = this.potentialCustomerForm.get('Role').value || '';
    let _socialLink = this.potentialCustomerForm.get('SocialLink').value || '';
    let _address = this.potentialCustomerForm.get('Address').value || '';
    let lat = this.overlays.length > 0 ? this.overlays[0].position.lat() : null;
    let lng = this.overlays.length > 0 ? this.overlays[0].position.lng() : null;

    let contactModel = new contactRequestModel;
    contactModel.ContactId = this.potentialCustomerContactModel.contactId;
    contactModel.ObjectId = this.potentialCustomerContactModel.objectId;
    contactModel.evaluateContactPeople = this.potentialCustomerContactModel.evaluateContactPeople;
    contactModel.FirstName = _potentialCustomerName ? _potentialCustomerName.trim() : '';
    contactModel.LastName = '';
    contactModel.Phone = _phone;
    contactModel.WorkPhone = _workPhone;
    contactModel.OtherPhone = _otherphone;
    contactModel.Email = _email;
    contactModel.WorkEmail = _workEmail;
    contactModel.OtherEmail = '';
    contactModel.TaxCode = _taxCode;
    contactModel.PotentialCustomerPosition = _potentialCustomerPosition;
    contactModel.Role = _role;
    contactModel.SocialUrl = _socialLink;
    contactModel.Gender = _gender ? _gender.value : "";
    contactModel.GeographicalAreaId = _area ? _area.geographicalAreaId : null;

    contactModel.Note = _note;
    contactModel.Address = _address;
    // contactModel.CreatedDate = new Date();
    contactModel.Active = true;
    contactModel.CompanyAddress = '';
    contactModel.CompanyName = '';
    contactModel.DistrictId = null;
    contactModel.ProvinceId = null;
    contactModel.WardId = null;
    contactModel.IdentityID = '';
    contactModel.MaritalStatusId = null;
    contactModel.ObjectType = '';
    contactModel.OptionPosition = '';
    contactModel.Longitude = lng;
    contactModel.Latitude = lat;

    return contactModel;
  }

  mappingPotentialPersonalCustomerFormToContactModel(): contactRequestModel {
    let _firstName = this.potentialPersonalCustomerForm.get('FirstName').value;
    let _lastName = this.potentialPersonalCustomerForm.get('LastName').value;
    let _potentialCustomerPosition = this.potentialPersonalCustomerForm.get('PotentialCustomerPosition').value;
    let _phone = this.potentialPersonalCustomerForm.get('Phone').value || '';
    let _otherphone = this.potentialPersonalCustomerForm.get('OtherPhone').value || '';
    let _taxCode = this.potentialPersonalCustomerForm.get('TaxCode').value || '';
    let _workEmail = this.potentialPersonalCustomerForm.get('WorkEmail').value || '';
    let _gender: genderModel = this.potentialPersonalCustomerForm.get('Gender').value || '';
    let _email = this.potentialPersonalCustomerForm.get('Email').value || '';
    let _workPhone = this.potentialPersonalCustomerForm.get('WorkPhone').value || '';
    let _role = this.potentialPersonalCustomerForm.get('Role').value || '';
    let _socialLink = this.potentialPersonalCustomerForm.get('SocialLink').value || '';
    let _address = this.potentialPersonalCustomerForm.get('Address').value || '';
    let lat = this.overlays.length > 0 ? this.overlays[0].position.lat() : null;
    let lng = this.overlays.length > 0 ? this.overlays[0].position.lng() : null;
    let _area = this.potentialPersonalCustomerForm.get('Area').value;
    let _note = this.potentialPersonalCustomerForm.get('NoteCompany').value;

    let contactModel = new contactRequestModel;
    contactModel.ContactId = this.potentialCustomerContactModel.contactId;
    contactModel.ObjectId = this.potentialCustomerContactModel.objectId;
    contactModel.FirstName = _firstName ? _firstName.trim() : '';
    contactModel.LastName = _lastName ? _lastName.trim() : '';
    contactModel.Phone = _phone;
    contactModel.WorkPhone = _workPhone;
    contactModel.OtherPhone = _otherphone;
    contactModel.Email = _email;
    contactModel.WorkEmail = _workEmail;
    contactModel.OtherEmail = '';
    contactModel.TaxCode = _taxCode;
    contactModel.PotentialCustomerPosition = _potentialCustomerPosition;
    contactModel.Role = _role;
    contactModel.SocialUrl = _socialLink;
    contactModel.Gender = _gender ? _gender.value : "";
    contactModel.GeographicalAreaId = _area ? _area.geographicalAreaId : null;

    contactModel.Note = _note;
    contactModel.Address = _address;
    // contactModel.CreatedDate = new Date();
    contactModel.Active = true;
    contactModel.CompanyAddress = '';
    contactModel.CompanyName = '';
    contactModel.DistrictId = null;
    contactModel.ProvinceId = null;
    contactModel.WardId = null;
    contactModel.IdentityID = '';
    contactModel.MaritalStatusId = null;
    contactModel.ObjectType = '';
    contactModel.OptionPosition = '';
    contactModel.Longitude = lng;
    contactModel.Latitude = lat;

    return contactModel;
  }

  getListOfDocumentRequest(): Array<linkOfDocumentRequest> {
    let result: Array<linkOfDocumentRequest> = [];
    this.listDocument.forEach(doc => {
      if (doc.typeDocument == "LINK") {
        let newRecord = new linkOfDocumentRequest();
        newRecord.LinkOfDocumentId = doc.linkOfDocumentId ? doc.linkOfDocumentId : this.emptyGuid;
        newRecord.LinkName = doc.linkName;
        newRecord.LinkValue = doc.linkValue;
        newRecord.ObjectId = this.emptyGuid;
        newRecord.ObjectType = '';
        newRecord.Active = true;
        newRecord.CreatedById = this.emptyGuid;
        newRecord.CreatedDate = new Date();
        newRecord.IsNewLink = doc.isNewLink;
        result.push(newRecord);
      }
    });
    return result;
  }

  getListProduct(): Array<potentialCustomerProductRequest> {
    const moneyRegex = /[^0-9\.-]+/g;

    let result: Array<potentialCustomerProductRequest> = [];

    //xóa những row không hợp lệ
    this.listPotentialCustomerProduct = this.listPotentialCustomerProduct.filter(e => Boolean(e.productName) != false);

    this.listPotentialCustomerProduct.forEach(customer => {
      let _productId = customer.selectedProduct ? customer.selectedProduct.productId : null;
      let item = new potentialCustomerProductRequest();
      item.PotentialCustomerProductId = this.emptyGuid;
      item.ProductId = _productId;
      item.CustomerId = this.emptyGuid;
      item.IsInTheSystem = customer.isInTheSystem || false;
      item.ProductName = customer.productName ? customer.productName.toString().trim() : "";
      item.ProductUnit = customer.productUnit ? customer.productUnit.toString().trim() : "";
      item.ProductFixedPrice = Number(customer.productFixedPrice ? customer.productFixedPrice.toString().replace(moneyRegex, '') : 0) || 0;
      item.ProductUnitPrice = Number(customer.productUnitPrice ? customer.productUnitPrice.toString().replace(moneyRegex, '') : 0) || 0;
      item.ProductNote = customer.productNote ? customer.productNote.toString().trim() : "";;

      item.Active = true;
      item.CreatedById = this.emptyGuid;
      item.CreatedDate = new Date();

      result = [...result, item];
    });

    return result;
  }

  getListContact(): Array<contactRequestModel> {
    let result: Array<contactRequestModel> = [];
    this.listContact = this.listContact.filter(e => Boolean(e.contactName) != false);
    this.listContact.forEach(contact => {
      let item = new contactRequestModel();
      item.ContactId = contact.contactId;
      item.ObjectId = contact.objectId;
      item.FirstName = contact.contactName ? contact.contactName.trim() : '';
      item.LastName = '';
      item.Role = contact.role ? contact.role.trim() : '';
      item.Phone = contact.phone ? contact.phone.trim() : '';
      // item.WorkPhone = contact.workPhone ? contact.workPhone.trim() : '';
      item.Email = contact.email ? contact.email.trim() : '';
      // item.WorkEmail = contact.workEmail ? contact.workEmail.trim() : '';
      item.Address = contact.address ? contact.address.trim() : '';
      item.LinkFace = contact.linkFace ? contact.linkFace.trim() : '';
      item.evaluateContactPeople = contact.evaluateContactPeople ? contact.evaluateContactPeople.trim() : '';
      item.DateOfBirth = contact.dateOfBirth ? convertToUTCTime(new Date(contact.dateOfBirth)) : contact.dateOfBirth;
      item.CreatedById = contact.createdById;
      item.CreatedDate = contact.createdDate;
      result = [...result, item];
    });

    return result;
  }

  /*Event Lưu các file được chọn*/
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

  deleteDocument(rowData: FileInFolder) {
    this.listDocument = this.listDocument.filter(file => file != rowData);
    if (rowData.fileInFolderId) {
      //file cu~
      this.listDocumentIdNeedRemove.push(rowData.fileInFolderId);
    }
  }

  async downloadDocument(rowData: FileInFolder) {
    console.log(rowData)
    if (rowData.fileInFolderId) {
      this.loading = true;
      this.folderService.downloadFile(rowData.fileInFolderId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
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
            // if (fileType.indexOf('image') !== -1) {
            //   window.open(fileURL);
            // } else {
            //   var anchor = document.createElement("a");
            //   anchor.download = rowData.fileName;
            //   anchor.href = fileURL;
            //   anchor.click();
            // }
            var anchor = document.createElement("a");
            anchor.download = rowData.fileName;
            anchor.href = fileURL;
            anchor.click();
          }
        }
        else {
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      });
    } else {
    }

  }

  openDocument(rowData: FileInFolder) {
    if (rowData.typeDocument == "LINK") {
      //mo lien ket
      window.open(rowData.linkValue, "_blank");
      return;
    }
  }

  openQuoteDetail(rowData: quote) {

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

  addQuote() {
    this.router.navigate(['/customer/quote-create', { customerId: this.customerId }]);
  }

  addLead() {
    this.router.navigate(['/lead/create-lead', { customerId: this.customerId }]);
  }



  resetNotification() {
    this.isInvalidForm = false;
    this.isInvalidPersonalForm = false;
    this.isOpenNotifiError = false;
    this.isOpenNotifiErrorPersonal = false;
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
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

  send_quick_email() {
    let ref = this.dialogService.open(TemplateQuickEmailComponent, {
      data: {
        sendTo: this.potentialCustomerContactModel.email,
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
        sendTo: this.potentialCustomerContactModel.phone,
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
        customerType: this.potentialCustomerModel.customerType,
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

  createLead() {
    this.router.navigate(['/lead/create-lead', { customerId: this.potentialCustomerModel.customerId }]);
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

  removeCustomerMeeting(week: CustomerMeetingForWeek) {
    if (week) {
      this.confirmationService.confirm({
        message: 'Bạn có chắc chắn muốn xóa lịch hẹn?',
        accept: () => {
          this.loading = true;
          this.cusotmerCareService.removeCustomerMeeting(week.customerMeetingId, this.auth.UserId).subscribe(response => {
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

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  cancel() {
    this.router.navigate(['/customer/potential-customer-list']);
  }

  deletePotentialCustomer() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.customerService.changeCustomerStatusToDelete(this.customerId).subscribe(response => {
          this.loading = false;
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Xóa khách hàng tiềm năng thành công' };
            this.showMessage(mgs);
            setTimeout(() => {
              this.router.navigate(['/customer/potential-customer-list']);
            }, 2000);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo', detail: 'Xóa khách hàng tiềm năng thất bại' };
            this.showMessage(mgs);
          }
        }, error => { this.loading = false; });
      }
    });
  }

  // setDefaultPic() {
  //   let personalInChange = this.listPersonalInChange[0];
  //   if (personalInChange) {
  //     this.convertToLeadForm.get('Pic').setValue(personalInChange);
  //   }
  // }

  isTypeCustomer: boolean = false;
  changeTypeLead(event: any) {
    if (event.value === null) {
      this.isTypeCustomer = false;
      return false;
    };
    let currentType: leadTypeModel = event.value;
    switch (currentType.categoryCode) {
      case "KPL":
        //khach hang ca nhan
        this.listCurrentReferenceCustomer = this.listLeadReferenceCustomer.filter(e => e.customerType === 2);
        this.isTypeCustomer = true;
        break;
      case "KCL":
        //khach hang doanh nghiep
        this.listCurrentReferenceCustomer = this.listLeadReferenceCustomer.filter(e => e.customerType === 1);
        this.isTypeCustomer = true;
        break;
      default:
        break;
    }
  }

  changeCustomer(event: any) {
    if (event.value == null) {
      this.listPersonalInChangeCus = [];
      return false;
    }
    let address = event.value.address + ' ' + event.value.addressWard;

    // if (event.value.personInChargeId) {
    //   this.leadService.getEmployeeSale(this.listPersonalInChange, event.value.personInChargeId).subscribe(response => {
    //     let result: any = response;
    //     this.listPersonalInChangeCus = result.listEmployee;
    //     this.listPersonalInChangeCus.forEach(item => {
    //       item.employeeName = item.employeeName;
    //     });
    //     let emp = this.listPersonalInChangeCus.find(e => e.employeeId == event.value.personInChargeId);
    //     this.convertToLeadForm.controls['Pic'].setValue(emp);
    //   });
    // } else {
    //   this.listPersonalInChangeCus = [];
    // }

    //nếu khách hàng không có người phụ trách thì kết quả = có người phụ trách là nhân viên đang đăng nhập
    let personInChargeId = event.value.personInChargeId;
    if (!personInChargeId) {
      personInChargeId = this.employeeId;
    }

    this.leadService.getEmployeeSale(this.listPersonalInChange, personInChargeId).subscribe(response => {
      let result: any = response;
      this.listPersonalInChangeCus = result.listEmployee || [];
      this.listPersonalInChangeCus.forEach(item => {
        item.employeeName = item.employeeName;
      });
      let emp = this.listPersonalInChangeCus.find(e => e.employeeId == personInChargeId);
    });
  }


  /*Event thay đổi nội dung ghi chú*/
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  uploadedNoteFiles: any[] = [];
  /*Event Thêm các file được chọn vào list file*/
  handleNoteFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= 10000000) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedNoteFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedNoteFiles.push(file);
          }
        }
      }
    }
  }

  /*Event Khi click xóa từng file*/
  removeNoteFile(event) {
    let index = this.uploadedNoteFiles.indexOf(event.file);
    this.uploadedNoteFiles.splice(index, 1);
  }

  /*Event Khi click xóa toàn bộ file*/
  clearAllNoteFile() {
    this.uploadedNoteFiles = [];
  }

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
          anchor.download = fileInfor.documentName;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    });
  }

  /*Hủy sửa ghi chú*/
  cancelNote() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn hủy ghi chú này?',
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedNoteFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();  //Xóa toàn bộ file trong control
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
    if (this.uploadedNoteFiles.length > 0) {
      await this.uploadFilesAsync(this.uploadedNoteFiles);
      for (var x = 0; x < this.uploadedNoteFiles.length; ++x) {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = this.uploadedNoteFiles[x].name;
        noteDocument.DocumentSize = this.uploadedNoteFiles[x].size.toString();
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
        this.uploadedNoteFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();  //Xóa toàn bộ file trong control
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
  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  toggle(index: number) {
    this.activeState[index] = !this.activeState[index];
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

  // patchGeoCodingForMap(lat: number, long: number, customerName: string) {
  //   if (!lat && !long) return;
  //   this.overlays = [
  //     new google.maps.Marker({ position: { lat: lat, lng: long }, title: customerName }),
  //   ];

  //   this.cdr.detectChanges();

  //   let bounds = new google.maps.LatLngBounds();
  //   this.overlays.forEach(marker => {
  //     bounds.extend(marker.getPosition());
  //   });
  //   setTimeout(() => { // map will need some time to load
  //     this.map.setCenter(bounds.getCenter());
  //     this.map.setZoom(this.zoom);
  //   }, 1);
  // }

  async getCustomerGeoCoding() {
    let address = '';
    let customerName = '';

    let customerType = Number(this.customerType);
    switch (customerType) {
      case 1:
        address = this.potentialCustomerForm.get('Address').value
        customerName = this.potentialCustomerForm.get('CustomerName').value;
        break
      case 2:
        address = this.potentialPersonalCustomerForm.get('Address').value
        let _firstName = this.potentialPersonalCustomerForm.get('FirstName').value;
        let _lastName = this.potentialPersonalCustomerForm.get('LastName').value;
        let _fullName = _firstName + " " + _lastName;

        customerName = _fullName ? _fullName.trim() : '';
        break
    }

    //reset map position
    this.overlays = [];

    if (!address) return;

    let geoCodingResponse = await this.googleService.getGeoCoding(address);

    try {
      let geoCodingResponse = await this.googleService.getGeoCoding(address);
      let listMatchPosition = geoCodingResponse.data.results;
      //lấy giá trị đầu tiên
      let position = listMatchPosition[0];
      this.overlays = [
        new google.maps.Marker({ position: { lat: position.geometry.lat, lng: position.geometry.lng }, title: customerName }),
      ];

      this.cdr.detectChanges();

      let bounds = new google.maps.LatLngBounds();
      this.overlays.forEach(marker => {
        bounds.extend(marker.getPosition());
      });
      setTimeout(() => { // map will need some time to load
        this.map.setCenter(bounds.getCenter());
        this.map.setZoom(this.zoom);
      }, 1);

    } catch (error) {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy vị trí' };
      this.showMessage(msg);
    }
  }

  reRenderNote(listNote) {
    this.noteHistory = listNote;
    this.handleNoteContent();
  }

  /* Mở popup Tạo nhanh sản phẩm */
  openQuickCreProductDialog() {
    let ref = this.dialogService.open(QuickCreateProductComponent, {
      data: {},
      header: 'Tạo nhanh sản phẩm',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        'overflow-x': 'hidden'
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        let newProduct: productFixedPrice = result.newProduct;
        this.listProductMasterdata = [newProduct, ...this.listProductMasterdata];

        //thêm vào table sản phẩm vừa tạo
        let newItem = new potentialCustomerProductResponse();
        newItem.selectedProduct = this.listProductMasterdata.find(e => e.productId == newProduct.productId);
        newItem.productName = newItem.selectedProduct?.productName ?? '';
        newItem.productFixedPrice = 0;
        this.listPotentialCustomerProduct = [newItem, ...this.listPotentialCustomerProduct];
      }
    });
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

  changeActive(item: StatusSupport) {
    this.listFormatStatusSupport.forEach(_item => {
      _item.isActive = false;
    });

    item.isActive = true;
  }

  changeStatusSupport() {
    //Lấy statusSupportId
    let statusSupportId = '';
    let statusSupport = this.listFormatStatusSupport.find(x => x.isActive == true);

    if (statusSupport.categoryCode == 'TEMP') {
      statusSupportId = this.selectedTempStatusSupport.categoryId;
    }
    else {
      statusSupportId = statusSupport.categoryId;
    }

    //Nếu là chuyển đổi
    if (statusSupport.categoryCode == 'E')
    {
      this.openConvertDialog();
    }
    //Nếu không phải chuyển đổi
    else
    {
      this.customerService.changeStatusSupport(this.customerId, statusSupportId).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo', 'Chuyển trạng thái thành công');
          this.buildListStatusSupport(statusSupportId);
        }
        else {
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      });
    }
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

const blobToDataUrl = blob => new Promise((resolve, reject) => {
  const reader = new FileReader();
  reader.onload = () => resolve(reader.result);
  reader.onerror = reject;
  reader.readAsDataURL(blob);
});

const blobToBase64 = blob => blobToDataUrl(blob).then((text: any) => text.slice(text.indexOf(",") + 1));


function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

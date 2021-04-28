import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { CustomerImportComponent } from '../customer-import/customer-import.component';
import { CustomerDownloadTemplateComponent } from '../customer-download-template/customer-download-template.component';
import { SendEmailDialogComponent } from '../../../shared/components/send-email-dialog/send-email-dialog.component';
import { SendSmsDialogComponent } from '../../../shared/components/send-sms-dialog/send-sms-dialog.component';
import { DialogImportPotentialcustomerComponent } from "../dialog-import-potentialcustomer/dialog-import-potentialcustomer.component";
import { CustomerImportDetailComponent } from '../customer-import-detail/customer-import-detail.component';

import { EmployeeService } from "../../../employee/services/employee.service";
import { TranslateService } from '@ngx-translate/core';
import { CategoryService } from "../../../shared/services/category.service";
import { CustomerService } from '../../services/customer.service';
import { CustomerModel } from "../../models/customer.model";
import { GetPermission } from '../../../shared/permission/get-permission';
import { LeadService } from "../../../lead/services/lead.service";
import { LeadModel } from "../../../lead/models/lead.model";
import { ContactModel, contactModel } from "../../../shared/models/contact.model";

import { ngxLoadingAnimationTypes } from "ngx-loading";
import { SortEvent } from 'primeng/api';
import { MenuItem } from "primeng/primeng";
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DialogService } from 'primeng/dynamicdialog';
import * as $ from 'jquery';
import * as XLSX from 'xlsx';
import { AddProtentialCustomerDialogComponent } from '../add-protential-customer-dialog/add-protential-customer-dialog.component';
import { PotentialConversionDialogComponent } from '../potential-conversion-dialog/potential-conversion-dialog.component';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DatePipe, formatNumber } from '@angular/common';

declare var google: any; //new added line

class importCustomerByExcelModel {
  firstName: string;
  lastName: string;
  isEmployee: boolean;
  isVendor: boolean;
  customerCode: string;
  isCompany: boolean;
  companyName: string;
  address: string;
  taxCode: string;
  phone: string;
  email: string;

  fullNameOfContact: string;
  genderOfContact: boolean;
  addressOfContact: string;
  phoneOfContact: string;
  emailOfContact: string;
  note: string;
}

class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefault: boolean;
}

class investFund {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class ColumnExcel {
  code: string;
  name: string;
  width: number;
}

class SearchCustomerModelResponse {
  public index: number;
  public customerId: string;
  public contactId: string;
  public customerName: string;
  public customerCode: string;
  public picName: string;
  public customerEmail: string;
  public customerPhone: string;
  public countCustomerInfo: number;
  public statusName: string;
  public backgroupStatus: string;
  public isConverted: boolean;
  public latitude: number;
  public longitude: number;
  public investmentFundId: string;
  public fullAddress: string;
  public personInChargeId: string;
  public careSateCode: string;
  public careStateName: string;
  public statusSupportName: string;
  public backgroupCareState: string;
  public color: string;
  public contactName: string;
  public createdDate: Date;

  constructor() {
    this.contactName = '';
    this.customerName = '';
    this.customerEmail = '';
    this.customerPhone = '';
    this.statusName = '';
    this.picName = '';
  }
}

class CustomerLevel {
  public customerServiceLevelId: string;
  public customerServiceLevelName: string;
}

class Employee {
  public employeeId: string;
  public employeeName: string;
  public employeeCode: string;
  public active: boolean
}

class CustomerGroup {
  public categoryId: string;
  public categoryName: string;
  public categoryCode: string;
}

class SearchCustomerModel {
  public FullName: string;
  public Phone: string;
  public Email: string;
  public Adress: string;
  public InvestmentFundId: Array<string>;
  public PersonInChargeId: Array<string>;
  public ListCareStateId: Array<string>;
  public ListCusTypeId: Array<string>;
  public ListCusGroupId: Array<string>;
  public ListAreaId: Array<string>;
  public StartDate: any;
  public EndDate: any;
  public IsConverted: boolean;

  constructor() {
    this.InvestmentFundId = [];
    this.PersonInChargeId = [];
    this.ListCareStateId = [];
    this.ListCusGroupId = [];
    this.ListCusTypeId = [];
    this.ListAreaId = [];
    this.IsConverted = false;
  }
}

class ImportPotentialCustomerModel {
  customerType: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  socialUrl: string;
  picCode: string;
  investmentFundName: string;
  note: string;

  constructor() {
    this.customerType = 0;
    this.firstName = '';
    this.lastName = '';
    this.email = '';
    this.phone = '';
    this.socialUrl = '';
    this.picCode = '';
    this.investmentFundName = '';
    this.note = '';
  }
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

  constructor() {
    this.customerName = '';
    this.isConverted = true;
  }
}

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

class probability {
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

class personalInChange {
  employeeCode: string;
  employeeId: string;
  employeeName: string;
}

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
}


@Component({
  selector: 'app-potential-customer-list',
  templateUrl: './potential-customer-list.component.html',
  styleUrls: ['./potential-customer-list.component.css'],
  providers: [DialogService, LeadService, DatePipe]
})
export class PotentialCustomerListComponent implements OnInit {
  emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  versionName = this.getDefaultApplicationName();
  @ViewChild('table') table: Table;
  filterGlobal: string;
  innerWidth: number = 0; //number window size first
  loading: boolean = false;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  customerCode: string = '';
  startDate: Date = new Date();
  minYear: number = 2015;
  currentYear: number = (new Date()).getFullYear();
  endDate: Date = new Date();
  maxEndDate: Date = new Date();
  selectedColumns: any[];
  detailCustomerColumns: any[];
  colsCustomer: any[];
  listPotentialCustomer: Array<SearchCustomerModelResponse> = [];
  rows = 10;
  selectedCustomers: Array<SearchCustomerModelResponse> = [];
  actions: MenuItem[] = [];
  filterForm: FormGroup;
  //master data
  customerGroups: Array<CustomerGroup> = [];
  serviceLevels: Array<CustomerLevel> = [];
  employees: Array<Employee> = [];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';

  isManager: boolean = null;
  employeeId: string = '';

  userPermission: any = localStorage.getItem("UserPermission").split(',');
  createPermission: string = 'customer/create';
  importPermission: string = 'import/create';
  exportPermission: string = 'export/create';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  currentEmployeeId = this.auth.EmployeeId;
  listCustomer: Array<CustomerModel> = [];
  selection: SelectionModel<CustomerModel>;
  selectedCustomerIdList: Array<string> = [];

  customerGroupCode: string = 'NHA';
  pics: Array<any> = [];
  filteredServiceLevel: Observable<string[]>;
  filteredCustomerGroup: Observable<string[]>;
  filteredPic: Observable<string[]>;
  serviceLevelCtrl = new FormControl();
  customerGroupCtrl = new FormControl();
  picCtrl = new FormControl();
  selectedServiceLevel: Array<any> = [];
  selectedCustomerGroup: Array<any> = [];

  customerGroup: string = '';
  serviceLevel: string = '';
  taxCode: string = '';

  //master data
  listPersonalInChange: Array<Employee> = [];
  listInvestFund: Array<investFund> = [];
  listCareState: Array<Category> = [];
  listArea: Array<any> = [];
  listCusGroup: Array<Category> = [];
  listCusType: Array<Category> = [];
  /* Action*/
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  actionImport: boolean = true;
  actionSMS: boolean = true;
  actionEmail: boolean = true;
  /*END*/

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  paramUrl: any;
  noPic: boolean = false;
  businessOnly: boolean = true;
  personalOnly: boolean = true;
  hkd: boolean = true;
  customerLevelName: string;

  //mode
  currentMode: boolean = true;

  //map
  options: any = {};
  overlays: any[] = [];
  map: google.maps.Map;
  zoom: number = 16;
  displayCustomerDetailDialog: boolean = false;
  selectedCustomerOnMap: SearchCustomerModelResponse = new SearchCustomerModelResponse();
  listPropsCustomerDetail: Array<SearchCustomerModelResponse> = [];

  //master data dialog chuyen doi
  listCustomerContact: Array<customerContactModel> = [];
  listEmailLead: Array<string> = []; listInterestedGroup: Array<interestedGroupModel> = [];
  listLeadType: Array<leadTypeModel> = [];
  listLeadGroup: Array<leadGroup> = [];
  listPersonalInChangeCus: Array<Employee> = [];
  listPhoneLead: Array<string> = [];
  listPotential: Array<potentialModel> = [];
  listBusinessType: Array<businessType> = [];
  listProbability: Array<probability> = [];
  listLeadReferenceCustomer: Array<leadReferenceCustomer> = [];
  listCurrentReferenceCustomer: Array<leadReferenceCustomer> = [];

  //dialog chuyển đổi
  isShowConvertDialog: boolean = false;
  selectedPotentialCustomer: SearchCustomerModelResponse = new SearchCustomerModelResponse();
  converdialog_isCreateCustomer: boolean = false;
  converdialog_isCreatelead: boolean = false;

  potentialCustomerModel: customerResponseModel = new customerResponseModel();

  selectedCustomerList: Array<any> = [];

  //import varriables
  displayChooseFileImportDialog: boolean = false;
  fileName: string = '';
  importFileExcel: any = null;

  constructor(
    private dialogService: DialogService,
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private translate: TranslateService,
    private customerService: CustomerService,
    private employeeService: EmployeeService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private leadService: LeadService,
    private datePipe: DatePipe,
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit(): void {
    //Kiểm tra nhân viên hoặc quản lý
    this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
    this.employeeId = JSON.parse(localStorage.getItem('auth')).EmployeeId;

    //Check permission
    let resource = "crm/customer/potential-customer-list";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);

    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownload = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("import") == -1) {
        this.actionImport = false;
      }
      if (listCurrentActionResource.indexOf("sms") == -1) {
        this.actionSMS = false;
      }
      if (listCurrentActionResource.indexOf("email") == -1) {
        this.actionEmail = false;
      }
      this.initTable();
      this.initForm();

      this.paramUrl = this.route.params.subscribe(param => {
        if (param['mode']) {
          if (param['mode'] == 'converted') {
            this.filterForm.get('IsConverted').patchValue(true);
          }
        }
      });
      this.getMasterData();
    }
  }

  initTable() {
    if(this.versionName == 'VNS'){
      this.selectedColumns = [
        { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left', display: 'table-cell' },
        { field: 'contactName', header: 'Tên người liên hệ', textAlign: 'left', display: 'table-cell' },
        { field: 'customerEmail', header: 'Email', textAlign: 'left', display: 'table-cell' },
        { field: 'customerPhone', header: 'Số điện thoại', textAlign: 'left', display: 'table-cell' },
        { field: 'createdDate', header: 'Ngày tạo', textAlign: 'left', display: 'table-cell' },
        { field: 'careStateName', header: 'Trạng thái chăm sóc', textAlign: 'center', display: 'table-cell' },
        { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
        { field: 'statusSupportName', header: 'Trạng thái phụ', textAlign: 'center', display: 'table-cell' },
        { field: 'picName', header: 'Người phụ trách', textAlign: 'left', display: 'table-cell' },
      ];
    }else{
      this.selectedColumns = [
        { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left', display: 'table-cell' },
        { field: 'customerEmail', header: 'Email', textAlign: 'left', display: 'table-cell' },
        { field: 'customerPhone', header: 'Số điện thoại', textAlign: 'left', display: 'table-cell' },
        { field: 'createdDate', header: 'Ngày tạo', textAlign: 'left', display: 'table-cell' },
        { field: 'careStateName', header: 'Trạng thái chăm sóc', textAlign: 'center', display: 'table-cell' },
        { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
        { field: 'statusSupportName', header: 'Trạng thái phụ', textAlign: 'center', display: 'table-cell' },
        { field: 'picName', header: 'Người phụ trách', textAlign: 'left', display: 'table-cell' },
      ];
    }
    
    this.colsCustomer = this.selectedColumns;

    this.detailCustomerColumns = [
      { field: 'label', header: '', textAlign: 'left', display: 'table-cell' },
      { field: 'value', header: '', textAlign: 'left', display: 'table-cell' },
    ]
  }

  initForm() {
    this.filterForm = new FormGroup({
      'Name': new FormControl(''),
      'Phone': new FormControl(''),
      'Email': new FormControl(''),
      'Address': new FormControl(''),
      'ListInvestmentFund': new FormControl([]),
      'Pic': new FormControl([]),
      'CareState': new FormControl([]),
      'CusGroup': new FormControl([]),
      'CusType': new FormControl([]),
      'IsConverted': new FormControl(false),
      'Area': new FormControl([]),
      'StartDate': new FormControl(null),
      'EndDate': new FormControl(null),
    });

  }

  resetTable() {
    if (!this.table) return;
    this.filterGlobal = '';
    this.table.sortField = '';
    this.table.reset();
  }

  resetForm() {
    this.filterForm.patchValue({
      'Name': '',
      'Phone': '',
      'Email': '',
      'Address': '',
      'ListInvestmentFund': [],
      'Pic': [],
      'CareState': [],
      'CusGroup': [],
      'CusType': [],
      'IsConverted': false,
      'Area': [],
      'StartDate': null,
      'EndDate': null,
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  setMap(event) {
    this.map = event.map;
  }

  switchMode(event: any) {
    if (!event.checked) {
      /* chuyển sang mode bản đồ */
      /* lấy những bản ghi có tọa độ hợp lệ */
      let listPotentialCustomerPosition = [];
      this.listPotentialCustomer.forEach(customer => {
        if (customer.longitude && customer.latitude) listPotentialCustomerPosition.push({
          lat: customer.latitude,
          lng: customer.longitude,
          title: customer.customerName,
          customerId: customer.customerId
        });
      });
      this.overlays = [];
      listPotentialCustomerPosition.forEach(pos => {
        this.overlays = [...this.overlays, new google.maps.Marker({ position: { lat: pos.lat, lng: pos.lng }, title: pos.title, customerId: pos.customerId })]
      });

      let bounds = new google.maps.LatLngBounds();
      this.overlays.forEach(marker => {
        bounds.extend(marker.getPosition());
      });
      setTimeout(() => { // map will need some time to load
        this.map.setCenter(new google.maps.LatLng(17.429441, 106.376713));
        this.map.setZoom(6);
      }, 1000);
    }
  }

  handleOverlayClick(event) {
    let customerId = event.overlay.customerId;

    let selectedCustomer = this.listPotentialCustomer.find(cus => cus.customerId == customerId);
    if (!selectedCustomer) return;

    this.displayCustomerDetailDialog = true;
    this.selectedCustomerOnMap = selectedCustomer;
  }

  async searchCustomer() {
    //reset table
    this.resetTable();
    let searchCustomerModel = this.mappingFomrToModel();
    this.loading = true;
    let result: any = await this.customerService.searchPotentialCustomer(searchCustomerModel, this.auth.UserId);
    if (result.statusCode === 200) {
      this.listPotentialCustomer = result.listPotentialCustomer;
      this.handleStatus();
      this.handlerbackgroundColorCareState();
      this.setIndex();
      if (this.listPotentialCustomer.length === 0) {
        let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy khách hàng nào!' };
        this.showMessage(mgs);
      }
    } else {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(mgs);
    }

    this.loading = false;
  }

  handleStatus() {
    this.listPotentialCustomer.forEach(customer => {
      if (customer.isConverted == true) {
        customer.statusName = "Đã chuyển đổi";
        customer.backgroupStatus = "#34C759"
      } else {
        customer.statusName = "Chưa chuyển đổi";
        customer.backgroupStatus = "#007aff"
      }
    });
  }

  handlerbackgroundColorCareState() {
    this.listPotentialCustomer.forEach(item => {
      switch (item.careSateCode) {
        case "CGD":
          item.backgroupCareState = "#FF0000";
          item.color = '#0C0C0C';
          break;
        case "DGD":
          item.backgroupCareState = "#00FF00";
          item.color = '#0C0C0C';
          break;
        case "GL":
          item.backgroupCareState = "#FFFF00";
          item.color = '#0C0C0C';
          break;
        case "DHL":
          item.backgroupCareState = "#FF00FF";
          item.color = '#0C0C0C';
          break;
        case "DD":
          item.backgroupCareState = "#00FFFF";
          item.color = '#0C0C0C';
          break;
        case "CST":
          item.backgroupCareState = "#808000";
          item.color = '#0C0C0C';
          break;
        case "DCD":
          item.backgroupCareState = " #000080";
          item.color = '#FFFFFF';
          break;
      }
    });
  }

  setIndex() {
    this.listPotentialCustomer.forEach((customer, index) => {
      customer.index = index + 1;
    });
  }

  async getMasterData() {
    this.loading = true;
    // let result: any = await this.customerService.getDataSearchPotentialCustomer(this.currentEmployeeId);

    let [result, resultLead]: any = await Promise.all([
      this.customerService.getDataSearchPotentialCustomer(this.currentEmployeeId),
      this.leadService.getDataCreateLead(this.auth.UserId)
    ])
    await this.searchCustomer();
    this.loading = false;
    if (result.statusCode === 200 && resultLead.statusCode === 200) {
      this.listPersonalInChange = result.listEmployeeModel;
      this.listInvestFund = result.listInvestFund;
      this.listCareState = result.listCareState;
      this.listCusType = result.listCusType;
      this.listCusGroup = result.listCusGroup;
      this.listArea = result.listArea;
      //patch data to convert form
      this.listCustomerContact = resultLead.listCustomerContact;
      this.listEmailLead = resultLead.listEmailLead;
      this.listInterestedGroup = resultLead.listInterestedGroup;
      this.listLeadType = resultLead.listLeadType;
      this.listLeadGroup = resultLead.listLeadGroup;
      // this.listPersonalInChange = result.listPersonalInChange;
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

      // this.setDefaultValue();
      // if (this.listPersonalInChange.length == 1) {
      //   this.setDefaultPic();
      // }
    } else {
      this.showToast('error', 'Thông báo', 'Lấy dữ liệu thất bại')
    }
  }

  mappingFomrToModel(): SearchCustomerModel {
    let _name = this.filterForm.get('Name').value;
    let _phone = this.filterForm.get('Phone').value;
    let _email = this.filterForm.get('Email').value;
    let _address = this.filterForm.get('Address').value;
    let _pic: Array<Employee> = this.filterForm.get('Pic').value;
    let _listInvestmentFund: Array<investFund> = this.filterForm.get('ListInvestmentFund').value;
    let _listCareState: Array<Category> = this.filterForm.get('CareState').value;
    let _isConverted = this.filterForm.get('IsConverted').value;
    var _listCusType: Array<Category> = this.filterForm.get('CusType').value;
    var _listCusGroup: Array<Category> = this.filterForm.get('CusGroup').value;
    var _listArea = this.filterForm.get('Area').value;
    let _startDate = this.filterForm.get('StartDate').value;
    let _endDate = this.filterForm.get('EndDate').value;

    if (_startDate) {
      _startDate.setHours(0, 0, 0, 0);
      _startDate = convertToUTCTime(_startDate);
    }
    if (_endDate) {
      _endDate.setHours(23, 59, 59, 999);
      _endDate = convertToUTCTime(_endDate);
    }

    let searchCustomer = new SearchCustomerModel();
    searchCustomer.FullName = _name ? _name : "";
    searchCustomer.Phone = _phone ? _phone : "";
    searchCustomer.Email = _email ? _email : "";
    searchCustomer.Adress = _address ? _address : "";
    searchCustomer.PersonInChargeId = _pic ? _pic.map(e => e.employeeId) : [];
    searchCustomer.InvestmentFundId = _listInvestmentFund ? _listInvestmentFund.map(e => e.categoryId) : [];
    searchCustomer.IsConverted = _isConverted ? _isConverted : false;
    searchCustomer.ListCareStateId = _listCareState ? _listCareState.map(e => e.categoryId) : [];
    searchCustomer.ListCusTypeId = _listCusType ? _listCusType.map(e => e.categoryId) : [];
    searchCustomer.ListCusGroupId = _listCusGroup ? _listCusGroup.map(e => e.categoryId) : [];
    searchCustomer.ListAreaId = _listArea ? _listArea.map(e => e.geographicalAreaId) : [];
    searchCustomer.StartDate = _startDate;
    searchCustomer.EndDate = _endDate;

    return searchCustomer;
  }

  goToCreate() {
    this.router.navigate(['/customer/potential-customer-create']);
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

    ref.onClose.subscribe(async (result: ResultDialog) => {
      if (result) {
        if (result.status) {
          this.showToast('success', 'Thông báo', 'Tạo khách hàng tiềm năng thành công');
          this.searchCustomer();
        } else {
          this.searchCustomer();
        }
      }
    });
  }

  //redirect đến trang tạo báo giá
  // goToNewCustomerQuote() {
  //   this.router.navigate(['/customer/quote-create']);
  // }

  goToCustomerQuote(customerId: string) {
    this.router.navigate(['/customer/quote-create', {
      customerId: customerId
    }]);
  }

  goToLead(customerId: string) {
    this.router.navigate(['/lead/create-lead', {
      customerId: customerId
    }]);
  }

  //redirect đến trang tạo Don Hang
  // goToNewCustomerOrder() {
  //   this.router.navigate(['/order/create']);
  // }


  goToCustomerOrder(customerId: string) {
    this.router.navigate(['/order/create', {
      customerId: customerId
    }]);
  }

  //send email
  send_email(customerId: any) {
    this.selectedCustomerList = [];
    this.selectedCustomerList.push(customerId);

    let ref = this.dialogService.open(SendEmailDialogComponent, {
      data: {
        leadIdList: this.selectedCustomerList
      },
      header: 'Gửi email',
      width: '70%',
      baseZIndex: 1030,
      closable: false,
      contentStyle: {
        "min-height": "280px",
        "max-height": "800px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          switch (result.message) {
            case "success":
              let msg1 = { severity: 'success', summary: 'Thông báo:', detail: 'Gửi email thành công' };
              this.showMessage(msg1);
              break;
            case "warning":
              let msg2 = { severity: 'warn', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg2);
              break;
            default:
              let msg3 = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg3);
              break;
          }
        }
      }
    });

  }

  //send sms
  send_sms(customerId: any) {
    this.selectedCustomerList = [];
    this.selectedCustomerList.push(customerId);
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

  importCustomer() {
    this.displayChooseFileImportDialog = true;
  }

  openImportDetailDialog(listImportPotentialCustomerImport: Array<ImportPotentialCustomerModel>) {
    let ref1 = this.dialogService.open(DialogImportPotentialcustomerComponent, {
      data: {
        listImportPotentialCustomerImport: listImportPotentialCustomerImport
      },
      header: 'Import Khách hàng tiềm năng',
      width: '85%',
      baseZIndex: 1050,
      contentStyle: {
        // "min-height": "400px",
        "max-height": "800px",
        "over-flow": "hidden"
      }
    });

    ref1.onClose.subscribe(async (result: any) => {
      if (result) {
        if (result.status) {
          this.loading = true;
          await this.searchCustomer();
          this.loading = false;
        }
      }
    });
  }

  del_customer(customerId: string) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.customerService.changeCustomerStatusToDelete(customerId).subscribe(response => {
          this.loading = false;
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            //remove from list
            this.listPotentialCustomer = this.listPotentialCustomer.filter(e => e.customerId != customerId);
            let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Xóa khách hàng thành công' };
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo', detail: 'Xóa khách hàng thất bại' };
            this.showMessage(mgs);
          }
        }, error => { this.loading = false; });
      }
    });
  }

  async downloadTemplateExcel() {
    this.loading = true;
    let result: any = await this.customerService.downloadTemplateImportCustomer(this.auth.UserId);
    this.loading = false;

    if (result.templateExcel != null && result.statusCode === 200) {
      const binaryString = window.atob(result.templateExcel);
      const binaryLen = binaryString.length;
      const bytes = new Uint8Array(binaryLen);
      for (let idx = 0; idx < binaryLen; idx++) {
        const ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      const fileName = result.fileName + ".xls";
      link.download = fileName;
      link.click();
    } else {
      this.displayChooseFileImportDialog = false;
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Download file thất bại' };
      this.showMessage(msg);
    }
  }

  // async downloadTemplate() {
  //   this.loading = true;
  //   let result: any = await this.customerService.DownloadTemplatePotentialCustomerAsync();
  //   this.loading = false;
  //   if (result.excelFile != null && (result.statusCode === 202 || result.statusCode === 200)) {
  //     const binaryString = window.atob(result.excelFile);
  //     const binaryLen = binaryString.length;
  //     const bytes = new Uint8Array(binaryLen);
  //     for (let idx = 0; idx < binaryLen; idx++) {
  //       const ascii = binaryString.charCodeAt(idx);
  //       bytes[idx] = ascii;
  //     }
  //     const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
  //     const link = document.createElement('a');
  //     link.href = window.URL.createObjectURL(blob);
  //     const fileName = result.nameFile + ".xlsx";
  //     link.download = fileName;
  //     link.click();

  //     let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Tải thành công" };
  //     this.showMessage(mgs);
  //   } else {
  //     let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
  //     this.showMessage(mgs);
  //     return;
  //   }
  // }

  importExcel() {
    if (this.fileName == "") {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Chưa chọn file cần nhập" };
      this.showMessage(mgs);
      return;
    }

    const targetFiles: DataTransfer = <DataTransfer>(this.importFileExcel);
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(targetFiles.files[0]);
    reader.onload = (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });
      let code = 'Danh mục KH';
      if (!workbook.Sheets[code]) {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
        this.showMessage(mgs);
        return;
      }

      //lấy data từ file excel
      const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[code];
      /* save data */
      let listCustomerRawData: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
      /* remove header */
      listCustomerRawData = listCustomerRawData.filter((e, index) => index != 0);
      /* nếu không nhập 2 trường required: tên + mã khách hàng thì loại bỏ */
      listCustomerRawData = listCustomerRawData.filter(e => (e[1] && e[4]));
      /* chuyển từ raw data sang model */
      let listCustomerImport: Array<importCustomerByExcelModel> = [];
      listCustomerRawData?.forEach(_rawData => {
        /*
      0: "Họ đệm "
      1: "Tên *"
      2: "Là nhân viên"
      3: "Là nhà cung cấp"
      4: "Mã khách hàng (*)"
      5: "Là doanh nghiệp?"
      6: "Thuộc Công Ty"
      7: "Địa chỉ"
      8: "Mã số thuế"
      9: "Điện thoại"
      10: "Email"
      11: "Họ và tên NLH"
      12: "Giới tính NLH"
      13: "Địa chỉ người liên hệ"
      14: "ĐT di động NLH"
      15: "Email người liên hệ"
      16: "Ghi chú"
       */
        let customer = new importCustomerByExcelModel();
        customer.firstName = _rawData[0]?.trim() ?? '';
        customer.lastName = _rawData[1]?.trim() ?? '';
        customer.isEmployee = _rawData[2]?.trim() === "1" ? true : false;
        customer.isVendor = _rawData[3]?.trim() === "1" ? true : false;
        customer.customerCode = _rawData[4]?.trim() ?? '';
        customer.isCompany = _rawData[5]?.trim() === "1" ? true : false;
        customer.companyName = _rawData[6]?.trim() ?? '';
        customer.address = _rawData[7]?.trim() ?? '';
        customer.taxCode = _rawData[8]?.trim() ?? '';
        customer.phone = _rawData[9]?.trim() ?? '';
        customer.email = _rawData[10]?.trim() ?? '';
        customer.fullNameOfContact = _rawData[11]?.trim() ?? '';
        customer.genderOfContact = _rawData[12]?.trim() ?? '';
        customer.addressOfContact = _rawData[13]?.trim() ?? '';
        customer.phoneOfContact = _rawData[14]?.trim() ?? '';
        customer.emailOfContact = _rawData[15]?.trim() ?? '';
        customer.note = _rawData[16]?.trim() ?? '';

        listCustomerImport = [...listCustomerImport, customer];
      });
      /* tắt dialog import file, bật dialog chi tiết khách hàng import */
      this.displayChooseFileImportDialog = false;
      this.openDetailImportDialog(listCustomerImport);
    }
  }

  chooseFile(event: any) {
    this.fileName = event.target?.files[0]?.name;
    this.importFileExcel = event.target;
  }

  openDetailImportDialog(listCustomerImport) {
    let ref = this.dialogService.open(CustomerImportDetailComponent, {
      data: {
        isPotentialCustomer: true,
        listCustomerImport: listCustomerImport
      },
      header: 'Import Khách hàng',
      width: '85%',
      baseZIndex: 1050,
      contentStyle: {
        "max-height": "800px",
        // "over-flow": "hidden"
      }
    });
    ref.onClose.subscribe((result: any) => {
      if (result?.status) {
        this.getMasterData();
      }
    });
  }

  //apply new UI
  refreshFilter() {
    this.resetForm();
    this.searchCustomer();
  }

  leftColNumber: number = 12;
  rightColNumber: number = 0;
  showFilter() {
    if (this.innerWidth < 768) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.rightColNumber = 4;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  pageChange(event: any) {
  }

  goToDetail(rowData: any) {
    this.router.navigate(['/customer/potential-customer-detail', { customerId: rowData.customerId }]);
  }

  onChangeAction(rowData: SearchCustomerModelResponse) {
    /*
    - Chuyển định danh
    - Tạo cơ hội
    - Tạo báo giá
    - Gửi email
    - Gửi SMS
    - Tạo hoạt động
    - Xóa (Chỉ cho phép xóa khi chưa chuyển đổi)
    */
    this.actions = [];

    let item1: MenuItem = {
      id: '1', label: 'Chuyển đổi', icon: 'pi pi-replay', command: () => {
        this.selectedPotentialCustomer = rowData;
        this.openConvertDialog(rowData);
      }
    }

    let item3: MenuItem = {
      id: '3', label: 'Tạo báo giá', icon: 'pi pi-plus', command: () => {
        this.router.navigate(['/customer/quote-create', { customerId: rowData.customerId }]);
      }
    }

    let item4: MenuItem = {
      id: '4', label: 'Gửi email', icon: 'pi pi-comment', command: () => {
        this.send_email(rowData.customerId);
      }
    }

    let item5: MenuItem = {
      id: '5', label: 'Gửi SMS', icon: 'pi pi-envelope', command: () => {
        this.send_sms(rowData.customerId);
      }
    }

    // let item6: MenuItem = {
    //   id: '6', label: 'Tạo hoạt động', icon: 'pi pi-plus', command: () => {
    //   }
    // }

    let item7: MenuItem = {
      id: '7', label: 'Xóa khách hàng', icon: 'pi pi-trash', command: () => {
        this.deletePotentialCustomer(rowData.customerId)
      }
    }

    if (!rowData.isConverted) {
      this.actions.push(item1);
    }
    this.actions.push(item3);

    if (this.actionEmail === true) {
      this.actions.push(item4);
    }

    if (this.actionSMS) {
      this.actions.push(item5);
    }

    // this.actions.push(item6);

    if (this.actionDelete === true && !rowData.isConverted) {
      this.actions.push(item7);
    }

  }

  deletePotentialCustomer(customerId) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.customerService.changeCustomerStatusToDelete(customerId).subscribe(response => {
          this.loading = false;
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.listPotentialCustomer = this.listPotentialCustomer.filter(e => e.customerId != customerId);
            let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Xóa khách hàng tiềm năng thành công' };
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo', detail: 'Xóa khách hàng tiềm năng thất bại' };
            this.showMessage(mgs);
          }
        }, error => { this.loading = false; });
      }
    });
  }

  openConvertDialog(rowData: any) {
    let ref = this.dialogService.open(PotentialConversionDialogComponent, {
      data: {
        customerId: rowData.customerId
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
          this.showToast('success', 'Thông báo', 'Chuyển đổi khách khách hàng tiềm năng thành công');
          this.searchCustomer();
        } else {
          this.searchCustomer();
        }
      }
    });
  }

  checkEnterPress(event: any) {
    if (event.code === "Enter") {
      this.searchCustomer();
    }
  }

  closeChooseFileImportDialog() {
    this.cancelFile();
  }

  cancelFile() {
    let fileInput = $("#importFileProduct")
    fileInput.replaceWith(fileInput.val('').clone(true));
    this.fileName = "";
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  exportExcel() {
    if (this.selectedCustomers.length > 0) 
    {
      if (this.selectedColumns.length > 0) 
      {
        let title = `Danh sách khách hàng tiềm năng`;

        let workBook = new Workbook();
        let worksheet = workBook.addWorksheet(title);

        let dataHeaderMain = "Danh sách khách hàng tiềm năng".toUpperCase();
        let headerMain = worksheet.addRow([dataHeaderMain]);
        headerMain.font = { size: 18, bold: true };
        worksheet.mergeCells(`A${1}:G${1}`);
        headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        worksheet.addRow([]);

        /* Header row */
        let buildColumnExcel = this.buildColumnExcel();
        let dataHeaderRow = buildColumnExcel.map(x => x.name);
        let headerRow = worksheet.addRow(dataHeaderRow);
        headerRow.font = { name: 'Times New Roman', size: 10, bold: true };
        dataHeaderRow.forEach((item, index) => {
          headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          headerRow.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: '8DB4E2' }
          };
        });
        headerRow.height = 40;

        /* Data table */
        let data: Array<any> = [];
        this.selectedCustomers.forEach(item => {
          let row: Array<any> = [];
          buildColumnExcel.forEach((_item, _index) => {
            if (_item.code == 'customerName') 
            {
              row[_index] = item.customerName;
            }
            else if (_item.code == 'contactName') 
            {
              row[_index] = item.contactName;
            }
            else if (_item.code == 'customerEmail') 
            {
              row[_index] = item.customerEmail;
            }
            else if (_item.code == 'customerPhone') 
            {
              row[_index] = item.customerPhone;
            }
            else if (_item.code == 'createdDate') 
            {
              row[_index] = this.datePipe.transform(item.createdDate, 'dd/MM/yyyy');
            }
            else if (_item.code == 'careStateName') 
            {
              row[_index] = item.careStateName;
            }
            else if (_item.code == 'statusName') 
            {
              row[_index] = item.statusName;
            }
            else if (_item.code == 'picName') 
            {
              row[_index] = item.picName;
            }
            else if (_item.code == 'statusSupportName')
            {
              row[_index] = item.statusSupportName;
            }
          });

          data.push(row);
        });

        data.forEach((el, index, array) => {
          let row = worksheet.addRow(el);
          row.font = { name: 'Times New Roman', size: 11 };

          buildColumnExcel.forEach((_item, _index) => {
            row.getCell(_index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
            if (_item.code == 'customerName' || _item.code == 'contactName' || _item.code == 'customerEmail'
              || _item.code == 'picName')
            {
              row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'left' };
            }
            else 
            {
              row.getCell(_index + 1).alignment = { vertical: 'middle', horizontal: 'center' };
            }
          });
        });

        /* fix with for column */
        buildColumnExcel.forEach((item, index) => {
          worksheet.getColumn(index + 1).width = item.width;
        });

        this.exportToExel(workBook, title);
      }
      else 
      {
        this.showToast('warn', 'Thông báo', 'Bạn phải chọn ít nhất 1 cột');
      }
    }
    else 
    {
      this.showToast('warn', 'Thông báo', 'Bạn chưa chọn khách hàng');
    }
  }

  buildColumnExcel(): Array<ColumnExcel> {
    let listColumn: Array<ColumnExcel> = [];

    this.selectedColumns.forEach(item => {
      let column = new ColumnExcel();
      column.code = item.field;
      column.name = item.header;
      
      if (item.field == 'customerName') 
      {
        column.width = 30;
      }
      else if (item.field == 'contactName')
      {
        column.width = 30;
      }
      else if (item.field == 'customerEmail')
      {
        column.width = 30;
      }
      else if (item.field == 'customerPhone')
      {
        column.width = 15;
      }
      else if (item.field == 'createdDate')
      {
        column.width = 15;
      }
      else if (item.field == 'careStateName')
      {
        column.width = 20;
      }
      else if (item.field == 'statusName')
      {
        column.width = 20;
      }
      else if (item.field == 'picName')
      {
        column.width = 30;
      }
      else if (item.field == 'statusSupportName')
      {
        column.width = 20;
      }

      listColumn.push(column);
    });

    return listColumn;
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  getDefaultApplicationName() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "ApplicationName")?.systemValueString;
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
function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};


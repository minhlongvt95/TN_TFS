import { Component, OnInit, ViewChild, ElementRef, HostListener, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { CustomerImportComponent } from '../customer-import/customer-import.component';
import { CustomerDownloadTemplateComponent } from '../customer-download-template/customer-download-template.component';
import { SendEmailDialogComponent } from '../../../shared/components/send-email-dialog/send-email-dialog.component';
import { SendSmsDialogComponent } from '../../../shared/components/send-sms-dialog/send-sms-dialog.component';
import { CustomerImportDetailComponent } from '../customer-import-detail/customer-import-detail.component';
import { EmployeeService } from "../../../employee/services/employee.service";
import { TranslateService } from '@ngx-translate/core';
import { CategoryService } from "../../../shared/services/category.service";
import { CustomerService } from '../../services/customer.service';
import { CustomerModel } from "../../models/customer.model";
import { GetPermission } from '../../../shared/permission/get-permission';

import { ngxLoadingAnimationTypes, NgxLoadingComponent } from "ngx-loading";
import { SortEvent } from 'primeng/api';
import { MenuItem } from "primeng/primeng";
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DialogService } from 'primeng/dynamicdialog';
import * as $ from 'jquery';
import * as XLSX from 'xlsx';
import { AddCustomerDialogComponent } from '../add-customer-dialog/add-customer-dialog.component';

declare var google: any; //new added line

class NewCustomerModel {
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
  public statusCareName: string;
  public backgroupStatus: string;
  public backgroundStatusCare: string;
  public colorStatusCare: string;
  public longitude: number;
  public latitude: number;
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
  public FirstName: string;
  public LastName: string;
  public Phone: string;
  public Email: string;
  public Address: string;
  public StatusCareId: string;
  public AreaId: string;
  // public SourceId: Array<string>;
  public CustomerServiceLevelIdList: Array<string>;
  public CustomerGroupIdList: Array<string>;
  public PersonInChargeIdList: Array<string>;
  public NoPic: boolean;
  public IsBusinessCus: boolean;
  public IsPersonalCus: boolean;
  public IsIdentificationCus: boolean;
  public IsFreeCus: boolean;
  public IsHKDCus: boolean;
  public CustomerCode: string;
  public TaxCode: string;
  public UserId: string;
  public FromDate: Date;
  public ToDate: Date;

  constructor() {
    // this.SourceId = [];
    this.CustomerServiceLevelIdList = [];
    this.CustomerGroupIdList = [];
    this.PersonInChargeIdList = [];
  }
}

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

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
}


@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.css'],
  providers: [DialogService]
})
export class CustomerListComponent implements OnInit, AfterViewInit {
  @ViewChild('table') table: Table;
  filterGlobal: string;
  innerWidth: number = 0; //number window size first
  loadingComponent: boolean = false;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  customerCode: string = '';
  startDate: Date = new Date();
  minYear: number = 2015;
  currentYear: number = (new Date()).getFullYear();
  endDate: Date = new Date();
  maxEndDate: Date = new Date();
  selectedColumns: any[];
  colsCustomer: any[];
  newListCustomer: Array<NewCustomerModel> = [];
  rows = 10;
  selectedCustomers: Array<NewCustomerModel> = [];
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

  //map
  currentMode: boolean = true;
  options: any = {};
  overlays: any[] = [];
  map: google.maps.Map;
  zoom: number = 16;
  displayCustomerDetailDialog: boolean = false;
  selectedCustomerOnMap: NewCustomerModel = new NewCustomerModel();

  //import varriables
  displayChooseFileImportDialog: boolean = false;
  fileName: string = '';
  importFileExcel: any = null;

  listStatusCare: Array<any> = [];
  listSource: Array<any> = [];
  listArea: Array<any> = [];

  @ViewChild('ngxLoading') ngxLoadingComponent: NgxLoadingComponent;
  public ngxLoadingAnimationTypes = ngxLoadingAnimationTypes;
  loadingConfig: any = {
    'animationType': ngxLoadingAnimationTypes.circle,
    'backdropBackgroundColour': 'rgba(0,0,0,0.1)',
    'backdropBorderRadius': '4px',
    'primaryColour': '#ffffff',
    'secondaryColour': '#999999',
    'tertiaryColour': '#ffffff'
  }
  selectedCustomerList: Array<any> = [];

  constructor(
    private ref: ChangeDetectorRef,
    private dialogService: DialogService,
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private translate: TranslateService,
    private customerService: CustomerService,
    private employeeService: EmployeeService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    //Kiểm tra nhân viên hoặc quản lý
    this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
    this.employeeId = JSON.parse(localStorage.getItem('auth')).EmployeeId;

    //Check permission
    let resource = "crm/customer/list";
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
          if (param['mode'] == 'Identification') {
            // this.filterForm.controls['IsIdentificationCustomer'].setValue(true);
            // this.filterForm.controls['IsFreeCustomer'].setValue(false);
            this.filterForm.updateValueAndValidity();
          } else if (param['mode'] == 'Free') {
            // this.filterForm.controls['IsIdentificationCustomer'].setValue(false);
            // this.filterForm.controls['IsFreeCustomer'].setValue(true);
            this.filterForm.updateValueAndValidity();
          }
          this.getMasterData();
        } else {
          // this.filterForm.controls['IsIdentificationCustomer'].setValue(true);
          // this.filterForm.controls['IsFreeCustomer'].setValue(true);
          this.filterForm.updateValueAndValidity();
          this.getMasterData();
        }
      });
    }
  }

  ngAfterViewInit() {
    this.ref.detectChanges();
  }

  setMap(event) {
    this.map = event.map;
  }

  initTable() {
    this.selectedColumns = [
      { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'customerEmail', header: 'Email', textAlign: 'left', display: 'table-cell' },
      { field: 'customerPhone', header: 'Số điện thoại', textAlign: 'left', display: 'table-cell' },
      { field: 'createdDate', header: 'Ngày tạo', textAlign: 'left', display: 'table-cell' },
      // { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
      { field: 'statusCareName', header: 'Trạng thái chăm sóc', textAlign: 'center', display: 'table-cell' },
      { field: 'picName', header: 'Người phụ trách', textAlign: 'left', display: 'table-cell' },
    ];
    this.colsCustomer = this.selectedColumns;
  }

  initForm() {
    this.filterForm = new FormGroup({
      'Name': new FormControl(''),
      'Phone': new FormControl(''),
      'Email': new FormControl(''),
      'Address': new FormControl(''),
      'FromDate': new FormControl(null),
      'ToDate': new FormControl(null),
      'Area': new FormControl(null),
      // 'Source': new FormControl(null),
      'Status': new FormControl(null),
      'Level': new FormControl(null),
      'Group': new FormControl(null),
      'Pic': new FormControl(null),
      'CustomerCode': new FormControl(''),
      'TaxCode': new FormControl(''),
      'HavePic': new FormControl(false),
      'IsCompanyCustomer': new FormControl(true),
      'IsPersonalCustomer': new FormControl(true),
      'IsHouseCustomer': new FormControl(true),
      'IsIdentificationCustomer': new FormControl(true),
      'IsFreeCustomer': new FormControl(true),
    });
  }

  resetTable() {
    this.filterGlobal = '';
    if (!this.table) return;
    this.table.sortOrder = 0;
    this.table.sortField = '';
    this.table.reset();
  }

  resetForm() {
    this.filterForm.patchValue({
      'Name': '',
      'Phone': '',
      'Email': '',
      'Address': '',
      'FromDate': null,
      'ToDate': null,
      'Area': null,
      // 'Source': null,
      'Status': null,
      'Level': null,
      'Group': null,
      'Pic': null,
      'CustomerCode': '',
      'TaxCode': '',
      'HavePic': false,
      'IsCompanyCustomer': true,
      'IsPersonalCustomer': true,
      'IsHouseCustomer': true,
      'IsIdentificationCustomer': true,
      'IsFreeCustomer': true,
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  searchCustomer() {
    //reset table
    this.resetTable();

    let searchCustomerModel = this.mappingFomrToModel();
    this.loadingComponent = true;
    this.customerService.searchCustomer(
      searchCustomerModel.NoPic,
      searchCustomerModel.IsBusinessCus,
      searchCustomerModel.IsPersonalCus,
      searchCustomerModel.StatusCareId,
      searchCustomerModel.CustomerGroupIdList,
      searchCustomerModel.PersonInChargeIdList,
      // searchCustomerModel.SourceId,
      searchCustomerModel.AreaId,
      searchCustomerModel.FromDate,
      searchCustomerModel.ToDate,
      searchCustomerModel.FirstName,
      searchCustomerModel.LastName,
      searchCustomerModel.Phone,
      searchCustomerModel.Email,
      searchCustomerModel.Address,
      // searchCustomerModel.CustomerServiceLevelIdList,
      // searchCustomerModel.IsIdentificationCus,
      // searchCustomerModel.IsFreeCus,
      // searchCustomerModel.IsHKDCus,
      // searchCustomerModel.CustomerCode,
      // searchCustomerModel.TaxCode,
      searchCustomerModel.UserId).subscribe(response => {
        this.loadingComponent = false;
        let result = <any>response;
        if (result.statusCode === 200) {
          this.newListCustomer = result.customer;
          
          this.setIndex();
          if (this.newListCustomer.length === 0) {
            let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy khách hàng nào!' };
            this.showMessage(mgs);
          }
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }

        $(".ui-button-text-empty").css("display", "none")
        $(".ui-button-icon-left").removeClass("pi pi-chevron-down")
        $(".ui-button-icon-left").addClass("pi pi-cog")
      });
  }

  setIndex() {
    this.newListCustomer.forEach((customer, index) => {
      customer.index = index + 1;
    });
  }

  async getMasterData() {
    this.loadingComponent = true;
    let [getListCustomerAsyncResponse, getEmployeeCareStaffAsyncResponse]: any = await Promise.all([
      this.customerService.getListCustomerAsync("NHA"),
      this.employeeService.getEmployeeCareStaffAsyc(this.isManager, this.employeeId)
    ]);
    //loai khach  hang portal
    this.listArea = getListCustomerAsyncResponse.listAreaModel;
    this.listStatusCare = getListCustomerAsyncResponse.listStatusCareModel;
    this.listSource = getListCustomerAsyncResponse.listSourceModel;
    this.customerGroups = getListCustomerAsyncResponse.listCategoryModel.filter(e => e.categoryCode !== "POR");
    this.serviceLevels = getListCustomerAsyncResponse.listCustomerServiceLevelModel;
    this.employees = getEmployeeCareStaffAsyncResponse.employeeList;

    this.searchCustomer();
  }

  switchMode(event: any) {
    this.ref.detectChanges();
    if (!event.checked) {
      /* chuyển sang mode bản đồ */
      /* lấy những bản ghi có tọa độ hợp lệ */
      let listCustomerPosition = [];
      this.newListCustomer.forEach(customer => {
        if (customer.longitude && customer.latitude) listCustomerPosition.push({
          lat: customer.latitude,
          lng: customer.longitude,
          title: customer.customerName,
          customerId: customer.customerId
        });
      });
      this.overlays = [];
      listCustomerPosition.forEach(pos => {
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

    let selectedCustomer = this.newListCustomer.find(cus => cus.customerId == customerId);
    if (!selectedCustomer) return;

    this.displayCustomerDetailDialog = true;
    this.selectedCustomerOnMap = selectedCustomer;
  }

  mappingFomrToModel(): SearchCustomerModel {
    let searchCustomer = new SearchCustomerModel();
    searchCustomer.FirstName = this.filterForm.get('Name').value;
    searchCustomer.LastName = ''; //default value = empty
    searchCustomer.Phone = this.filterForm.get('Phone').value;
    searchCustomer.Email = this.filterForm.get('Email').value;
    searchCustomer.Address = this.filterForm.get('Address').value;

    //get trang thai cham soc khach hang

    let statusCare = this.filterForm.get('Status').value;
    let statusCareId = statusCare === null ? this.emptyGuid : statusCare.categoryId;
    if (statusCareId !== this.emptyGuid) {
      searchCustomer.StatusCareId = statusCareId;
    }

    //get level id
    // let levelIdFormValue = this.filterForm.get('Level').value;
    // let levelId = levelIdFormValue === null ? this.emptyGuid : levelIdFormValue.customerServiceLevelId;
    // if (levelId !== this.emptyGuid) {
    //   searchCustomer.CustomerServiceLevelIdList.push(levelId);
    // }

    //get nhom kh
    let groupIdFormValue = this.filterForm.get('Group').value;
    let groupId: Array<string> = [];
    if (groupIdFormValue !== null) {
      groupId = groupIdFormValue.map(x => x.categoryId);
      groupId.forEach(x => {
        searchCustomer.CustomerGroupIdList.push(x);
      });
    }


    //get nv phu trach
    let picIdValue = this.filterForm.get('Pic').value;
    let picId = picIdValue === null ? this.emptyGuid : picIdValue.employeeId;
    if (picId !== this.emptyGuid) {
      searchCustomer.PersonInChargeIdList.push(picId);
    }

    //get nguon tiem nang
    // let source = this.filterForm.get('Source').value;
    // let sourceId: Array<string> = [];
    // if (source !== null) {
    //   sourceId = source.map(x => x.categoryId);
    //   sourceId.forEach(x => {
    //     searchCustomer.SourceId.push(x);
    //   });
    // }

    //get khu vuc
    let area = this.filterForm.get('Area').value;
    let areaId = area === null ? this.emptyGuid : area.areaId;
    if (areaId !== this.emptyGuid) {
      searchCustomer.AreaId = areaId;
    }

    // get thowi gian tao
    let formDate = this.filterForm.get('FromDate').value;
    let toDate = this.filterForm.get('ToDate').value;

    if (formDate) {
      formDate.setHours(0, 0, 0, 0);
      formDate = convertToUTCTime(formDate);
      searchCustomer.FromDate = formDate;
    }
    if (toDate) {
      toDate.setHours(23, 59, 59, 999);
      toDate = convertToUTCTime(toDate);
      searchCustomer.ToDate = toDate;
    }

    searchCustomer.NoPic = this.filterForm.get('HavePic').value;
    searchCustomer.IsBusinessCus = this.filterForm.get('IsCompanyCustomer').value;
    searchCustomer.IsPersonalCus = this.filterForm.get('IsPersonalCustomer').value;
    // searchCustomer.IsHKDCus = this.filterForm.get('IsHouseCustomer').value;
    // searchCustomer.CustomerCode = this.filterForm.get('CustomerCode').value;
    // searchCustomer.TaxCode = this.filterForm.get('TaxCode').value;
    searchCustomer.UserId = this.auth.UserId;
    // searchCustomer.IsIdentificationCus = this.filterForm.get('IsIdentificationCustomer').value;
    // searchCustomer.IsFreeCus = this.filterForm.get('IsFreeCustomer').value;

    return searchCustomer;
  }

  goToCreate() {
    this.router.navigate(['/customer/create']);
  }

  openAddCustomerDialog(){
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

    ref.onClose.subscribe(async (result: ResultDialog) => {
      if (result) {
        if (result.status) {
          let msg1 = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo khách hàng thành công' };
          this.showMessage(msg1);
          this.searchCustomer();
        } else {
          this.searchCustomer();
        }
      }
    });
  }

  //redirect đến trang tạo báo giá
  goToNewCustomerQuote() {
    this.router.navigate(['/customer/quote-create']);
  }

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
  goToNewCustomerOrder() {
    this.router.navigate(['/order/create']);
  }


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
    // let ref = this.dialogService.open(CustomerImportComponent, {
    //   header: 'Import khách hàng',
    //   width: '50%',
    //   baseZIndex: 1030,
    //   contentStyle: {
    //     "max-height": "800px",
    //   }
    // });

    // ref.onClose.subscribe((result: any) => {
    //   if (result) {
    //     if (result.statusImport == true) {
    //       this.getMasterData();
    //     }
    //   }
    // });
    this.displayChooseFileImportDialog = true;
  }

  del_customer(customerId: string) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loadingComponent = true;
        this.customerService.changeCustomerStatusToDelete(customerId).subscribe(response => {
          this.loadingComponent = false;
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            //remove from list
            this.newListCustomer = this.newListCustomer.filter(e => e.customerId != customerId);
            let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Xóa khách hàng thành công' };
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo', detail: 'Xóa khách hàng thất bại' };
            this.showMessage(mgs);
          }
        }, error => { this.loadingComponent = false; });
      }
    });
  }

  downloadTemplate() {
    let ref = this.dialogService.open(CustomerDownloadTemplateComponent, {
      header: 'Tải mẫu Excel',
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "200px",
        "max-height": "600px"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          if (result.message == "success") {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tải thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        }
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
        this.leftColNumber = 7;
        this.rightColNumber = 5;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  pageChange(event: any) {
  }

  goToDetail(rowData: any) {
    this.router.navigate(['/customer/detail', { customerId: rowData.customerId, contactId: rowData.contactId }]);
  }

  onChangeAction(rowData: any) {
    this.actions = [];
    let createQuote: MenuItem = {
      id: '1', label: 'Tạo báo giá', icon: 'pi pi-clone', command: () => {
        if (rowData.picName === undefined || rowData.picName.trim() === "" || rowData.picName === null) {
          let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Khách hàng chưa được gán người phụ trách!' };
          this.showMessage(mgs);
        }
        else {
          this.goToCustomerQuote(rowData.customerId);
        }
      }
    }

    let createOrder: MenuItem = {
      id: '2', label: 'Thêm đơn hàng', icon: 'pi pi-shopping-cart', command: () => {
        this.goToCustomerOrder(rowData.customerId);
      }
    }

    let sendSMS: MenuItem = {
      id: '3', label: 'Gửi SMS', icon: 'pi pi-envelope', command: () => {
        this.send_sms(rowData.customerId);
      }
    }

    let sendEmail: MenuItem = {
      id: '4', label: 'Gửi Email', icon: 'pi pi-comment', command: () => {
        this.send_email(rowData.customerId);
      }
    }

    let deleteCustomer: MenuItem = {
      id: '5', label: 'Xóa khách hàng', icon: 'pi pi-trash', command: () => {
        this.del_customer(rowData.customerId);
      }
    }

    /* tạo cơ hội */
    let createLead: MenuItem = {
      id: '6', label: 'Tạo cơ hội', icon: 'pi pi-clone', command: () => {
        this.goToLead(rowData.customerId);
      }
    }

    if (this.actionAdd === true) {
      this.actions.push(createQuote);
      this.actions.push(createLead);
      this.actions.push(createOrder);
    }

    if (this.actionSMS === true) {
      this.actions.push(sendSMS);
    }

    if (this.actionEmail === true) {
      this.actions.push(sendEmail);
    }

    if (this.actionDelete === true && rowData.countCustomerInfo === 0) {
      this.actions.push(deleteCustomer);
    }
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

  async downloadTemplateExcel() {
    this.loadingComponent = true;
    let result: any = await this.customerService.downloadTemplateImportCustomer(this.auth.UserId);
    this.loadingComponent = false;

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

  chooseFile(event: any) {
    this.fileName = event.target?.files[0]?.name;
    this.importFileExcel = event.target;
  }

  onClickImportBtn(event: any) {
    /* clear value của file input */
    event.target.value = ''
  }

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
        customer.firstName = _rawData[0] ? _rawData[0].toString().trim() : '';
        customer.lastName = _rawData[1] ? _rawData[1].toString().trim() : '';
        customer.isEmployee = _rawData[2] == "1" ? true : false;
        customer.isVendor = _rawData[3] == "1" ? true : false;
        customer.customerCode = _rawData[4] ? _rawData[4].toString().trim() : '';
        customer.isCompany = _rawData[5] == "1" ? true : false;
        customer.companyName = _rawData[6] ? _rawData[6].toString().trim() : '';
        customer.address = _rawData[7] ? _rawData[7].toString().trim() : '';
        customer.taxCode = _rawData[8] ? _rawData[8].toString().trim() : '';
        customer.phone = _rawData[9] ? _rawData[9].toString().trim() : '';
        customer.email = _rawData[10] ? _rawData[10].toString().trim() : '';
        customer.fullNameOfContact = _rawData[11] ? _rawData[11].toString().trim() : '';
        customer.genderOfContact = _rawData[12] ? _rawData[12].toString().trim() : '';
        customer.addressOfContact = _rawData[13] ? _rawData[13].toString().trim() : '';
        customer.phoneOfContact = _rawData[14] ? _rawData[14].toString().trim() : '';
        customer.emailOfContact = _rawData[15] ? _rawData[15].toString().trim() : '';
        customer.note = _rawData[16] ? _rawData[16].toString().trim() : '';

        listCustomerImport = [...listCustomerImport, customer];
      });
      /* tắt dialog import file, bật dialog chi tiết khách hàng import */
      this.displayChooseFileImportDialog = false;
      this.openDetailImportDialog(listCustomerImport);
    }
  }

  openDetailImportDialog(listCustomerImport) {
    let ref = this.dialogService.open(CustomerImportDetailComponent, {
      data: {
        isPotentialCustomer: false,
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

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

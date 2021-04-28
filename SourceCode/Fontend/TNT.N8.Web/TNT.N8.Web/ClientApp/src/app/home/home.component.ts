import { Component, OnInit, ViewChild, ElementRef, Renderer2 } from '@angular/core';
import { DatePipe } from '@angular/common';

import { Router, ActivatedRoute } from '@angular/router';
import { CompanyConfigModel } from '../shared/models/companyConfig.model';
import { BreadCrumMenuModel } from '../shared/models/breadCrumMenu.model';
import { ChangepasswordComponent } from '../shared/components/changepassword/changepassword.component';
import { UserprofileComponent } from "../userprofile/userprofile.component"
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { EventEmitterService } from '../shared/services/event-emitter.service';
import { NotificationService } from '../shared/services/notification.service';
import { DashboardHomeService } from '../shared/services/dashboard-home.service';
import { AuthenticationService } from '../shared/services/authentication.service';
// Full canlendar
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { ConfirmationService, MessageService } from 'primeng/api';
import { MenuItem } from 'primeng/api';

import * as $ from 'jquery';
import { GetPermission } from '../shared/permission/get-permission';
import { FullCalendar } from 'primeng/fullcalendar';
import { title } from 'process';
import { utc } from 'moment';
import { MeetingDialogComponent } from '../customer/components/meeting-dialog/meeting-dialog.component';
import { DialogService } from 'primeng';
interface Quote {
  quoteId: string;
  quoteCode: string;
  amount: number;
  objectTypeId: string;
  objectType: string;
  customerName: string;
  customerContactId: string;
  seller: string;
  sellerName: string;
  quoteDate: Date;
}

interface Customer {
  customerId: string;
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  picName: string;
  picContactId: string;
  dateOfBirth: Date;
}

interface Order {
  orderId: string;
  orderCode: string;
  customerId: string;
  customerName: string;
  amount: number;
  seller: string;
  sellerName: string;
  sellerContactId: string;
}

interface CustomerMeeting {
  customerMeetingId: string;
  customerId: string;
  employeeId: string;
  startDate: Date;
  endDate: Date;
  title: string;
  content: string;
  locationMeeting: string;
  customerName: string;
  employeeName: string;
  createByName: string;
  participants: string;
}

interface LeadMeeting {
  leadMeetingId: string;
  leadId: string;
  employeeId: string;
  startDate: Date;
  endDate: Date;
  title: string;
  content: string;
  locationMeeting: string;
  leadName: string;
  employeeName: string;
  createByName: string;
  isShowLink: boolean;
}

interface Employee {
  employeeId: string;
  employeeName: string;
  organizationId: string;
  organizationName: string;
  positionId: string;
  positionName: string;
  dateOfBirth: Date;
  contactId: string;
}

class Calendar {
  id: string;
  title: string;
  start: Date;
  end: Date;
  backgroundColor: string;
  participants: string;
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  providers: [
    DatePipe
  ]
})
export class HomeComponent implements OnInit {
  @ViewChild('toggleNotifi') toggleNotifi: ElementRef;
  isOpenNotifi: boolean = false;

  @ViewChild('toggleConfig') toggleConfig: ElementRef;
  isOpenConfig: boolean = false;

  @ViewChild('toggleCreateElement') toggleCreateElement: ElementRef;
  isOpenCreateElement: boolean = false;

  @ViewChild('toggleUser') toggleUser: ElementRef;
  isOpenUser: boolean = false;

  @ViewChild('dropdownMenus') dropdownMenus: ElementRef;

  @ViewChild('calendar') calendar: FullCalendar;
  /**/
  listPermissionResource: Array<string> = localStorage.getItem('ListPermissionResource').split(',');
  listPermissionResourceActive: string = localStorage.getItem("ListPermissionResource");
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  applicationName = this.getDefaultApplicationName();
  listModule: Array<string> = [];
  listResource: Array<string> = [];
  lstBreadCrumLeftMenu: Array<BreadCrumMenuModel> = [];
  lstBreadCrum: Array<BreadCrumMenuModel> = [];
  listParticipants: Array<Employee> = [];
  /*Module name*/
  moduleCrm = 'crm'; //Module Quản trị quan hệ khách hàng
  moduleSal = 'sal'; //Module Quản lý bán hàng
  moduleBuy = 'buy'; //Module Quản lý mua hàng
  moduleAcc = 'acc'; //Module Quản lý tài chính
  moduleHrm = 'hrm'; //Module Quản trị nhân sự
  moduleSys = 'sys'; //Module Quản trị hệ thống
  /*End*/

  isCustomer = false;
  isSales = false;
  isShopping = false;
  isAccounting = false;
  isHrm = false;
  isAdmin = false;
  isWarehouse = false;
  isProject = false;
  companyConfigModel = new CompanyConfigModel();
  userAdmin = false;
  permissionAdmin = false;
  moduled: string;
  isToggleCick: Boolean = false;

  notificationNumber: number = 0;
  NotificationContent: string;
  notificationList: Array<any> = [];
  auth: any = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;

  username: string;
  userAvatar: string;
  userFullName: string;
  userEmail: string;
  dialogRef: MatDialogRef<ChangepasswordComponent>;
  dialogPopup: MatDialogRef<UserprofileComponent>;

  // full calendar
  events: Array<Calendar> = [];
  options: any;

  lstSubmenuLevel3: Array<BreadCrumMenuModel> = [
    //Quan ly he thong
    { Name: "Cấu hình thông tin chung", Path: "/admin/company-config", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "settings", IsDefault: true, CodeParent: "sys_chttc", LstChildren: [], Display: "none", Code: '' },
    { Name: "Cấu hình thư mục", Path: "/admin/folder-config", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "settings", IsDefault: true, CodeParent: "sys_chtm", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý thông báo", Path: "/admin/notifi-setting-list", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "settings", IsDefault: true, CodeParent: "sys_tb", LstChildren: [], Display: "none", Code: '' },
    { Name: "Tham số hệ thống", Path: "/admin/system-parameter", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "settings_applications", IsDefault: true, CodeParent: "sys_tsht", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý mẫu Email", Path: "/admin/email-configuration", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "device_hub", IsDefault: true, CodeParent: "Systems_QLE", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý sơ đồ tổ chức", Path: "/admin/organization", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "device_hub", IsDefault: true, CodeParent: "sys_sdtc", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý dữ liệu danh mục", Path: "/admin/masterdata", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "category", IsDefault: true, CodeParent: "sys_dldm", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý nhóm quyền", Path: "/admin/permission", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "format_list_bulleted", IsDefault: true, CodeParent: "sys_nq", LstChildren: [], Display: "none", Code: '' },
    { Name: "Phân hạng khách hàng", Path: "/admin/config-level-customer", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "filter_list", IsDefault: true, CodeParent: "sys_phkh", LstChildren: [], Display: "none", Code: '' },
    { Name: "Quản lý quy trình làm việc", Path: "/admin/workflow/workflow-list", ObjectType: "sys", LevelMenu: 3, Active: false, nameIcon: "swap_horiz", IsDefault: true, CodeParent: "sys_qtlv", LstChildren: [], Display: "none", Code: '' },
  ];

  /*Data chart revenue employee*/
  dataRevenueEmployee: any;
  optionsRevenueEmployee: any;
  /*End*/

  /*Data chart Chance*/
  dataChance: any;
  optionsDataChance: any;
  /*End*/

  isManager: boolean = false;

  colsQuote: Array<any> = [];
  colsCustomer: Array<any> = [];
  colsOrder: Array<any> = [];
  colsCustomerMeeting: Array<any> = [];
  colsCusBirthdayOfWeek: Array<any> = [];
  colsEmployeeBirthDayOfWeek: Array<any> = [];
  selectedColsCustomerMeeting: Array<any> = [];
  colsLeadMeeting: Array<any> = [];
  selectedColsLeadMeeting: Array<any> = [];

  totalSalesOfWeek: number = 0;
  totalSalesOfMonth: number = 0;
  totalSalesOfQuarter: number = 0;

  totalSalesOfWeekPress: number = 0;
  totalSalesOfMonthPress: number = 0;
  totalSalesOfQuarterPress: number = 0;

  salesOfWeek: number = 0;
  salesOfMonth: number = 0;
  salesOfQuarter: number = 0;

  listQuote: Array<Quote> = [];
  listCustomer: Array<Customer> = [];
  listOrderNew: Array<Order> = [];
  listCustomerMeeting: Array<CustomerMeeting> = [];
  listLeadMeeting: Array<LeadMeeting> = [];
  listCusBirthdayOfWeek: Array<Customer> = [];
  listEmployeeBirthDayOfWeek: Array<Employee> = [];
  lstSubmenuLevel1: Array<BreadCrumMenuModel> = [
    //Module CRM
    {
      Name: "Quản trị khách hàng", Path: "", ObjectType: "crm", LevelMenu: 1, Active: false, nameIcon: "fa-street-view", IsDefault: false, CodeParent: "Lead_QLKHTN_Module", Display: "none",
      LstChildren: [
        {
          Name: "Khách hàng tiềm năng", Path: "", ObjectType: "customer", LevelMenu: 2, Active: false, nameIcon: "fa-user", IsDefault: false, CodeParent: "crm_khtn", Display: "none",
          LstChildren: [
            { Name: "Dashboard", Path: "/customer/potential-customer-dashboard", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u39.png", IsDefault: true, CodeParent: "crm_khtn", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tạo mới", Path: "/customer/potential-customer-create", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u43.png", IsDefault: false, CodeParent: "crm_khtn", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm", Path: "/customer/potential-customer-list", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "crm_khtn", LstChildren: [], Display: "none", Code: 'Customer_TK' },
            { Name: "Phê duyệt", Path: "/customer/request-approval", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "crm_khtn", LstChildren: [], Display: "none", Code: 'Customer_TK' },
          ], Code: ''
        },
        {
          Name: "Cơ hội", Path: "", ObjectType: "lead", LevelMenu: 2, Active: false, nameIcon: "fa-binoculars", IsDefault: false, CodeParent: "crm_ch", Display: "none",
          LstChildren: [
            { Name: "Dashboard", Path: "/lead/dashboard", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u39.png", IsDefault: true, CodeParent: "crm_ch", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tạo mới", Path: "/lead/create-lead", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u43.png", IsDefault: false, CodeParent: "crm_ch", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm", Path: "/lead/list", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "crm_ch", LstChildren: [], Display: "none", Code: 'Lead_TK' },
            { Name: "Báo cáo", Path: "/lead/report-lead", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u39.png", IsDefault: false, CodeParent: "crm_ch", LstChildren: [], Display: "none", Code: '' }
          ], Code: ''
        },
        {
          Name: "Hồ sơ thầu", Path: "", ObjectType: "customer", LevelMenu: 2, Active: false, nameIcon: "fa-file-archive-o", IsDefault: false, CodeParent: "crm_hst", Display: "none",
          LstChildren: [
            { Name: "Dashboard", Path: "/sale-bidding/dashboard", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u39.png", IsDefault: true, CodeParent: "crm_hst", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tạo mới", Path: "#", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u43.png", IsDefault: false, CodeParent: "crm_hst", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm", Path: "/sale-bidding/list", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "crm_hst", LstChildren: [], Display: "none", Code: 'CustomerHST_TK' },
            { Name: "Phê duyệt", Path: "/sale-bidding/approved", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "crm_hst", LstChildren: [], Display: "none", Code: 'CustomerHST_PD' },
          ], Code: ''
        },
        {
          Name: "Báo giá", Path: "", ObjectType: "customer", LevelMenu: 2, Active: false, nameIcon: "fa-quote-right", IsDefault: false, CodeParent: "crm_bg", Display: "none",
          LstChildren: [
            { Name: "Dashboard", Path: "/customer/quote-dashboard", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u39.png", IsDefault: true, CodeParent: "crm_bg", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tạo mới", Path: "/customer/quote-create", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u43.png", IsDefault: false, CodeParent: "crm_bg", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm", Path: "/customer/quote-list", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "crm_bg", LstChildren: [], Display: "none", Code: 'Quote_TK' },
            { Name: "Phê duyệt", Path: "/customer/quote-approval", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "crm_bg", LstChildren: [], Display: "none", Code: 'Quote_TK' },
          ], Code: ''
        },
        {
          Name: "Khách hàng", Path: "", ObjectType: "customer", LevelMenu: 2, Active: false, nameIcon: "fa-user", IsDefault: false, CodeParent: "crm_kh", Display: "none",
          LstChildren: [
            { Name: "Dashboard", Path: "/customer/dashboard", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u39.png", IsDefault: true, CodeParent: "crm_kh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tạo mới", Path: "/customer/create", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u43.png", IsDefault: false, CodeParent: "crm_kh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm", Path: "/customer/list", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "crm_kh", LstChildren: [], Display: "none", Code: 'Customer_TK' },
          ], Code: ''
        },
        {
          Name: "Chăm sóc KH", Path: "", ObjectType: "customer", LevelMenu: 2, Active: false, nameIcon: "fa-gift", IsDefault: false, CodeParent: "crm_cskh", Display: "none",
          LstChildren: [
            { Name: "Dashboard", Path: "/customer/care-dashboard", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u39.png", IsDefault: true, CodeParent: "crm_cskh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tạo chương trình chăm sóc", Path: "/customer/care-create", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u43.png", IsDefault: false, CodeParent: "crm_cskh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Theo dõi chăm sóc khách hàng", Path: "/customer/care-list", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "crm_cskh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tạo CT khuyến mãi", Path: "/promotion/create", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "crm_cskh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Danh sách CT khuyến mãi", Path: "/promotion/list", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "crm_cskh", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
        {
          Name: "Báo cáo", Path: "", ObjectType: "customer", LevelMenu: 2, Active: false, nameIcon: "fa-pencil-square-o", IsDefault: false, CodeParent: "customer_BaoCao", Display: "none",
          LstChildren: [
            { Name: "Báo cáo thống kê chương trình CSKH", Path: "#", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "customer_BaoCao", LstChildren: [], Display: "none", Code: 'CustomerBC_TK' },
            { Name: "Báo cáo thống kê phản hồi khách hàng", Path: "#", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "customer_BaoCao", LstChildren: [], Display: "none", Code: 'CustomerBC_TK' },
            { Name: "Báo cáo doanh số thống kê khách hàng", Path: "#", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "customer_BaoCao", LstChildren: [], Display: "none", Code: 'CustomerBC_TK' },
          ], Code: ''
        },
        {
          Name: "Quản lý liên hệ", Path: "", ObjectType: "customer", LevelMenu: 2, Active: false, nameIcon: "fa-pencil-square-o", IsDefault: false, CodeParent: "crm_lh", Display: "none",
          LstChildren: [
            { Name: "Danh sách liên hệ", Path: "/customer/list-contact-customer", ObjectType: "cus", LevelMenu: 3, Active: false, nameIcon: "u41.png", IsDefault: false, CodeParent: "customer_BaoCao", LstChildren: [], Display: "none", Code: 'Customer_LH' },
          ], Code: ''
        },
      ], Code: ''
    },
    //Module Bán hàng
    {
      Name: "Quản lý bán hàng", Path: "", ObjectType: "sal", LevelMenu: 1, Active: false, nameIcon: "fa-university", IsDefault: false, CodeParent: "Sale_Module", Display: "none",
      LstChildren: [
        {
          Name: "Danh mục", Path: "", ObjectType: "sales", LevelMenu: 2, Active: false, nameIcon: "fa-book", IsDefault: false, CodeParent: "sal_dm", Display: "none",
          LstChildren: [
            { Name: "Danh mục giá bán", Path: "/product/price-list", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "category", IsDefault: false, CodeParent: "sal_dm", LstChildren: [], Display: "none", Code: 'customer_CTE_TK' },
          ], Code: ''
        },
        {
          Name: "Sản phẩm dịch vụ", Path: "", ObjectType: "sales", LevelMenu: 2, Active: false, nameIcon: "fa-binoculars", IsDefault: false, CodeParent: "sal_spdv", Display: "none",
          LstChildren: [
            { Name: "danh mục", Path: "/admin/list-product-category", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "category", IsDefault: false, CodeParent: "sal_spdv", LstChildren: [], Display: "none", Code: 'customer_CSKH_TK' },
            { Name: "Tạo mới", Path: "/product/create", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "note_add", IsDefault: false, CodeParent: "sal_spdv", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm", Path: "/product/list", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: true, CodeParent: "sal_spdv", LstChildren: [], Display: "none", Code: 'Sale_Product_TK' },
          ], Code: ''
        },
        {
          Name: "Hợp đồng bán", Path: "", ObjectType: "sales", LevelMenu: 2, Active: false, nameIcon: "fa-file-o", IsDefault: false, CodeParent: "sal_hdb", Display: "none",
          LstChildren: [
            { Name: "Dashboard", Path: "/sales/contract-dashboard", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "fa-file-o", IsDefault: true, CodeParent: "sal_hdb", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tạo hợp đồng bán", Path: "/sales/contract-create", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "fa-file-o", IsDefault: false, CodeParent: "sal_hdb", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm hợp đồng bán", Path: "/sales/contract-list", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "fa-file-o", IsDefault: false, CodeParent: "sal_hdb", LstChildren: [], Display: "none", Code: 'sal_hdB_TK' },
          ], Code: ''
        },
        {
          Name: "Đơn hàng", Path: "", ObjectType: "sales", LevelMenu: 2, Active: false, nameIcon: "fa-file-archive-o", IsDefault: false, CodeParent: "sal_dh", Display: "none",
          LstChildren: [
            { Name: "Dashboard", Path: "/sales/dashboard", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "dashboard", IsDefault: true, CodeParent: "sal_dh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tạo đơn hàng", Path: "/order/create", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "note_add", IsDefault: false, CodeParent: "sal_dh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm đơn hàng", Path: "/order/list", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: false, CodeParent: "sal_dh", LstChildren: [], Display: "none", Code: 'Sale_Order_TK' },
            { Name: "Tạo đơn hàng dịch vụ", Path: "/order/order-service-create", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "note_add", IsDefault: false, CodeParent: "sal_dh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Thanh toán đơn hàng", Path: "/order/pay-order-service", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "note_add", IsDefault: false, CodeParent: "sal_dh", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
        {
          Name: "Hóa đơn", Path: "", ObjectType: "sales", LevelMenu: 2, Active: false, nameIcon: "fa-file-o", IsDefault: false, CodeParent: "sal_hd", Display: "none",
          LstChildren: [
            { Name: "Tạo hóa đơn", Path: "/bill-sale/create", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "fa-file-o", IsDefault: false, CodeParent: "sal_hd", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm hóa đơn", Path: "/bill-sale/list", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "fa-file-o", IsDefault: true, CodeParent: "sal_hd", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
        {
          Name: "Báo cáo", Path: "", ObjectType: "sales", LevelMenu: 2, Active: false, nameIcon: "fa-pencil-square-o", IsDefault: false, CodeParent: "sal_bc", Display: "none",
          LstChildren: [
            { Name: "Báo cáo doanh số theo nhân viên", Path: "/sales/top-revenue", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "description", IsDefault: true, CodeParent: "sal_bc", LstChildren: [], Display: "none", Code: '' },
            { Name: "Báo cáo lợi nhuận theo sản phẩm", Path: "/sales/product-revenue", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "description", IsDefault: false, CodeParent: "sal_bc", LstChildren: [], Display: "none", Code: '' },
            { Name: "Báo cáo lợi nhuận theo khách hàng", Path: "/order/list-profit-according-customers", ObjectType: "sale", LevelMenu: 3, Active: false, nameIcon: "description", IsDefault: false, CodeParent: "sal_bc", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
      ], Code: ''
    },
    // Module Mua hàng
    {
      Name: "Quản lý mua hàng", Path: "", ObjectType: "buy", LevelMenu: 1, Active: false, nameIcon: "fa-shopping-cart", IsDefault: false, CodeParent: "Shopping_Module", Display: "none",
      LstChildren: [
        {
          Name: "Bảng giá nhà cung cấp", Path: "", ObjectType: "shopping", LevelMenu: 2, Active: false, nameIcon: "fa-book", IsDefault: false, CodeParent: "buy_bgncc", Display: "none",
          LstChildren: [
            { Name: "Tìm kiếm", Path: "/vendor/list-vendor-price", ObjectType: "shop", LevelMenu: 3, Active: false, nameIcon: "category", IsDefault: true, CodeParent: "buy_bgncc", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
        {
          Name: "Nhà cung cấp", Path: "", ObjectType: "shopping", LevelMenu: 2, Active: false, nameIcon: "fa-binoculars", IsDefault: false, CodeParent: "buy_ncc", Display: "none",
          LstChildren: [
            { Name: "Tạo mới nhà cung cấp", Path: "/vendor/create", ObjectType: "shop", LevelMenu: 3, Active: false, nameIcon: "add_to_photos", IsDefault: false, CodeParent: "buy_ncc", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm nhà cung cấp", Path: "/vendor/list", ObjectType: "shop", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: true, CodeParent: "buy_ncc", LstChildren: [], Display: "none", Code: 'Shop_Vendor_TK' },
          ], Code: ''
        },
        {
          Name: "Đề xuất mua hàng", Path: "", ObjectType: "shopping", LevelMenu: 2, Active: false, nameIcon: "fa-file-archive-o", IsDefault: false, CodeParent: "buy_dxmh", Display: "none",
          LstChildren: [
            { Name: "Tạo đề xuất mua hàng", Path: "/procurement-request/create", ObjectType: "shop", LevelMenu: 3, Active: false, nameIcon: "add_to_photos", IsDefault: false, CodeParent: "buy_dxmh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm đề xuất mua hàng", Path: "/procurement-request/list", ObjectType: "shop", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: true, CodeParent: "buy_dxmh", LstChildren: [], Display: "none", Code: 'Shop_procurement-request_TK' },
          ], Code: ''
        },
        {
          Name: "Giấy đề nghị báo giá NCC", Path: "", ObjectType: "vendor-quote", LevelMenu: 2, Active: false, nameIcon: "fa-book", IsDefault: false, CodeParent: "buy_dnbg", Display: "none",
          LstChildren: [
            { Name: "Tạo đề nghị báo giá NCC", Path: "/vendor/vendor-quote-create", ObjectType: "shop", LevelMenu: 3, Active: false, nameIcon: "add_to_photos", IsDefault: false, CodeParent: "buy_dnbg", LstChildren: [], Display: "none", Code: '' },
            { Name: "Danh sách giấy đề nghị báo giá NCC", Path: "/vendor/list-vendor-quote", ObjectType: "shop", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: true, CodeParent: "buy_dnbg", LstChildren: [], Display: "none", Code: 'Shop_vendor_quote_request_TK' },
          ], Code: ''
        },
        {
          Name: "Đơn hàng mua", Path: "", ObjectType: "shopping", LevelMenu: 2, Active: false, nameIcon: "fa-pencil-square-o", IsDefault: false, CodeParent: "buy_dhm", Display: "none",
          LstChildren: [
            { Name: "Tạo đơn hàng mua", Path: "/vendor/create-order", ObjectType: "shop", LevelMenu: 3, Active: false, nameIcon: "add_to_photos", IsDefault: false, CodeParent: "buy_dhm", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm đơn hàng mua", Path: "/vendor/list-order", ObjectType: "shop", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: true, CodeParent: "buy_dhm", LstChildren: [], Display: "none", Code: 'Shop_VendorOrder_TK' },
          ], Code: ''
        },
        {
          Name: "Báo cáo", Path: "", ObjectType: "shopping", LevelMenu: 2, Active: false, nameIcon: "fa-pencil-square-o", IsDefault: false, CodeParent: "buy_bc", Display: "none",
          LstChildren: [
            { Name: "Báo cáo tình trạng đề xuất mua hàng", Path: "/procurement-request/list-report", ObjectType: "shop", LevelMenu: 3, Active: false, nameIcon: "add_to_photos", IsDefault: false, CodeParent: "buy_bc", LstChildren: [], Display: "none", Code: '' },
            { Name: "Báo cáo tình trạng đơn hàng mua", Path: "/vendor/vendor-order-report", ObjectType: "shop", LevelMenu: 3, Active: false, nameIcon: "add_to_photos", IsDefault: false, CodeParent: "buy_bc", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
      ], Code: ''
    },
    // Module Quản lý kho
    {
      Name: "Quản lý kho", Path: "", ObjectType: "war", LevelMenu: 1, Active: false, nameIcon: "fa-cubes", IsDefault: false, CodeParent: "Warehouse_Module", Display: "none",
      LstChildren: [
        {
          Name: "Danh sách kho", Path: "", ObjectType: "warehouse", LevelMenu: 2, Active: false, nameIcon: "fa-binoculars", IsDefault: false, CodeParent: "war_kho", Display: "none",
          LstChildren: [
            { Name: "Danh sách kho", Path: "/warehouse/list", ObjectType: "WH", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: true, CodeParent: "war_kho", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
        {
          Name: "Nhập kho", Path: "", ObjectType: "warehouse", LevelMenu: 2, Active: false, nameIcon: "fa-file-archive-o", IsDefault: false, CodeParent: "war_nk", Display: "none",
          LstChildren: [
            { Name: "Tạo mới phiếu nhập kho", Path: "/warehouse/inventory-receiving-voucher/create", ObjectType: "WH", LevelMenu: 3, Active: false, nameIcon: "person_add", IsDefault: false, CodeParent: "war_nk", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm phiếu nhập kho", Path: "/warehouse/inventory-receiving-voucher/list", ObjectType: "WH", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: true, CodeParent: "war_nk", LstChildren: [], Display: "none", Code: 'WH_QLNTK' },
          ], Code: ''
        },
        {
          Name: "Xuất kho", Path: "", ObjectType: "warehouse", LevelMenu: 2, Active: false, nameIcon: "fa-quote-right", IsDefault: false, CodeParent: "war_xk", Display: "none",
          LstChildren: [
            { Name: "Tạo mới phiếu xuất kho", Path: "/warehouse/inventory-delivery-voucher/create-update", ObjectType: "WH", LevelMenu: 3, Active: false, nameIcon: "person_add", IsDefault: false, CodeParent: "war_xk", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm phiếu xuất kho", Path: "/warehouse/inventory-delivery-voucher/list", ObjectType: "WH", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: true, CodeParent: "war_xk", LstChildren: [], Display: "none", Code: 'WH_QLNXK' },
          ], Code: ''
        },
        {
          Name: "Hàng tồn kho", Path: "", ObjectType: "warehouse", LevelMenu: 2, Active: false, nameIcon: "fa-pencil-square-o", IsDefault: false, CodeParent: "war_htk", Display: "none",
          LstChildren: [
            { Name: "Danh sách hàng tồn kho", Path: "/warehouse/in-stock-report/list", ObjectType: "WH", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: true, CodeParent: "war_htk", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
      ], Code: ''
    },
    // Module Kế toán - tài chính
    {
      Name: "kế toán - tài chính", Path: "", ObjectType: "acc", LevelMenu: 1, Active: false, nameIcon: "fa-money", IsDefault: false, CodeParent: "Accounting_Module", Display: "none",
      LstChildren: [
        {
          Name: "Danh mục phí", Path: "", ObjectType: "accounting", LevelMenu: 2, Active: false, nameIcon: "fa-book", IsDefault: false, CodeParent: "acc_dmp", Display: "none",
          LstChildren: [
            { Name: "Danh sách", Path: "/accounting/cost-create", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "add_to_photos", IsDefault: false, CodeParent: "acc_dmp", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
        {
          Name: "Tiền mặt", Path: "", ObjectType: "accounting", LevelMenu: 2, Active: false, nameIcon: "fa-binoculars", IsDefault: false, CodeParent: "acc_tm", Display: "none",
          LstChildren: [
            { Name: "Tạo phiếu thu", Path: "/accounting/cash-receipts-create", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "add_to_photos", IsDefault: false, CodeParent: "acc_tm", LstChildren: [], Display: "none", Code: '' },
            { Name: "Danh sách phiếu thu", Path: "/accounting/cash-receipts-list", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "format_list_bulleted", IsDefault: true, CodeParent: "acc_tm", LstChildren: [], Display: "none", Code: 'Accounting_cash-receipts_TK' },
            { Name: "Tạo phiếu chi", Path: "/accounting/cash-payments-create", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "add_to_photos", IsDefault: false, CodeParent: "acc_tm", LstChildren: [], Display: "none", Code: '' },
            { Name: "Danh sách phiếu chi", Path: "/accounting/cash-payments-list", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "format_list_bulleted", IsDefault: false, CodeParent: "acc_tm", LstChildren: [], Display: "none", Code: 'Accounting_cash-payment_TK' },
            { Name: "Sổ quỹ tiền mặt", Path: "/accounting/cash-book", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "import_contacts", IsDefault: false, CodeParent: "acc_tm", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
        {
          Name: "Ngân hàng", Path: "", ObjectType: "accounting", LevelMenu: 2, Active: false, nameIcon: "fa-file-archive-o", IsDefault: false, CodeParent: "acc_nh", Display: "none",
          LstChildren: [
            { Name: "Tạo báo có", Path: "/accounting/bank-receipts-create", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "add_to_photos", IsDefault: false, CodeParent: "acc_nh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Danh sách báo có", Path: "/accounting/bank-receipts-list", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "list", IsDefault: true, CodeParent: "acc_nh", LstChildren: [], Display: "none", Code: 'accounting_bank-receipts_TK' },
            { Name: "Tạo UNC", Path: "/accounting/bank-payments-create", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "add_to_photos", IsDefault: false, CodeParent: "acc_nh", LstChildren: [], Display: "none", Code: '' },
            { Name: "Danh sách UNC", Path: "/accounting/bank-payments-list", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "list", IsDefault: false, CodeParent: "acc_nh", LstChildren: [], Display: "none", Code: 'Accounting_bank-payments-TK' },
            { Name: "Sổ quỹ ngân hàng", Path: "/accounting/bank-book", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "import_contacts", IsDefault: false, CodeParent: "acc_nh", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
        {
          Name: "Công nợ", Path: "", ObjectType: "accounting", LevelMenu: 2, Active: false, nameIcon: "fa-quote-right", IsDefault: false, CodeParent: "acc_cn", Display: "none",
          LstChildren: [
            { Name: "Nhà cung cấp", Path: "/accounting/receivable-vendor-report", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "monetization_on", IsDefault: true, CodeParent: "acc_cn", LstChildren: [], Display: "none", Code: 'Accounting_Vendor_Report' },
            { Name: "Khách hàng", Path: "/accounting/receivable-customer-report", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "monetization_on", IsDefault: false, CodeParent: "acc_cn", LstChildren: [], Display: "none", Code: 'Accounting_Customer_Report' },
          ], Code: ''
        },
        {
          Name: "Báo cáo kết quả kinh doanh", Path: "", ObjectType: "accounting", LevelMenu: 2, Active: false, nameIcon: "fa-pencil-square-o", IsDefault: false, CodeParent: "acc_kqkd", Display: "none",
          LstChildren: [
            { Name: "Báo cáo kết quả kinh doanh", Path: "/accounting/sales-report", ObjectType: "accouting", LevelMenu: 3, Active: false, nameIcon: "description", IsDefault: true, CodeParent: "acc_kqkd", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
      ], Code: ''
    },
    // // Module quản lý sản xuất
    // { Name: "Quản lý sản xuất", Path: "", ObjectType: "manufacturing", LevelMenu: 1, Active: false, nameIcon: "u119.png", IsDefault: false, CodeParent: "Manufacturing_Module",
    // LstChildren: [
    // ], Code: '' },
    {
      Name: "Nhân sự", Path: "", ObjectType: "hrm", LevelMenu: 1, Active: false, nameIcon: "fa-users", IsDefault: false, CodeParent: "Employee_Module", Display: "none",
      LstChildren: [
        {
          Name: "Nhân viên", Path: "", ObjectType: "employee", LevelMenu: 2, Active: false, nameIcon: "fa-user", IsDefault: false, CodeParent: "hrm_nv", Display: "none", LstChildren: [
            { Name: "Dashboard", Path: "/employee/dashboard", ObjectType: "HRM", LevelMenu: 3, Active: false, nameIcon: "dashboard", IsDefault: true, CodeParent: "hrm_nv", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tạo nhân viên", Path: "/employee/create", ObjectType: "HRM", LevelMenu: 3, Active: false, nameIcon: "person_add", IsDefault: false, CodeParent: "hrm_nv", LstChildren: [], Display: "none", Code: '' },
            { Name: "Tìm kiếm nhân viên", Path: "/employee/list", ObjectType: "HRM", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: false, CodeParent: "hrm_nv", LstChildren: [], Display: "none", Code: 'HRM_EmployeeTK' },
            { Name: "Đề xuất xin nghỉ", Path: "/employee/request/list", ObjectType: "HRM", LevelMenu: 3, Active: false, nameIcon: "format_list_bulleted", IsDefault: false, CodeParent: "hrm_nv", LstChildren: [], Display: "none", Code: 'HRM_Request_TK' },
            { Name: "Danh sách nhân viên đã nghỉ việc", Path: "/employee/employee-quit-work", ObjectType: "HRM", LevelMenu: 3, Active: false, nameIcon: "search", IsDefault: false, CodeParent: "hrm_nv", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
        {
          Name: "Tiền lương", Path: "", ObjectType: "employee", LevelMenu: 2, Active: false, nameIcon: "fa-pencil-square-o", IsDefault: false, CodeParent: "hrm_tl", Display: "none", LstChildren: [
            { Name: "Bảng lương nhân viên", Path: "/employee/employee-salary/list", ObjectType: "HRM", LevelMenu: 3, Active: false, nameIcon: "format_list_bulleted", IsDefault: true, CodeParent: "hrm_tl", LstChildren: [], Display: "none", Code: '' },
          ], Code: ''
        },
      ], Code: ''
    },
  ];

  items: MenuItem[] = [
    {
      label: 'CRM',
      items: [
        { label: 'Tạo khách hàng tiềm năng', routerLink: '/customer/potential-customer-create' },
        { label: 'Tạo cơ hội', routerLink: '/lead/create-lead' },
        { label: 'Tạo báo giá', routerLink: '/customer/quote-create' },
        { label: 'Tạo Khách hàng', routerLink: '/customer/create' },
      ]
    },
    {
      label: 'Bán hàng',
      items: [
        { label: 'Tạo sản phẩm', routerLink: '/product/create' },
        { label: 'Tạo hợp đồng bán', routerLink: '/sales/contract-create' },
        { label: 'Tạo đơn hàng', routerLink: '/order/create' },
        { label: 'Tạo hóa đơn', routerLink: '/bill-sale/create' },
      ]
    },
    {
      label: 'Mua hàng',
      items: [
        { label: 'Tạo nhà cung cấp', routerLink: '/vendor/create' },
        { label: 'Tạo đề xuất mua hàng', routerLink: '/procurement-request/create' },
      ]
    },
    {
      label: 'Tài chính',
      items: [
        { label: 'Tạo phiếu thu', routerLink: '/accounting/cash-receipts-create' },
        { label: 'Tạo phiếu chi', routerLink: '/accounting/cash-payments-create' },
        { label: 'Tạo báo có', routerLink: '/accounting/bank-receipts-create' },
        { label: 'Tạo UNC', routerLink: '/accounting/bank-payments-create' },
      ]
    },
    {
      label: 'Nhân sự',
      items: [
        { label: 'Nhân viên', routerLink: '/employee/create' },
      ]
    },
    {
      label: 'Kho',
      items: [
        { label: 'Tạo phiếu nhập kho', routerLink: '/warehouse/inventory-receiving-voucher/create' },
        { label: 'Tạo phiếu xuất kho', routerLink: '/warehouse/inventory-delivery-voucher/create-update' }
      ]
    }
  ];

  listEmployee: Array<any> = [];
  selectedEmployee: Array<any> = [];
  totalEvents: Array<Calendar> = [];

  constructor(private route: ActivatedRoute,
    private router: Router,
    private eventEmitterService: EventEmitterService,
    private getPermission: GetPermission,
    private notiService: NotificationService,
    private dashboardHomeService: DashboardHomeService,
    private authenticationService: AuthenticationService,
    public dialog: MatDialog,
    private messageService: MessageService,
    private renderer: Renderer2,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private datePipe: DatePipe,
  ) {
    $("body").addClass('sidebar-collapse');
    this.renderer.listen('window', 'click', (e: Event) => {
      if (this.toggleNotifi) {
        //ẩn hiện khi click Thông báo
        if (this.toggleNotifi.nativeElement.contains(e.target)) {
          this.isOpenNotifi = !this.isOpenNotifi;
        } else {
          this.isOpenNotifi = false;
        }

        //ẩn hiện khi click Tạo mới
        if (this.toggleCreateElement.nativeElement.contains(e.target)) {
          this.isOpenCreateElement = !this.isOpenCreateElement;
        } else {
          this.isOpenCreateElement = false;
        }

        //ẩn hiện khi click Config
        if (this.toggleConfig.nativeElement.contains(e.target)) {
          this.isOpenConfig = !this.isOpenConfig;
        } else {
          this.isOpenConfig = false;
        }

        //ẩn hiện khi click User
        if (this.toggleUser.nativeElement.contains(e.target)) {
          this.isOpenUser = !this.isOpenUser;
        } else {
          this.isOpenUser = false;
        }
      }

      if (this.dropdownMenus) {
        // ẩn hiện khi click menu items Tạo mới
        if (this.dropdownMenus.nativeElement.contains(e.target)) {
          this.isOpenCreateElement = !this.isOpenCreateElement;
        } else {
          this.isOpenCreateElement = false;
        }
      }
    });
  }

  ngOnInit() {
    this.getPemistion();
    this.getListModuleAndResource();
    this.getNotification();
    this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
    this.username = localStorage.getItem("Username");
    this.userAvatar = localStorage.getItem("UserAvatar");
    this.userFullName = localStorage.getItem("UserFullName");
    this.userEmail = localStorage.getItem("UserEmail");

    //var leftMenu = localStorage.getItem('lstBreadCrumLeftMenu');

    var leftMenu = JSON.stringify(this.lstSubmenuLevel1);

    this.lstBreadCrumLeftMenu = [];
    this.lstBreadCrumLeftMenu = JSON.parse(leftMenu);

    var X = localStorage.getItem('menuMapPath');
    this.lstBreadCrum = JSON.parse(X);

    //kiem tra xem co toggle ko
    if ($("body").hasClass("sidebar-collapse")) {
      this.isToggleCick = true;
    }
    else {
      this.isToggleCick = false;
    }

    if (localStorage.getItem('IsAdmin') == 'true') {
      this.userAdmin = true;
      this.isAdmin = true;
      this.permissionAdmin = true;
      this.moduled = 'admin';
      localStorage.setItem('menuIndex', 'admin');
    }
    else if (localStorage.getItem('IsAdmin') == 'false') {
      this.userAdmin = false;
      this.isAdmin = false;
      this.permissionAdmin = false;
    }

    if (this.eventEmitterService.subsVar == undefined) {
      this.eventEmitterService.subsVar = this.eventEmitterService.
        invokeFirstComponentFunction.subscribe((name: string) => {
          this.updateLeftMenu();
        });
    }

    //Call Update IsToggle với eventEmitterService
    if (this.eventEmitterService.subsVar2 == undefined) {
      this.eventEmitterService.subsVar2 = this.eventEmitterService.
        invokeUpdateIsToggleFunction.subscribe((name: string) => {
          this.updateLeftIsToggle();
        });
    }

    this.getMasterData();

    //setting full calendar
    this.options = {
      plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
      defaultDate: new Date(),
      header: {
        left: 'prev,next',
        center: 'title',
        right: 'dayGridMonth,timeGridWeek,timeGridDay'
      },
      editable: true,
      buttonText: {
        dayGridMonth: 'Tháng',
        timeGridWeek: 'Tuần',
        timeGridDay: 'Ngày',
      },
      eventClick: this.handleEventClick.bind(this),
      eventDrop: (event) => {
        this.changeEventFullCalendar(event);
      },
      eventResize: (event) => {
        this.changeEventFullCalendar(event);
      },
      selectOverlap: true,
    }
  }

  getMasterData() {
    this.setTable();
    this.loading = true;
    this.dashboardHomeService.getDataDashboardHome().subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.totalSalesOfWeek = result.totalSalesOfWeek;
        this.totalSalesOfMonth = result.totalSalesOfMonth;
        this.totalSalesOfQuarter = result.totalSalesOfQuarter;
        this.totalSalesOfWeekPress = result.totalSalesOfWeekPress;
        this.totalSalesOfMonthPress = result.totalSalesOfMonthPress;
        this.totalSalesOfQuarterPress = result.totalSalesOfQuarterPress;
        this.listQuote = result.listQuote;
        this.listCustomer = result.listCustomer;
        this.listOrderNew = result.listOrderNew;
        this.listParticipants = result.listParticipants;
        this.salesOfWeek = this.totalSalesOfWeek - this.totalSalesOfWeekPress;
        this.salesOfMonth = this.totalSalesOfMonth - this.totalSalesOfMonthPress;
        this.salesOfQuarter = this.totalSalesOfQuarter - this.totalSalesOfQuarterPress;
        this.listEmployee = result.listEmployee;

        this.listCustomerMeeting = result.listCustomerMeeting;
        this.listCustomerMeeting.forEach(item => {
          if (item.startDate) item.startDate = new Date(item.startDate);
        });

        this.listLeadMeeting = result.listLeadMeeting;
        this.listLeadMeeting.forEach(item => {
          if (item.startDate) item.startDate = new Date(item.startDate);

        });

        this.listCusBirthdayOfWeek = result.listCusBirthdayOfWeek;
        this.listCusBirthdayOfWeek.forEach(item => {
          if (item.dateOfBirth) item.dateOfBirth = new Date(item.dateOfBirth);
        });

        this.listEmployeeBirthDayOfWeek = result.listEmployeeBirthDayOfWeek;
        this.listEmployeeBirthDayOfWeek.forEach(item => {
          if (item.dateOfBirth) item.dateOfBirth = new Date(item.dateOfBirth);
        });

        this.setCalendar();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  changeEventFullCalendar(els: any) {
    if (els) {
      let startDate = null;
      if (els.event.start) {
        startDate = convertToUTCTime(els.event.start)
      }
      let endDate = null;
      if (els.event.end) {
        endDate = convertToUTCTime(els.event.end);
      }
      let message: string = '';
      if (els.event.end) {
        message = "Bạn có muốn chỉnh sửa thời gian lịch hẹn bắt đầu từ : " + this.datePipe.transform(els.event.start, "h:mm dd/MM/yyyy ") + " đến " + this.datePipe.transform(els.event.end, "h:mm dd/MM/yyyy");
      } else {
        message = "Bạn có muốn chỉnh sửa thời gian lịch hẹn bắt đầu từ : " + this.datePipe.transform(els.event.start, "h:mm dd/MM/yyyy ");
      }
      this.confirmationService.confirm({
        message: message,
        accept: () => {
          this.loading = true;
          this.dashboardHomeService.updateCustomerMeeting(els.event.id, startDate, endDate, this.auth.UserId).subscribe(response => {
            let result: any = response;
            this.loading = false;
            if (result.statusCode == 200) {
              this.getMasterData();
              let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật lịch hẹn thành công' };
              this.showMessage(msg);
            } else {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          });
        },
        reject: () =>{
          this.getMasterData();
        }
      });

    }
  }

  // changeTimeEventFullCalendar(els: any) {
  //   if (els) {
  //     let startDate = null;
  //     if (els.event.start) {
  //       startDate = convertToUTCTime(els.event.start)
  //     }
  //     let endDate = null;
  //     if (els.event.end) {
  //       endDate = convertToUTCTime(els.event.end);
  //     }
  //     let message: string = '';
  //     if (els.event.end) {
  //       message = "Bạn có muốn chỉnh sửa thời gian lịch hẹn bắt đầu từ : " + this.datePipe.transform(els.event.start, "h:mm dd/MM/yyyy ") + " đến " + this.datePipe.transform(els.event.end, "h:mm dd/MM/yyyy");
  //     } else {
  //       message = "Bạn có muốn chỉnh sửa thời gian lịch hẹn bắt đầu từ : " + this.datePipe.transform(els.event.start, "h:mm dd/MM/yyyy ");
  //     }
  //     this.confirmationService.confirm({
  //       message: message,
  //       accept: () => {
  //         this.loading = true;
  //         this.dashboardHomeService.updateCustomerMeeting(els.event.id, startDate, endDate).subscribe(response => {
  //           let result: any = response;
  //           this.loading = false;
  //           if (result.statusCode == 200) {
  //             this.getMasterData();
  //             let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật lịch hẹn thành công' };
  //             this.showMessage(msg);
  //           } else {
  //             let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
  //             this.showMessage(msg);
  //           }
  //         });
  //       }
  //     });
  //   }
  // }

  handleEventClick(eventClick) {
    if (eventClick) {
      let id = eventClick.event.id;
      let ref = this.dialogService.open(MeetingDialogComponent, {
        data: {
          // customerId: this.customerId,
          customerMeetingId: id,
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
            this.getMasterData();
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật lịch hẹn thành công' };
            this.showMessage(msg);
          }
        }
      });
    }
  }

  setCalendar() {
    console.log(this.listCustomerMeeting)
    this.events = [];
    if (this.listCustomerMeeting) {
      this.listCustomerMeeting.forEach(item => {
        let meeting = new Calendar();
        meeting.id = item.customerMeetingId;
        meeting.title = item.customerName;
        meeting.start = item.startDate;
        meeting.end = item.endDate;
        meeting.participants = item.participants;

        if (meeting.start < new Date()) {
          meeting.backgroundColor = "#DD0000";
        }
        this.events = [...this.events, meeting];
      });
    }
    // if (this.listLeadMeeting) {
    //   this.listLeadMeeting.forEach(item => {
    //     let meeting = new Calendar();
    //     meeting.id = item.leadMeetingId;
    //     meeting.title = item.leadName;
    //     meeting.start = item.startDate;
    //     meeting.end = item.endDate;
    //     meeting.employeeId = item.employeeId;

    //     if (meeting.start < new Date()) {
    //       meeting.backgroundColor = "#DD0000";
    //     }
    //     this.events = [...this.events, meeting];
    //   });
    // }

    this.totalEvents = this.events;
  }


  // getPemistionMenu2() {
  //   this.lstSubmenuLevel3Create.forEach(element => {
  //     let resource = element.ObjectType + element.Path;
  //     let permission: any = this.getPermission.getPermission(this.listPermissionResourceActive, resource);
  //     if (permission.status == false) {
  //       element.Active = true;
  //     }
  //     else {
  //       element.Active = false;
  //     }
  //   });
  // }

  getPemistion() {
    this.lstSubmenuLevel1.forEach(item => {
      item.LstChildren.forEach(element => {
        element.LstChildren.forEach(role => {
          let resource = item.ObjectType + role.Path;
          let permission: any = this.getPermission.getPermission(this.listPermissionResourceActive, resource);
          if (permission.status == false) {
            role.Active = true;
          }
          else {
            role.Active = false;
          }
        });
        let countElement = element.LstChildren.filter(f => f.Active == true);
        if (countElement.length == element.LstChildren.length) {
          element.Active = true;
        }
        else element.Active = false;
      });
      let countItem = item.LstChildren.filter(f => f.Active == true);
      if (countItem.length == item.LstChildren.length) {
        item.Active = true;
      }
      else item.Active = false;
    });
  }
  setTable() {
    this.colsQuote = [
      { field: 'quoteCode', header: 'Mã báo giá', textAlign: 'left' },
      { field: 'amount', header: 'Trị giá báo giá', textAlign: 'right' },
      { field: 'customerName', header: 'Khách hàng', textAlign: 'left' },
    ];

    this.colsCustomer = [
      { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left' },
      { field: 'customerPhone', header: 'Số điện thoại', textAlign: 'right' },
      { field: 'customerEmail', header: 'Email', textAlign: 'left' },
    ];

    this.colsOrder = [
      { field: 'orderCode', header: 'Mã đơn hàng', textAlign: 'left' },
      { field: 'amount', header: 'Trị giá đơn hàng', textAlign: 'right' },
      { field: 'customerName', header: 'Khách hàng', textAlign: 'left' },
    ];

    this.colsCustomerMeeting = [
      { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left' },
      { field: 'title', header: 'Tiêu đề', textAlign: 'left' },
      { field: 'createByName', header: 'Người tạo lịch hẹn', textAlign: 'left' },
      { field: 'startDate', header: 'Thời gian', textAlign: 'left' },
      { field: 'locationMeeting', header: 'Địa điểm', textAlign: 'left' },
      { field: 'content', header: 'Nội dung', textAlign: 'left' },
    ];
    this.selectedColsCustomerMeeting = this.colsCustomerMeeting.filter(e => e.field == "customerName" || e.field == "title"
      || e.field == "startDate" || e.field == "locationMeeting" || e.field == "content");

    this.colsLeadMeeting = [
      { field: 'leadName', header: 'Tên cơ hội', textAlign: 'left' },
      { field: 'title', header: 'Tiêu đề', textAlign: 'left' },
      { field: 'createByName', header: 'Người tạo lịch hẹn', textAlign: 'left' },
      { field: 'startDate', header: 'Thời gian', textAlign: 'left' },
      { field: 'startDate', header: 'Địa điểm', textAlign: 'left' },
      { field: 'content', header: 'Nội dung', textAlign: 'left' },
    ];
    this.selectedColsLeadMeeting = this.colsLeadMeeting.filter(e => e.field == "leadName" || e.field == "title"
      || e.field == "startDate" || e.field == "startDate" || e.field == "content");

    this.colsCusBirthdayOfWeek = [
      { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left' },
      { field: 'customerPhone', header: 'Số điện thoại', textAlign: 'right' },
      { field: 'customerEmail', header: 'Email', textAlign: 'left' },
      { field: 'dateOfBirth', header: 'Sinh nhật', textAlign: 'left' },
    ];

    this.colsEmployeeBirthDayOfWeek = [
      { field: 'employeeName', header: 'Tên khách hàng', textAlign: 'left' },
      { field: 'organizationName', header: 'Phòng ban', textAlign: 'right' },
      { field: 'positionName', header: 'Chức vụ', textAlign: 'left' },
      { field: 'dateOfBirth', header: 'Sinh nhật', textAlign: 'left' },
    ];
  }

  setStepSize(data) {
    //Chia nhiều nhất là 10 mốc dữ liệu
    return this.formatRoundNumber(data[0] / 10);
  }

  //Làm tròn số
  formatRoundNumber(number) {
    number = number.toString();
    let stt = number.length;
    let first_number = number.slice(0, 1);
    let result: number;
    switch (first_number) {
      case '1':
        result = this.addZero(2, stt);
        break;
      case '2':
        result = this.addZero(3, stt);
        break;
      case '3':
        result = this.addZero(4, stt);
        break;
      case '4':
        result = this.addZero(5, stt);
        break;
      case '5':
        result = this.addZero(6, stt);
        break;
      case '6':
        result = this.addZero(7, stt);
        break;
      case '7':
        result = this.addZero(8, stt);
        break;
      case '9':
        result = this.addZero(9, stt);
        break;
      default:
        break;
    }
    return result;
  }

  //Thêm số chữ số 0 vào sau một ký tự
  addZero(tmp: number, stt: number) {
    if (tmp == 9) {
      stt = stt + 1;
      tmp = 1;
    }
    let num = tmp.toString();
    for (let i = 0; i < stt - 1; i++) {
      num += "0";
    }
    return Number(num);
  }

  //Lấy ra list module của user
  getListModuleAndResource() {
    if (this.listPermissionResource.length > 0) {
      this.listPermissionResource.forEach(item => {
        let moduleName = item.slice(0, item.indexOf('/'));
        if (this.listModule.indexOf(moduleName) == -1) {
          this.listModule.push(moduleName);
        }

        let resourceName = item.slice(item.indexOf('/') + 1, item.lastIndexOf('/'));
        if (this.listResource.indexOf(resourceName) == -1) {
          this.listResource.push(resourceName);
        }
      });
    }
  }

  //Kiểm tra user được quyền thấy các module nào trên trang home:
  checkModule(moduleName) {
    let result = false;
    if (this.listModule.indexOf(moduleName) !== -1) {
      result = true;
    }
    return result;
  }

  //Kiểm tra user có được quyền nhìn thấy các resource nào trên menu:
  checkUserResource(resourceName) {
    let result = false;
    if (this.listResource.indexOf(resourceName) !== -1) {
      result = true;
    }
    return result;
  }

  // checkUserResourceModule(resourceName: string[]) {
  //   let result = false;
  //   for (var i = 0; i < resourceName.length; i++) {
  //     if (this.listResource.indexOf(resourceName[i]) !== -1) {
  //       result = true;
  //       return result;
  //     }
  //   }
  //   return result;
  // }

  updateLeftMenu() {
    var leftMenu = localStorage.getItem('lstBreadCrumLeftMenu');
    this.lstBreadCrumLeftMenu = [];
    this.lstBreadCrumLeftMenu = JSON.parse(leftMenu);
  }

  updateLeftIsToggle() {
    this.isToggleCick = JSON.parse(localStorage.getItem('isToggleCick'));
  }

  openMenuLevel4(resource, resourceParent: BreadCrumMenuModel) {
    //kiem tra co menu level 3 chua
    // let checkexistMenulevel3 = this.lstBreadCrum.findIndex(e => e.LevelMenu == 3);
    // let getMenuLevel1 = this.lstBreadCrum.filter(e => e.LevelMenu == 1);
    // this.lstBreadCrum = [];
    // this.lstBreadCrum.push.apply(this.lstBreadCrum, getMenuLevel1);

    // this.lstBreadCrum.push(resourceParent);


    // var breadCrumMenuitem = new BreadCrumMenuModel();
    // breadCrumMenuitem.Name = resource.Name;
    // breadCrumMenuitem.LevelMenu = 3;
    // breadCrumMenuitem.ObjectType = resource.ObjectType;
    // breadCrumMenuitem.Path = "";
    // breadCrumMenuitem.Active = true;
    // breadCrumMenuitem.LstChildren = [];
    // this.lstBreadCrum.push(breadCrumMenuitem);

    // localStorage.setItem('menuMapPath', JSON.stringify(this.lstBreadCrum));
    // this.eventEmitterService.updateMenuMapPath();
    this.router.navigate([resource.Path]);
  }

  getNotification() {
    this.notiService.getNotification(this.auth.EmployeeId, this.auth.UserId).subscribe(response => {
      var result = <any>response;
      this.notificationNumber = result.numberOfUncheckedNoti;
      this.notificationList = result.shortNotificationList;
    });
  }

  goToNotiUrl(item: any, notificationId: string, id: string, code: string) {
    this.notiService.removeNotification(notificationId, this.auth.UserId).subscribe(response => {
      this.loading = true;
      if (code == "PRO_REQ") {
        this.router.navigate(['/procurement-request/view', { id: id }]);
      }
      if (code == "PAY_REQ") {
        this.router.navigate(['/accounting/payment-request-detail', { requestId: id }]);
      }
      if (code == "EMP_REQ") {
        this.router.navigate(['/employee/request/detail', { requestId: id }]);
      }
      if (code == "EMP_SLR") {
        this.NotificationContent = item.content;
        let month = this.NotificationContent.split(" ")[this.NotificationContent.split(" ").indexOf("tháng") + 1];
        this.router.navigate(['employee/employee-salary/list', { MonthSalaryRequestParam: month }]);
      }
      if (code == "TEA_SLR") {
        this.router.navigate(['/employee/teacher-salary/list']);
      }
      if (code == "AST_SLR") {
        this.router.navigate(['/employee/assistant-salary/list']);
      }
      this.loading = false;
    });
  }

  goToNotification() {
    //this.notificationNumber = 0;
    this.router.navigate(['/notification']);
  }

  logout() {
    this.authenticationService.logout();
    this.router.navigate(['/login']);
  }

  // Mở giao diện đổi Password
  openChangePassword() {
    let account = this.username;
    let _name = this.userFullName;
    let _email = this.userEmail;
    let _avatar = this.userAvatar;
    this.dialogRef = this.dialog.open(ChangepasswordComponent,
      {
        data: { accountName: account, name: _name, email: _email, avatar: _avatar }
      });
    this.dialogRef.afterClosed().subscribe(result => {
    });
  }
  //Ket thuc

  // Mo giao dien UserProfile
  goToViewProfile() {
    this.router.navigate(['/userprofile']);
  }

  goToUrlSysConfig(Path) {
    this.router.navigate([Path]);
  }

  goListQuote() {
    this.router.navigate(['/customer/quote-list']);
  }

  onViewQuoteDetail(id: string) {
    this.router.navigate(['/customer/quote-detail', { quoteId: id }]);
  }

  goListCustomer() {
    this.router.navigate(['/customer/list']);
  }

  onViewCustomerDetail(id: string) {
    this.router.navigate(['/customer/detail', { customerId: id }]);
  }

  onViewLeadDetail(id: string) {
    
  }

  onViewObjectDetail(id: string, contactId: string, type: string) {
    if (type == 'CUSTOMER') {
      this.router.navigate(['/customer/detail', { customerId: id }]);
    } else if (type = 'LEAD') {
      this.router.navigate(['/lead/detail', { leadId: id }]);
    }
  }

  goListOrder() {
    this.router.navigate(['/order/list']);
  }

  onViewOrderDetail(id: string) {
    this.router.navigate(['/order/order-detail', { customerOrderID: id }]);
  }

  onViewEmployeeDetail(employeeId: string, contactId: string) {
    this.router.navigate(['/employee/detail', { employeeId: employeeId, contactId: contactId }]);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  isHomepage() {
    if (this.router.url === '/home') {
      return true;
    } else {
      return false;
    }
  }

  rowclick: number = -1;
  rowclickParent: number = -1;
  active: boolean = true;
  activeParent: boolean = true;
  countLengthParrent: number = 0;

  addRemoveIcon(index) {
    for (let i = 0; i < this.lstBreadCrumLeftMenu.length; i++) {
      $(".module-remove" + i).hide();
      $(".module-add" + i).show();
    }
    if (this.rowclick !== index) {
      $(".module-remove" + index).show();
      $(".module-add" + index).hide();
      this.active = true;

      for (let i = 0; i < this.countLengthParrent; i++) {
        $(".module-remove-parent" + i).hide();
        $(".module-add-parent" + i).show();
      }
      this.activeParent = true;
    }
    else {
      if (!this.active) {
        $(".module-remove" + index).show();
        $(".module-add" + index).hide();
      }
      else {
        $(".module-remove" + index).hide();
        $(".module-add" + index).show();
      }
      this.active = !this.active;
    }

    this.rowclick = index;
  }

  addRemoveIconParren(index, countLength) {
    this.countLengthParrent = countLength;
    for (let i = 0; i < countLength; i++) {
      $(".module-remove-parent" + i).hide();
      $(".module-add-parent" + i).show();
    }
    if (this.rowclickParent !== index) {
      $(".module-remove-parent" + index).show();
      $(".module-add-parent" + index).hide();
      this.activeParent = true;
    }
    else {
      if (!this.activeParent) {
        $(".module-remove-parent" + index).show();
        $(".module-add-parent" + index).hide();
      }
      else {
        $(".module-remove-parent" + index).hide();
        $(".module-add-parent" + index).show();
      }
      this.activeParent = !this.activeParent;
    }

    this.rowclickParent = index;
  }

  getDefaultApplicationName() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "ApplicationName")?.systemValueString;
  }

  searchMeetingForEmployee() {
    this.events = this.totalEvents;
    let listEmployeeId = this.selectedEmployee.map(x => x.employeeId);
    
    if (listEmployeeId.length > 0) {
      let currentEvent: Array<any> = [];
      this.events.forEach(item => {
        let listParticipants = item.participants.split(';');

        let flag = false;
        listEmployeeId.forEach(_item => {
          if (listParticipants.includes(_item)) {
            flag = true;
          }
        });

        if (flag) {
          currentEvent.push(item);
        }
      });

      this.events = [];
      this.events = [...currentEvent];
    }
    else {
      this.events = this.totalEvents;
    }
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

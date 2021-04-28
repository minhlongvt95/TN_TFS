import { Component, OnInit } from '@angular/core';
import { Chart } from 'angular-highcharts';
import { Router } from '@angular/router';
import { DashboardService } from '../../services/dashboard.service';
import { LeadService } from '../../services/lead.service';
import { TranslateService } from '@ngx-translate/core';
import { NoteService } from '../../../shared/services/note.service';
import { GetPermission } from '../../../shared/permission/get-permission';

import { MessageService, ConfirmationService, SortEvent, MenuItem } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';

class dataChart {
  name: string;
  y: number;
  color: string;
}

class Month {
  label: string;
  result: number;
}

class charModel {
  count: number;
  code: string;
  name: string;
}

class colorConfig {
  color: string;
}

const COLOR_CONFIG: Array<colorConfig> = [
  // { color: "#ffcc00" },
  // { color: "#002d9c" },
  // { color: "#4d5358" },
  // { color: "#f2f4f8" },
  // { color: "#c1c7cd" },
  // { color: "#78a9ff" },
  // { color: "#ff3b30" },
  // { color: "#34c759" },
  // { color: "#5ac8fa" },
  // { color: "#5856d6" },
  // { color: "#af52de" },
  // { color: "#ff2d55" },
]

const COLOR_CONFIG_2: Array<string> = ["#63b598", "#ce7d78", "#ea9e70", "#a48a9e", "#c6e1e8", "#648177", "#0d5ac1",
  "#f205e6", "#1c0365", "#14a9ad", "#4ca2f9", "#a4e43f", "#d298e2", "#6119d0",
  "#d2737d", "#c0a43c", "#f2510e", "#651be6", "#79806e", "#61da5e", "#cd2f00",
  "#9348af", "#01ac53", "#c5a4fb", "#996635", "#b11573", "#4bb473", "#75d89e",
  "#2f3f94", "#2f7b99", "#da967d", "#34891f", "#b0d87b", "#ca4751", "#7e50a8",
  "#c4d647", "#e0eeb8", "#11dec1", "#289812", "#566ca0", "#ffdbe1", "#2f1179",
  "#935b6d", "#916988", "#513d98", "#aead3a", "#9e6d71", "#4b5bdc", "#0cd36d",
  "#250662", "#cb5bea", "#228916", "#ac3e1b", "#df514a", "#539397", "#880977",
  "#f697c1", "#ba96ce", "#679c9d", "#c6c42c", "#5d2c52", "#48b41b", "#e1cf3b",
  "#5be4f0", "#57c4d8", "#a4d17a", "#225b8", "#be608b", "#96b00c", "#088baf",
  "#f158bf", "#e145ba", "#ee91e3", "#05d371", "#5426e0", "#4834d0", "#802234",
  "#6749e8", "#0971f0", "#8fb413", "#b2b4f0", "#c3c89d", "#c9a941", "#41d158",
  "#fb21a3", "#51aed9", "#5bb32d", "#807fb", "#21538e", "#89d534", "#d36647",
  "#7fb411", "#0023b8", "#3b8c2a", "#986b53", "#f50422", "#983f7a", "#ea24a3",
  "#79352c", "#521250", "#c79ed2", "#d6dd92", "#e33e52", "#b2be57", "#fa06ec",
  "#1bb699", "#6b2e5f", "#64820f", "#1c271", "#21538e", "#89d534", "#d36647",
  "#7fb411", "#0023b8", "#3b8c2a", "#986b53", "#f50422", "#983f7a", "#ea24a3",
  "#79352c", "#521250", "#c79ed2", "#d6dd92", "#e33e52", "#b2be57", "#fa06ec",
  "#1bb699", "#6b2e5f", "#64820f", "#1c271", "#9cb64a", "#996c48", "#9ab9b7",
  "#06e052", "#e3a481", "#0eb621", "#fc458e", "#b2db15", "#aa226d", "#792ed8",
  "#73872a", "#520d3a", "#cefcb8", "#a5b3d9", "#7d1d85", "#c4fd57", "#f1ae16",
  "#8fe22a", "#ef6e3c", "#243eeb", "#1dc18", "#dd93fd", "#3f8473", "#e7dbce",
  "#421f79", "#7a3d93", "#635f6d", "#93f2d7", "#9b5c2a", "#15b9ee", "#0f5997",
  "#409188", "#911e20", "#1350ce", "#10e5b1", "#fff4d7", "#cb2582", "#ce00be",
  "#32d5d6", "#17232", "#608572", "#c79bc2", "#00f87c", "#77772a", "#6995ba",
  "#fc6b57", "#f07815", "#8fd883", "#060e27", "#96e591", "#21d52e", "#d00043",
  "#b47162", "#1ec227", "#4f0f6f", "#1d1d58", "#947002", "#bde052", "#e08c56",
  "#28fcfd", "#bb09b", "#36486a", "#d02e29", "#1ae6db", "#3e464c", "#a84a8f",
  "#911e7e", "#3f16d9", "#0f525f", "#ac7c0a", "#b4c086", "#c9d730", "#30cc49",
  "#3d6751", "#fb4c03", "#640fc1", "#62c03e", "#d3493a", "#88aa0b", "#406df9",
  "#615af0", "#4be47", "#2a3434", "#4a543f", "#79bca0", "#a8b8d4", "#00efd4",
  "#7ad236", "#7260d8", "#1deaa7", "#06f43a", "#823c59", "#e3d94c", "#dc1c06",
  "#f53b2a", "#b46238", "#2dfff6", "#a82b89", "#1a8011", "#436a9f", "#1a806a",
  "#4cf09d", "#c188a2", "#67eb4b", "#b308d3", "#fc7e41", "#af3101", "#ff065",
  "#71b1f4", "#a2f8a5", "#e23dd0", "#d3486d", "#00f7f9", "#474893", "#3cec35",
  "#1c65cb", "#5d1d0c", "#2d7d2a", "#ff3420", "#5cdd87", "#a259a4", "#e4ac44",
  "#1bede6", "#8798a4", "#d7790f", "#b2c24f", "#de73c2", "#d70a9c", "#25b67",
  "#88e9b8", "#c2b0e2", "#86e98f", "#ae90e2", "#1a806b", "#436a9e", "#0ec0ff",
  "#f812b3", "#b17fc9", "#8d6c2f", "#d3277a", "#2ca1ae", "#9685eb", "#8a96c6",
  "#dba2e6", "#76fc1b", "#608fa4", "#20f6ba", "#07d7f6", "#dce77a", "#77ecca"]

class leadReportModel {
  leadId: string;
  leadName: string;
  contactId: string;
  phone: string;
  email: string;
  personInChangeId: string;
  personInChangeName: string;
  customerId: string;
  customerName: string;
  statusId: string;
  statusName: string;
}

class leadReportByMonthModel {
  categoryId: string;
  categoryName: string;
  month: number;
  percentValue: number;
  //client color
  color: string;
}

class leadSumaryByMonth {
  month: number;
  totalInvestFundByMonth: number;
  totalPotentialByMonth: number;
  totalInterestedByMonth: number;
}

class leadDashBoardModel {
  listInvestFundReport: Array<leadReportByMonthModel>;
  listPotentialReport: Array<leadReportByMonthModel>;
  listInterestedGroupReport: Array<leadReportByMonthModel>;
  topNewestLead: Array<leadReportModel>;
  topApprovalLead: Array<leadReportModel>
  listLeadSumaryByMonth: Array<leadSumaryByMonth>;
  constructor() {
    this.listInvestFundReport = [];
    this.listPotentialReport = [];
    this.listInterestedGroupReport = [];
    this.topNewestLead = [];
    this.topApprovalLead = [];
    this.listLeadSumaryByMonth = [];
  }
}

@Component({
  selector: 'app-lead-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  providers: [DashboardService]
})
export class LeadDashboardComponent implements OnInit {
  loading: boolean = false;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  currentYear: any = new Date().getFullYear();
  listMonth: Array<Month> = [
    {
      label: 'Tháng 1', result: 1
    },
    {
      label: 'Tháng 2', result: 2
    },
    {
      label: 'Tháng 3', result: 3
    },
    {
      label: 'Tháng 4', result: 4
    },
    {
      label: 'Tháng 5', result: 5
    },
    {
      label: 'Tháng 6', result: 6
    },
    {
      label: 'Tháng 7', result: 7
    },
    {
      label: 'Tháng 8', result: 8
    },
    {
      label: 'Tháng 9', result: 9
    },
    {
      label: 'Tháng 10', result: 10
    },
    {
      label: 'Tháng 11', result: 11
    },
    {
      label: 'Tháng 12', result: 12
    }
  ];
  selectedMonth: Month = null;
  //master data
  leadDashBoard: leadDashBoardModel = new leadDashBoardModel();

  currentMonth: number = 0;
  totalInvestFundByMonth: number = 0;
  totalPotentialByMonth: number = 0;
  totalInterestedByMonth: number = 0;

  currentListInvestFundReport: Array<leadReportByMonthModel> = [];
  currentListlPotentialReport: Array<leadReportByMonthModel> = [];
  currentInterestedReport: Array<leadReportByMonthModel> = [];
  //biểu đồ mức độ chuyển đổi
  convertRatingChart: Chart; //biểu đồ
  potentialChart: Chart; //biểu đồ
  interestedChart: Chart;
  //table
  topNewestLead: Array<leadReportModel> = [];
  topApprovalLead: Array<leadReportModel> = [];

  colsNewestLead: any;
  colsWatingQuoting: any;
  selectedColumnsQuoting: any;
  selectedColumns: any;
  selectedNewestLead: leadReportModel;
  selectedWaitingLead: leadReportModel;

  userPermission: any = localStorage.getItem("UserPermission").split(",");
  isManager: boolean = localStorage.getItem("IsManager") === 'true';
  createPermission: string = "lead/create";
  viewLeadDashboard: string = "lead/dashboard"
  hasCreatePermission: boolean = false;
  unFollowPermisison: string = "lead/unfollow";

  //list action in page
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionApproval: boolean = true;
  actionApprove: boolean = true;
  actionReject: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  /*Khoi tao Constructor*/
  constructor(private translate: TranslateService,
    private getPermission: GetPermission,
    private service: DashboardService,
    private leadService: LeadService,
    private noteService: NoteService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private router: Router) {
    translate.setDefaultLang('vi');
  }
  /*Ket thuc*/

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  /*Link den trang detail cua 1 Lead*/
  onViewDetail(leadId, contactId) {
    this.router.navigate(['/lead/detail', { leadId: leadId }]);
  }
  /*Ket thuc*/

  ngOnInit() {
    //Check permission
    let resource = "crm/lead/dashboard";
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

      this.initTable();
      this.getMasterdata();
    }
  }

  initTable() {
    this.colsNewestLead = [
      { field: 'leadName', header: 'Cơ hội', textAlign: 'left' },
      { field: 'phone', header: 'Số điện thoại', textAlign: 'right' },
      { field: 'email', header: 'Email', textAlign: 'left' },
      { field: 'personInChangeName', header: 'Người phụ trách', textAlign: 'left' },
      { field: 'customerName', header: 'Khách hàng', textAlign: 'left' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', width: '100px' },
    ];

    this.selectedColumns = this.colsNewestLead.filter(e => e.field == "leadName" || e.field == "customerName" || e.field == "statusName");

    this.colsWatingQuoting = [
      { field: 'leadName', header: 'Cơ hội', textAlign: 'left' },
      { field: 'phone', header: 'Số điện thoại', textAlign: 'right' },
      { field: 'email', header: 'Email', textAlign: 'left' },
      { field: 'personInChangeName', header: 'Người phụ trách', textAlign: 'left' },
      { field: 'customerName', header: 'Khách hàng', textAlign: 'left' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', width: '100px' },
    ];
    this.selectedColumnsQuoting = this.colsNewestLead.filter(e => e.field == "leadName" || e.field == "customerName" || e.field == "statusName");

    let currentMonth = new Date().getMonth() + 1;
    this.selectedMonth = this.listMonth.find(e => e.result == currentMonth);
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.service.getDataLeadDashboard();
    this.loading = false;
    if (result.statusCode === 200) {

      //success
      this.leadDashBoard = result.leadDashBoard;
      this.topNewestLead = this.leadDashBoard.topNewestLead;
      this.topApprovalLead = this.leadDashBoard.topApprovalLead;
      this.setcurrentMonth();
      this.setCurrentTotalByMonth(this.currentMonth);
      this.handleInvestFundChart();
      this.handlePotentialChart();
      this.handleInterestedGroupChart();
    } else {
      let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  setcurrentMonth() {
    let current = new Date();
    this.currentMonth = current.getMonth() + 1;
  }

  setCurrentTotalByMonth(month: number) {
    let leadSumaryByMonth = this.leadDashBoard.listLeadSumaryByMonth.find(e => e.month == month);
    this.totalInvestFundByMonth = leadSumaryByMonth ? leadSumaryByMonth.totalInvestFundByMonth : 0;
    this.totalPotentialByMonth = leadSumaryByMonth ? leadSumaryByMonth.totalPotentialByMonth : 0;
    this.totalInterestedByMonth = leadSumaryByMonth ? leadSumaryByMonth.totalInterestedByMonth : 0;
  }

  changeChooseMonth(value: Month) {
    this.currentMonth = value.result;
    this.setCurrentTotalByMonth(this.currentMonth);
    this.handleInvestFundChart();
    this.handlePotentialChart();
    this.handleInterestedGroupChart();
  }

  handleInvestFundChart() {
    let leadDashboard = this.leadDashBoard.listInvestFundReport.filter(e => e.month == this.currentMonth);
    this.currentListInvestFundReport = leadDashboard;
    let data: Array<dataChart> = [];
    leadDashboard.forEach((e, index) => {
      let item = new dataChart();
      // item.name = `${e.categoryName} ${e.percentValue}`;
      item.name = `${e.categoryName}`;
      item.y = e.percentValue;
      //set color
      // let colorConfig = COLOR_CONFIG[index];
      // item.color = colorConfig ? colorConfig.color : "#34c759";
      // this.currentListInvestFundReport[index].color = colorConfig ? colorConfig.color : "#34c759";
      let colorConfig = COLOR_CONFIG_2[index];
      item.color = colorConfig ? colorConfig : COLOR_CONFIG_2[0];
      this.currentListInvestFundReport[index].color = colorConfig ? colorConfig : COLOR_CONFIG_2[0];

      data = [item, ...data];
    });
    let chart = new Chart({
      chart: {
        type: 'pie',
        alignTicks: false,
        height: '100%'
      },
      title: {
        text: '', //text in center
        align: 'center',
        verticalAlign: 'middle',
        style: { 'color': '#6D98E7', 'font-size': '20px', 'line-height': '10px' }
      },
      credits: {
        enabled: false
      },
      tooltip: {
        pointFormat: '{series.name}: {point.percentage:.2f}%'

        // formatter: function () {
        //   // return 'The value for <b>' + this.x + '</b> is <b>' + this.y + '</b>, in series ' + this.series.name;
        //   return `${this.series.data.name}: ${this.series.data.y}%`;
        // }
      },
      series: [{
        name: 'Tỉ lệ',
        innerSize: '70%',
        data: data,
      }],
      plotOptions: {
        pie: {
          allowPointSelect: true,
          cursor: 'pointer',
          dataLabels: {
            enabled: false
          }
        }
      }
    });
    this.convertRatingChart = chart;
  }

  handlePotentialChart() {
    let leadDashboard = this.leadDashBoard.listPotentialReport.filter(e => e.month == this.currentMonth);
    this.currentListlPotentialReport = leadDashboard;
    let data: Array<dataChart> = [];
    leadDashboard.forEach((e, index) => {
      let item = new dataChart();
      item.name = `${e.categoryName}`;
      item.y = e.percentValue;
      //set color
      // let colorConfig = COLOR_CONFIG[index];
      // item.color = colorConfig ? colorConfig.color : "#34c759";
      // this.currentListlPotentialReport[index].color = colorConfig ? colorConfig.color : "#34c759";

      let colorConfig = COLOR_CONFIG_2[index];
      item.color = colorConfig ? colorConfig : COLOR_CONFIG_2[0];
      this.currentListlPotentialReport[index].color = colorConfig ? colorConfig : COLOR_CONFIG_2[0];

      data = [item, ...data];
    });
    let chart = new Chart({
      chart: {
        type: 'pie',
        alignTicks: false,
        height: '100%'
      },
      title: {
        text: '', //text in center
        align: 'center',
        verticalAlign: 'middle',
        style: { 'color': '#6D98E7', 'font-size': '20px', 'line-height': '10px' }
      },
      credits: {
        enabled: false
      },
      tooltip: {
        pointFormat: '{series.name}: {point.percentage:.2f}%'
      },
      series: [{
        name: 'Tỉ lệ',
        innerSize: '70%',
        data: data,
      }],
      plotOptions: {
        pie: {
          allowPointSelect: true,
          cursor: 'pointer',
          dataLabels: {
            enabled: false
          }
        }
      }
    });
    this.potentialChart = chart;
  }

  handleInterestedGroupChart() {
    let leadDashboard = this.leadDashBoard.listInterestedGroupReport.filter(e => e.month == this.currentMonth);
    this.currentInterestedReport = leadDashboard;
    let data: Array<dataChart> = [];
    leadDashboard.forEach((e, index) => {
      let item = new dataChart();
      // item.name = `${e.categoryName} ${e.percentValue}`;
      item.name = `${e.categoryName}`;
      item.y = e.percentValue;
      //set color
      // let colorConfig = COLOR_CONFIG[index];
      // item.color = colorConfig ? colorConfig.color : "#34c759";
      // this.currentInterestedReport[index].color = colorConfig ? colorConfig.color : "#34c759";

      let colorConfig = COLOR_CONFIG_2[index];
      item.color = colorConfig ? colorConfig : COLOR_CONFIG_2[0];
      this.currentInterestedReport[index].color = colorConfig ? colorConfig : COLOR_CONFIG_2[0];

      data = [item, ...data];
    });
    let chart = new Chart({
      chart: {
        type: 'pie',
        alignTicks: false,
        height: '100%'
      },
      title: {
        text: '', //text in center
        align: 'center',
        verticalAlign: 'middle',
        style: { 'color': '#6D98E7', 'font-size': '20px', 'line-height': '10px' }
      },
      credits: {
        enabled: false
      },
      tooltip: {
        pointFormat: '{series.name}: {point.percentage:.2f}%'
      },
      series: [{
        name: 'Tỉ lệ',
        innerSize: '70%',
        data: data,
      }],
      plotOptions: {
        pie: {
          allowPointSelect: true,
          cursor: 'pointer',
          dataLabels: {
            enabled: false
          }
        }
      }
    });
    this.interestedChart = chart;
  }

  quickCreateLead() {
    this.router.navigate(['/lead/create-lead']);
  }

  goTolist(status: string) {
    this.router.navigate(['/lead/list', { status: status }]);
  }

  goToDetail(rowData: leadReportModel) {
    this.router.navigate(['/lead/detail', { leadId: rowData.leadId }]);
  }

  /*Tạo báo giá từ cơ hội*/
  createLeadQuote(selectedLead: leadReportModel) {
    // this.router.navigate(['/customer/quote-create', {
    //   leadId: selectedLead.leadId
    // }]);
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }
}

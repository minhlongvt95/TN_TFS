import { Component, OnInit, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { Chart, ChartData } from 'chart.js';
import { Router } from '@angular/router';
import { SalesDashboardService } from './services/sales-dashboard.service';
import { DatePipe } from '@angular/common';
import { DateParameter } from '../../models/dateParameter.model';
import { WarningComponent } from '../../../shared/toast/warning/warning.component';
import { GetPermission } from '../../../shared/permission/get-permission';

import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';

import { OrgSelectDialogComponent } from '../../../employee/components/org-select-dialog/org-select-dialog.component';

//ADAPTER Date: dd/mm/yyyy
import { AppDateAdapter, APP_DATE_FORMATS } from '../../../shared/adapter/date.adapter';

import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
@Component({
  selector: 'app-sales-dashboard',
  templateUrl: './sales-dashboard.component.html',
  styleUrls: ['./sales-dashboard.component.css'],
  providers: [
    {
      provide: DateAdapter, useClass: AppDateAdapter
    },
    {
      provide: MAT_DATE_FORMATS, useValue: APP_DATE_FORMATS
    }
  ]
})
export class SalesDashboardComponent implements OnInit, AfterViewInit {
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  loading: boolean = false;
  userId: string = JSON.parse(localStorage.getItem('auth')).UserId;
  isManager: boolean = localStorage.getItem('IsManager') === "true" ? true : false;

  @ViewChild('canvas') canvas: ElementRef;
  @ViewChild('canvas_2') canvas_2: ElementRef;
  @ViewChild('canvas_status') canvas_status: ElementRef;
  @ViewChild('canvas_month') canvas_month: ElementRef;
  chart: any;
  chart_2: any;
  chart_status: any;
  chart_month: any;

  //data doughnut
  chartShow: boolean = true;
  chartShowByProductCategoryGorup: boolean = true;
  doughnutData: ChartData = {
    datasets: [],
    labels: []
  }
  list_color: any = ["#ff6384", "#ffcd56", "#dd4895", "#36a2eb", "#8beb7a", "#4bc0c0", "#ff9f40", "#e5e5e5"];
  current_list_color: any = [];
  current_percent: any = [];
  current_money: any = [];
  current_title: any = [];

  //data bar
  chartBarShow: boolean = false;
  chartBarShowStatus: boolean = false;
  chartBarShowMonth: boolean = false;
  current_list_name: any = [];
  current_list_revenue: any = [];
  month_list_name: any = [];
  month_list_revenue: any = [];
  status_list_name: any = [];
  status_list_revenue: any = [];

  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };
  pageFirstLoad: any;
  err_server: boolean = false;
  seller: string = '';
  positionCode: string = '';
  orgNameDisplay: string = '';
  //listOrganization: Array<any>;
  listTemp: Array<any> = [];
  listSelectTemp: Array<any> = [];
  model: DateParameter = {
    OrderStartDate: null,
    OrderEndDate: null,
    OrganizationId: null,
    OrganizationName: null
  }
  LevelGroupProduct: number = 0;
  listLevelGroupProduct: Array<any> = [];

  //data table
  isAscending = true;
  displayedColumns = ['code', 'value', 'name'];
  dataListOrder: any = [];
  orderList: Array<any>;

  dataTop5ListOrder: any = [];
  orderTop5List: Array<any>;
  lstOrderBill: Array<any>;
  lstOrderInventory: Array<any>;
  selectedOrderStatus: Array<any> = [];

  displayedColumnsAdmin = ['name', 'value', 'organizationName'];
  dataListEmployee: any = [];
  employeeList: Array<any>;

  dialogOrg: MatDialogRef<OrgSelectDialogComponent>;
  organizationId: string = '';

  //total amount
  total_money: number = 0;
  total_money_search: number = 0;

  currentTimeString = this.convertTimeToString();
  colsEmp: any;
  colsOrderNew: any;
  colsOrderBill: any;
  colsOrderInventory: any;

  // list các tháng để xem doanh số giữa các tháng ( mục giá trị đơn hàng giữa các tháng )
  monthOrderName: any = [];
  // list doanh số của các tháng tại list trên
  monthOrderRevenue: any = [];
  listMonthOption: Array<any> = [];
  months: any;
  orderStartDate: Date = new Date();

  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private salesDashboardService: SalesDashboardService,
    public snackBar: MatSnackBar,
    public dialog: MatDialog,
    private messageService: MessageService,
    private dialogService: DialogService,
  ) { }

  async ngOnInit() {
    this.listMonthOption = [
      { id: 3, name: '3 tháng' },
      { id: 6, name: '6 tháng' },
      { id: 12, name: '12 tháng' },
    ];
    this.months = this.listMonthOption[0];
    let resource = "sal/sales/dashboard";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      var today = new Date();
      this.model.OrderEndDate = today;
      this.model.OrderStartDate = new Date((new Date).setDate(1));
      this.seller = JSON.parse(localStorage.getItem('auth')).EmployeeId;

      // if (this.isManager) {
      const result: any = await this.salesDashboardService.getChildrenOrganizationById(this.seller);
      if (result && result.statusCode === 200) {
        this.listTemp.push(result.organizationParent[0]);
        this.isManager = result.isManager;
        this.filterData(result.listOrganization);
        //Mặc định khi hiển thị lúc đầu của ô select Phòng Ban là Phòng ban của người đang đăng nhập
        this.model.OrganizationId = result.organizationParent[0].OrganizationId;
        this.organizationId = result.organizationParent[0].OrganizationId;
        this.orgNameDisplay = result.organizationParent[0].OrganizationName;
        this.search_admin();
        this.getMonthsOrder();
      } else {
        this.err_server = true;
      }
      // } else {
      //   this.search();
      // }
      this.setTable();
    }
  }

  setTable() {
    this.colsEmp = [
      { field: 'employeeName', header: 'Tên nhân viên' },
      { field: 'organizationName', header: 'Phòng' },
      { field: 'total', header: 'Doanh số' },
    ];

    this.colsOrderNew = [
      { field: 'orderCode', header: 'Mã đơn hàng' },
      { field: 'customerName', header: 'Khách hàng' },
      { field: 'amount', header: 'Tổng giá trị' },
    ];

    this.colsOrderBill = [
      { field: 'orderCode', header: 'Mã đơn hàng', textAlign: 'left', display: 'table-cell', width: '110px' },
      { field: 'customerName', header: 'Khách hàng', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'amount', header: 'Tổng giá trị', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'orderDate', header: 'Ngày đặt hàng', textAlign: 'left', display: 'table-cell', width: '100px' },
    ];

    this.colsOrderInventory = [
      { field: 'orderCode', header: 'Mã đơn hàng', textAlign: 'left', display: 'table-cell', width: '110px' },
      { field: 'customerName', header: 'Khách hàng', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'amount', header: 'Tổng giá trị', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'orderDate', header: 'Ngày đặt hàng', textAlign: 'left', display: 'table-cell', width: '100px' },
    ];
  }
  ngAfterViewInit() {
    /*Sẽ comment để làm mới*/
    if (this.isManager) {
      this.setDoughnut(this.current_percent, this.current_list_color, this.current_title);
      this.setBar(this.current_list_name, this.current_list_revenue);
    } else {
      this.setDoughnut(this.current_percent, this.current_list_color, this.current_title);
    }
    /*End*/
  }

  async getMonthsOrder() {
    var dateFrom = convertToUTCTime(this.orderStartDate);
    const result: any = await this.salesDashboardService.getMonthsList(this.userId, dateFrom, this.months.id, this.model.OrganizationId);
    
    this.monthOrderName = [];
    this.monthOrderRevenue = [];

    var monthListPrice = result.monthOrderList;
    var count = 0;
    monthListPrice.forEach(x => {
      if (x.total == 0){
        count++;
      }
    });

    if (result.monthOrderList.length != count) {
      this.chartBarShowMonth = true;
      for (let i = 0; i < result.monthOrderList.length; i++) {
        this.monthOrderName[i] = result.monthOrderList[i].monthName;
        this.monthOrderRevenue[i] = result.monthOrderList[i].total;
      }
      this.setBarMonthOrder(this.monthOrderName, this.monthOrderRevenue);
    } else {
      this.chartBarShowMonth = false;
      let mgs = { severity: 'info', summary: 'Thông báo:', detail: 'Không có dữ liệu trong khoảng thời gian được chọn' };
      this.showMessage(mgs);
    }
  }

  convertTimeToString(): string {
    let now = new Date();
    let day = now.getDay() + 1;
    let stringDay = '';
    let date = now.getDate();
    let stringDate = '';
    let month = now.getMonth() + 1;
    let stringMonth = '';
    let year = now.getFullYear();
    let stringYear = '';

    if (day == 1) {
      stringDay = "Chủ nhật"
    } else {
      stringDay = "Thứ " + day;
    }
    stringDate = "ngày " + date;
    stringMonth = "tháng " + month;
    stringYear = "năm " + year;

    return stringDay + ", " + stringDate + " " + stringMonth + " " + stringYear;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  setDataDoughnutNull() {
    this.current_percent = [];
    this.current_list_color = [];
    this.current_title = [];
    this.setDoughnut(this.current_percent, this.current_list_color, this.current_title);
  }

  setDataBarNull() {
    this.current_list_name = [];
    this.current_list_revenue = [];
    this.setBar(this.current_list_name, this.current_list_revenue);
  }

  setDoughnut(current_percent, current_list_color, current_title) {
    setTimeout(() => {
      if (this.chart) {
        this.chart.destroy();
      }
      if (this.canvas != null) {
        this.chart = new Chart(this.canvas.nativeElement.getContext('2d'), {
          type: 'doughnut',
          data: {
            datasets: [{
              data: current_percent,
              data_total: this.current_money,
              backgroundColor: current_list_color,
              borderWidth: 1
            }],
            labels: current_title
          },
          options: {
            cutoutPercentage: 70,
            responsive: true,
            legend: {
              display: false,
              position: 'bottom',
            },
            tooltips: {
              enabled: true,
              callbacks: {
                label: function (tooltipItem, data) {
                  var label = data.labels[tooltipItem.index];
                  var datasetLabel = data.datasets[tooltipItem.datasetIndex].data_total[tooltipItem.index].toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
                  return label + ': ' + datasetLabel;
                }
              }
            }
          }
        });
        this.doughnutData = this.chart.data;
      }
    }, 100);
  }

  setBar(current_list_name, current_list_revenue) {
    setTimeout(() => {
      if (this.chart_2) {
        this.chart_2.destroy();
      }
      const maxwidthofLabel = 19 - current_list_name.length;
      if (this.canvas_2 != null) {
        this.chart_2 = new Chart(this.canvas_2.nativeElement.getContext('2d'), {
          type: 'bar',
          data: {
            labels: current_list_name.map(t => this.formatLabel(t, maxwidthofLabel)),
            datasets: [
              {
                label: "Doanh số (VND)",
                data: current_list_revenue,
                fill: false,
                backgroundColor: "#6d98e7",
                borderWidth: 1
              }
            ]
          },
          options: {
            responsive: true,
            legend: {
              display: false
            },
            tooltips: {
              titleFontSize: 0,
              enabled: true,
              mode: 'single',
              callbacks: {
                label: function (tooltipItem, data) {
                  var label = data.labels[tooltipItem.index];
                  var datasetLabel = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
                  datasetLabel += '';
                  var x = datasetLabel.split('.');
                  var x1 = x[0];
                  var x2 = x.length > 1 ? '.' + x[1] : '';
                  var rgx = /(\d+)(\d{3})/;
                  while (rgx.test(x1)) {
                    x1 = x1.replace(rgx, '$1' + ',' + '$2');
                  }
                  var new_data = x1 + x2;
                  return label + ': ' + new_data;
                }
              }
            },
            scales: {
              yAxes: [{
                ticks: {
                  callback: function (value, index, values) {
                    // add comma as thousand separator
                    value += '';
                    var x = value.split('.');
                    var x1 = x[0];
                    var x2 = x.length > 1 ? '.' + x[1] : '';
                    var rgx = /(\d+)(\d{3})/;
                    while (rgx.test(x1)) {
                      x1 = x1.replace(rgx, '$1' + ',' + '$2');
                    }
                    var new_data = x1 + x2;
                    return new_data
                  },
                  min: 0, //Giá trị min của mốc dữ liệu
                  stepSize: this.setStepSize(current_list_revenue),  //Khoảng cách giữa các mốc dữ liệu
                }
              }],
              xAxes: [{
                barPercentage: 0.5,
                gridLines: {
                  display: false
                }
              }]
            }
          }
        });
      }
    }, 200);
  }

  setBarStatustOrder(current_list_name, current_list_revenue) {
    setTimeout(() => {
      if (this.chart_status) {
        this.chart_status.destroy();
      }
      const maxwidthofLabel = 19 - current_list_name.length;
      if (this.canvas_status != null) {
        this.chart_status = new Chart(this.canvas_status.nativeElement.getContext('2d'), {
          type: 'bar',
          data: {
            labels: current_list_name.map(t => this.formatLabel(t, maxwidthofLabel)),
            datasets: [
              {
                label: "Doanh số (VND)",
                data: current_list_revenue,
                fill: false,
                backgroundColor: "#6d98e7",
                borderWidth: 1
              }
            ]
          },
          options: {
            responsive: true,
            legend: {
              display: false
            },
            tooltips: {
              titleFontSize: 0,
              enabled: true,
              mode: 'single',
              callbacks: {
                label: function (tooltipItem, data) {
                  var label = data.labels[tooltipItem.index];
                  var datasetLabel = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
                  datasetLabel += '';
                  var x = datasetLabel.split('.');
                  var x1 = x[0];
                  var x2 = x.length > 1 ? '.' + x[1] : '';
                  var rgx = /(\d+)(\d{3})/;
                  while (rgx.test(x1)) {
                    x1 = x1.replace(rgx, '$1' + ',' + '$2');
                  }
                  var new_data = x1 + x2;
                  return label + ': ' + new_data;
                }
              }
            },
            scales: {
              yAxes: [{
                ticks: {
                  callback: function (value, index, values) {
                    // add comma as thousand separator
                    value += '';
                    var x = value.split('.');
                    var x1 = x[0];
                    var x2 = x.length > 1 ? '.' + x[1] : '';
                    var rgx = /(\d+)(\d{3})/;
                    while (rgx.test(x1)) {
                      x1 = x1.replace(rgx, '$1' + ',' + '$2');
                    }
                    var new_data = x1 + x2;
                    return new_data
                  },
                  min: 0, //Giá trị min của mốc dữ liệu
                  stepSize: this.setStepSize(current_list_revenue),  //Khoảng cách giữa các mốc dữ liệu
                }
              }],
              xAxes: [{
                barPercentage: 0.5,
                gridLines: {
                  display: false
                }
              }]
            }
          }
        });
      }
    }, 200);
  }

  setBarMonthOrder(current_list_name, current_list_revenue) {
    setTimeout(() => {
      if (this.chart_month) {
        this.chart_month.destroy();
      }
      const maxwidthofLabel = 19 - current_list_name.length;
      if (this.canvas_month != null) {
        this.chart_month = new Chart(this.canvas_month.nativeElement.getContext('2d'), {
          type: 'bar',
          data: {
            labels: current_list_name.map(t => this.formatLabel(t, maxwidthofLabel)),
            datasets: [
              {
                label: "Doanh số (VND)",
                data: current_list_revenue,
                fill: false,
                backgroundColor: "#6d98e7",
                borderWidth: 1
              }
            ]
          },
          options: {
            responsive: true,
            legend: {
              display: false
            },
            tooltips: {
              titleFontSize: 0,
              enabled: true,
              mode: 'single',
              callbacks: {
                label: function (tooltipItem, data) {
                  var label = data.labels[tooltipItem.index];
                  var datasetLabel = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
                  datasetLabel += '';
                  var x = datasetLabel.split('.');
                  var x1 = x[0];
                  var x2 = x.length > 1 ? '.' + x[1] : '';
                  var rgx = /(\d+)(\d{3})/;
                  while (rgx.test(x1)) {
                    x1 = x1.replace(rgx, '$1' + ',' + '$2');
                  }
                  var new_data = x1 + x2;
                  return label + ': ' + new_data;
                }
              }
            },
            scales: {
              yAxes: [{
                ticks: {
                  callback: function (value, index, values) {
                    // add comma as thousand separator
                    value += '';
                    var x = value.split('.');
                    var x1 = x[0];
                    var x2 = x.length > 1 ? '.' + x[1] : '';
                    var rgx = /(\d+)(\d{3})/;
                    while (rgx.test(x1)) {
                      x1 = x1.replace(rgx, '$1' + ',' + '$2');
                    }
                    var new_data = x1 + x2;
                    return new_data
                  },
                  min: 0, //Giá trị min của mốc dữ liệu
                  stepSize: this.setStepSize(current_list_revenue),  //Khoảng cách giữa các mốc dữ liệu
                }
              }],
              xAxes: [{
                barPercentage: 0.5,
                gridLines: {
                  display: false
                }
              }]
            }
          }
        });
      }
    }, 200);
  }

  /*Format Label bar chart wrapping*/
  formatLabel(str, maxwidth) {
    let sections = [];
    let words = str.split(" ");
    let temp = "";

    words.forEach((item, index) => {
      if ((temp + " " + item).trim().length <= maxwidth) {
        temp = (temp + " " + item).trim();
      } else {
        if (temp.trim() != "") sections.push(temp);
        temp = item;
      }
      if (index == words.length - 1 && temp.trim() != "") {
        sections.push(temp);
      }
    });

    return sections;
  }
  // End

  setStepSize(data) {
    //Chia nhiều nhất là 10 mốc dữ liệu
    let max = Math.max(...data);
    return this.formatRoundNumber(max / 10);
  }

  //Làm tròn số
  formatRoundNumber(number) {
    number = number.toString();
    if(number.includes('.')){
      var index = number.indexOf('.');
      number = number.substring(0, index);
    }
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

  // async search() {
  //   this.loading = true;
  //   this.setDataDoughnutNull();

  //   var OrderDateStartToSearch = this.model.OrderStartDate;
  //   if (this.model.OrderStartDate !== null) {
  //     OrderDateStartToSearch = convertToUTCTime(setStartDate(this.model.OrderStartDate));
  //   }
  //   var OrderDateEndToSearch = this.model.OrderEndDate;
  //   if (this.model.OrderEndDate !== null) {
  //     OrderDateEndToSearch = convertToUTCTime(setEndDate(this.model.OrderEndDate));
  //   }

  //   const result:any = await this.salesDashboardService.getCustomerOrderBySellerList(this.seller, OrderDateStartToSearch, OrderDateEndToSearch);
  //       if (result.orderList.length === 0) {
  //         this.chartShow = false;
  //         this.chartShowByProductCategoryGorup = false;
  //         if (!this.pageFirstLoad) {
  //           this.snackBar.openFromComponent(WarningComponent, { data: 'Không tìm thấy đơn hàng nào!', ... this.warningConfig });
  //         }
  //         this.LevelGroupProduct = 0;
  //         this.dataListOrder = new MatTableDataSource([]);
  //         this.orderList = result.orderList;
  //         this.total_money = 0;
  //         this.total_money_search = 0;

  //       } else {
  //         this.chartShow = true;
  //         this.chartShowByProductCategoryGorup = true;
  //         this.dataListOrder = new MatTableDataSource(result.orderList);
  //         this.orderList = result.orderList;
  //         this.total_money = result.totalProduct;

  //         this.total_money_search = result.totalProduct;

  //         this.LevelGroupProduct = 0;
  //         this.listLevelGroupProduct = [];
  //         for (let i = 0; i <= result.levelMaxProductCategory; i++) {
  //           let item = {
  //             LevelGroupProduct: i,
  //             LevelGroupProductName: 'Cấp ' + i
  //           }
  //           this.listLevelGroupProduct.push(item);
  //         }

  //         if (result.lstResult.length <= 8) {
  //           //Nếu tổng số nhóm sản phẩm bé hơn 8 thì
  //           for (let i = 0; i < result.lstResult.length; i++) {
  //             this.current_list_color[i] = this.list_color[i];
  //             this.current_percent[i] = result.lstResult[i].Percent;
  //             this.current_money[i] = result.lstResult[i].Total;
  //             this.current_title[i] = result.lstResult[i].ProductCategoryName;
  //           }
  //           this.setDoughnut(this.current_percent, this.current_list_color, this.current_title);
  //         } else {
  //           //Nếu tổng số nhóm sản phẩm lớn hơn 8 thì
  //           let total_top_7 = 0;
  //           for (let i = 0; i < 8; i++) {
  //             this.current_list_color[i] = this.list_color[i];
  //             this.current_percent[i] = result.lstResult[i].Percent;
  //             this.current_money[i] = result.lstResult[i].Total;
  //             this.current_title[i] = result.lstResult[i].ProductCategoryName;
  //             total_top_7 += result.lstResult[i].Percent;
  //             if (i == 7) {
  //               this.current_list_color[i] = this.list_color[i];
  //               this.current_percent[i] = (100 - total_top_7).toFixed(1);
  //               this.current_money[i] = result.lstResult[i].Total;
  //               this.current_title[i] = "Khác";
  //             }
  //           }
  //           this.setDoughnut(this.current_percent, this.current_list_color, this.current_title);
  //         }
  //       }
  //   await this.getTop5Order();
  //   this.loading = false;
  // }
  //Get top 5 order

  async getTop5Order() {
    var OrderDateStartToSearch = this.model.OrderStartDate;
    if (this.model.OrderStartDate !== null) {
      OrderDateStartToSearch = convertToUTCTime(setStartDate(this.model.OrderStartDate));
    }
    var OrderDateEndToSearch = this.model.OrderEndDate;
    if (this.model.OrderEndDate !== null) {
      OrderDateEndToSearch = convertToUTCTime(setEndDate(this.model.OrderEndDate));
    }

    const result: any = await this.salesDashboardService.getTop3NewCustomerOrderList("", "", "", "", OrderDateStartToSearch, OrderDateEndToSearch, 1, 5, this.listSelectTemp.map(item => item.OrganizationId));
    if (result.orderTop5List.length === 0) {
      this.dataTop5ListOrder = new MatTableDataSource([]);
      this.orderTop5List = result.orderTop5List;
    } else {
      this.dataTop5ListOrder = new MatTableDataSource(result.orderTop5List);
      this.orderTop5List = result.orderTop5List;
    }
  }

  async search_admin() {
    this.loading = true;
    this.setDataDoughnutNull();
    this.setDataBarNull();

    var OrderDateStartToSearch = this.model.OrderStartDate;
    if (this.model.OrderStartDate !== null) {
      OrderDateStartToSearch = convertToUTCTime(setStartDate(this.model.OrderStartDate));
    }
    var OrderDateEndToSearch = this.model.OrderEndDate;
    if (this.model.OrderEndDate !== null) {
      OrderDateEndToSearch = convertToUTCTime(setEndDate(this.model.OrderEndDate));
    }
    this.status_list_name = [];
    this.status_list_revenue = [];
    this.month_list_name = [];
    this.month_list_revenue = [];
    const result: any = await this.salesDashboardService.getEmployeeListByOrganizationId(this.userId,
      OrderDateStartToSearch, OrderDateEndToSearch, this.model.OrganizationId);
    if (result.employeeList.length === 0 && result.lstOrderBill.length === 0 && result.lstOrderInventoryDelivery.length === 0
      && result.lstResult.length === 0 && result.monthOrderList.length === 0 && result.statusOrderList.length === 0) {
      this.chartBarShow = false;
      // this.chartBarShowMonth = false;
      this.chartBarShowStatus = false;
      this.chartShow = false;
      this.chartShowByProductCategoryGorup = false;
      if (!this.pageFirstLoad) {
        this.snackBar.openFromComponent(WarningComponent, { data: 'Không tìm thấy bản ghi nào!', ... this.warningConfig });
      }
      this.LevelGroupProduct = 0;
      this.dataListEmployee = new MatTableDataSource([]);
      this.employeeList = result.employeeList;
      this.total_money_search = 0;
    } else {
      if (result.employeeList.length !== 0) {
        this.chartBarShow = true;
        this.chartShow = true;
        this.chartShowByProductCategoryGorup = true;
        this.dataListEmployee = new MatTableDataSource(result.employeeList);
        this.employeeList = result.employeeList;

        this.LevelGroupProduct = 0;
        this.listLevelGroupProduct = [];
        for (let i = 0; i <= result.levelMaxProductCategory; i++) {
          let item = {
            LevelGroupProduct: i,
            LevelGroupProductName: 'Cấp ' + i
          }
          this.listLevelGroupProduct.push(item);
        }
        this.total_money_search = result.employeeList[0].totalSearch;

        for (let i = 0; i < this.employeeList.length; i++) {
          this.current_list_name[i] = this.employeeList[i].employeeName;
          this.current_list_revenue[i] = this.employeeList[i].total;
        }

        this.setBar(this.current_list_name, this.current_list_revenue);
        this.chartShow = true;
        this.chartShowByProductCategoryGorup = true;
        if (result.lstResult.length <= 8) {
          //Nếu tổng số nhóm sản phẩm bé hơn 8 thì
          for (let i = 0; i < result.lstResult.length; i++) {
            this.current_list_color[i] = this.list_color[i];
            this.current_percent[i] = result.lstResult[i].Percent;
            this.current_money[i] = result.lstResult[i].Total;
            this.current_title[i] = result.lstResult[i].ProductCategoryName;
          }
          this.setDoughnut(this.current_percent, this.current_list_color, this.current_title);
        } else {
          //Nếu tổng số nhóm sản phẩm lớn hơn 8 thì
          let total_top_7 = 0;
          for (let i = 0; i < 8; i++) {
            if (i < 7) {
              this.current_list_color[i] = this.list_color[i];
              this.current_percent[i] = result.lstResult[i].Percent;
              this.current_title[i] = result.lstResult[i].ProductCategoryName;
              this.current_money[i] = result.lstResult[i].Total;
              total_top_7 += result.lstResult[i].Percent;
            } else if (i == 7) {
              this.current_list_color[i] = this.list_color[i];
              this.current_percent[i] = (100 - total_top_7).toFixed(1);
              this.current_money[i] = result.lstResult[i].Total;
              this.current_title[i] = "Khác";
            }
          }
          this.setDoughnut(this.current_percent, this.current_list_color, this.current_title);
        }
      }
      if (result.statusOrderList.length !== 0) {
        this.chartBarShowStatus = true;
        for (let i = 0; i < result.statusOrderList.length; i++) {
          this.status_list_name[i] = result.statusOrderList[i].statusName;
          this.status_list_revenue[i] = result.statusOrderList[i].total;
        }

        this.setBarStatustOrder(this.status_list_name, this.status_list_revenue);
      }
      // if (result.monthOrderList.length !== 0) {
      //   this.chartBarShowMonth = true;
      //   for (let i = 0; i < result.monthOrderList.length; i++) {
      //     this.month_list_name[i] = result.monthOrderList[i].monthName;
      //     this.month_list_revenue[i] = result.monthOrderList[i].total;
      //   }

      //   this.setBarMonthOrder(this.month_list_name, this.month_list_revenue);
      // }
    }
    this.getMonthsOrder();
    this.lstOrderBill = result.lstOrderBill;
    this.lstOrderInventory = result.lstOrderInventoryDelivery;
    await this.getTop5Order();
    this.loading = false;
  }

  sort(property: string) {
    this.isAscending = !this.isAscending;
    const value = this.isAscending;
    this.orderList.sort((a: any, b: any) => {
      let x: any;
      let y: any;
      switch (property) {
        case 'code':
          x = a.orderCode.toLowerCase().trim();
          y = b.orderCode.toLowerCase().trim();
          break;
        case 'value':
          x = a.amount;
          y = b.amount;
          break;
        case 'name':
          x = a.customerName.toLowerCase().trim();
          y = b.customerName.toLowerCase().trim();
          break;
        default:
          break;
      }

      if (property === 'value') return (value ? x - y : y - x);
      return (value ? (x.localeCompare(y) === -1 ? -1 : 1) : (x.localeCompare(y) > -1 ? -1 : 1));
    });
    this.dataListOrder = new MatTableDataSource(this.orderList);
  }

  sortTop5(property: string) {
    this.isAscending = !this.isAscending;
    const value = this.isAscending;
    this.orderTop5List.sort((a: any, b: any) => {
      let x: any;
      let y: any;
      switch (property) {
        case 'code':
          x = a.orderCode.toLowerCase().trim();
          y = b.orderCode.toLowerCase().trim();
          break;
        case 'value':
          x = a.amount;
          y = b.amount;
          break;
        case 'name':
          x = a.customerName.toLowerCase().trim();
          y = b.customerName.toLowerCase().trim();
          break;
        default:
          break;
      }

      if (property === 'value') return (value ? x - y : y - x);
      return (value ? (x.localeCompare(y) === -1 ? -1 : 1) : (x.localeCompare(y) > -1 ? -1 : 1));
    });
    this.dataTop5ListOrder = new MatTableDataSource(this.orderTop5List);
  }

  sort_admin(property: string) {
    this.isAscending = !this.isAscending;
    const value = this.isAscending;
    this.employeeList.sort((a: any, b: any) => {
      let x: any;
      let y: any;
      switch (property) {
        case 'organizationName':
          x = a.OrganizationName.toLowerCase().trim();
          y = b.OrganizationName.toLowerCase().trim();
          break;
        case 'value':
          x = a.Total;
          y = b.Total;
          break;
        case 'name':
          x = a.EmployeeName.toLowerCase().trim();
          y = b.EmployeeName.toLowerCase().trim();
          break;
        default:
          break;
      }

      if (property === 'value') return (value ? x - y : y - x);
      return (value ? (x.localeCompare(y) === -1 ? -1 : 1) : (x.localeCompare(y) > -1 ? -1 : 1));
    });
    this.dataListEmployee = new MatTableDataSource(this.employeeList);
  }

  goToDetail(id: string) {
    this.router.navigate(['/order/order-detail', { customerOrderID: id }]);
  }

  goToCreateOrder() {
    this.router.navigate(['/order/create']);
  }

  onViewOrderDetail(customerId, contactId) {
    this.router.navigate(['/customer/detail', { customerId: customerId }]);
  }

  goToTopRevenue() {
    let startDate: Date = this.model.OrderStartDate;
    let endDate: Date = this.model.OrderEndDate;
    var datePipe = new DatePipe("en-US");
    let string_current_start_date = datePipe.transform(startDate, 'yyyy-MM-dd');
    let string_current_end_date = datePipe.transform(endDate, 'yyyy-MM-dd');
    this.router.navigate(['/sales/top-revenue', {
      startDate: string_current_start_date,
      endDate: string_current_end_date
    }]);
  }


  goToOrderList() {
    let EmployeeId = this.seller;
    let startDate: any = this.model.OrderStartDate;
    let endDate: Date = this.model.OrderEndDate;
    var datePipe = new DatePipe("en-US");
    let current_start_date = new Date(startDate);
    let string_current_start_date = datePipe.transform(current_start_date, 'yyyy-MM-dd');
    endDate.setHours(7, 0, 0, 0);
    let string_current_end_date = datePipe.transform(endDate, 'yyyy-MM-dd');

    // this.router.navigate(['/order/list', {
    //   startDate: string_current_start_date,
    //   endDate: string_current_end_date,
    //   employeeId: EmployeeId
    // }]);

    this.router.navigate(['/order/list']);
  }

  goToTopOrderList() {
    let startDate: any = this.model.OrderStartDate;
    let endDate: Date = this.model.OrderEndDate;
    var datePipe = new DatePipe("en-US");
    let current_start_date = new Date(startDate);
    let string_current_start_date = datePipe.transform(current_start_date, 'yyyy-MM-dd');
    endDate.setHours(7, 0, 0, 0);
    let string_current_end_date = datePipe.transform(endDate, 'yyyy-MM-dd');

    // this.router.navigate(['/order/list', {
    //   startDate: string_current_start_date,
    //   endDate: string_current_end_date
    // }]);

    this.router.navigate(['/order/list']);
  }

  goToEmployeeOrderList(EmployeeId: string) {
    let startDate: any = this.model.OrderStartDate;
    let endDate: Date = this.model.OrderEndDate;
    var datePipe = new DatePipe("en-US");
    let current_start_date = new Date(startDate);
    let string_current_start_date = datePipe.transform(current_start_date, 'yyyy-MM-dd');
    endDate.setHours(7, 0, 0, 0);
    let string_current_end_date = datePipe.transform(endDate, 'yyyy-MM-dd');

    // this.router.navigate(['/order/list', {
    //   startDate: string_current_start_date,
    //   endDate: string_current_end_date,
    //   employeeId: EmployeeId
    // }]);

    this.router.navigate(['/order/list']);
  }

  //Filter Validator ngày không <= ngày hiện tại
  myFilter = (d: Date): boolean => {
    let day = d;
    const now = new Date();
    return (day <= now);
  }

  //get day default of OrderStartDate
  getDefaultOrderStartDate() {
    let now = new Date();
    //Kiểm tra hôm nay là thứ mấy
    let n = now.getDay();
    if (n == 1) {
      //Nếu là thứ 2 thì trả về thứ 2 luôn
      return now;
    }

    //Tính thời gian từ thứ 2 đến ngày hiện tại có bao nhiêu ngày
    var sub = 0;
    switch (n) {
      case 0:
        //Nếu hôm nay là chủ nhật
        sub = 6;
        break;
      case 2:
        //Nếu hôm nay là thứ ba
        sub = 1;
        break;
      case 3:
        //Nếu hôm nay là thứ tư
        sub = 2;
        break;
      case 4:
        //Nếu hôm nay là thứ năm
        sub = 3;
        break;
      case 5:
        //Nếu hôm nay là thứ sáu
        sub = 4;
        break;
      case 6:
        //Nếu hôm nay là thứ bảy
        sub = 5;
        break;
      default:
        break;
    }
    //Nếu không phải thứ 2 thì tính Miliseconds
    let current_miliseconds = now.getTime();
    sub = now.getDate();
    if (sub > 1)
      sub = sub - 1;
    //Ngày thứ hai trước đó là
    let result_miliseconds = current_miliseconds - sub * 1000 * 60 * 60 * 24;
    let result = new Date(result_miliseconds);
    return result;
  }

  listNew: Array<any> = [];
  filterData(data) {
    for (let i = 0; i < data.length; i++) {
      let item = {
        OrganizationId: data[i].organizationId,
        OrganizationName: data[i].organizationName,
        Level: data[i].level,
        ParentId: data[i].parentId
      }
      this.listTemp.push(item);
      if (data[i].orgChildList.length > 0) {
        this.filterData(data[i].orgChildList);
      }
    }
    return 'done';
  }

  async changeDoughnutEmployee(productCategoryLevel: number) {
    this.loading = true;
    this.setDataDoughnutNull();
    var OrderDateStartToSearch = this.model.OrderStartDate;
    if (this.model.OrderStartDate !== null) {
      OrderDateStartToSearch = convertToUTCTime(setStartDate(this.model.OrderStartDate));
    }
    var OrderDateEndToSearch = this.model.OrderEndDate;
    if (this.model.OrderEndDate !== null) {
      OrderDateEndToSearch = convertToUTCTime(setEndDate(this.model.OrderEndDate));
    }
    const result: any = await this.salesDashboardService.getProductCategoryGroupByLevel(this.seller,
      OrderDateStartToSearch, OrderDateEndToSearch, productCategoryLevel);
    if (result && result.statusCode === 200) {
      if (result.lstResult.length === 0) {
        this.chartShowByProductCategoryGorup = false;
      } else {
        this.chartShowByProductCategoryGorup = true;
        if (result.lstResult.length <= 8) {
          //Nếu tổng số nhóm sản phẩm bé hơn 8 thì
          for (let i = 0; i < result.lstResult.length; i++) {
            this.current_list_color[i] = this.list_color[i];
            this.current_percent[i] = result.lstResult[i].Percent;
            this.current_money[i] = result.lstResult[i].Total;
            this.current_title[i] = result.lstResult[i].ProductCategoryName;
          }
          this.setDoughnut(this.current_percent, this.current_list_color, this.current_title);
        } else {
          //Nếu tổng số nhóm sản phẩm lớn hơn 8 thì
          let total_top_7 = 0;
          for (let i = 0; i < 8; i++) {
            if (i < 7) {
              this.current_list_color[i] = this.list_color[i];
              this.current_percent[i] = result.lstResult[i].Percent;
              this.current_title[i] = result.lstResult[i].ProductCategoryName;
              this.current_money[i] = result.lstResult[i].Total;
              total_top_7 += result.lstResult[i].Percent;
            } else if (i == 7) {
              this.current_list_color[i] = this.list_color[i];
              this.current_percent[i] = (100 - total_top_7).toFixed(1);
              this.current_money[i] = result.lstResult[i].Total;
              this.current_title[i] = "Khác";
            }
          }
          this.setDoughnut(this.current_percent, this.current_list_color, this.current_title);
        }
      }
    } else {
      this.err_server = true;
    }
    this.loading = false;
  }

  openOrgDialog() {
    this.dialogOrg = this.dialog.open(OrgSelectDialogComponent,
      {
        width: '500px',
        autoFocus: false,
        data: { selectedId: this.organizationId }
      });
      
      this.dialogOrg.afterClosed().subscribe(result => {
      if (result.chosenItem) {
        this.model.OrganizationId = result.selectedId;
        this.orgNameDisplay = result.selectedName;
      }
      this.getMonthsOrder();
    });
  }

  async changeDoughnutManager(productCategoryLevel: number) {
    this.loading = true;
    this.setDataDoughnutNull();
    var OrderDateStartToSearch = this.model.OrderStartDate;
    if (this.model.OrderStartDate !== null) {
      OrderDateStartToSearch = convertToUTCTime(setStartDate(this.model.OrderStartDate));
    }
    var OrderDateEndToSearch = this.model.OrderEndDate;
    if (this.model.OrderEndDate !== null) {
      OrderDateEndToSearch = convertToUTCTime(setEndDate(this.model.OrderEndDate));
    }

    const result: any = await this.salesDashboardService.getProductCategoryGroupByManager(this.userId,
      OrderDateStartToSearch, OrderDateEndToSearch, this.model.OrganizationId, productCategoryLevel);
    if (result && result.statusCode === 200) {
      if (result.lstResult.length === 0) {
        this.chartShowByProductCategoryGorup = false;
      } else {
        this.chartShowByProductCategoryGorup = true;
        if (result.lstResult.length <= 8) {
          //Nếu tổng số nhóm sản phẩm bé hơn 8 thì
          for (let i = 0; i < result.lstResult.length; i++) {
            this.current_list_color[i] = this.list_color[i];
            this.current_percent[i] = result.lstResult[i].Percent;
            this.current_money[i] = result.lstResult[i].Total;
            this.current_title[i] = result.lstResult[i].ProductCategoryName;
          }
          this.setDoughnut(this.current_percent, this.current_list_color, this.current_title);
        } else {
          //Nếu tổng số nhóm sản phẩm lớn hơn 8 thì
          let total_top_7 = 0;
          for (let i = 0; i < 8; i++) {
            if (i < 7) {
              this.current_list_color[i] = this.list_color[i];
              this.current_percent[i] = result.lstResult[i].Percent;
              this.current_title[i] = result.lstResult[i].ProductCategoryName;
              this.current_money[i] = result.lstResult[i].Total;
              total_top_7 += result.lstResult[i].Percent;
            } else if (i == 7) {
              this.current_list_color[i] = this.list_color[i];
              this.current_percent[i] = (100 - total_top_7).toFixed(1);
              this.current_money[i] = result.lstResult[i].Total;
              this.current_title[i] = "Khác";
            }
          }
          this.setDoughnut(this.current_percent, this.current_list_color, this.current_title);
        }
      }
    } else {
      this.err_server = true;
    }
    this.loading = false;
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function setStartDate(time: Date) {
  time.setHours(0);
  return time;
}

function setEndDate(time: Date) {
  time.setHours(23, 59, 59);

  return time;
}

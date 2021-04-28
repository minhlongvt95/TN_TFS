import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
// import { Chart } from 'angular-highcharts';
import { Chart, ChartData } from 'chart.js';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { NoteService } from '../../../shared/services/note.service';
import { GetPermission } from '../../../shared/permission/get-permission';

import { MessageService, ConfirmationService, SortEvent, MenuItem } from 'primeng/api';
import { Table } from 'primeng/table';
import { DatePipe } from '@angular/common';
import { ContractService } from '../../services/contract.service';
import * as moment from 'moment';
import 'moment/locale/pt-br';

interface ContractDashboardModel {
  code: string;
  name: string;
  value: string;
}

class Month {
  label: string;
  result: number;
}

interface Contract {
  contractId: string,
  contractCode: string,
  quoteCode: string,
  customerId: string,
  effectiveDate: string,
  valueContract: string,
  employeeId: string,
  contractDescription: string,
  contractTimeUnit: string,
  statusId: string,
  createdDate: string,
  createdById: string,
  nameCustomer: string,
  nameEmployee: string,
  nameStatus: string,
  nameCreateBy: string;
  statusCode: string;
  backgroupStatus: string;
  dayLeft: number;
}

@Component({
  selector: 'app-contract-dashboard',
  templateUrl: './contract-dashboard.component.html',
  styleUrls: ['./contract-dashboard.component.css']
})
export class ContractDashboardComponent implements OnInit {
  innerWidth: number = 0; //number window size first
  emitParamUrl: any;
  @ViewChild('myTable') myTable: Table;
  
  @ViewChild('canvas_status') canvas_status: ElementRef;
  @ViewChild('canvas_month') canvas_month: ElementRef;

  chart_status: any;
  chart_month: any;
  loading: boolean = false;
  cols: any;
  selectedColumns: any[];
  selectedColumnsWorking: any[];
  selectedColumnsExpiredDate: any[];
  //Biến lưu giá trị trả về
  listContractNewStatus: Array<Contract> = [];
  listContractWorkimg: Array<Contract> = [];
  listContractExpiredDate: Array<Contract> = [];
  listValueOfStatus: Array<ContractDashboardModel> = [];
  listValueOfMonth: Array<ContractDashboardModel> = [];
  data: any;
  dataFollowMonth: any;
  options: any;
  numberMonth: number = 6;
  currentTimeString = this.convertTimeToString();
  isShow = true;
  pieChartLabelStatus: Array<string> = [];
  pieCharValueStatus: Array<number> = [];
  pieChartLabelFollowMonth: Array<string> = [];
  pieCharValueFollowMonth: Array<number> = [];
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  auth = JSON.parse(localStorage.getItem('auth'));
  contractCode: string = '';
  listMonth: Array<Month> = [
    {
      label: '3 Tháng', result: 3
    },
    {
      label: '6 Tháng', result: 6
    },
    {
      label: '12 Tháng', result: 12
    },
  ];
  selectedMonth: Month = this.listMonth[1];

  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private datePipe: DatePipe,
    private confirmationService: ConfirmationService,
    private contractService: ContractService
  ) {
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "sal/sales/contract-dashboard/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      this.setTable();
      this.getMasterData();
    }
  }

  setTable() {
    this.cols = [
      { field: 'contractCode', header: 'Mã hợp đồng', textAlign: 'left', display: 'table-cell', width: '160px' },
      { field: 'nameCustomer', header: 'Khách hàng', textAlign: 'left', display: 'table-cell', width: '160px'  },
      { field: 'effectiveDate', header: 'Ngày hiệu lực', textAlign: 'right', display: 'table-cell', width: '120px' },
      { field: 'contractTime', header: 'Thời gian hiệu lực', textAlign: 'right', display: 'table-cell', width: '150px' },
      { field: 'dayLeft', header: 'Số ngày còn lại', textAlign: 'right', display: 'table-cell', width: '130px' },
      { field: 'expiredDate', header: 'Ngày hết hạn', textAlign: 'right', display: 'table-cell', width: '120px' },
      { field: 'valueContract', header: 'Giá trị hợp đồng', textAlign: 'right', display: 'table-cell', width: '150px' },
      { field: 'nameEmployee', header: 'Nhân viên bán hàng', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'contractDescription', header: 'Diễn giải', textAlign: 'right', display: 'table-cell', width: '200px'  },
      { field: 'nameStatus', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '160px' },
      { field: 'createdDate', header: 'Ngày tạo', textAlign: 'right', display: 'table-cell', width: '120px'  },
      { field: 'nameCreateBy', header: 'Người tạo', textAlign: 'left', display: 'table-cell', width: '160px'  },
    ];
    this.selectedColumns = this.cols.filter(e => e.field == "contractCode" || e.field == "nameCustomer" || e.field == "effectiveDate"
      || e.field == "valueContract" || e.field == "nameEmployee" || e.field == "nameStatus");

    this.selectedColumnsWorking = this.cols.filter(e => e.field == "contractCode" || e.field == "nameCustomer" || e.field == "effectiveDate"
      || e.field == "valueContract" || e.field == "nameEmployee" || e.field == "nameStatus");

    this.selectedColumnsExpiredDate = this.cols.filter(e => e.field == "contractCode" || e.field == "nameCustomer" || e.field == "effectiveDate"
      || e.field == "dayLeft" || e.field == "expiredDate"  || e.field == "valueContract"
      || e.field == "nameEmployee" || e.field == "nameStatus");
  }

  getMasterData() {
    this.loading = true;
    this.contractService.getMasterDataDashboardContract(this.numberMonth, this.contractCode, this.auth.UserId).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.resetPieChart();
        this.listContractNewStatus = result.listContractNew;
        this.listContractWorkimg = result.listContractWorking;
        this.listContractExpiredDate = result.listContractExpiredDate;
        this.listValueOfStatus = result.listValueOfStatus;
        this.listValueOfMonth = result.listValueOfMonth;
        this.pieChartLabelStatus = this.listValueOfStatus.map(c => c.name);
        this.listValueOfStatus.forEach(item => {
          let temp = parseFloat(item.value.replace(/,/g, ''));
          this.pieCharValueStatus.push(temp);
        });

        for(let i = this.listValueOfMonth.length - 1; i >= 0; i--){
          this.pieChartLabelFollowMonth.push(this.listValueOfMonth[i].name);
          let temp = parseFloat(this.listValueOfMonth[i].value.replace(/,/g, ''));
          this.pieCharValueFollowMonth.push(temp);
        }     
        this.setChartContractFollowStatus(this.pieChartLabelStatus ,this.pieCharValueStatus);
        this.setChartContractFollowMonth(this.pieCharValueFollowMonth);
        this.handleBackGroundColorForStatus();
      } else {
      }
    });
  }
  
  handleBackGroundColorForStatus() {
    this.listContractWorkimg.forEach(item => {
      switch (item.statusCode) {
        case "APPR"://Xac nhan
          item.backgroupStatus = "#34c759";
          break;
        case "HTH"://dong
          item.backgroupStatus = "#5ac8fa";
          break;
        case "MOI"://nhap
          item.backgroupStatus = "#78a9ff";
          break;
        case "HUY"://huy
          item.backgroupStatus = "#ff2d55";
          break;
        case "DTH"://huy
          item.backgroupStatus = "#2c96c7";
          break;
        default:
          break;
      }
    });
    this.listContractNewStatus.forEach(item => {
      switch (item.statusCode) {
        case "APPR"://Xac nhan
          item.backgroupStatus = "#34c759";
          break;
        case "HTH"://dong
          item.backgroupStatus = "#5ac8fa";
          break;
        case "MOI"://nhap
          item.backgroupStatus = "#78a9ff";
          break;
        case "HUY"://huy
          item.backgroupStatus = "#ff2d55";
          break;
        case "DTH"://huy
          item.backgroupStatus = "#2c96c7";
          break;
        default:
          break;
      }
    });
    this.listContractExpiredDate.forEach(item => {
      switch (item.statusCode) {
        case "APPR"://Xac nhan
          item.backgroupStatus = "#34c759";
          break;
        case "HTH"://dong
          item.backgroupStatus = "#5ac8fa";
          break;
        case "MOI"://nhap
          item.backgroupStatus = "#78a9ff";
          break;
        case "HUY"://huy
          item.backgroupStatus = "#ff2d55";
          break;
        case "DTH"://huy
          item.backgroupStatus = "#2c96c7";
          break;
        default:
          break;
      }

      switch (item.contractTimeUnit) {
        case 'DAY':
          item.contractTimeUnit = 'Ngày';
          break;
        case 'MONTH':
          item.contractTimeUnit = 'Tháng';
          break;
        case 'YEAR':
          item.contractTimeUnit = 'Năm';
          break;
      }
    });
  }
  
  resetPieChart(){
    this.pieCharValueStatus = [];
    this.pieCharValueStatus = [];
    this.pieChartLabelFollowMonth = [];
    this.pieCharValueFollowMonth = [];
  }

  changeChooseMonth(e){
    if(this.selectedColumns){
      this.numberMonth = this.selectedMonth.result;
    }
    this.getMasterData();
  }

  setChartContractFollowStatus(current_list_name, current_list_revenue){
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

  setChartContractFollowMonth(current_list_revenue){
    setTimeout(() => {
      if (this.chart_month) {
        this.chart_month.destroy();
      }
      // const maxwidthofLabel = 19 - current_list_name.length;
      if (this.canvas_month != null) {
        this.chart_month = new Chart(this.canvas_month.nativeElement.getContext('2d'), {
          type: 'bar',
          data: {
            labels: this.pieChartLabelFollowMonth,
            datasets: [
              {
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
  
  goToDetail(rowData: any) {
    this.router.navigate(['/sales/contract-detail', { contractId: rowData }]);
  }

  dateFieldFormat: string = 'DD/MM/YYYY';

  sortColumnInList(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      /**Customize sort date */
      if (event.field == 'createdDate') {
        const date1 = moment(value1, this.dateFieldFormat);
        const date2 = moment(value2, this.dateFieldFormat);

        let result: number = -1;
        if (moment(date2).isBefore(date1, 'day')) { result = 1; }

        return result * event.order;
      }
      /**End */

      let result = null;

      if (value1 == null && value2 != null)
        result = -1;
      else if (value1 != null && value2 == null)
        result = 1;
      else if (value1 == null && value2 == null)
        result = 0;
      else if (typeof value1 === 'string' && typeof value2 === 'string')
        result = value1.localeCompare(value2);
      else
        result = (value1 < value2) ? -1 : (value1 > value2) ? 1 : 0;

      return (event.order * result);
    });
  }

  goToContractList(){
    this.router.navigate(['/sales/contract-list']);
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

}

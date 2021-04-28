import { Component, OnInit } from '@angular/core';
import { Chart, ChartData } from 'chart.js';
import * as $ from 'jquery';
import { CustomerService } from '../../services/customer.service';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';

class Customer {
  customerId: string;
  customerName: string;
  contactId: string;
  customerPhone: string;
  customerEmail: string;
  picName: string;
  picContactId: string;
  totalSaleValue: number;
  statusName: string;
  backgroupStatus: string;
}

class investmentChartItem{
  public categoryId: string;
  public categoryName: string;
  public value: number;
  public percentValue: number;
}

class funnelChartPotentialDasboardModel{
  public totalPotentialCustomerConverted: number;
  public totalLead: number;
  public totalQuote: number;
  public totalCustomerOrder: number;
  public percentPotentialToLead: string;
  public percentLeadToQuote: string;
  public percentQuoteToOrder: string;
  public isFetchedData: boolean;
  constructor(){
    this.totalPotentialCustomerConverted = 0;
    this.totalLead = 0;
    this.totalQuote = 0;
    this.totalCustomerOrder = 0;
    this.percentPotentialToLead = '';
    this.percentLeadToQuote = '';
    this.percentQuoteToOrder = '';
    this.isFetchedData = false;
  }
}

@Component({
  selector: 'app-potential-customer-dasboard',
  templateUrl: './potential-customer-dasboard.component.html',
  styleUrls: ['./potential-customer-dasboard.component.css']
})
export class PotentialCustomerDasboardComponent implements OnInit {
  loading: boolean = false;
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  auth: any = JSON.parse(localStorage.getItem("auth"));

  /* Action*/
  actionAdd: boolean = true;
  /*END*/

  /*Declare chart*/
  doughnutData: ChartData = {
    datasets: [],
    labels: []
  }
  barChartData1: ChartData = {
    datasets: [],
    labels: []
  }
  barChartData2: ChartData = {
    datasets: [],
    labels: []
  }
  doughnutChart: Chart;
  barChart: Chart;
  /*End*/
  paramUrl: any;
  currentTimeString = this.convertTimeToString();
  customerName = '';
  colsCus: any;
  //colsCusIdentification: any;
  //colsCusFree: any;
  listCusIsNewest: Array<Customer> = [];
  selectedTableCusNew: Customer;
  // listCusIsNewBought: Array<Customer> = [];
  selectedTableCusNewBought: Customer;
  selectedTableCusIden: Customer;
  selectedTableCusFree: Customer;
  //colsCusTopRevenue: any;
  listCusTopRevenueInMonth: Array<Customer> = [];
  selectedTableCusTopRevenue: Customer;
  listCusFollowProduct: Array<any> = [];
  listCusIdentification: Array<Customer> = [];
  listCusFree: Array<Customer> = [];
  listTopPic: Array<any> = [];
  listCusCreatedInThisYear: Array<any> = [];


  //master data
  listInvestmentFundDasboard: Array<investmentChartItem> = [];
  listPotentialcustomer: Array<Customer> = [];
  listPotentialcustomerConverted: Array<Customer> = [];
  funnelChartPotentialDasboardModel: funnelChartPotentialDasboardModel = new funnelChartPotentialDasboardModel();
  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private customerService: CustomerService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService
  ) {
    this.translate.setDefaultLang('vi');
  }

  ngOnInit(): void {
    //Check permission
    let resource = "crm/customer/potential-customer-dashboard";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.setTable();

      this.getDashBoardCustomer();
    }
  }

  setTable() {
    this.colsCus = [
      { field: 'customerName', header: 'Họ và Tên', width: '25%', textAlign: 'left' },
      { field: 'customerPhone', header: 'Số điện thoại', width: '20%', textAlign: 'right' },
      { field: 'customerEmail', header: 'Email', width: '25%', textAlign: 'left' },
      { field: 'picName', header: 'Người phụ trách', width: '25%', textAlign: 'left' },
    ];
  }

  async getDashBoardCustomer() {
    this.loading = true;
    let result: any = await this.customerService.getDataDashboardPotentialCustomer(this.auth.UserId);

    this.loading = false;
    if (result.statusCode == 200) {
      this.listInvestmentFundDasboard = result.listInvestmentFundDasboard || [];
      this.listPotentialcustomer = result.topNewestCustomer || [];
      this.listPotentialcustomerConverted = result.topNewestCustomerConverted || [];
      this.funnelChartPotentialDasboardModel = result.funnelChartPotentialDasboardModel;
      this.funnelChartPotentialDasboardModel.isFetchedData = true;
      setTimeout(() => {
        this.createDoughnutChart(this.listInvestmentFundDasboard);
        // this.createBarChart(this.listTopPic, 1);
        // this.createBarChart(this.listCusCreatedInThisYear, 2);
      }, 0);
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }

  }

  /*Event nhấn phím Enter khi đang forcus vào ô tìm kiếm theo tên khách hàng*/
  onKeydown($event: KeyboardEvent) {
    if ($event.key === 'Enter') {
      this.getDashBoardCustomer();
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
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

  gotoCreate() {
    this.router.navigate(['/customer/potential-customer-create']);
  }

  gotoList() {
    this.router.navigate(['/customer/potential-customer-list']);
  }

  gotoListConverted() {
    this.router.navigate(['/customer/potential-customer-list', {mode : 'converted'}]);
  }

  onViewCustomerDetail(rowData:Customer) {
    this.router.navigate(['/customer/potential-customer-detail', { customerId: rowData.customerId}])
  }

  onViewEmployeeDetail(employeeId, contactId) {
    this.router.navigate(['/employee/detail', { employeeId: employeeId, contactId: contactId }]);
  }

  // Tạo Doughnut Chart
  createDoughnutChart(listInvestmentFundDasboard: Array<investmentChartItem>) {
    let _sum = 0;
    listInvestmentFundDasboard.forEach(e => _sum += e.value);
    const color = ["#ff6384", "#ffcd56", "#dd4895", "#36a2eb", "#8beb7a", "#4bc0c0", "#ff9f40", "#e5e5e5"];
    this.doughnutChart = new Chart($('#doughnutChart'), {
      type: 'doughnut',
      data:
      {
        datasets: [{
          data: listInvestmentFundDasboard.map(m => m.value),
          backgroundColor: color.slice(0, listInvestmentFundDasboard.length),
          borderWidth: 1
        }],
        labels: listInvestmentFundDasboard.map(m => m.categoryName)
      },
      options: {
        responsive: true,
        legend: {
          display: false
        },
        tooltips: {
          callbacks: {
            label: function (tooltipItems, data) {
              return data.labels[tooltipItems.index] + ': ' + (<number>data.datasets[tooltipItems.datasetIndex].data[tooltipItems.index] / _sum * 100).toFixed(2) + '%';
            }
          }
        }
      }
    });
    this.doughnutData = this.doughnutChart.data;
  }

  // Tạo Bar Chart
  createBarChart(array: Array<any>, element: number) {
    let color = ['#6d98e7', '#6d98e7', '#6d98e7', '#6d98e7', '#6d98e7', '#6d98e7', '#6d98e7', '#6d98e7', '#6d98e7', '#6d98e7', '#6d98e7', '#6d98e7'];
    if (element == 1) {
      const labels = Array.from(new Set(array.map(m => m.personInChargeId)));
      const maxwidthofLabel = 13 - (labels.length > 6 ? 6 : labels.length);
      let _data = [];
      labels.forEach(l => {
        _data.push({
          count: array.filter(a => a.personInChargeId == l).length,
          name: this.formatLabel(array.find(a => a.personInChargeId == l).picName, maxwidthofLabel)
        })
      });
      _data.sort((a: any, b: any) => {
        return b.count - a.count;
      })
      let step = 2;
      if (_data.length > 0) {
        if (_data[0].count % 10 != 0) {
          step = Math.round(_data[0].count / 10) * 10 / 2;
        } else step = _data[0].count / 2;
      }
      _data = _data.slice(0, 6);
      this.barChart = new Chart($('#barChart1'), {
        type: "bar",
        data: {
          datasets: [{
            data: _data.map(m => m.count),
            backgroundColor: color.slice(0, _data.length),
          }],
          labels: _data.map(m => m.name),
        },
        options: {
          layout: {
            padding: {
              right: 10
            }
          },
          scales: {
            yAxes: [{
              ticks: {
                beginAtZero: true,
                min: 0,
                stepSize: step == 0 ? 2 : step,
              }
            }],
            xAxes: [{
              barPercentage: 0.5,
              gridLines: {
                display: false
              },
              ticks: {
                showLabelBackdrop: true
              }
            }]
          },
          legend: {
            display: false
          },
          responsive: true,

          tooltips: {
            titleFontSize: 0,
            callbacks: {
              label: function (tooltipItems, data) {
                return "Số khách hàng phụ trách: " + data.datasets[0].data[tooltipItems.index].toString();
              }
            }
          }
        }
      });
      this.barChartData1 = this.barChart.data;
      if (this.barChartData1.labels.length == 0) {
        $('#barChart1').hide();
      } else {
        $('#barChart1').show();
      }
    }
    else {
      let labels = Array.from(new Set(array.map(m => m.maximumDebtDays)));
      for (let i = 1; i <= Math.max(...labels); i++) {
        if (!labels.includes(i)) labels.push(i);
      }
      labels.sort((a: any, b: any) => {
        return a - b;
      })
      let _data = [];
      const stringLabel = (labels.length > 6) ? "T" : "Tháng "
      labels.forEach(l => {
        _data.push({
          count: array.filter(a => a.maximumDebtDays == l).length,
          name: stringLabel + l.toString()
        })
      });
      let step = 2;
      step = Math.round(Math.max(..._data.map(m => m.count)) / 10) * 10 / 2;
      if (_data.length > 0) {
        if (Math.max(..._data.map(m => m.count)) % 10 != 0) {
        } else step = Math.max(..._data.map(m => m.count)) / 2;
      }
      this.barChart = new Chart($('#barChart2'), {
        type: "bar",
        data: {
          datasets: [{
            data: _data.map(m => m.count),
            backgroundColor: color.slice(0, _data.length)
          }],
          labels: _data.map(m => m.name),
        },
        options: {
          layout: {
            padding: {
              right: (labels.length <= 11) ? 10 : 5
            }
          },
          scales: {
            yAxes: [{
              ticks: {
                beginAtZero: true,
                min: 0,
                stepSize: step
              }
            }],
            xAxes: [{
              barPercentage: 0.5,
              gridLines: {
                display: false
              }
            }]
          },
          legend: {
            display: false
          },
          responsive: true,
          tooltips: {
            titleFontSize: 0,
            callbacks: {
              label: function (tooltipItems, data) {
                return "Số khách hàng được tạo: " + data.datasets[0].data[tooltipItems.index].toString();
              }
            }
          }
        }
      });
      this.barChartData2 = this.barChart.data;
      if (this.barChartData2.labels.length == 0) {
        $('#barChart2').hide();
      } else {
        $('#barChart2').show();
      }
    }
  }

  // Format Label bar chart wrapping
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

}

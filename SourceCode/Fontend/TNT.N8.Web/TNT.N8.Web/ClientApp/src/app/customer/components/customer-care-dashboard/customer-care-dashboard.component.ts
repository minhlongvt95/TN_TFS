import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Chart, ChartData } from 'chart.js';
import * as $ from 'jquery';
import { Router } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { CustomerCareService } from '../../services/customer-care.service';
import { NgxLoadingComponent, ngxLoadingAnimationTypes } from 'ngx-loading';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-customer-care-dashboard',
  templateUrl: './customer-care-dashboard.component.html',
  styleUrls: ['./customer-care-dashboard.component.css']
})
export class CustomerCareDashboardComponent implements OnInit {
  @ViewChild('canvas_2') canvas_2: ElementRef;
  current_month: number = (new Date()).getMonth() + 1;
  current_year: number = (new Date()).getFullYear();
  month: number = this.current_month;
  year: number = this.current_year;

  defaultAvatar: string = '../../../../assets/images/no-avatar.png';

  /*BAR CHART*/
  barChart: Chart;
  barChartData: ChartData = {
    datasets: [],
    labels: []
  }
  /*END*/
  listCusBidth: any[] = [];
  listCustomerNewCS: any[] = [];
  listCustomerCareActive = [];
  listTotal: any[] = [];
  careTotal: number = 0;
  listChar: any[] = [];
  listCharTitle: any[] = [];
  listCharTotalPro: any[] = [];
  listCharTotalCS: any[] = [];
  actionAdd: boolean = true;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  constructor(
    private getPermission: GetPermission,
    private customerCareService: CustomerCareService,
    private messageService: MessageService,
    private router: Router) { }

  // Loading config
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
  loading: boolean = false;

  ngOnInit() {
    //Check permission
    let resource = "crm/customer/care-dashboard/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.messageService.add({ severity: 'warn', summary: 'Thông báo', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' });
      this.router.navigate(['/home']);
    }
    else {
      this.loading = true;
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.getCustomerBirthDay();
      this.getTotalInteractive();
      this.getCustomerNewCS();
      this.getCustomerCareActive();
      this.getCharCustomerCS();
    }
  }

  goToOrder(customerId, contactId) {
    this.router.navigate(['/order/create', { customerId: customerId, contactId: contactId }]);
  }

  setBar(current_list_name, current_list_revenue, current_list_revenue2) {
    setTimeout(() => {
      if (this.barChart) {
        this.barChart.destroy();
      }
      const maxwidthofLabel = 19 - current_list_name.length;
      if (this.canvas_2 != null) {

        this.barChart = new Chart(this.canvas_2.nativeElement.getContext('2d'), {
          type: 'bar',
          data: {
            labels: current_list_name.map(t => this.formatLabel(t, maxwidthofLabel)),
            datasets: [
              {
                label: "Doanh thu (VND)",
                data: current_list_revenue,
                fill: false,
                backgroundColor: "#6d98e7",
                borderWidth: 1
              },
              {
                label: "Doanh thu (VND)",
                data: current_list_revenue2,
                fill: false,
                backgroundColor: "#FE9A2E",
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
        }

        )
      }
    }, 200);

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
  goToListCustomerCare() {
    this.router.navigate(['/customer/care-list']);
  }
  goToListCustomer(){
    this.router.navigate(['/customer/list']);
  }
  gotoViewCustomer(customerId, contactId) {
    this.router.navigate(['/customer/detail', { customerId: customerId, contactId: contactId }]);
  }
  getCustomerBirthDay() {
    this.loading = true;
    this.customerCareService.getCustomerBirthDay().subscribe(response => {
      const result = <any>response;
      this.listCusBidth = result.listBirthDay;
      this.loading = false;
    });
  }

  getCustomerNewCS() {
    this.loading = true;
    this.customerCareService.getCustomerNewCS().subscribe(response => {
      const result = <any>response;
      this.listCustomerNewCS = result.listCustomerNewOrder;
      this.loading = false;
    });
  }

  getCharCustomerCS() {
    this.loading = true;
    this.customerCareService.getCharCustomerCS().subscribe(response => {
      const result = <any>response;
      this.listChar = result.listChar;
      for (var i = 0; i < result.listChar.length; i++) {
        this.listCharTitle.push(result.listChar[i].month);
        this.listCharTotalPro.push(result.listChar[i].totalCustomerProgram);
        this.listCharTotalCS.push(result.listChar[i].totalCustomerCSKH);
      }
      this.setBar(this.listCharTitle, this.listCharTotalPro, this.listCharTotalCS);
      this.loading = false;
    });
  }

  getCustomerCareActive() {
    this.loading = true;
    this.customerCareService.getCustomerCareActive().subscribe(response => {
      const result = <any>response;
      this.listCustomerCareActive = result.listCategoryCare;
      this.loading = false;
    });
  }

  getTotalInteractive() {
    this.loading = true;
    this.customerCareService.getTotalInteractive(this.month, this.year).subscribe(response => {
      const result = <any>response;
      this.listTotal = result.listCate;
      this.careTotal = result.totalCare;
      this.loading = false;
    });
  }

  changeMonth(handle: string) {
    if (handle == "left") {
      if (this.month == 1) {
        this.year--;
        this.month = 13;
      }
      this.month--;
    } else if (handle == "right") {
      if (this.year < this.current_year) {
        if (this.month == 12) {
          this.year++;
          this.month = 0;
        }
        this.month++;
      } else if (this.year == this.current_year) {
        if (this.month < this.current_month) {
          this.month++;
        }
      }
    }
    this.getTotalInteractive();
  }

  createBarChart() {
    let color = ['#6d98e7','#6d98e7','#6d98e7','#6d98e7','#6d98e7','#6d98e7','#6d98e7','#6d98e7','#6d98e7','#6d98e7','#6d98e7','#6d98e7'];
    this.barChart = new Chart($('#barChart'), {
      type: "bar",
      data: {
        datasets: [{
          data: [],
          backgroundColor: [],
        }],
        labels: [],
      },
    });
  }

  gotoCreate() {
    this.router.navigate(['/customer/care-create']);
  }

}

import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Chart } from 'angular-highcharts';
import { Router } from '@angular/router';
import { QuoteService } from '../../services/quote.service';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission';
import { forkJoin } from 'rxjs';
import { MessageService } from 'primeng/api';

interface Month {
  label: string,
  result: number
}

interface Quote {
  quoteId: string,
  quoteName: string,
  quoteCode: string,
  status: string,
  amount: number,
  sendQuoteDate: string,
  customerName: string,
  employeeName: string,
  intendedQuoteDate: string
}

interface Lead {
  leadId: string,
  contactId: string,
  personInChargeId: string,
  personInChargeName: string,
  email: string,
  phone: string,
  leadFirstName: string,
  leadLastName: string,
  leadName: string
}

@Component({
  selector: 'app-quote-dashboard',
  templateUrl: './customer-quote-dashboard.component.html',
  styleUrls: ['./customer-quote-dashboard.component.css'],
  providers: [QuoteService]
})
export class CustomerQuoteDashboardComponent implements OnInit {
  /*Khai bao bien*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  userPermission: any = localStorage.getItem("UserPermission").split(",");
  loading: boolean = false;
  currentDate: Date = new Date();
  currentMonth: number = (new Date()).getMonth() + 1;
  currentYear: number = (new Date()).getFullYear();
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
  monthQuoteDashBoard: number;
  yearQuoteDashBoard: number;
  chart: Chart;
  chartPie: Chart;
  showPie: boolean = false;
  data: any;
  options: any;
  selectedColumns: any[];
  selectedColumnsWeek: any[];

  //Khởi tạo convertRate//
  convertOutOfDate: any = { countNew: 0, countInProgress: 0, countQuotation: 0, countWaiting: 0, countUnfollowed: 0, countSigned: 0 }
  convertInWeek: any = { countNew: 0, countInProgress: 0, countQuotation: 0, countWaiting: 0, countUnfollowed: 0, countSigned: 0 }
  convertEmploy: any = { countNew: 0, countInProgress: 0, countQuotation: 0, countWaiting: 0, countUnfollowed: 0, countSigned: 0 }
  convertRate: any = { countNew: 0, countInProgress: 0, countQuotation: 0, countWaiting: 0, countUnfollowed: 0, countSigned: 0 }
  colsOutOfDate: any;
  colsOutOfWeek: any;
  listOutOfDate: Array<Quote> = [];
  listInWeek: Array<Quote> = [];
  colsListLead: any;
  listLead: Array<Lead> = [];
  colsListQuoteApproval: any;
  listQuoteApproval: Array<any> = [];
  pipe = new DatePipe('en-US');
  monthQuote: number;
  yearQuote: number;

  //totalQuote: number;
  newRate: any;
  inProgressRate: any;
  quotationRate: any;
  waitingRate: any;
  unfollowRate: any;
  signedRate: any;
  searchName: string;
  sumOutOfDate: number;
  sumInWeek: number;
  sumEmploy: number;
  sumConvertRate: number = 0;
  sumConvertRatePie: number = 0;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  /*Ket thuc*/

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  /*Khoi tao Constructor*/
  constructor(
    private translate: TranslateService,
    private service: QuoteService,
    private getPermission: GetPermission,
    private messageService: MessageService,
    private router: Router) {
    translate.setDefaultLang('vi');
  }
  /*Ket thuc*/

  /*Link den trang tao bao gia*/
  onGoCreateQuote() {
    this.router.navigate(['/customer/quote-create']);
  }

  /*Link den trang tim kiem bao gia voi dieu kien tim kiem: Bao gia qua han*/
  onGoSearchQuoteOutOfDate() {
    this.router.navigate(['/customer/quote-list', { mode: 'OutOfDate' }]);
  }

  /*Link den trang tim kiem bao gia voi dieu kien tim kiem: Bao gia phai hoan thanh trong tuan*/
  onGoSearchQuoteInWeek() {
    this.router.navigate(['/customer/quote-list', { mode: 'InWeek' }]);
  }

  /*Link den trang tim kiem Lead*/
  onGoSearchLead() {
    this.router.navigate(['/customer/list']);
  }

  /*Link den trang detail Lead*/
  onViewLeadDetail(leadId, contactId) {
    this.router.navigate(['/customer/detail', { customerId: leadId, contactId: contactId }]);
  }

  /*Link den trang detail Lead*/
  onViewQuoteDetail(id: string) {
    this.router.navigate(['/customer/quote-detail', { quoteId: id }]);
  }

  /*Link đến trang list báo giá chờ phê duyệt*/
  onGoSearchQuoteApproval() {
    this.router.navigate(['/customer/quote-approval']);
  }

  /*Ham khoi tao khi page load*/
  ngOnInit() {
    //Check permission
    let resource = "crm/customer/quote-dashboard";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    }
    else {
      this.loading = true;
      const now = Date.now();
      this.monthQuote = parseInt(this.pipe.transform(now, 'MM'));
      this.yearQuote = parseInt(this.pipe.transform(now, 'yyyy'));

      this.colsOutOfDate = [
        { field: 'quoteName', header: 'Tên báo giá', textAlign: 'left', display: 'table-cell', width : '15px'  },
        { field: 'amount', header: 'Trị giá báo giá', textAlign: 'right', display: 'table-cell', width : '15px' },
        { field: 'status', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width : '15px' },
        { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left', display: 'table-cell', width : '15px' },
        { field: 'intendedQuoteDate', header: 'Ngày dự kiến', textAlign: 'right', display: 'table-cell', width : '15px' },        
      ];
      this.selectedColumns = [
        { field: 'quoteName', header: 'Tên báo giá', textAlign: 'left', display: 'table-cell', width : '15px'  },
        { field: 'amount', header: 'Trị giá báo giá', textAlign: 'right', display: 'table-cell', width : '15px' },
        { field: 'status', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width : '15px' },        
      ];;

      this.colsOutOfWeek = [
        { field: 'quoteName', header: 'Tên báo giá', textAlign: 'left', display: 'table-cell', width : '15px'  },
        { field: 'amount', header: 'Trị giá báo giá', textAlign: 'right', display: 'table-cell', width : '15px' },
        { field: 'status', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width : '15px' },
        { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left', display: 'table-cell', width : '15px' },
        { field: 'intendedQuoteDate', header: 'Ngày dự kiến', textAlign: 'right', display: 'table-cell', width : '15px' },        
      ];
      this.selectedColumnsWeek = [
        { field: 'quoteName', header: 'Tên báo giá', textAlign: 'left', display: 'table-cell', width : '15px'  },
        { field: 'amount', header: 'Trị giá báo giá', textAlign: 'right', display: 'table-cell', width : '15px' },
        { field: 'status', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width : '15px' },        
      ];;

      this.colsListLead = [
        { field: 'leadName', header: 'Họ tên', textAlign: 'left' },
        { field: 'phone', header: 'Số điện thoại', textAlign: 'right' },
        { field: 'email', header: 'Email', textAlign: 'left' },
        { field: 'personInChargeName', header: 'Nhân viên bán hàng', textAlign: 'left' },
      ];

      this.colsListQuoteApproval = [
        { field: 'quoteCode', header: 'Mã báo giá', textAlign: 'left', display: 'table-cell' },
        { field: 'customerName', header: 'Đối tượng', textAlign: 'left', display: 'table-cell' },
        { field: 'quoteDate', header: 'Ngày đặt hàng', textAlign: 'right', display: 'table-cell' },
        { field: 'amount', header: 'Tổng giá trị', textAlign: 'right', display: 'table-cell' }
      ];

      const getTop3QuotesOverdue = this.service.GetTop3QuotesOverdue(this.auth.EmployeeId);
      const getTop3WeekQuotesOverdue = this.service.GetTop3WeekQuotesOverdue(this.auth.EmployeeId);
      const getTop3PotentialCustomers = this.service.GetTop3PotentialCustomers(this.auth.EmployeeId);
      const getListQuoteApproval = this.service.searchQuoteApproval(null, null, null, [], false, false);

      const arrayRequest = [];
      arrayRequest.push(getTop3QuotesOverdue);
      arrayRequest.push(getTop3WeekQuotesOverdue);
      arrayRequest.push(getTop3PotentialCustomers);
      arrayRequest.push(getListQuoteApproval);

      forkJoin(arrayRequest).subscribe(results => {
        /*Danh sach top 3 báo giá quá hạn*/
        let resultGetTop3QuotesOverdue: any = results[0];
        this.convertOutOfDate = resultGetTop3QuotesOverdue.quoteList;
        this.sumOutOfDate = this.convertOutOfDate.length;
        this.listOutOfDate = [];
        resultGetTop3QuotesOverdue.quoteList.forEach(item => {
          item.quoteDate = item.quoteDate.substr(0, 10);
          item.sendQuoteDate = item.sendQuoteDate != null ? item.sendQuoteDate.substr(0, 10) : null;
          // item.intendedQuoteDate = item.intendedQuoteDate != null ? item.intendedQuoteDate.substr(0, 10) : null;
          this.listOutOfDate.push(item);
        });
        /*End*/

        /*Danh sach top 3 phải hoàn thành trong tuần */
        let resultGetTop3WeekQuotesOverdue: any = results[1];
        this.convertInWeek = resultGetTop3WeekQuotesOverdue.quoteList;
        this.sumInWeek = this.convertInWeek.length;
        this.listInWeek = [];
        resultGetTop3WeekQuotesOverdue.quoteList.forEach(item => {
          item.quoteDate = item.quoteDate.substr(0, 10);
          this.listInWeek.push(item);
        });
        /*End*/

        /*Danh sach top 3 khách hàng tiềm năng mới được giao phụ trách*/
        let resultGetTop3PotentialCustomers: any = results[2];
        this.convertEmploy = resultGetTop3PotentialCustomers.quoteList;
        this.sumEmploy = this.convertEmploy.length;
        this.listLead = [];
        resultGetTop3PotentialCustomers.quoteList.forEach(item => {
          this.listLead.push(item);
        });
        this.listLead.forEach(item => {
          item.leadName = ((item.leadFirstName != null ? item.leadFirstName.trim() : "") +
            " " + (item.leadLastName != null ? item.leadLastName.trim() : "")).trim();
          item.phone = item.phone != null ? item.phone : "";
          item.email = item.email != null ? item.email.trim() : "";
          item.personInChargeName = item.personInChargeName != null ? item.personInChargeName.trim() : "";
        });
        /*End*/

        /*Danh sách top 5 khách hàng tiềm năng mới được giao phụ trách*/
        let resultGetListQuoteApproval: any = results[3];
        //Chỉ lấy top 5 báo giá chờ phê duyệt
        this.listQuoteApproval = resultGetListQuoteApproval.listQuote.splice(0, 6);
        /*End*/

        this.loading = false;
      });

      let currentMonth = this.currentDate.getMonth() + 1;
      let indexMonth = this.listMonth.indexOf(this.listMonth.find(x => x.result == currentMonth));
      this.listMonth = this.listMonth.splice(0, indexMonth + 1);
      this.selectedMonth = {
        label: this.listMonth.find(x => x.result == currentMonth).label,
        result: this.listMonth.find(x => x.result == currentMonth).result
      }
      this.viewChar(currentMonth, this.currentYear);
      this.viewChartPie(currentMonth, this.currentYear);
    }
  }
  /*Ket thuc*/

  changeChooseMonth(value: Month) {
    let chooseMonth = value.result;
    this.viewChar(chooseMonth, this.currentYear);
    this.viewChartPie(chooseMonth, this.currentYear);
  }


  viewChartPie(month: number, year: number) {
    this.loading = true;
    this.service.getDataQuoteToPieChart(month, year).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let categoriesPieChart: Array<string> = result.categoriesPieChart;
        let dataPieChart: Array<number> = result.dataPieChart;
        if (categoriesPieChart.length > 0) {
          this.showPie = true;
        } else {
          this.showPie = false;
        }
        this.sumConvertRatePie = 0;
        dataPieChart.forEach(item=>{
          this.sumConvertRatePie = this.sumConvertRatePie + item;
        });

        this.data = {
          labels: categoriesPieChart,
          datasets: [
            {
              label: 'Doanh thu',
              data: dataPieChart,
              fill: false,
              borderColor: '#4bc0c0',
              backgroundColor: '#4bc0c0',
            }
          ]
        };

        this.options = {
          scales: {
            yAxes: [{
              ticks: {
                beginAtZero: true,
                callback: function(value, index, values) {
                  if((parseInt(value) >= 1000000 && parseInt(value) < 1000000000) || (parseInt(value) <= -1000000 && parseInt(value) > -1000000000)){
                    let tempValue = Number(value) / 1000000;
                    // return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    return tempValue.toString() + "M";
                  } else if(Number(value) >= 1000000000 || Number(value) <= -1000000000) {
                    let tempValue = Number(value) / 1000000000;
                    return tempValue.toString() + "B";
                  }else{
                    return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                  }
                }
              }
            }]
          },
          tooltips: {
            callbacks: {
              label: function (tooltipItems, data) {
                return data.datasets[tooltipItems.datasetIndex].label + ': ' + (<number>data.datasets[tooltipItems.datasetIndex].data[tooltipItems.index]).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
              }
            }
          }
        }
        // let chart = new Chart({
        //   chart: {
        //     height: 330,
        //     type: 'line'
        //   },
        //   title: {
        //     text: '',
        //   },
        //   credits: {
        //     enabled: false
        //   },
        //   xAxis: {
        //     categories: categoriesPieChart
        //   },
        //   yAxis: {
        //     title: {
        //       text: 'VND'
        //     },
        //     plotLines: [{
        //       value: 0,
        //       width: 1,
        //       color: '#808080'
        //     }]
        //   },
        //   tooltip: {
        //     valueSuffix: ''
        //   },
        //   series: [
        //     {
        //       name: 'Doanh thu',
        //       data: dataPieChart
        //     }
        //   ]
        // });

        // this.chartPie = chart;
      } else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  viewChar(month: number, year: number) {
    this.loading = true;
    this.service.GetDashBoardQuote(this.auth.EmployeeId, month, year).subscribe(result => {
      this.convertRate = result.dashBoardQuote;
      this.sumConvertRate = this.sumObject(this.convertRate);
      this.newRate = ((this.convertRate.countNew / this.sumConvertRate) * 100).toFixed(1) + "%";
      this.inProgressRate = ((this.convertRate.countInProgress / this.sumConvertRate) * 100).toFixed(1) + "%";
      this.quotationRate = ((this.convertRate.countDone / this.sumConvertRate) * 100).toFixed(1) + "%";
      this.waitingRate = ((this.convertRate.countWaiting / this.sumConvertRate) * 100).toFixed(1) + "%";
      this.unfollowRate = ((this.convertRate.countAbort / this.sumConvertRate) * 100).toFixed(1) + "%";
      this.signedRate = ((this.convertRate.countClose / this.sumConvertRate) * 100).toFixed(1) + "%";

      let chart = new Chart({
        chart: {
          type: 'pie',
          alignTicks: false,
          height: '50%'
        },
        title: {
          text: this.quotationRate,
          align: 'center',
          verticalAlign: 'middle',
          style: { 'color': '#6D98E7', 'font-size': '20px', 'line-height': '10px' }
        },
        credits: {
          enabled: false
        },
        tooltip: {
          pointFormat: '{series.name}: {point.percentage:.1f}%'
        },
        series: [{
          name: 'Tỉ lệ',
          innerSize: '70%',
          data: [
            {
              name: "Mới: " + this.convertRate.countNew,
              y: this.convertRate.countNew,
              color: '#FFC050'
            },
            {
              name: "Chờ duyệt: " + this.convertRate.countInProgress,
              y: this.convertRate.countInProgress,
              color: '#88D859'
            },
            {
              name: "Báo giá: " + this.convertRate.countDone,
              y: this.convertRate.countDone,
              color: '#6D98E7'
            },
            {
              name: "Đã duyệt: " + this.convertRate.countWaiting,
              y: this.convertRate.countWaiting,
              color: '#4e6ba5'
            },
            {
              name: "Huỷ: " + this.convertRate.countAbort,
              y: this.convertRate.countAbort,
              color: '#999999'
            },
            {
              name: "Đóng - Không trúng: " + this.convertRate.countClose,
              y: this.convertRate.countClose,
              color: '#F94E4E'
            }
          ]
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

      this.chart = chart;
      this.loading = false;
    });
  }

  /*Tinh tong cua cac properties trong 1 object*/
  sumObject(obj) {
    let sum = 0;
    for (var el in obj) {
      if (obj.hasOwnProperty(el)) {
        sum += parseFloat(obj[el]);
      }
    }
    return sum;
  }
  /*Ket thuc*/

}

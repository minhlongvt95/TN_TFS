import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import * as $ from 'jquery';
import { GetPermission } from '../../../shared/permission/get-permission';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { MessageService } from 'primeng/api';
import { SaleBiddingService } from '../../services/sale-bidding.service';
import * as Highcharts from 'highcharts';

interface SaleBidding {
  saleBiddingId: string,
  saleBiddingName: string,
  saleBiddingCode: string,
  startDate: string,
  bidStartDate: string,
  valueBid: string,
  ros: number,
  statusId: string,
  statusName: string,
  typeContractId: string,
  typeContractName: string,
  slowDay: number,
  effecTime: number
}
class Category {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}
@Component({
  selector: 'app-sale-bidding-dashboard',
  templateUrl: './sale-bidding-dashboard.component.html',
  styleUrls: ['./sale-bidding-dashboard.component.css']
})
export class SaleBiddingDashboardComponent implements OnInit {
  loading: boolean = false;
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  auth: any = JSON.parse(localStorage.getItem("auth"));
  /* Action*/
  actionAdd: boolean = true;
  /*END*/
  selectedSalerBidding: SaleBidding;
  currentTimeString = this.convertTimeToString();
  listSaleBiddingWaitApproval: Array<SaleBidding> = [];
  listSaleBiddingExpired: Array<SaleBidding> = [];
  listSaleBiddingSlowStartDate: Array<SaleBidding> = [];
  listSaleBiddingInWeek: Array<SaleBidding> = [];
  listSaleBiddingChart: Array<SaleBidding> = [];
  listTypeContact: Array<Category> = [];
  listStatus: Array<Category> = [];
  // column of listSaleBiddingWaitApproval
  colWaitApproval: any;
  selectedColWaiApproval: any;
  // column of listSaleBiddingExpired
  colExpired: any;
  selectedColExpired: any;
  isShow: boolean = true;
  // column of listSaleBiddingSlowStartDate
  colSlow: any;
  selectedColSlow: any;
  // column of listSaleBiddingInWeek
  colInWeek: any;
  selectedColInWeek: any;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  effectiveDate: string;
  saleBiddingName: string;
  dataChart: any;
  statusCho: any;

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private salebBiddingService: SaleBiddingService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService
  ) {
    this.translate.setDefaultLang('vi');
    Highcharts.setOptions({
      lang: {
        thousandsSep: ','
      }
    });
  }

  ngOnInit() {
    let resource = "crm/sale-bidding/dashboard";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      this.setTable();
      this.getDashBoardSaleBidding();
    }
  }

  setTable() {
    this.colWaitApproval = [
      { field: 'startDate', header: 'Ngày mở thầu', textAlign: 'right', width: '10%' },
      { field: 'saleBiddingCode', header: 'Mã gói thầu', textAlign: 'left', width: '10%' },
      { field: 'saleBiddingName', header: 'Tên gói thầu', textAlign: 'left', width: '20%' },
      { field: 'valueBid', header: 'Giá trị thầu', textAlign: 'right', width: '10%' },
      { field: 'ros', header: 'Tỉ suất lợi nhuận tạm tính(%)', textAlign: 'right', width: '15%' },
      { field: 'statusName', header: 'Trạng thái gói thầu', textAlign: 'center', width: '15%' },
      { field: 'typeContractName', header: 'Loại hợp đồng', textAlign: 'left', width: '15%' },
    ];
    // Defalut column table
    this.selectedColWaiApproval = this.colWaitApproval;

    this.colExpired = [
      { field: 'startDate', header: 'Ngày mở thầu', textAlign: 'right', width: '80px' },
      { field: 'bidStartDate', header: 'Ngày nộp thầu', textAlign: 'right', width: "80px" },
      { field: 'saleBiddingCode', header: 'Mã gói thầu', textAlign: 'left', width: '100px' },
      { field: 'saleBiddingName', header: 'Tên gói thầu', textAlign: 'left', width: '180px' },
      { field: 'valueBid', header: 'Giá trị thầu', textAlign: 'right', width: '80px' },
      { field: 'ros', header: 'Tỉ suất lợi nhuận tạm tính(%)', textAlign: 'right', width: '10%' },
      { field: 'statusName', header: 'Trạng thái gói thầu', textAlign: 'center', width: '140px' },
      { field: 'typeContractName', header: 'Loại hợp đồng', textAlign: 'left', width: '140px' },
      { field: 'effecTime', header: 'Thời gian hiệu lực', textAlign: 'right', width: '100px' },
    ];
    this.selectedColExpired = this.colExpired.filter(e => e.field == "bidStartDate" || e.field == "saleBiddingCode" || e.field == "saleBiddingName"
      || e.field == "valueBid" || e.field == "ros" || e.field == "statusName" || e.field == "effecTime");

    this.colSlow = [
      { field: 'startDate', header: 'Ngày mở thầu', textAlign: 'right', width: '80px' },
      { field: 'bidStartDate', header: 'Ngày nộp thầu', textAlign: 'right', width: "80px" },
      { field: 'saleBiddingCode', header: 'Mã gói thầu', textAlign: 'left', width: '100px' },
      { field: 'saleBiddingName', header: 'Tên gói thầu', textAlign: 'left', width: '180px' },
      { field: 'valueBid', header: 'Giá trị thầu', textAlign: 'right', width: '80px' },
      { field: 'ros', header: 'Tỉ suất lợi nhuận tạm tính(%)', textAlign: 'right', width: '10%' },
      { field: 'statusName', header: 'Trạng thái gói thầu', textAlign: 'center', width: '140px' },
      { field: 'typeContractName', header: 'Loại hợp đồng', textAlign: 'left', width: '140px' },
      { field: 'slowDay', header: 'Số ngày nộp chậm', textAlign: 'right', width: '100px' },
    ];

    // Defalut column table
    this.selectedColSlow = this.colSlow.filter(e => e.field == "bidStartDate" || e.field == "saleBiddingCode" || e.field == "saleBiddingName"
      || e.field == "valueBid" || e.field == "ros" || e.field == "statusName" || e.field == "slowDay");

    this.colInWeek = [
      { field: 'startDate', header: 'Ngày mở thầu', textAlign: 'right', width: '10%' },
      { field: 'saleBiddingCode', header: 'Mã gói thầu', textAlign: 'left', width: '10%' },
      { field: 'saleBiddingName', header: 'Tên gói thầu', textAlign: 'left', width: '20%' },
      { field: 'valueBid', header: 'Giá trị thầu', textAlign: 'right', width: '10%' },
      { field: 'ros', header: 'Tỉ suất lợi nhuận tạm tính(%)', textAlign: 'right', width: '13%' },
      { field: 'statusName', header: 'Trạng thái gói thầu', textAlign: 'center', width: '10%' },
      { field: 'typeContractName', header: 'Loại hợp đồng', textAlign: 'left', width: '10%' },
    ];
    // Defalut column table
    this.selectedColInWeek = this.colInWeek;
  }

  setChart() {
    let listWin: Array<number> = [];
    let listLose: Array<number> = [];
    if (this.listSaleBiddingChart.length > 0) {
      let statuWin = this.listStatus.find(x => x.categoryCode == "WIN");
      let statuLose = this.listStatus.find(x => x.categoryCode == "LOSE");
      this.listTypeContact.forEach(item => {
        let array: Array<SaleBidding> = this.listSaleBiddingChart.filter(x => x.typeContractId == item.categoryId);
        let sumWin = 0;
        let sumLosse = 0;
        if (array.length > 0) {
          let tempArray1 = array.filter(x => x.statusId == statuWin.categoryId);
          let tempArray2 = array.filter(x => x.statusId == statuLose.categoryId);
          sumWin = this.sumArray(tempArray1);
          listWin.push(sumWin);
          sumLosse = this.sumArray(tempArray2);
          listLose.push(sumLosse);
        }
        else {
          listWin.push(0);
          listLose.push(0);
        }
      });

      let arrayCate: Array<string> = this.listTypeContact.map(x => x.categoryName);
      Highcharts.setOptions({
        chart: {
          style: {
            fontFamily: 'Inter UI'
          }
        }
      })

      Highcharts.chart('container', {

        chart: {
          type: 'column'
        },
        title: {
          text: "Biểu đồ giá trị HST theo loại hồ sơ thầu"
        },
        subtitle: {
          text: '',
        },
        xAxis: {
          categories: arrayCate,
          crosshair: true,
          title: {
            text: ''
          }
        },
        yAxis: {
          min: 0,
          title: {
            text: '',
          }
        },
        tooltip: {
          headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
          pointFormat: '<tr><td style="color:{series.color};padding:0;font-family:Inter UI">{series.name}: </td>' +
            '<td style="padding:0;font-family:Inter UI"><b>{point.y} VNĐ</b></td></tr>',
          footerFormat: '</table>',
          shared: true,
          useHTML: true
        },
        plotOptions: {
          column: {
            pointPadding: 0.2,
            borderWidth: 0
          }
        },
        series: [{
          name: statuWin.categoryName,
          data: listWin,

        }, {
          name: statuLose.categoryName,
          data: listLose
        }]
      });
    }
    else {
      this.isShow = false;
    }
  }

  sumArray(array: Array<SaleBidding>): number {
    let sum = 0;
    array.forEach(item => {
      sum = sum + parseFloat(item.valueBid);
    });
    return sum;
  }
  getDashBoardSaleBidding() {
    let effectiveDate = null;
    if (this.effectiveDate) {
      effectiveDate = this.effectiveDate;
      effectiveDate = convertToUTCTime(effectiveDate);
    }
    else {
      effectiveDate = new Date();
      effectiveDate = convertToUTCTime(effectiveDate);
    }
    if (this.saleBiddingName) {
      this.saleBiddingName = this.saleBiddingName.trim();
    }
    this.loading = true;
    this.salebBiddingService.getMasterDataSaleBiddingDashboard(this.auth.UserId, effectiveDate, this.saleBiddingName).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listSaleBiddingWaitApproval = result.listSaleBiddingWaitApproval;
        this.listSaleBiddingExpired = result.listSaleBiddingExpired;
        this.listSaleBiddingSlowStartDate = result.listSaleBiddingSlowStartDate;
        this.listSaleBiddingInWeek = result.listSaleBiddingInWeek;
        this.listSaleBiddingChart = result.listSaleBiddingChart;
        this.listTypeContact = result.listTypeContact;
        this.listStatus = result.listStatus;
        this.statusCho = this.listStatus.find(x => x.categoryCode == 'CHO');
        this.setChart();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Event nhấn phím Enter khi đang forcus vào ô tìm kiếm theo tên khách hàng*/
  onKeydown($event: KeyboardEvent) {
    if ($event.key === 'Enter') {
      this.getDashBoardSaleBidding();
    }
  }
  gotoCreate() {
    this.router.navigate(['/sale-bidding/create']);
  }

  goToListSaleBidding(statusMode, date?) {
    this.router.navigate(['/sale-bidding/list', { mode: statusMode, date: date }]);
  }

  goToSaleBiddingList() {
    this.router.navigate(['/sale-bidding/list']);
  }

  onViewSaleBiddingDetail(data) {
    this.router.navigate(['/sale-bidding/detail', { saleBiddingId: data.saleBiddingId }]);
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
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

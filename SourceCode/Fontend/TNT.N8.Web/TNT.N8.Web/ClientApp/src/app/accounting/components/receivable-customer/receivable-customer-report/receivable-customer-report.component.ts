import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AccountingService } from '../../../services/accounting.service';
import { GetPermission } from '../../../../shared/permission/get-permission';

import { SortEvent, MessageService } from 'primeng/api';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { Table } from 'primeng/table';

class yearModel {
  label: string;
  value: number;
}

class periodModel {
  id: string;
  name: string;
}

class resultMonthModel {
  startMonth: number;
  endMonth: number;
}

class receivableCustomerReport {
  createDateOrder: Date;
  createDateReceiptInvoice: Date;
  createdBy: string;
  createdByName: string;
  customerCode: string;
  customerId: string;
  customerName: string;
  customerOrderId: string;
  descriptionReceipt: string;
  nearestTransaction: Date;
  orderCode: string;
  orderName: string;
  orderValue: string;
  productId: string;
  receiptCode: string;
  receiptId: string;
  receiptInvoiceValue: string;
  status: string;
  totalPaid: number;
  totalReceipt: number;
  totalSales: number;
  totalUnpaid: number;
}

@Component({
  selector: 'app-receivable-customer-report',
  templateUrl: './receivable-customer-report.component.html',
  styleUrls: ['./receivable-customer-report.component.css'],
})
export class ReceivableCustomerReportComponent implements OnInit {
  loading: boolean = false;
  nowDate: Date = new Date();
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  leftColNumber: number = 12;
  rightColNumber: number = 0;
  colSumarySection: number = 4;
  //master data
  totalPurchase: number = 0; //Tính tổng đặt hàng
  totalPaid: number = 0;  //Tính tổng đã thanh toán
  totalReceipt: number = 0; //Tính tổng còn phải thu 
  listReceivableCustomerReport: Array<receivableCustomerReport> = [];
  //table
  colsList: any;
  selectedColumns: any[];
  @ViewChild('myTable') myTable: Table;
  //filter
  searchForm: FormGroup;
  currentYear: number = new Date().getFullYear();
  minYear: number = this.currentYear - 10;
  maxStartDate: Date = new Date();
  maxEndDate: Date = new Date();
  filterGlobal : string;
  receivalbeDateTo = new Date();

  customerNameOrCustomerCode: string;
  year: number;
  startMonth: number;
  endMonth: number;
  lastDayofMonth: number;
  startDay: Date;
  endDay: Date;

  yearControl = new FormControl();
  periodControl = new FormControl();
  startDayControl = new FormControl();
  endDayControl = new FormControl();

  // years: number[] = [];
  listyears: Array<yearModel> = [];

  periods: { id: string, name: string }[] = [
    { id: "Q1", name: "Quý 1" },
    { id: "Q2", name: "Quý 2" },
    { id: "Q3", name: "Quý 3" },
    { id: "Q4", name: "Quý 4" },
    { id: "T1", name: "Tháng 1" },
    { id: "T2", name: "Tháng 2", },
    { id: "T3", name: "Tháng 3" },
    { id: "T4", name: "Tháng 4" },
    { id: "T5", name: "Tháng 5" },
    { id: "T6", name: "Tháng 6" },
    { id: "T7", name: "Tháng 7" },
    { id: "T8", name: "Tháng 8" },
    { id: "T9", name: "Tháng 9" },
    { id: "T10", name: "Tháng 10" },
    { id: "T11", name: "Tháng 11" },
    { id: "T12", name: "Tháng 12" },
    { id: "CUS", name: "Tùy chọn" }
  ];

  constructor(
    private translate: TranslateService,
    private accountingService: AccountingService,
    private getPermission: GetPermission,
    public messageService: MessageService,
    private router: Router
  )
  {
    this.translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }
  auth: any = JSON.parse(localStorage.getItem('auth'));

  //list action in page
  actionAdd: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  ngOnInit() {
    //Check permission
    let resource = "acc/accounting/receivable-customer-report/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      this.getYears();
      this.initSearchForm();
      this.initTable();
      this.initFilter();
      this.search();
    }
  }

  search() {
    //reset table
    this.listReceivableCustomerReport = [];

    let startDate = this.searchForm.get('FromDate').value;
    let toDate = this.searchForm.get('ToDate').value;
    let customerNameCode = this.searchForm.get('CustomerNameCode').value;

    if (!startDate || !toDate) {
      this.showToast('error', 'Thông báo', 'Chọn ngày tháng');
      return;
    }

    if (startDate) startDate = convertToUTCTime(startDate);
    if (toDate) toDate = convertToUTCTime(toDate);

    this.loading = true;
    this.accountingService.searchReceivableCustomerReport(customerNameCode, startDate, toDate).subscribe(response => {
      this.loading = false;
      const result = <any>response;
      if (result.receivableCustomerReport != null) {
        this.listReceivableCustomerReport = result.receivableCustomerReport;
        this.totalPurchase = result.totalPurchase; //tổng doanh thu
        this.totalPaid = result.totalPaid; //tổng đã thanh toán
        this.totalReceipt = result.totalReceipt; //tổng còn phải thu

        if (this.listReceivableCustomerReport.length === 0) {
          this.showToast('warn', 'Thông báo', 'Không tìm thấy khách hàng nào');
        }

        this.listReceivableCustomerReport.forEach(item => {
          item.customerName = item.customerCode + " - " + item.customerName;
        });
      } else {
      }
    }, error => { this.loading = false; });

  }

  // Goto detail receivable vendor
  goToDetail(rowData: receivableCustomerReport) {
    let startDate = this.searchForm.get('FromDate').value;
    let toDate = this.searchForm.get('ToDate').value;
    this.router.navigate(['/accounting/receivable-customer-detail', { id: rowData.customerId, fromDate: startDate.toISOString(), toDate: toDate.toISOString() }]);
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  initSearchForm() {
    this.searchForm = new FormGroup({
      'Year': new FormControl(null),
      'Period': new FormControl(null),
      'FromDate': new FormControl(null),
      'ToDate': new FormControl(null),
      'CustomerNameCode': new FormControl('')
    });
  }

  initTable() {
    this.colsList = [
      { field: 'customerName', header: 'Mã - Tên khách hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'nearestTransaction', header: 'Ngày thanh toán gần nhất', textAlign: 'right', display: 'table-cell' },
      { field: 'totalSales', header: 'Tổng đặt hàng(VND)', textAlign: 'right', display: 'table-cell' },
      { field: 'totalPaid', header: 'Tổng đã thanh toán(VND)', textAlign: 'right', display: 'table-cell' },
      { field: 'totalReceipt', header: 'Tổng còn phải thu(VND)', textAlign: 'right', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsList;
  }

  initFilter() {
    let currentYear: yearModel = this.listyears.find(e => e.value == this.currentYear);
    this.searchForm.get('Year').setValue(currentYear);
    let startDate = new Date(currentYear.value, 0, 1);
    let endDate = new Date(currentYear.value, 12, 0);
    this.searchForm.get('FromDate').setValue(startDate);
    this.searchForm.get('ToDate').setValue(endDate);
  }

  refreshFilter() {
    this.listReceivableCustomerReport = [];
    this.searchForm.reset();
    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.colSumarySection = 4;
    this.filterGlobal = '';
    this.initFilter();
    this.search();
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 9;
        this.rightColNumber = 3;
        this.colSumarySection = 6;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
        this.colSumarySection = 4;
      }
    }
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

  getYears() {
    for (let i = this.minYear; i <= this.currentYear; i++) {
      let item: yearModel = new yearModel();
      item.label = String(i);
      item.value = i;
      this.listyears.push(item);
    }
  }

  changeYear(event: any) {
    this.searchForm.get('Period').reset();
    this.searchForm.get('FromDate').reset();
    this.searchForm.get('ToDate').reset();

    let selectedYear: yearModel = event.value;
    if (!selectedYear) return;

    let startDate = new Date(selectedYear.value, 0, 1);
    let endDate = new Date(selectedYear.value, 12, 0);
    this.searchForm.get('FromDate').setValue(startDate);
    this.searchForm.get('ToDate').setValue(endDate);
  }

  changePeriod(event: any) {
    let year: yearModel = this.searchForm.get('Year').value;
    this.searchForm.get('FromDate').reset();
    this.searchForm.get('ToDate').reset();
    if (!year) {
      this.showToast('error', 'Thông báo', 'Chọn năm');
      return;
    }
    let selectedPeriod: periodModel = event.value;
    if (!selectedPeriod) return;

    if (selectedPeriod.id === "CUS") {
      this.searchForm.get('Year').reset();
      return;
    }

    let _getMonth = this.getMonth(selectedPeriod);

    let startDate = new Date(year.value, _getMonth.startMonth, 1);
    let endDate = new Date(year.value, _getMonth.endMonth + 1, 0);

    this.searchForm.get('FromDate').setValue(startDate);
    this.searchForm.get('ToDate').setValue(endDate);
  }

  getMonth(period: any): resultMonthModel {
    let result = new resultMonthModel();
    switch (period.id) {
      case "Q1":
        result.startMonth = 0;
        result.endMonth = 2;
        break;
      case "Q2":
        result.startMonth = 3;
        result.endMonth = 5;
        break;
      case "Q3":
        result.startMonth = 6;
        result.endMonth = 8;
        break;
      case "Q4":
        result.startMonth = 9;
        result.endMonth = 11;
        break;
      case "T1":
        result.startMonth = 0;
        result.endMonth = 0;
        break;
      case "T2":
        result.startMonth = 1;
        result.endMonth = 1;
        break;
      case "T3":
        result.startMonth = 2;
        result.endMonth = 2;
        break;
      case "T4":
        result.startMonth = 3;
        result.endMonth = 3;
        break;
      case "T5":
        result.startMonth = 4;
        result.endMonth = 4;
        break;
      case "T6":
        result.startMonth = 5;
        result.endMonth = 5;
        break;
      case "T7":
        result.startMonth = 6;
        result.endMonth = 6;
        break;
      case "T8":
        result.startMonth = 7;
        result.endMonth = 7;
        break;
      case "T9":
        result.startMonth = 8;
        result.endMonth = 8;
        break;
      case "T10":
        result.startMonth = 9;
        result.endMonth = 9;
        break;
      case "T11":
        result.startMonth = 10;
        result.endMonth = 10;
        break;
      case "T12":
        result.startMonth = 11;
        result.endMonth = 11;
        break;
    }

    return result;
  }

  onChangeFromDate(event: any) {
    //clear năm và kỳ
    this.searchForm.get('Year').reset();
    this.searchForm.get('Period').reset();
    let custom = this.periods.find(e => e.id == "CUS");
    this.searchForm.get('Period').setValue(custom);
  }

  onChangeToDate(event: any) {
    //clear năm và kỳ
    this.searchForm.get('Year').reset();
    this.searchForm.get('Period').reset();
    let custom = this.periods.find(e => e.id == "CUS");
    this.searchForm.get('Period').setValue(custom);
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

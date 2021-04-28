import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

import { AccountingService } from '../../../services/accounting.service';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { DatePipe } from '@angular/common';
import { Table } from 'primeng/table';
import { MessageService, SortEvent } from 'primeng/api';
import * as moment from 'moment';
import 'moment/locale/pt-br';

interface ReceivableVendorReport {
  index : number;
  status: string;
  totalPaid: number;
  totalPurchase: number;
  nearestTransaction: string;
  vendorCode: string;
  vendorId: string;
  vendorName: string;
  debt : number;
}

@Component({
  selector: 'app-receivable-vendor-report',
  templateUrl: './receivable-vendor-report.component.html',
  styleUrls: ['./receivable-vendor-report.component.css'],
  providers: [
    DatePipe
  ]
})

export class ReceivableVendorReportComponent implements OnInit {
  vendorCode: Array<any> = [];
  vendorName: string;

  fromDate: Date = new Date();
  toDate: Date = null;
  nowDate: Date = new Date();
  filterGlobar : string;
  innerWidth: number = 0; //number window size first
  totalPurchase: any;
  totalPaid: any;

  listVendor : Array<any> = [];
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  leftColNumber: number = 12;
  rightColNumber: number = 0;
  colSumarySection: number = 4;

  actionAdd: boolean = true;
  actionDownload: boolean = true;
  loading: boolean = false;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();

  /*Check user permission*/
  @ViewChild('myTable') myTable: Table;
  colsList: any;
  selectedColumns: any[];
  auth: any = JSON.parse(localStorage.getItem('auth'));
  messages: any;
  listReceivableVendorReport: Array<ReceivableVendorReport> = [];
  // @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private accountingService: AccountingService,
    private router: Router,
    private datePipe: DatePipe,
    private messageService: MessageService,
  ) {
    this.translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }


  ngOnInit() {
    let resource = "acc/accounting/receivable-vendor-report/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      this.initTable();
      this.loading = true;
      this.accountingService.GetDataSearchReceivableVendor().subscribe(response =>{
        let result : any = response;
        this.loading = false;
        if(result.statusCode == 200){
          this.listVendor = result.listVendor;
          if(this.listVendor.length > 0){
            this.listVendor.forEach(item =>{
              item.vendorCode = item.vendorCode + ' - ' + item.vendorName;
            });
          }
          this.searchReceiableVendor();
        }else{
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  initTable() {
    this.colsList = [
      { field: 'vendorName', header: 'Mã - Tên NCC', textAlign: 'left', display: 'table-cell' },
      { field: 'nearestTransaction', header: 'Giao dịch gần nhất', textAlign: 'right', display: 'table-cell' },
      { field: 'totalPurchase', header: 'Tổng đặt hàng(VND)', textAlign: 'right', display: 'table-cell'},
      { field: 'totalPaid', header: 'Tổng đã thanh toán(VND)', textAlign: 'right', display: 'table-cell'},
      { field: 'debt', header: 'Số tiền còn phải trả(VND)', textAlign: 'right', display: 'table-cell'},
    ];

    this.selectedColumns = this.colsList;
  }

  searchReceiableVendor(){
    let listVendorCode : Array<string> = [];
    listVendorCode = this.vendorCode.map(c => c.vendorId);

    let toDate = null;
    if(this.toDate){
      toDate = this.toDate;
      toDate.setHours(23, 59, 59, 999);
      toDate = convertToUTCTime(toDate);    
    }
    if(this.vendorName){
      this.vendorName = this.vendorName.trim();
    }

    this.loading = true;
    this.accountingService.searchReceivableVendorReport(toDate, listVendorCode, this.vendorName).subscribe(response => {
      let result = <any> response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listReceivableVendorReport = result.receivableVendorReport;        
        this.setIndex();
        this.totalPaid = result.totalPaid;
        this.totalPurchase = result.totalPurchase;
        if (this.listReceivableVendorReport.length == 0) {
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy nhà cung cấp nào!' };
            this.showMessage(msg);
        } else {
          this.listReceivableVendorReport.forEach(item => {
            item.nearestTransaction = this.datePipe.transform(item.nearestTransaction, 'dd/MM/yyyy');
            item.debt = item.totalPurchase - item.totalPaid;
          });
          this.listReceivableVendorReport = this.listReceivableVendorReport.sort((a, b) => b.debt - a.debt);
        }
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setIndex(){
    this.listReceivableVendorReport.forEach((item, index) =>{
      item.index = index + 1;
    });
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

  refreshFilter() {
    this.listReceivableVendorReport = [];
    this.vendorCode = [];
    this.vendorName = '';
    this.toDate = null;
    this.filterGlobar = '';
    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.colSumarySection = 4;
    this.searchReceiableVendor();
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

  // Goto detail receivable vendor
  goToDetail(rowdata) {
    this.router.navigate(['/accounting/receivable-vendor-detail', { id: rowdata.vendorId }]);
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};


import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { WarningComponent } from '../../../../shared/toast/warning/warning.component';

import { CategoryService } from '../../../../shared/services/category.service';
import { AuthenticationService } from '../../../../shared/services/authentication.service';
import { AccountingService } from '../../../services/accounting.service';
import { GetPermission } from '../../../../shared/permission/get-permission';

import { SortEvent, MessageService } from 'primeng/api';
import * as moment from 'moment';
import { Table } from 'primeng/table';

interface Receipt {
  index : number;
  payableInvoiceId : string;
  bankPayableInvoiceId : string;
  createDateReceiptInvoice: string;
  createdByName: string;
  descriptionReceipt: string;
  receiptCode: string;
  receiptInvoiceValue: number;
  router : string;
}

interface ReceivableVendorDetail {
  index : number;
  createDateOrder: string;
  createdByName: string;
  orderCode: string;
  orderName: string;
  orderValue: number;
  vendorId: string;
  vendorOrderId: string;
}

@Component({
  selector: 'app-receivable-vendor-detail',
  templateUrl: './receivable-vendor-detail.component.html',
  styleUrls: ['./receivable-vendor-detail.component.css']
})

export class ReceivableVendorDetailComponent implements OnInit {
  vendorId: any;
  auth: any = JSON.parse(localStorage.getItem('auth'));
  messages: any;
  listReceivableVendorDetail: Array<ReceivableVendorDetail> = [];
  listReceipt: Array<Receipt> = [];
  vendorName: any;
  totalReceivableBefore: any;
  totalReceivable: any;
  totalReceivableInPeriod: any;
  totalValueReceipt: any;
  totalValueOrder: any;
  isAscending = false;
  innerWidth: number = 0; //number window size first
  fromDate = null;
  toDate = null;

  leftColNumber: number = 12;
  rightColNumber: number = 0;
  colSumarySection: number = 3;

  actionAdd: boolean = true;
  actionDownload: boolean = true;
  loading: boolean = false;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();
  nowDate: Date = new Date();

  @ViewChild('myTable') myTable: Table;
  colsList: any;
  selectedColumns: any[];

  // @ViewChild('table') table: Table;
  colsListRe: any;
  selectedColumnsRe: any[];

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  constructor(private translate: TranslateService,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private accountingService: AccountingService,
    private router: Router,
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
      this.route.params.subscribe(params => { this.vendorId = params['id']; });
      this.initTable();
      this.search();
    }
  }

  initTable() {

    this.colsList = [
      { field: 'createDateOrder', header: 'Ngày tạo', textAlign: 'right', display: 'table-cell' },
      { field: 'orderCode', header: 'Mã phiếu', textAlign: 'left', display: 'table-cell' },
      { field: 'orderName', header: 'Sản phẩm/dịch vụ', textAlign: 'left', display: 'table-cell' },
      { field: 'createdByName', header: 'Người tạo', textAlign: 'left', display: 'table-cell' },
      { field: 'orderValue', header: 'Số tiền còn phải trả(VND)', textAlign: 'right', display: 'table-cell' },
    ];

    this.selectedColumns = this.colsList;

    this.colsListRe = [
      { field: 'createDateReceiptInvoice', header: 'Ngày hạch toán', textAlign: 'right', display: 'table-cell' },
      { field: 'receiptCode', header: 'Mã phiếu', textAlign: 'left', display: 'table-cell' },
      { field: 'descriptionReceipt', header: 'Diễn giải', textAlign: 'left', display: 'table-cell' },
      { field: 'createdByName', header: 'Người tạo', textAlign: 'left', display: 'table-cell' },
      { field: 'receiptInvoiceValue', header: 'Số tiền còn phải trả(VND)', textAlign: 'right', display: 'table-cell' },
    ];

    this.selectedColumnsRe = this.colsListRe;
  }

  search() {
    let fromDate = null;
    if (this.fromDate) {
      fromDate = this.fromDate;
      fromDate.setHours(0, 0, 0, 0);
      fromDate = convertToUTCTime(fromDate);
    }

    let toDate = null;
    if (this.toDate) {
      toDate = this.toDate;
      toDate.setHours(23, 59, 59, 999);
      toDate = convertToUTCTime(toDate);
    }

    this.loading = true;
    this.accountingService.searchReceivableVendorDetail(this.vendorId, fromDate, toDate).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listReceivableVendorDetail = result.receivableVendorDetail;
        this.listReceipt = result.receiptsList;
        this.vendorName = result.vendorName;
        this.totalReceivableBefore = result.totalReceivableBefore;  //D ư nợ đầu kỳ
        this.totalReceivable = result.totalReceivable;  //Dư nợ cuối kỳ
        this.totalReceivableInPeriod = result.totalReceivableInPeriod;
        this.totalValueReceipt = result.totalValueReceipt;  //Thanh toán trong kỳ
        this.totalValueOrder = result.totalValueOrder;  //Nợ phát sinh trong kỳ
      }
      this.setIndex();
    });
  }
  setIndex() {
    this.listReceivableVendorDetail.forEach((item, index) => {
      item.index = index + 1;
    });

    this.listReceipt.forEach((item, index) => {
      item.index = index + 1;
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 9;
        this.rightColNumber = 3;
        this.colSumarySection = 4;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
        this.colSumarySection = 3;
      }
    }
  }

  refreshFilter() {
    this.fromDate = null;
    this.toDate = null;
    this.search();
  }
  goToOrderDetail(rowdata: any) {
    this.router.navigate(['/vendor/detail-order', { vendorOrderId: rowdata.vendorOrderId }]);
  }

    // Back to list vendor receivable
    cancel() {
      this.router.navigate(['/accounting/receivable-vendor-report']);
    }

  gotoDetailReceipt(rowdata : any){
    
    if(rowdata.router === 'UNC'){
      this.router.navigate(['/accounting/bank-payments-detail', rowdata.bankPayableInvoiceId]);
    }else{
      this.router.navigate(['/accounting/cash-payments-view', rowdata.payableInvoiceId]);
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
}
function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};


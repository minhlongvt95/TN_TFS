import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { AccountingService } from './../../services/accounting.service';
import { BankBookService } from './../../services/bank-book.service';
import { Router } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { WarningComponent } from '../../../shared/toast/warning/warning.component';

import { MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { TranslateService } from '@ngx-translate/core';

class CashBookModel {
  id: string;
  code: string;
  createdDate: Date;
  cashDate: Date;
  createdName: string;
  reason: string;
  content: string;
  receiptMoney = null;
  payableMoney = null;
  note: string;
  route: string;
}

@Component({
  selector: 'app-bankbook',
  templateUrl: './bankbook.component.html',
  styleUrls: ['./bankbook.component.css'],
  providers: [
    DatePipe
  ]
})
export class BankbookComponent implements OnInit {
  // @ViewChild(MatPaginator) paginator: MatPaginator;
  // @HostListener('document:keyup', ['$event'])
  // handleDeleteKeyboardEvent(event: KeyboardEvent) {
  //   if (event.key === 'Enter') {
  //     this.getData();
  //   }
  // }

  // displayedColumns = ['code', 'createdDate', 'paidDate', 'createdName', 'reason', 'content', 'receiptMoney', 'payableMoney', 'note'];

  userPermission: any = localStorage.getItem("UserPermission").split(',');
  auth: any = JSON.parse(localStorage.getItem("auth"));
  filterGlobal : any;
  first : number =  0;
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  loading: boolean = false;
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();


  // toPaidDateParameter: Date = null;

  // datesearch: Date = null;
  // dataSource: MatTableDataSource<CashBookModel>;
  openingBalance: number = 0;
  closingBalance: number = 0;
  typeOfBook = "RETY"
  totalBankReceipts: number = 0;
  totalBankPay: number = 0;

  listBankAccount: Array<any> = [];
  listEmployee: Array<any> = [];
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  leftColNumber: number = 12;
  rightColNumber: number = 0;
  colSumarySection: number = 3;

  bankAccountIds: Array<any> = [];
  createdByIds: Array<any> = [];
  fromDate: Date = null;
  toDate: Date = null;
  nowDate: Date = new Date();

  /*Check user permission*/
  @ViewChild('myTable') myTable: Table;
  colsList: any;
  selectedColumns: any[];
  frozenCols: any[];

  listCashBook: Array<CashBookModel> = [];
  listReceiptInv = [];
  listPayableInv = [];
  listTypeOfBook = [];
  arrayBankAccount = [];
  constructor(
    private accountingService: AccountingService,
    private router: Router,
    private translate: TranslateService,
    private getPermission: GetPermission,
    private bankBookService: BankBookService,
    private datePipe: DatePipe,
    private messageService: MessageService,
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "acc/accounting/bank-book/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      this.loading = true;
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownload = false;
      }
      this.initTable();
      this.accountingService.getMasterDataSearchBankBook().subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.listEmployee = result.listEmployee;
          this.listBankAccount = result.listBankAccount;
          this.listEmployee.forEach(item => {
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;
          });
          this.searchBankBook();
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  initTable() {
    this.colsList = [
      { field: 'code', header: 'Mã chứng từ', textAlign: 'left', display: 'table-cell', width : '12%' },
      { field: 'createdDate', header: 'Ngày giao dịch', textAlign: 'right', display: 'table-cell' },
      { field: 'cashDate', header: 'Ngày hạch toán', textAlign: 'right', display: 'table-cell', width:'12%' },
      { field: 'createdName', header: 'Người tạo', textAlign: 'left', display: 'table-cell' },
      { field: 'reason', header: 'Lý do', textAlign: 'left', display: 'table-cell' },
      { field: 'content', header: 'Nội dung', textAlign: 'left', display: 'table-cell' },
      { field: 'receiptMoney', header: 'Số tiền thu', textAlign: 'right', display: 'table-cell'},
      { field: 'payableMoney', header: 'Số tiền chi', textAlign: 'right', display: 'table-cell'},
      { field: 'note', header: 'Ghi chú', textAlign: 'left', display: 'table-cell' },
    ];

    this.selectedColumns = this.colsList;
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
    this.listCashBook = [];
    this.bankAccountIds = [];
    this.fromDate = null;
    this.toDate = null;
    this.createdByIds = [];
    this.resetTable();
    this.searchBankBook();
  }

  resetTable() {
    this.filterGlobal = '';
    this.first = 0;
  }
  async searchBankBook() {
    this.listCashBook = [];
    let listBankAccount: Array<string> = [];
    listBankAccount = this.bankAccountIds.map(x => x.bankAccountId);

    let listCreateByIds: Array<string> = [];
    listCreateByIds = this.createdByIds.map(x => x.employeeId);

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
    this.bankBookService.searchBankBook(this.auth.UserId, fromDate, toDate, listBankAccount).subscribe(response => {
      let result = <any>response;
      this.openingBalance = result.openingBalance;
      this.closingBalance = result.closingBalance;
    }, error => { });

    this.loading = true;
    let [receipt, payments]: any = await Promise.all([
      this.accountingService.searchBankBookReceiptAsync(this.auth.UserId, fromDate, toDate, listBankAccount, listCreateByIds),
      this.accountingService.searchBankBookPayableInvoiceAsync(this.auth.UserId, fromDate, toDate, listBankAccount, listCreateByIds)
    ]);
    this.listReceiptInv = receipt.bankReceiptInvoiceList;
    this.listPayableInv = payments.bankPayableInvoiceList;

    if (this.listReceiptInv.length != 0) {
      this.addDataToCashBook("receipt");
      for (var i = 0; i < receipt.bankReceiptInvoiceList.length; i++) {
        this.totalBankReceipts += receipt.bankReceiptInvoiceList[i].bankReceiptInvoiceAmount;
      }
    }
    if (this.listPayableInv.length != 0) {
      this.addDataToCashBook("payable");
      for (var i = 0; i < payments.bankPayableInvoiceList.length; i++) {
        this.totalBankPay += payments.bankPayableInvoiceList[i].bankPayableInvoiceAmount;
      }
      this.closingBalance = this.openingBalance + this.totalBankReceipts - this.totalBankPay;
    }
    this.loading = false;
    // var bankreceipt: any = await this.accountingService.searchBankReceiptInvoiceAsync(this.auth.UserId, '', [], [], fromDate, toDate, [], []);
    // for (var i = 0; i < bankreceipt.bankReceiptInvoiceList.length; i++) {
    //   this.totalBankReceipts += bankreceipt.bankReceiptInvoiceList[i].bankReceiptInvoiceAmount;
    // }

    // var bankpay: any = await this.accountingService.searchBankPayableInvoiceAsync(this.auth.UserId, '', [], [], fromDate, toDate, [], []);
    // for (var i = 0; i < bankpay.bankPayableInvoiceList.length; i++) {
    //   this.totalBankPay += bankpay.bankPayableInvoiceList[i].bankPayableInvoiceAmount;
    // }
    // this.closingBalance = this.openingBalance + this.totalBankReceipts - this.totalBankPay;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  // async getData() {
  //   this.listCashBook = [];
  //   if (this.toPaidDateParameter !== null) {
  //     var fromDate = new Date(this.toPaidDateParameter.getFullYear(), this.toPaidDateParameter.getMonth() - 1, 1);
  //     this.fromPaidDateParameter = fromDate;
  //   }
  //   else {
  //     this.toPaidDateParameter = new Date();
  //     var fromDate = new Date(this.toPaidDateParameter.getFullYear(), this.toPaidDateParameter.getMonth()- 1, 1);
  //     this.fromPaidDateParameter = fromDate;
  //   }

  //   this.companyService.getCompanyConfig().subscribe(response => {
  //     let result = <any>response;
  //     if (result.statusCode === 202 || result.statusCode === 200) {
  //       this.arrayBankAccount = result.listBankAccount;
  //     }
  //   }, error => { });

  //   this.bankBookService.searchBankBook(this.auth.UserId, this.fromPaidDateParameter, this.toPaidDateParameter, this.listBankAccount).subscribe(response => {
  //     let result = <any>response;
  //     this.openingBalance = result.openingBalance;
  //     this.closingBalance = result.closingBalance;
  //   }, error => { }
  //   )

  //   this.accountingService.searchBankBookReceipt(this.auth.UserId, this.fromPaidDateParameter, this.toPaidDateParameter, this.listBankAccount, []).subscribe(response => {
  //     let result = <any>response;
  //     this.listReceiptInv = result.bankReceiptInvoiceList;
  //     this.addDataToCashBook("receipt");

  //     this.sort();
  //     this.dataSource = new MatTableDataSource<CashBookModel>(this.listCashBook);
  //     this.paginationFunction();

  //   }, error => { }
  //   )

  //   this.accountingService.searchBankBookPayableInvoice(this.auth.UserId, this.fromPaidDateParameter, this.toPaidDateParameter, this.listBankAccount, []).subscribe(response => {
  //     let result = <any>response;
  //     this.listPayableInv = result.bankPayableInvoiceList;
  //     this.addDataToCashBook("payable")

  //     this.sort();
  //     this.dataSource = new MatTableDataSource<CashBookModel>(this.listCashBook);
  //     this.paginationFunction();
  //   }, error => { }
  //   )
  //   var bankreceipt: any = await this.accountingService.searchBankReceiptInvoiceAsync(this.auth.UserId, '', [], [], this.fromPaidDateParameter, this.toPaidDateParameter, [], []);
  //   for (var i = 0; i < bankreceipt.bankReceiptInvoiceList.length; i++) {
  //     this.totalBankReceipts += bankreceipt.bankReceiptInvoiceList[i].bankReceiptInvoiceAmount;
  //   }

  //   var bankpay: any = await this.accountingService.searchBankPayableInvoiceAsync(this.auth.UserId, '', [], [], this.fromPaidDateParameter, this.toPaidDateParameter, [], []);
  //   for (var i = 0; i < bankpay.bankPayableInvoiceList.length; i++) {
  //     this.totalBankPay += bankpay.bankPayableInvoiceList[i].bankPayableInvoiceAmount;
  //   }
  //   this.closingBalance = this.openingBalance + this.totalBankReceipts - this.totalBankPay;
  // }

  addDataToCashBook(typeOfCashBook: string) {
    let tmp;
    if (typeOfCashBook === 'payable') {
      this.listPayableInv.forEach(array => {
        //typeB = this.listTypeOfBook.find(function (element) {
        //  return element.categoryId === array.registerType;
        //});
        tmp = <CashBookModel>{
          id: array.bankPayableInvoiceId,
          code: array.bankPayableInvoiceCode,
          createdDate: array.createdDate,
          cashDate: array.bankPayableInvoicePaidDate,
          createdName: array.createdByName,
          reason: array.bankPayableInvoiceReasonText === null ? '' : array.bankPayableInvoiceReasonText,
          content: array.bankPayableInvoiceDetail === null ? '' : array.bankPayableInvoiceDetail,
          payableMoney: array.bankPayableInvoiceAmount.toString(),
          note: array.bankPayableInvoiceNote === null ? '' : array.bankPayableInvoiceNote,
          route: 'payable'
        }
        this.listCashBook.push(tmp);
      })
    } else {
      this.listReceiptInv.forEach(array => {
        tmp = <CashBookModel>{
          id: array.bankReceiptInvoiceId,
          code: array.bankReceiptInvoiceCode,
          createdDate: array.createdDate,
          cashDate: array.bankReceiptInvoicePaidDate,
          createdName: array.createdByName,
          reason: array.bankReceiptInvoiceReasonText === null ? '' : array.bankReceiptInvoiceReasonText,
          content: array.bankReceiptInvoiceDetail === null ? '' : array.bankReceiptInvoiceDetail,
          receiptMoney: array.bankReceiptInvoiceAmount.toString(),
          note: array.bankReceiptInvoiceNote === null ? '' : array.bankReceiptInvoiceNote,
          route: 'receipt'
        }
        this.listCashBook.push(tmp);
      })
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

  gotoView(element) {
    if (element.route === 'receipt') {
      this.router.navigate(['/accounting/bank-receipts-detail', element.id]);
    } else {
      this.router.navigate(['/accounting/bank-payments-detail', element.id]);
    }

  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};


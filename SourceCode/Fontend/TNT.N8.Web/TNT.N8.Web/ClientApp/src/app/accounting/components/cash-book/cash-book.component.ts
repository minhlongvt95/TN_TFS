import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl, FormArray } from '@angular/forms';
import { AccountingService } from './../../services/accounting.service';
import { CashBookService } from './../../services/cash-book.service';
import { CategoryService } from './../../../shared/services/category.service';
import { OrganizationService } from './../../../shared/services/organization.service';
import { Router } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { SortEvent, MessageService } from 'primeng/api';
import * as moment from 'moment';
import { Table } from 'primeng/table';

class organizationModel {
  organizationId: string;
  organizationCode: string;
  organizationName: string;
}

class typeOfBookModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}

class receiptInvoiceModel {
  active: boolean;
  amount: number;
  amountText: string;
  backgroundColorForStatus: string;
  createByAvatarUrl: string;
  createdById: string;
  createdDate: Date;
  currencyUnit: string;
  currencyUnitName: string;
  exchangeRate: string;
  nameCreateBy: string;
  nameObjectReceipt: string;
  nameReceiptInvoiceReason: string;
  organizationId: string;
  organizattionName: string;
  receiptDate: Date;
  receiptInvoiceCode: string;
  receiptInvoiceDetail: string;
  receiptInvoiceId: string;
  receiptInvoiceNote: string;
  receiptInvoiceReason: string;
  recipientAddress: string;
  recipientName: string;
  registerType: string;
  statusId: string;
  statusName: string;
  unitPrice: number;
}

class payableinvoiceModel {
  active: boolean;
  amount: number;
  amountText: string;
  backgroundColorForStatus: string;
  createByAvatarUrl: string;
  createdByName: string;
  createdById: string;
  createdDate: Date;
  currencyUnit: string;
  currencyUnitName: string;
  exchangeRate: string;
  objectId: string;
  objectName: string;
  organizationId: string;
  organizationName: string;
  paidDate: Date;
  payableInvoiceCode: string;
  payableInvoiceDetail: string;
  payableInvoiceId: string;
  payableInvoiceNote: string;
  payableInvoicePrice: number;
  payableInvoicePriceCurrency: string;
  payableInvoiceReason: string;
  payableInvoiceReasonText: string;
  recipientAddress: string;
  recipientName: string;
  registerType: string;
  statusId: string;
  statusName: string;
}

class CashBookModel {
  id: string;
  code: string;
  createdDate: Date;
  cashDate: Date;
  createdName: string;
  reason: string;
  type: string;
  content: string;
  receiptMoney = null;
  payableMoney = null;
  note: string;
  route: string;
}

@Component({
  selector: 'app-cash-book',
  templateUrl: './cash-book.component.html',
  styleUrls: ['./cash-book.component.css'],
})

export class CashBookComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  //filter
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  innerWidth: number = 0; //number window size first
  filterGlobal: string = '';
  @ViewChild('myTable') myTable: Table;
  //master data
  listOrganization: Array<organizationModel> = [];
  listEmployee: Array<any> = [];
  //table
  rows: number = 10;
  colsListProduct: any;
  selectedColumns: any[];
  //form
  searchForm: FormGroup;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxStartDate: Date = new Date();
  maxEndDate: Date = new Date();
  //sumary section
  openingSurplus: number = 0;  //Số dư đầu kỳ
  closingSurplus: number = 0;  //Số dư hiện tại
  totalCashReceipt: number = 0; //tổng số tiền thu
  totalCashPayments: number = 0; //tổng số tiền chi

  organizationIds: Array<any> = [];
  createdByIds: Array<any> = [];
  fromDate: Date = null;
  toDate: Date = null;

  nowDay : Date = new Date();
  toPaidDateParameter: Date = new Date();


  listCashBook: Array<CashBookModel> = [];
  listReceiptInvoice: Array<receiptInvoiceModel> = [];
  listPayableInvoice: Array<payableinvoiceModel> = [];

  listTypeOfBook: Array<typeOfBookModel> = [];
  loading: boolean = false;
  actionDownload: boolean = true;
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  constructor(
    private accountingService: AccountingService,
    private getPermission: GetPermission,
    //public snackBar: MatSnackBar,
    private router: Router,
    private categoryService: CategoryService,
    private cashbookService: CashBookService,
    private orgService: OrganizationService,
    private messageService: MessageService,
  ) {
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "acc/accounting/cash-book/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownload = false;
      }
      // this.loading = true;
      this.initTable();
      this.loading = true;
      this.accountingService.GetDataSearchCashBook().subscribe(response =>{
        let result : any = response;
        this.loading = false;
        if(result.statusCode == 200){
          this.listEmployee = result.listEmployee;
          this.listOrganization = result.listOrganization;
          this.listEmployee.forEach(item =>{
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;
          });
          this.getData();
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
    this.colsListProduct = [
      { field: 'code', header: 'Mã chứng từ', textAlign: 'left', display: 'table-cell', width : '13%'},
      { field: 'createdDate', header: 'Ngày lập', textAlign: 'right', display: 'table-cell' },
      { field: 'cashDate', header: 'Ngày hạch toán', textAlign: 'right', display: 'table-cell', width: '12%' },
      { field: 'createdName', header: 'Người tạo', textAlign: 'left', display: 'table-cell' },
      { field: 'reason', header: 'Lý do', textAlign: 'left', display: 'table-cell' },
      { field: 'type', header: 'Loại sổ', textAlign: 'left', display: 'table-cell' },
      { field: 'content', header: 'Nội dung', textAlign: 'left', display: 'table-cell' },
      { field: 'receiptMoney', header: 'Số tiền thu', textAlign: 'right', display: 'table-cell' },
      { field: 'payableMoney', header: 'Số tiền chi', textAlign: 'right', display: 'table-cell' },
      { field: 'note', header: 'Ghi chú', textAlign: 'left', display: 'table-cell' }
    ];
    this.selectedColumns = this.colsListProduct;
  }

  // search payableInv và ReceiptInv
  async getData() {
    this.listCashBook = [];
    this.openingSurplus = 0;  //Số dư đầu kỳ
    this.closingSurplus = 0;  //Số dư hiện tại
    this.totalCashReceipt = 0; //tổng số tiền thu
    this.totalCashPayments = 0; //tổng số tiền chi

    let typeOfBook = "LSO";

    let listOrganizationId : Array<string> = [];
    listOrganizationId = this.organizationIds.map(x => x.organizationId);

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


    this.loading = true;
    let [category, receipt, payments, surplus]: any = await Promise.all([
      this.categoryService.getAllCategoryByCategoryTypeCodeAsyc(typeOfBook),
      this.accountingService.searchCashBookReceiptInvoiceAsync(listCreateByIds, fromDate, toDate, listOrganizationId, this.auth.UserId),
      this.accountingService.searchCashBookPayableInvoiceAsync(this.auth.UserId, listCreateByIds, fromDate, toDate, listOrganizationId),
      this.cashbookService.getSurplusCashBookPerMonthAsync(this.auth.UserId, fromDate, toDate, listOrganizationId)
    ]);
    this.loading = false;

    this.listTypeOfBook = category.category;
    this.listReceiptInvoice = receipt.lstReceiptInvoiceEntity;
    this.listPayableInvoice = payments.payableInvList;
    this.openingSurplus = surplus.openingSurplus;
    this.closingSurplus = surplus.closingSurplus;

    this.mapDataToCashBook('receiptInvoice', this.listReceiptInvoice, []);
    this.mapDataToCashBook('payableinvoice', [], this.listPayableInvoice);

    this.listCashBook.forEach(e => {
      if (!e.receiptMoney) e.receiptMoney = 0;
      if (!e.payableMoney) e.payableMoney = 0;

      this.totalCashReceipt += e.receiptMoney;
      this.totalCashPayments += e.payableMoney;

    });
  }

  mapDataToCashBook(type: string, listReceiptInvoice: Array<receiptInvoiceModel>, listPayableInvoice: Array<payableinvoiceModel>) {
    switch (type) {
      case "receiptInvoice":
        listReceiptInvoice.forEach(e => {
          let typeBook = this.listTypeOfBook.find(_typebook => _typebook.categoryId == e.registerType);
          let typeBookName = typeBook ? typeBook.categoryName : "";

          let temp: CashBookModel = new CashBookModel();
          temp.id = e.receiptInvoiceId;
          temp.code = e.receiptInvoiceCode;
          temp.createdDate = e.createdDate;
          temp.createdName = e.nameCreateBy;
          temp.reason = e.nameReceiptInvoiceReason;
          temp.type = typeBookName;
          temp.content = e.receiptInvoiceDetail;
          temp.receiptMoney = e.unitPrice;
          temp.note = e.receiptInvoiceNote;
          temp.route = 'receipt';

          this.listCashBook = [...this.listCashBook, temp];
        });
        break;
      case "payableinvoice":
        this.listPayableInvoice.forEach(e => {
          let typeBook = this.listTypeOfBook.find(_typebook => _typebook.categoryId == e.registerType);
          let typeBookName = typeBook ? typeBook.categoryName : "";

          let temp: CashBookModel = new CashBookModel();
          temp.id = e.payableInvoiceId;
          temp.code = e.payableInvoiceCode;
          temp.createdDate = e.createdDate;
          temp.cashDate = e.paidDate;
          temp.createdName = e.createdByName;
          temp.reason = e.payableInvoiceReasonText;
          temp.type = typeBookName;
          temp.content = e.payableInvoiceDetail;
          temp.payableMoney = e.payableInvoicePrice;
          temp.note = e.payableInvoiceNote;
          temp.route = 'payable';

          this.listCashBook = [...this.listCashBook, temp];
        });
        break;
      default:
        break;
    }
  }

  gotoView(element: CashBookModel) {
    if (element.route === 'receipt') {
      this.router.navigate(['/accounting/cash-receipts-view', element.id]);
    } else {
      this.router.navigate(['/accounting/cash-payments-view', element.id]);
    }

  }
  changeFinancial(event) {
    // this.arrayUnitMoneySelected = event.value;
  }

  refreshFilter() {
    this.filterGlobal = '';
    this.listCashBook = [];
    this.organizationIds = [];
    this.createdByIds = [];
    this.fromDate = null;
    this.toDate = null;   
    this.getData();
  }

  leftColNumber: number = 12;
  rightColNumber: number = 0;
  colSumarySection: number = 3;
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

  dateFieldFormat: string = 'DD/MM/YYYY';
  customSort(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      /**Customize sort date */
      if (event.field == 'orderDate') {
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
}

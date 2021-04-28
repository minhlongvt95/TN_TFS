import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { FormControl, FormGroup } from '@angular/forms';
import { CategoryService } from '../../../../shared/services/category.service';
import { EmployeeService } from '../../../../employee/services/employee.service';

import { AccountingService } from '../../../services/accounting.service';
import { VendorService } from '../../../../vendor/services/vendor.service';
import { CustomerService } from '../../../../customer/services/customer.service';
import { GetPermission } from '../../../../shared/permission/get-permission';

import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';


class SubjectModel {
  subjectId: string;
  subjectName: string;
  subjectType: string;
}

interface BankReceiptInvoice {
  bankReceiptInvoiceId: string,
  bankReceiptInvoiceCode: string,
  bankReceiptInvoiceDetail: string,
  bankReceiptInvoicePrice: number,
  bankReceiptInvoicePriceCurrency: string,
  bankReceiptInvoiceExchangeRate: number,
  bankReceiptInvoiceReason: string,
  bankReceiptInvoiceNote: string,
  bankReceiptInvoiceBankAccountId: string,
  bankReceiptInvoiceAmount: number,
  bankReceiptInvoiceAmountText: string,
  bankReceiptInvoicePaidDate: string,
  organizationId: string,
  statusId: string,
  active: boolean,
  createdById: string,
  createdDate: string,
  updatedById: string,
  updatedDate: string,
  bankReceiptInvoiceReasonText: string,
  createdByName: string,
  avatarUrl: string,
  objectName: string,
  statusName: string,
  backgroundColorForStatus: string,
}

@Component({
  selector: 'app-bank-receipts-list',
  templateUrl: './bank-receipts-list.component.html',
  styleUrls: ['./bank-receipts-list.component.css'],
  providers: [
    DatePipe
  ]
})

export class BankReceiptsListComponent implements OnInit {

  listBankReceiptsInvoice: Array<BankReceiptInvoice> = [];
  filterForm: FormGroup;
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  auth: any = JSON.parse(localStorage.getItem("auth"));

  actionAdd: boolean = true;
  actionDownload: boolean = true;
  loading: boolean = false;
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();
  repperList: Array<any> = [];
  repper: string = '';
  filterGlobal: string;

  leftColNumber: number = 12;
  rightColNumber: number = 0;

  listReason: Array<any> = [];
  listStatus: Array<any> = [];
  listEmployee: Array<any> = [];

  bankReceiptInvoiceCode: string = '';
  bankReceiptInvoiceReasonIds: any;
  objectIds: Array<any> = [];
  createdByIds: Array<any> = [];
  fromDate: Date = null;
  toDate: Date = null;
  statusIds: Array<any> = [];
  nowDate: Date = new Date();

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  @ViewChild('myTable') myTable: Table;
  colsList: any;
  selectedColumns: any[];
  frozenCols: any[];
  constructor(
    private router: Router,
    private translate: TranslateService,
    private getPermission: GetPermission,
    private accountingService: AccountingService,
    private route: ActivatedRoute,
    private categoryService: CategoryService,
    private vendorService: VendorService,
    private employeeService: EmployeeService,
    private customerService: CustomerService,
    private datePipe: DatePipe,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "acc/accounting/bank-receipts-list/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.initTable();
      this.accountingService.getMasterDataSearchBankReceiptInvoice().subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.listReason = result.reasonOfPaymentList;
          this.listStatus = result.statusOfPaymentList;
          this.listEmployee = result.employeeList;
          this.listEmployee.forEach(item => {
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;
          });
          this.searchBankReceiptInvoice();
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  initTable() {
    this.colsList = [
      { field: 'bankReceiptInvoiceCode', header: 'Mã chứng từ', textAlign: 'left', display: 'table-cell' },
      { field: 'bankReceiptInvoiceReasonText', header: 'Lý do thu', textAlign: 'left', display: 'table-cell' },
      { field: 'objectName', header: 'Đối tượng thu', textAlign: 'left', display: 'table-cell' },
      { field: 'bankReceiptInvoiceDetail', header: 'Nội dung thu', textAlign: 'left', display: 'table-cell' },
      { field: 'bankReceiptInvoiceAmount', header: 'Số tiền', textAlign: 'right', display: 'table-cell' },
      { field: 'createdDate', header: 'Ngày tạo', textAlign: 'right', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
      { field: 'createdByName', header: 'Người tạo', textAlign: 'left', display: 'table-cell' },
    ];

    this.selectedColumns = this.colsList;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  searchBankReceiptInvoice() {
    if(this.bankReceiptInvoiceCode){
      this.bankReceiptInvoiceCode = this.bankReceiptInvoiceCode.trim();
    }
    let listBankReceiptInvoiceReasonIds: Array<string> = [];
    if (this.bankReceiptInvoiceReasonIds) {
      listBankReceiptInvoiceReasonIds.push(this.bankReceiptInvoiceReasonIds.categoryId);
    } else {
      listBankReceiptInvoiceReasonIds = this.bankReceiptInvoiceReasonIds;
    }

    let listObjectIds: Array<string> = [];
    if (this.repper === 'TVI') {
      listObjectIds = this.objectIds.map(x => x.employeeId);
    } else if (this.repper === 'TTA') {
      listObjectIds = this.objectIds.map(x => x.vendorId);
    } else if (this.repper === 'THA') {
      listObjectIds = this.objectIds.map(x => x.customerId);
    } else {
      listObjectIds = this.objectIds.map(x => x.objectIds);
    }

    let listCreatedByIds: Array<string> = [];
    listCreatedByIds = this.createdByIds.map(x => x.employeeId);
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

    let listStatusIds: Array<string> = [];
    listStatusIds = this.statusIds.map(x => x.categoryId);

    this.loading = true;
    this.accountingService.searchBankReceiptInvoice(this.auth.userId, this.bankReceiptInvoiceCode, listBankReceiptInvoiceReasonIds,
      listCreatedByIds, fromDate, toDate, listStatusIds, listObjectIds).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.listBankReceiptsInvoice = result.bankReceiptInvoiceList;

          if (this.listBankReceiptsInvoice.length == 0) {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy báo có nào!' };
            this.showMessage(msg);
          } else {
            this.listBankReceiptsInvoice.forEach(item => {
              item.createdDate = this.datePipe.transform(item.createdDate, 'dd/MM/yyyy');
            });
          }
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
  }

  refreshFilter() {
    this.bankReceiptInvoiceCode = '';
    this.bankReceiptInvoiceReasonIds = null;
    this.repper = '';
    this.objectIds = [];
    this.fromDate = null;
    this.toDate = null;
    this.statusIds = [];
    this.createdByIds = [];
    this.filterGlobal = '';
    this.listBankReceiptsInvoice = [];
    
    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.searchBankReceiptInvoice();
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 9;
        this.rightColNumber = 3;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
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

  changeReasonPay(value: any) {
    this.repperList = [];
    this.loading = true;
    this.repper = '';
    this.objectIds = [];
    this.categoryService.getCategoryById(value.categoryId).subscribe(response => {
      const result = <any>response;
      this.loading = false;
      this.repper = result.category.categoryCode;
      if (this.repper === 'TVI') {
        this.loading = true;
        this.employeeService.searchEmployee('', '', '', '', [], '').subscribe(response1 => {
          const result1 = <any>response1;
          this.loading = false;
          this.repperList = result1.employeeList;
          this.repperList.forEach(item => {
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;
          });
        }, error => { });
      } else if (this.repper === 'TTA') {
        this.loading = true;
        this.vendorService.getAllVendorToPay().subscribe(response2 => {
          const result2 = <any>response2;
          this.loading = false;
          this.repperList = result2.vendorList;
          this.repperList.forEach(item => {
            item.vendorName = item.vendorCode + ' - ' + item.vendorName;
          });
        }, error => { });
      } else if (this.repper === 'THA') {
        this.loading = true;
        this.customerService.getAllCustomer().subscribe(response3 => {
          const result3 = <any>response3;
          this.loading = false;
          this.repperList = result3.customerList;
          this.repperList.forEach(item => {
            item.customerName = item.customerCode + ' - ' + item.customerName;
          });
        }, error => { });
      } else {
        this.repperList = [];
        this.repper = '';
      }
    }, error => { });
  }

  goToDetail(rowData: any) {
    this.router.navigate(['/accounting/bank-receipts-detail', { id: rowData.bankReceiptInvoiceId }]);
  }

  goToCreate() {
    this.router.navigate(['/accounting/bank-receipts-create'])
  }
}
function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};


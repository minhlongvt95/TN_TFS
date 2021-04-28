import { Component, OnInit, ViewChild} from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { FormGroup } from '@angular/forms';
import { CategoryService } from "../../../../shared/services/category.service";
import { EmployeeService } from "../../../../employee/services/employee.service";
import { AccountingService } from "../../../services/accounting.service";
import { VendorService } from "../../../../vendor/services/vendor.service";
import { CustomerService } from "../../../../customer/services/customer.service";
import { GetPermission } from '../../../../shared/permission/get-permission';

import { MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';

interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string
}

interface BankPayableInvoice {
  bankPayableInvoiceId: string,
  bankPayableInvoiceCode: string,
  bankPayableInvoiceDetail: string,
  bankPayableInvoicePrice: number,
  bankPayableInvoicePriceCurrency: string,
  bankPayableInvoiceExchangeRate: number,
  bankPayableInvoiceReason: string,
  bankPayableInvoiceNote: string,
  bankPayableInvoiceBankAccountId: string,
  bankPayableInvoiceAmount: number,
  bankPayableInvoiceAmountText: string,
  bankPayableInvoicePaidDate: string,
  organizationId: string,
  statusId: string,
  receiveAccountNumber: string,
  receiveAccountName: string,
  receiveBankName: string,
  receiveBranchName: string,
  active: boolean,
  createdById: string,
  createdDate: string,
  updatedById: string,
  updatedDate: string,
  bankPayableInvoiceReasonText: string,
  createdByName: string,
  avatarUrl: string,
  objectName: string,
  statusName: string,
  backgroundColorForStatus : string,
}
@Component({
  selector: 'app-bank-payments-list',
  templateUrl: './bank-payments-list.component.html',
  styleUrls: ['./bank-payments-list.component.css'],
  providers: [
    DatePipe
  ]
})
export class BankPaymentsListComponent implements OnInit {
  listBankPayableInvoice: Array<BankPayableInvoice> = [];
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
  payerList : Array<any> = [];
  payer : string = '';
  filterGlobar : string;

  listReason: Array<any> = [];
  listStatus: Array<any> = [];
  listEmployee: Array<any> = [];

  bankPayableInvoiceCode: string = '';
  bankPayableInvoiceReasonIds: any;
  objectIds: Array<any> = [];
  createdByIds: Array<any> = [];
  fromDate: Date = null;
  toDate: Date = null;
  statusIds: Array<any> = [];
  nowDate : Date = new Date();

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
    ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "acc/accounting/bank-payments-list/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownload = false;
      }

      this.initTable();

      this.accountingService.getMasterDataSearchBankPayableInvoice().subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.listReason = result.reasonOfPaymentList;
          this.listStatus = result.statusOfPaymentList;
          this.listEmployee = result.employeeList;
          this.listEmployee.forEach(item =>{
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;
          });
          this.searchPayableInvoice();
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  initTable() {

    // this.frozenCols = [
      
    // ];
    this.colsList = [
      { field: 'bankPayableInvoiceCode', header: 'Mã chứng từ', textAlign: 'left', display: 'table-cell'},
      { field: 'bankPayableInvoiceReasonText', header: 'Lý do chi', textAlign: 'left', display: 'table-cell'},
      { field: 'objectName', header: 'Đối tượng chi', textAlign: 'left', display: 'table-cell'},
      { field: 'bankPayableInvoiceDetail', header: 'Nội dung chi', textAlign: 'left', display: 'table-cell' },
      { field: 'bankPayableInvoiceAmount', header: 'Số tiền', textAlign: 'right', display: 'table-cell'},
      { field: 'createdDate', header: 'Ngày tạo', textAlign: 'right', display: 'table-cell'},
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
      { field: 'createdByName', header: 'Người tạo', textAlign: 'left', display: 'table-cell'},
    ];

    this.selectedColumns = this.colsList;

  }

  goToCreate() {
    this.router.navigate(['/accounting/bank-payments-create']);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  searchPayableInvoice() {
    let listBankPayableInvoiceReasonIds: Array<string> = [];
    if(this.bankPayableInvoiceReasonIds){
      listBankPayableInvoiceReasonIds.push(this.bankPayableInvoiceReasonIds.categoryId);
    }else{
      listBankPayableInvoiceReasonIds = this.bankPayableInvoiceReasonIds;
    }
    
    let listObjectIds: Array<string> = [];
    if(this.payer === 'CVI'){
      listObjectIds = this.objectIds.map(x => x.employeeId);
    }else if(this.payer == 'CTA'){
      listObjectIds = this.objectIds.map(x => x.vendorId);
    }else if(this.payer === 'CHA'){
      listObjectIds = this.objectIds.map(x => x.customerId);
    }else{
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
    this.accountingService.searchBankPayableInvoice(this.auth.userId, this.bankPayableInvoiceCode, listBankPayableInvoiceReasonIds,
      listCreatedByIds, fromDate, toDate, listStatusIds, listObjectIds).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.listBankPayableInvoice = result.bankPayableInvoiceList;
          if (this.listBankPayableInvoice.length == 0) {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy ủy nhiệm chi nào!' };
            this.showMessage(msg);
          } else {
            this.listBankPayableInvoice.forEach(item => {
              item.createdDate = this.datePipe.transform(item.createdDate, "dd/MM/yyyy");
            });
          }
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
  }

  refreshFilter() {
    this.bankPayableInvoiceCode = '';
    this.bankPayableInvoiceReasonIds = null;
    this.objectIds = [];
    this.fromDate = null;
    this.toDate = null;
    this.statusIds = [];
    this.createdByIds = [];
    this.payer = '';
    this.listBankPayableInvoice = [];
    this.filterGlobar = '';
    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.searchPayableInvoice();
  }
  leftColNumber: number = 12;
  rightColNumber: number = 0;

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
  
  changeReasonPay(value: any) {
    this.payerList = [];
    this.loading = true;
    this.payer = '';
    this.objectIds = [];    
    this.categoryService.getCategoryById(value.categoryId).subscribe(response => {
      const result = <any>response;
      this.loading = false;
      this.payer = result.category.categoryCode;
      if (this.payer === 'CVI') {
        this.loading = true;        
        this.employeeService.searchEmployee('', '', '', '', [], '').subscribe(response1 => {
          const result1 = <any>response1;
          this.loading = false;
          this.payerList = result1.employeeList;
          this.payerList.forEach(item => {
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;
          });
        }, error => { });
      } else if (this.payer === 'CTA') {
        this.loading = true;
        this.vendorService.getAllVendorToPay().subscribe(response2 => {
          const result2 = <any>response2;
          this.loading = false;
          this.payerList = result2.vendorList;
          this.payerList.forEach(item => {
            item.vendorName = item.vendorCode + ' - ' + item.vendorName;
          });
        }, error => { });
      } else if (this.payer === 'CHA') {
        this.loading = true;
        this.customerService.getAllCustomer().subscribe(response3 => {
          const result3 = <any>response3;
          this.loading = false;
          this.payerList = result3.customerList;
          this.payerList.forEach(item => {
            item.customerName = item.customerCode + ' - ' + item.customerName;
          });
        }, error => { });
      }else{
        this.payerList = [];
        this.payer = ''; 
      } 
    }, error => { });
  }
  
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


  goToDetail(rowData: any) {
    this.router.navigate(['/accounting/bank-payments-detail', { id: rowData.bankPayableInvoiceId }]);
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};


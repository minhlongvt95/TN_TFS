import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { FormControl, FormGroup } from '@angular/forms';
import { CategoryService } from "../../../../shared/services/category.service";
import { EmployeeService } from "../../../../employee/services/employee.service";

import { AccountingService } from "../../../services/accounting.service";
import { VendorService } from "../../../../vendor/services/vendor.service";
import { AuthenticationService } from "../../../../shared/services/authentication.service";
import { CustomerService } from "../../../../customer/services/customer.service";
import { GetPermission } from '../../../../shared/permission/get-permission';

import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";

interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string
}

interface ReceiptInvoice {
  receiptInvoiceId: string,
  receiptInvoiceDetail: string,
  receiptInvoiceReason: string,
  receiptInvoiceNote: string,
  registerType: string,
  organizationId: string,
  statusId: string,
  recipientName: string,
  recipientAddress: string,
  unitPrice: number,
  currencyUnit: string,
  exchangeRate: number,
  amount: number,
  amountText: string,
  active: boolean,
  createdById: string,
  createdDate: string,
  updatedById: string,
  updatedDate: string,
  createByAvatarUrl: string,
  nameReceiptInvoiceReason: string,
  nameCreateBy: string,
  receiptDate: string,
  nameObjectReceipt: string,
  receiptInvoiceCode: string,
  vouchersDate: string,
  statusName: string,
}


@Component({
  selector: 'app-cash-receipts-list',
  templateUrl: './cash-receipts-list.component.html',
  styleUrls: ['./cash-receipts-list.component.css'],
  providers: [
    DatePipe,
  ]
})
export class CashReceiptsListComponent implements OnInit {

  listReceiptInvoice: Array<ReceiptInvoice> = [];
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  auth: any = JSON.parse(localStorage.getItem("auth"));
  filterForm: FormGroup;

  filterGoal : string;
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  actionImport: boolean = true;
  loading: boolean = false;
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();
  receipterList: Array<any> = [];
  receipter: string = '';

  listReason: Array<any> = [];
  listStatus: Array<any> = [];
  listEmployee: Array<any> = [];


  receiptInvoiceCode: string = '';
  receiptInvoiceReasonIds: any;
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
    let resource = "acc/accounting/cash-receipts-list/";
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
      if (listCurrentActionResource.indexOf("import") == -1) {
        this.actionImport = false;
      }

      this.initTable();

      this.accountingService.getMasterDataSearchReceiptInvoice().subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.listReason = result.listReason;
          this.listStatus = result.listStatus;
          this.listEmployee = result.listEmployee;
          this.listEmployee.forEach(item => {
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;

          });
          this.searchReceiptInvoice();
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
      { field: 'receiptInvoiceCode', header: 'Mã chứng từ', textAlign: 'left', display: 'table-cell' },
      { field: 'nameReceiptInvoiceReason', header: 'Lý do thu', textAlign: 'left', display: 'table-cell' },
      { field: 'nameObjectReceipt', header: 'Đối tượng thu', textAlign: 'left', display: 'table-cell' },
      { field: 'receiptInvoiceDetail', header: 'Nội dung thu', textAlign: 'left', display: 'table-cell' },
      { field: 'amount', header: 'Số tiền', textAlign: 'right', display: 'table-cell' },
      { field: 'createdDate', header: 'Ngày tạo', textAlign: 'right', display: 'table-cell' },
      { field: 'nameCreateBy', header: 'Người tạo', textAlign: 'left', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsList;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  refreshFilter() {
    this.receiptInvoiceCode = '';
    this.receiptInvoiceReasonIds = null;
    this.objectIds = [];
    this.fromDate = null;
    this.toDate = null;
    this.statusIds = [];
    this.createdByIds = [];
    this.listReceiptInvoice = [];
    this.filterGoal = '';
    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.searchReceiptInvoice();
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
    this.receipterList = [];
    this.loading = true;
    this.receipter = '';
    this.objectIds = [];
    this.categoryService.getCategoryById(value.categoryId).subscribe(response => {
      const result = <any>response;
      this.loading = false;
      this.receipter = result.category.categoryCode;
      if (this.receipter === 'TVI') {
        this.loading = true;
        this.employeeService.searchEmployee('', '', '', '', [], '').subscribe(response1 => {
          const result1 = <any>response1;
          this.loading = false;
          this.receipterList = result1.employeeList;
          this.receipterList.forEach(item => {
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;
          });
        }, error => { });
      } else if (this.receipter === 'TTA') {
        this.loading = true;
        this.vendorService.getAllVendorToPay().subscribe(response2 => {
          const result2 = <any>response2;
          this.loading = false;
          this.receipterList = result2.vendorList;
          this.receipterList.forEach(item => {
            item.vendorName = item.vendorCode + ' - ' + item.vendorName;
          });
        }, error => { });
      } else if (this.receipter === 'THA') {
        this.loading = true;
        this.customerService.getAllCustomer().subscribe(response3 => {
          const result3 = <any>response3;
          this.loading = false;
          this.receipterList = result3.customerList;
          this.receipterList.forEach(item => {
            item.customerName = item.customerCode + ' - ' + item.customerName;
          });
        }, error => { });
      } else {
        this.receipterList = [];
        this.receipter = '';
      }
    }, error => { });
  }

  searchReceiptInvoice() {
    let listReceiptInvoiceReasonIds: Array<string> = [];
    if (this.receiptInvoiceReasonIds) {
      listReceiptInvoiceReasonIds.push(this.receiptInvoiceReasonIds.categoryId);
    } else {
      listReceiptInvoiceReasonIds = this.receiptInvoiceReasonIds;
    }

    let listObjectIds: Array<string> = [];
    if (this.receipter == 'TVI') {
      listObjectIds = this.objectIds.map(x => x.employeeId);
    } else if (this.receipter == 'TTA') {
      listObjectIds = this.objectIds.map(x => x.vendorId);
    } else if (this.receipter == 'THA') {
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
    this.accountingService.searchReceiptInvoice(this.receiptInvoiceCode, listReceiptInvoiceReasonIds, listCreatedByIds,
      fromDate, toDate, listStatusIds, listObjectIds, this.auth.userId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.listReceiptInvoice = result.lstReceiptInvoiceEntity;

          if (this.listReceiptInvoice.length == 0) {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy phiếu thu tiền mặt nào!' };
            this.showMessage(msg);
          } else {
            this.listReceiptInvoice.forEach(item => {
              item.createdDate = this.datePipe.transform(item.createdDate, "dd/MM/yyyy");
            });
          }
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });

  }

  checkNull(value: any) {
    if (value) {
      return true;
    } else {
      return false;
    }
  }

  goToDetail(rowData: any) {
    this.router.navigate(['/accounting/cash-receipts-view', { receiptInvoiceId: rowData.receiptInvoiceId }]);
  }

  goToCreate(rowData: any) {
    this.router.navigate(['/accounting/cash-receipts-create']);
  }


  exportExcel() {
    let title = "Danh sách phiếu thu tiền mặt";
    let workBook = new Workbook();
    let worksheet = workBook.addWorksheet(title);
    let dataHeaderRow = ['STT', 'Mã chứng từ', 'Lý do chi', 'Đối tượng chi', 'Nội dụng chi', 'Số tiền', 'Ngày tạo', 'Người tạo', 'Trạng thái'];
    let headerRow = worksheet.addRow(dataHeaderRow);
    headerRow.font = { name: 'Time New Roman', size: 10, bold: true };
    dataHeaderRow.forEach((item, index) => {
      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });
    headerRow.height = 40;

    let data: Array<any> = [];

    this.listReceiptInvoice.forEach((item, index) => {
      let row: Array<any> = [];
      row[0] = index + 1;
      row[1] = item.receiptInvoiceCode.toString();
      row[2] = item.nameReceiptInvoiceReason.toString();
      row[3] = item.nameObjectReceipt.toString();
      row[4] = item.receiptInvoiceDetail.toString();
      row[5] = item.amount.toString();
      row[6] = item.createdDate.toString();
      row[7] = item.nameCreateBy.toString();
      row[8] = item.statusName.toString();

      data.push(row);
    });

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };

      row.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(1).alignment = { vertical: 'middle', horizontal: 'left' };

      row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(5).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(7).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(8).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(8).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(9).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(9).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    });

    /* fix with for column */
    worksheet.getColumn(1).width = 5;
    worksheet.getColumn(2).width = 25;
    worksheet.getColumn(3).width = 25;
    worksheet.getColumn(4).width = 30;
    worksheet.getColumn(5).width = 30;
    worksheet.getColumn(6).width = 20;
    worksheet.getColumn(7).width = 20;
    worksheet.getColumn(8).width = 25;
    worksheet.getColumn(9).width = 15;

    this.exportToExel(workBook, title);
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

}
function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};


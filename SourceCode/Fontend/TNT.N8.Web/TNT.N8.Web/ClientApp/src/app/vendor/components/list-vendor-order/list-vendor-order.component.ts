import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { Router, ActivatedRoute } from '@angular/router';
import { WarningComponent } from '../../../shared/toast/warning/warning.component';
import { TranslateService } from '@ngx-translate/core';
import { OrderstatusService } from "../../../shared/services/orderstatus.service";
import { VendorService } from "../../services/vendor.service";
import { GetPermission } from '../../../shared/permission/get-permission';
import { DatePipe, formatNumber } from '@angular/common';
import { Table } from 'primeng/table';
import { MessageService, SortEvent } from 'primeng/api';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";

//model dùng để build template excel
class columnModel {
  column1: string;
  column2: string;
}

class companyConfigModel {
  companyName: string;
  companyAddress: string;
  phone: string;
  email: string;
  website: string;
  constructor() {
    this.companyName = '';
    this.companyAddress = '';
    this.phone = '';
    this.email = '';
    this.website = '';
  }
}

interface VendorOrder {
  index: number;
  vendorOrderId: string;
  vendorOrderCode: string;
  vendorOrderDate: string;
  customerOrderId: string;
  orderer: string;
  ordererName: string;
  description: string;
  note: string;
  vendorId: string;
  vendorContactId: string;
  paymentMethod: string;
  bankAccountId: string;
  receivedDate: string;
  receivedHour: string;
  recipientName: string;
  locationOfShipment: string;
  shippingNote: string;
  recipientPhone: string;
  recipientEmail: string;
  placeOfDelivery: string;
  amount: number;
  discountValue: number;
  statusId: string;
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  active: boolean;
  discountType: boolean;
  createdByName: string;
  avatarUrl: string;
  statusName: string;
  vendorName: string;
  vendorCode: string;
  backgroundColorForStatus: string;
  ListProcurementName: string;
}

class exportExcelModel {
  vendorOrderDate: string;
  vendorOrderCode: string;
  vendorCode: string;
  vendorName: string;
  description: string;
  amount: string;
  statusName: string;
}

@Component({
  selector: 'app-list-vendor-order',
  templateUrl: './list-vendor-order.component.html',
  styleUrls: ['./list-vendor-order.component.css'],
  providers: [
    DatePipe
  ]
})
export class ListVendorOrderComponent implements OnInit {

  listVendorOrder: Array<VendorOrder> = [];
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
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

  lisOrderStatus: Array<any> = [];
  listVendor: Array<any> = [];
  listEmployee: Array<any> = [];
  listProcurementRequest: Array<any> = [];
  listProduct: Array<any> = [];

  leftColNumber: number = 12;
  rightColNumber: number = 0;
  filterGlobal: string;
  vendors: Array<any> = [];
  vendorOrderCode: string = '';
  fromDate: Date = null;
  toDate: Date = null;
  orderStatusIds: Array<any> = [];
  createByIds: Array<any> = [];
  listSelectedProcurementRequest: Array<any> = [];
  listSelectedProduct: Array<any> = [];
  nowDate: Date = new Date();

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };
  /*Check user permission*/

  companyConfig = new companyConfigModel();

  @ViewChild('myTable') myTable: Table;
  colsList: any;
  selectedColumns: any[];
  frozenCols: any[];
  selectedVendorOrder: Array<VendorOrder> = [];

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private vendorService: VendorService,
    public snackBar: MatSnackBar,
    private router: Router,
    private datePipe: DatePipe,
    private messageService: MessageService,
  ) {
    this.translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "buy/vendor/list-order/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.snackBar.openFromComponent(WarningComponent, { data: "Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ", ...this.warningConfig });
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      this.initTable();

      this.vendorService.getMasterDataSearchVendorOrder().subscribe(response => {
        let result: any = response;
        if (result.statusCode == 200) {
          this.listVendor = result.listVendor;
          this.lisOrderStatus = result.listOrderStatus;
          this.listVendor.forEach(item => {
            item.vendorName = item.vendorCode + ' - ' + item.vendorName;
          });
          this.listEmployee = result.listEmployee;
          this.listEmployee.forEach(item => {
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;
          });
          this.listProcurementRequest = result.listProcurementRequest;
          this.listProduct = result.listProduct;
          this.companyConfig = result.companyConfig;
          this.searchVendorOrder();
        } else {
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
      { field: 'vendorOrderCode', header: 'Mã', width: '100px', textAlign: 'left', display: 'table-cell' },
      // { field: 'createdDate', header: 'Ngày tạo', width: '110px', textAlign: 'right', display: 'table-cell' },
      { field: 'vendorOrderDate', header: 'Ngày đặt hàng', width: '150px', textAlign: 'right', display: 'table-cell' },
      { field: 'vendorName', header: 'Tên nhà cung cấp', width: '160px', textAlign: 'left', display: 'table-cell' },
      { field: 'description', header: 'Diễn giải', width: '150px', textAlign: 'left', display: 'table-cell' },
      // { field: '', header: 'Tổng tiền chiết khấu', width: '150px', textAlign: 'left', display: 'table-cell' },
      { field: 'amount', header: 'Tổng tiền thanh toán', width: '180px', textAlign: 'right', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái', width: '150px', textAlign: 'center', display: 'table-cell' },
      { field: 'ordererName', header: 'Người tạo', width: '120px', textAlign: 'left', display: 'table-cell' },
      { field: 'listProcurementName', header: 'Số đề xuất', width: '150px', textAlign: 'left', display: 'table-cell' },
    ];

    this.selectedColumns = this.colsList;
  }

  // setIndex(){
  //   this.listVendorOrder.forEach((item, index) =>{
  //     item.index = index + 1;
  //   });
  // }

  searchVendorOrder() {
    if (this.vendorOrderCode) {
      this.vendorOrderCode = this.vendorOrderCode.trim();
    }

    let listVendorId: Array<string> = [];
    listVendorId = this.vendors.map(c => c.vendorId);

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

    let listOrderStatusId: Array<string> = [];
    listOrderStatusId = this.orderStatusIds.map(c => c.purchaseOrderStatusId);

    let createByIds: Array<string> = [];
    createByIds = this.createByIds.map(c => c.employeeId);
    let listProcurementRequestId: Array<string> = this.listSelectedProcurementRequest.map(x => x.procurementRequestId);
    let listProductId: Array<string> = this.listSelectedProduct.map(x => x.productId);
    this.loading = true;
    this.vendorService.searchVendorOrder(listVendorId, this.vendorOrderCode, fromDate, toDate,
      listOrderStatusId, createByIds, this.auth.UserId, listProcurementRequestId, listProductId
    ).subscribe(response => {
      var result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listVendorOrder = result.vendorOrderList;

        // this.setIndex();
        if (this.listVendorOrder.length == 0) {
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy phiếu đặt SP/DV nào!' };
          this.showMessage(msg);
        } else {
          // this.listVendorOrder.forEach(item => {
          //   item.createdDate = this.datePipe.transform(item.createdDate, 'dd/MM/yyyy');
          //   item.vendorOrderDate = this.datePipe.transform(item.vendorOrderDate, 'dd/MM/yyyy');
          // });
        }
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  refreshFilter() {
    this.vendorOrderCode = '';
    this.vendors = [];
    this.fromDate = null;
    this.toDate = null;
    this.orderStatusIds = [];
    this.createByIds = [];
    this.filterGlobal = '';
    this.listVendorOrder = [];
    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.searchVendorOrder();
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
      if (event.field == 'vendorOrderDate') {
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

  exportExcel() {
    if (this.selectedVendorOrder.length > 0) {
      let title = `Danh sách đơn hàng mua`;

      let workBook = new Workbook();
      let worksheet = workBook.addWorksheet(title);

      //LOGO - infor section
      let imgBase64 = this.getBase64Logo();
      var imgLogo = workBook.addImage({
        base64: imgBase64,
        extension: 'png',
      });
      worksheet.addImage(imgLogo, {
        tl: { col: 0.5, row: 0.5 },
        ext: { width: 140, height: 95 }
      });

      let dataInforRow_1 = `${this.companyConfig.companyName}`
      let dataInforRow_2 = `Địa chỉ: ${this.companyConfig.companyAddress}`;
      let dataInforRow_3 = `Điện thoại: ${this.companyConfig.phone}`;
      let dataInforRow_4 = `Email: ${this.companyConfig.email}`;
      let dataInforRow_5 = `Website dịch vụ: `

      let inforRow_1 = worksheet.addRow(['', '', dataInforRow_1]);
      let inforRow_2 = worksheet.addRow(['', '', dataInforRow_2]);
      let inforRow_3 = worksheet.addRow(['', '', dataInforRow_3]);
      let inforRow_4 = worksheet.addRow(['', '', dataInforRow_4]);
      let inforRow_5 = worksheet.addRow(['', '', dataInforRow_5]);

      inforRow_1.font = { size: 14, bold: true };
      worksheet.mergeCells(`C${1}:E${1}`);
      worksheet.mergeCells(`C${2}:${2}`);
      worksheet.mergeCells(`C${3}:E${3}`);
      worksheet.mergeCells(`C${4}:E${4}`);
      worksheet.mergeCells(`C${5}:E${5}`);

      worksheet.addRow([]);
      /* title */
      let dataHeaderMain = "Danh sách đơn hàng mua".toUpperCase();
      let headerMain = worksheet.addRow([dataHeaderMain]);
      headerMain.font = { size: 18, bold: true };
      worksheet.mergeCells(`A${7}:G${7}`);
      headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      worksheet.addRow([]);

      let listExcelColumns = this.getListExcelColumns();
      let dataHeaderRow1: Array<string> = listExcelColumns.map(e => e.column1);
      let dataHeaderRow2: Array<string> = listExcelColumns.map(e => e.column2);
      let headerRow1 = worksheet.addRow(dataHeaderRow1);
      let headerRow2 = worksheet.addRow(dataHeaderRow2);
      headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
      headerRow2.font = { name: 'Time New Roman', size: 10, bold: true };

      //merge header column
      worksheet.mergeCells(`A${9}:B${9}`);

      worksheet.mergeCells(`C${9}:C${10}`);
      worksheet.mergeCells(`D${9}:D${10}`);
      worksheet.mergeCells(`E${9}:E${10}`);
      worksheet.mergeCells(`F${9}:F${10}`);
      worksheet.mergeCells(`G${9}:G${10}`);

      headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
      dataHeaderRow1.forEach((item, index) => {
        headerRow1.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow1.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow1.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      });

      headerRow2.font = { name: 'Time New Roman', size: 10, bold: true };
      dataHeaderRow2.forEach((item, index) => {
        headerRow2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        headerRow2.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      });

      let data: Array<any> = [];
      let dataExportExcel: Array<exportExcelModel> = this.getDataExportExcel(this.selectedVendorOrder);
      //tính tổng thành tiền
      let sumAmount = 0;
      this.selectedVendorOrder.forEach(order => sumAmount += order.amount);

      dataExportExcel?.forEach((item, index) => {
        let row: Array<any> = [];
        row[0] = item.vendorOrderDate;
        row[1] = item.vendorOrderCode;
        row[2] = item.vendorCode;
        row[3] = item.vendorName;
        row[4] = item.description;
        row[5] = item.amount;
        row[6] = item.statusName;

        data.push(row);
      });

      data.forEach((el, index, array) => {
        let row = worksheet.addRow(el);
        row.font = { name: 'Times New Roman', size: 11 };

        row.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

        row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

        row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

        row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(5).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

        row.getCell(7).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        row.getCell(7).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      });

      let summaryRowData: Array<any> = [];
      summaryRowData[0] = '';
      summaryRowData[1] = '';
      summaryRowData[2] = '';
      summaryRowData[3] = '';
      summaryRowData[4] = 'Tổng cộng';
      summaryRowData[5] = formatNumber(Number(sumAmount), 'en-US', '1.0-5');;
      summaryRowData[6] = '';

      let summaryRow = worksheet.addRow(summaryRowData);
      summaryRow.font = { name: 'Times New Roman', size: 11, bold: true };
      summaryRow.getCell(6).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      /* fix with for column */
      worksheet.getColumn(1).width = 15;
      worksheet.getColumn(2).width = 15;
      worksheet.getColumn(3).width = 15;
      worksheet.getColumn(4).width = 30;
      worksheet.getColumn(5).width = 30;
      worksheet.getColumn(6).width = 20;
      worksheet.getColumn(7).width = 15;

      this.exportToExel(workBook, title);
    }
    else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Chưa có đơn hàng mua được chọn' };
      this.showMessage(msg);
    }
  }

  getDataExportExcel(listVendorOrder: Array<VendorOrder>) {
    let result = [];
    listVendorOrder?.forEach(order => {
      let newItem = new exportExcelModel();
      newItem.vendorOrderDate = this.datePipe.transform(order.vendorOrderDate, 'dd/MM/yyyy');
      newItem.vendorOrderCode = order.vendorOrderCode;
      newItem.vendorCode = order.vendorCode;
      newItem.vendorName = order.vendorName;
      newItem.description = order.description;
      newItem.amount = formatNumber(Number(order.amount), 'en-US', '1.0-5');
      newItem.statusName = order.statusName;

      result = [...result, newItem];
    });

    return result;
  }

  goToCreate() {
    this.router.navigate(['/vendor/create-order']);
  }

  onViewDetail(rowData: any) {
    this.router.navigate(['/vendor/detail-order', { vendorOrderId: rowData.vendorOrderId }]);
  }

  getListExcelColumns(): Array<columnModel> {
    let listColumns: Array<columnModel> = [];

    /* cột ngày */
    let columGroup_1 = new columnModel();
    columGroup_1.column1 = "Chứng từ";
    columGroup_1.column2 = "Ngày";

    let columGroup_2 = new columnModel();
    columGroup_2.column1 = "";
    columGroup_2.column2 = "Số";

    let columGroup_3 = new columnModel();
    columGroup_3.column1 = "Mã NCC";
    columGroup_3.column2 = "";

    let columGroup_4 = new columnModel();
    columGroup_4.column1 = "Tên nhà cung cấp";
    columGroup_4.column2 = "";

    let columGroup_5 = new columnModel();
    columGroup_5.column1 = "Diễn giải";
    columGroup_5.column2 = "";

    let columGroup_6 = new columnModel();
    columGroup_6.column1 = "Thành tiền";
    columGroup_6.column2 = "";

    let columGroup_7 = new columnModel();
    columGroup_7.column1 = "Trạng thái";
    columGroup_7.column2 = "";

    listColumns = [columGroup_1, columGroup_2, columGroup_3, columGroup_4, columGroup_5,
      columGroup_6, columGroup_7];

    return listColumns;
  }

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo?.systemValueString;
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


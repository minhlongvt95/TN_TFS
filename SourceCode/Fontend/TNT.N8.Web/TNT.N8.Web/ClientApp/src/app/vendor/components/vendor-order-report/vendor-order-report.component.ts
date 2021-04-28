import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { VendorService } from '../../services/vendor.service';
import { Router } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { Table } from 'primeng/table';
import { MessageService, SortEvent } from 'primeng/api';
import * as moment from 'moment';
import { DatePipe } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { formatNumber } from '@angular/common';
import * as pdfMake from 'pdfmake/build/pdfmake.js';
import * as pdfFonts from 'pdfmake/build/vfs_fonts.js';
pdfMake.vfs = pdfFonts.pdfMake.vfs;

class Vendor {
  vendorId: string;
  vendorCode: string;
  vendorName: string;
}

class PurchaseOrderStatus {
  purchaseOrderStatusId: string;
  description: string;
}

class ProcurementRequest {
  procurementRequestId: string;
  procurementCode: string;
}

class Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
}

//model dùng để build template excel
class columnModel {
  column1: string;
  column2: string;
}

class exportExcelModel {
  vendorOrderDate: string;
  vendorOrderCode: string;
  productName: string;
  unitName: string;
  priceWarehouse: string;
  priceValueWarehouse: Number;
  quantity: string;
  quantity_1: string;
  quantity_2: string;
  quantity_3: string;
  quantity_4: string;
  statusName: string;
  isSummary: boolean;
}

@Component({
  selector: 'app-vendor-order-report',
  templateUrl: './vendor-order-report.component.html',
  styleUrls: ['./vendor-order-report.component.css']
})
export class VendorOrderReportComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  loading: boolean = false;
  innerWidth: number = 0; //number window size first

  listVendorOrderReport: Array<any> = [];
  nowDate: Date = new Date();
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  isShowFilterUpTop: boolean = false;
  isShowFilterUpLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();
  filterGlobal: string;

  vendorOrderCode: string = '';
  fromDate: Date = null;
  toDate: Date = null;
  productCode: string = '';
  purchaseContractName: string = '';
  description: string = '';

  @ViewChild('myTable') myTable: Table;
  colsList: any;
  selectedColumns: any[];

  // Khai báo các mảng/biến chứa masterData
  listVendor: Array<Vendor> = [];
  listSelectedVendor: Array<Vendor> = [];
  listStatus: Array<PurchaseOrderStatus> = [];
  listSelectedStatus: Array<PurchaseOrderStatus> = [];
  listProcurementRequest: Array<ProcurementRequest> = [];
  listSelectedProcurementRequest: Array<ProcurementRequest> = [];
  listEmployee: Array<Employee> = [];
  listSelectedEmployee: Array<Employee> = [];

  dateFieldFormat: string = 'DD/MM/YYYY';
  leftColNumber: number = 12;
  rightColNumber: number = 0;

  constructor(
    private getPermission: GetPermission,
    private router: Router,
    private messageService: MessageService,
    private translate: TranslateService,
    private datePipe: DatePipe,
    private vendorService: VendorService
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "buy/vendor/vendor-order-report";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      this.initTable();

      this.loading = true;
      this.vendorService.getMasterDataVendorOrderReport().subscribe(response => {
        let result: any = response;
        this.loading = false;

        if (result.statusCode == 200) {
          this.listVendor = result.listVendor;
          this.listStatus = result.listStatus;
          this.listProcurementRequest = result.listProcurementRequest;
          this.listEmployee = result.listEmployee;

          this.searchVendorOrderReport();
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  initTable() {
    this.colsList = [
      { field: 'stt', header: 'STT', textAlign: 'center', display: 'table-cell', width: '90px' },
      { field: 'vendorOrderCode', header: 'Số đơn hàng', textAlign: 'left', display: 'table-cell', width: '130px' },
      { field: 'vendorOrderDate', header: 'Ngày đơn hàng', textAlign: 'right', display: 'table-cell', width: '150px' },
      { field: 'description', header: 'Diễn giải', textAlign: 'left', display: 'table-cell', width: '180px' },
      { field: 'productCode', header: 'Mã hàng', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'productName', header: 'Tên hàng', textAlign: 'left', display: 'table-cell', width: '180px' },
      { field: 'unitName', header: 'Đơn vị tính', textAlign: 'left', display: 'table-cell', width: '120px' },
      { field: 'quantity', header: 'Số lượng đặt hàng', textAlign: 'right', display: 'table-cell', width: '170px' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '150px' },
    ];
    this.selectedColumns = this.colsList;
  }

  searchVendorOrderReport() {
    let vendorOrderCode: string = null;
    let listSelectedVendorId: Array<any> = [];
    let fromDate: Date = null;
    let toDate: Date = null;
    let productCode: string = null;
    let listSelectedStatusId: Array<any> = [];
    let listSelectedProcurementRequestId: Array<any> = [];
    let purchaseContractName: string = null;
    let description: string = null;
    let listSelectedEmployeeId: Array<any> = [];

    if (this.vendorOrderCode) {
      vendorOrderCode = this.vendorOrderCode.trim();
    }

    if (this.listSelectedVendor.length > 0) {
      listSelectedVendorId = this.listSelectedVendor.map(x => x.vendorId);
    }

    if (this.fromDate) {
      fromDate = convertToUTCTime(this.fromDate);
    }

    if (this.toDate) {
      toDate = convertToUTCTime(this.toDate);
    }

    if (this.productCode) {
      productCode = this.productCode.trim();
    }

    if (this.listSelectedStatus.length > 0) {
      listSelectedStatusId = this.listSelectedStatus.map(x => x.purchaseOrderStatusId);
    }

    if (this.listSelectedProcurementRequest.length > 0) {
      listSelectedProcurementRequestId = this.listSelectedProcurementRequest.map(x => x.procurementRequestId);
    }

    if (this.purchaseContractName) {
      purchaseContractName = this.purchaseContractName.trim();
    }

    if (this.description) {
      description = this.description.trim();
    }

    if (this.listSelectedEmployee.length > 0) {
      listSelectedEmployeeId = this.listSelectedEmployee.map(x => x.employeeId);
    }

    this.loading = true;
    this.vendorService.searchVendorOrderReport(vendorOrderCode, listSelectedVendorId, fromDate, toDate,
      productCode, listSelectedStatusId, listSelectedProcurementRequestId, purchaseContractName,
      description, listSelectedEmployeeId).subscribe(response => {
        let result: any = response;
        this.loading = false;

        if (result.statusCode == 200) {
          this.listVendorOrderReport = result.listVendorOrderReport;
          if (this.listVendorOrderReport.length == 0) {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy bản ghi nào' };
            this.showMessage(msg);
          } else {
            this.listVendorOrderReport.forEach(item => {
              item.vendorOrderDate = this.datePipe.transform(item.vendorOrderDate, 'dd/MM/yyyy');
            });
          }
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
  }

  refreshFilter() {
    this.vendorOrderCode = '';
    this.listSelectedVendor = [];
    this.fromDate = null;
    this.toDate = null;
    this.productCode = '';
    this.listSelectedStatus = [];
    this.listSelectedProcurementRequest = [];
    this.purchaseContractName = '';
    this.description = '';
    this.listSelectedEmployee = [];
    this.filterGlobal = '';
    this.myTable.filterGlobal(this.filterGlobal.trim(), 'contains');

    this.searchVendorOrderReport();
  }

  showFilter() {
    this.isShowFilterUpTop = false;
    this.isShowFilterUpLeft = false;
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

  showFilterUp() {
    this.isShowFilterTop = false;
    this.isShowFilterLeft = false;
    if (this.innerWidth < 1024) {
      this.isShowFilterUpTop = !this.isShowFilterUpTop;
    } else {
      this.isShowFilterUpLeft = !this.isShowFilterUpLeft;
      if (this.isShowFilterUpLeft) {
        this.leftColNumber = 9;
        this.rightColNumber = 3;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

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

  goToDetail(value: any) {
    this.router.navigate(['/vendor/detail-order', { vendorOrderId: value.vendorOrderId }])
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  exportExcel() {
    //Nhóm chi tiết sản phẩm, dịch vụ trong đơn hàng theo nhà cung cấp

    let dataExportExcel = this.getDataExportExcel(this.listVendorOrderReport);

    let currentDate = moment(new Date()).format('DD_MM_YYYY');
    let title = `BÁO CÁO TÌNH TRẠNG ĐƠN HÀNG MUA - ${currentDate}`;

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
      ext: { width: 150, height: 95 }
    });

    let dataInforRow_1 = "CÔNG TY TNHH TRÍ NGHĨA";
    let dataInforRow_2 = "Địa chỉ: Lô 10 BT2 Trung Văn, Nam Từ Liêm, Hà Nội";
    let dataInforRow_3 = "Điện thoại: 09130166667";
    let dataInforRow_4 = "Email: info@tringhiatech.vn";
    let dataInforRow_5 = "Website dịch vụ:";
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

    let statusName = '';
    if (this.listSelectedStatus.length == 0 || this.listSelectedStatus.length == this.listStatus.length) {
      statusName = 'Tất cả'
    } else {
      statusName = this.listSelectedStatus.map(e => e.description).join(', ');
    }

    let fromDate = this.fromDate ? moment(new Date(this.fromDate)).format('DD/MM/YYYY') : '';
    let toDate = this.toDate ? moment(new Date(this.toDate)).format('DD/MM/YYYY') : '';

    /* title */
    let dataHeaderMain = "BÁO CÁO TÌNH TRẠNG ĐƠN HÀNG MUA";
    let dataAdditionalInfor_1 = `Trạng thái: ${statusName}`;
    let dataAdditionalInfor_2 = `Từ ngày ${fromDate} - đến ngày ${toDate}`;

    let headerMain = worksheet.addRow([dataHeaderMain]);
    let additionalInfor_1 = worksheet.addRow([dataAdditionalInfor_1]);
    let additionalInfor_2 = worksheet.addRow([dataAdditionalInfor_2]);

    headerMain.font = { size: 18, bold: true };
    worksheet.mergeCells(`A${6}:L${6}`);
    worksheet.mergeCells(`A${7}:L${7}`);
    worksheet.mergeCells(`A${8}:L${8}`);

    headerMain.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    additionalInfor_1.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    additionalInfor_2.getCell(1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    worksheet.addRow([]);

    //data grid
    let listExcelColumns = this.getListExcelColumns();
    let dataHeaderRow1: Array<string> = listExcelColumns.map(e => e.column1);
    let dataHeaderRow2: Array<string> = listExcelColumns.map(e => e.column2);
    let headerRow1 = worksheet.addRow(dataHeaderRow1);
    let headerRow2 = worksheet.addRow(dataHeaderRow2);
    headerRow1.font = { name: 'Time New Roman', size: 10, bold: true };
    headerRow2.font = { name: 'Time New Roman', size: 10, bold: true };
    //merge header column
    worksheet.mergeCells(`A${10}:B${10}`);

    worksheet.mergeCells(`C${10}:C${11}`);
    worksheet.mergeCells(`D${10}:D${11}`);
    worksheet.mergeCells(`E${10}:E${11}`);
    worksheet.mergeCells(`F${10}:F${11}`);
    worksheet.mergeCells(`G${10}:G${11}`);
    worksheet.mergeCells(`H${10}:H${11}`);
    worksheet.mergeCells(`I${10}:I${11}`);
    worksheet.mergeCells(`J${10}:J${11}`);
    worksheet.mergeCells(`K${10}:K${11}`);
    worksheet.mergeCells(`L${10}:L${11}`);

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
    // headerRow1.height = 40;

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
    // headerRow2.height = 40;

    let data: Array<any> = [];

    dataExportExcel.forEach((item, index) => {
      let row: Array<any> = [];
      row[0] = item.vendorOrderDate ? item.vendorOrderDate.toString() : ""; //ngay
      row[1] = item.vendorOrderCode ? item.vendorOrderCode.toString() : ""; //so
      row[2] = item.productName ? item.productName.toString() : ""; //vat tu
      row[3] = item.unitName ? item.unitName.toString() : ""; //don vi tinh
      row[4] = item.priceWarehouse ? formatNumber(Number(item.priceWarehouse), 'en-US', '1.0-0') : ""; //don gia
      row[5] = item.priceValueWarehouse ? formatNumber(Number(item.priceValueWarehouse), 'en-US', '1.0-0') : "";//thanh tien
      row[6] = item.quantity ? formatNumber(Number(item.quantity), 'en-US', '1.0-0') : ""; //so luong
      row[7] = item.quantity_1; //sl da nhan
      row[8] = item.quantity_2; //sl hoa don
      row[9] = item.quantity_3; //sl tra lai
      row[10] = item.quantity_4; //sl con lai
      row[11] = item.statusName ? item.statusName : ""; //sl con lai
      data.push(row);
    });

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };

      if (dataExportExcel[index].isSummary) row.font.bold = true;

      row.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };

      row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(7).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(8).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(9).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(10).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(11).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(12).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(12).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
    });

    //sign reagion
    worksheet.addRow([]);

    let signData_1 = 'Ngày....... tháng ........năm ................';
    let signData_2 = 'NGƯỜI LẬP';
    let signData_3 = '(Ký, họ tên)';

    let signRow_1 = worksheet.addRow(['', '', '', '', '', '', '', '', '', signData_1]);
    let signRow_2 = worksheet.addRow(['', '', '', '', '', '', '', '', '', signData_2]);
    let signRow_3 = worksheet.addRow(['', '', '', '', '', '', '', '', '', signData_3]);

    signRow_1.getCell(10).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    signRow_2.getCell(10).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    signRow_3.getCell(10).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    worksheet.mergeCells(`J${signRow_1.number}:L${signRow_1.number}`);
    worksheet.mergeCells(`J${signRow_2.number}:L${signRow_2.number}`);
    worksheet.mergeCells(`J${signRow_3.number}:L${signRow_3.number}`);

    /* fix with for column */
    worksheet.getColumn(1).width = 15;
    worksheet.getColumn(2).width = 20;
    worksheet.getColumn(3).width = 30;
    worksheet.getColumn(4).width = 10;
    worksheet.getColumn(5).width = 15;
    worksheet.getColumn(6).width = 15;
    worksheet.getColumn(7).width = 15;
    worksheet.getColumn(8).width = 15;
    worksheet.getColumn(9).width = 15;
    worksheet.getColumn(10).width = 15;
    worksheet.getColumn(11).width = 15;
    worksheet.getColumn(12).width = 15;

    this.exportToExel(workBook, title);
  }

  getDataExportExcel(listVendorOrderReport) {

    listVendorOrderReport.sort((a, b) => (a.vendorOrderCode < b.vendorOrderCode) ? 1 : ((b.vendorOrderCode < a.vendorOrderCode) ? -1 : 0));

    let uniqueVendorOrderCode = listVendorOrderReport.map(item => item.vendorOrderCode).filter((value, index, self) => self.indexOf(value) === index);

    let result = [];

    uniqueVendorOrderCode.forEach(orderCode => {
      //danh sách đơn hàng con
      let listDetailOrder = this.listVendorOrderReport.filter(e => e.vendorOrderCode == orderCode);
      //tính tổng tiền đơn
      let totalMoney = 0;
      listDetailOrder.forEach(detail => {
        totalMoney += Number(detail.priceValueWarehouse);
      });
      //thêm record sumary
      let sumaryRecord = new exportExcelModel();
      sumaryRecord.vendorOrderDate = listDetailOrder[0].vendorOrderDate;
      sumaryRecord.vendorOrderCode = orderCode;
      sumaryRecord.productName = listDetailOrder[0].vendorName;
      sumaryRecord.unitName = '';
      sumaryRecord.priceWarehouse = '';
      sumaryRecord.priceValueWarehouse = totalMoney;
      sumaryRecord.quantity = '';
      sumaryRecord.quantity_1 = '';
      sumaryRecord.quantity_2 = '';
      sumaryRecord.quantity_3 = '';
      sumaryRecord.quantity_4 = '';
      sumaryRecord.statusName = '';
      sumaryRecord.isSummary = true;
      result = [...result, sumaryRecord];

      listDetailOrder.forEach(detail => {
        //thêm record đơn hàng con
        let detailRecord = new exportExcelModel();
        detailRecord.vendorOrderDate = '';
        detailRecord.vendorOrderCode = '';
        detailRecord.productName = detail.productName;
        detailRecord.unitName = detail.unitName;
        detailRecord.priceWarehouse = detail.priceWarehouse;
        detailRecord.priceValueWarehouse = detail.priceValueWarehouse;
        detailRecord.quantity = detail.quantity;
        detailRecord.quantity_1 = '0';
        detailRecord.quantity_2 = '0';
        detailRecord.quantity_3 = '0';
        detailRecord.quantity_4 = '0';
        detailRecord.statusName = detail.statusName;
        detailRecord.isSummary = false;
        result = [...result, detailRecord];
      });
    });

    return result;
  }

  getListExcelColumns(): Array<columnModel> {
    let listColumns: Array<columnModel> = [];

    /* cột ngày */
    let columGroup_1 = new columnModel();
    columGroup_1.column1 = "Số đơn đặt hàng";
    columGroup_1.column2 = "Ngày";

    let columGroup_2 = new columnModel();
    columGroup_2.column1 = "";
    columGroup_2.column2 = "Số";

    let columGroup_3 = new columnModel();
    columGroup_3.column1 = "Vật tư";
    columGroup_3.column2 = "";

    let columGroup_4 = new columnModel();
    columGroup_4.column1 = "Đơn vị tính";
    columGroup_4.column2 = "";

    let columGroup_5 = new columnModel();
    columGroup_5.column1 = "Đơn giá";
    columGroup_5.column2 = "";

    let columGroup_6 = new columnModel();
    columGroup_6.column1 = "Thành tiền";
    columGroup_6.column2 = "";

    let columGroup_7 = new columnModel();
    columGroup_7.column1 = "Số lượng đặt";
    columGroup_7.column2 = "";

    let columGroup_8 = new columnModel();
    columGroup_8.column1 = "Số lượng đã nhận";
    columGroup_8.column2 = "";

    let columGroup_9 = new columnModel();
    columGroup_9.column1 = "Số lượng hóa đơn";
    columGroup_9.column2 = "";

    let columGroup_10 = new columnModel();
    columGroup_10.column1 = "Số lượng trả lại";
    columGroup_10.column2 = "";

    let columGroup_11 = new columnModel();
    columGroup_11.column1 = "Số lượng còn lại";
    columGroup_11.column2 = "";

    let columGroup_12 = new columnModel();
    columGroup_12.column1 = "Trạng thái";
    columGroup_12.column2 = "";

    listColumns = [columGroup_1, columGroup_2, columGroup_3, columGroup_4, columGroup_5,
      columGroup_6, columGroup_7, columGroup_8, columGroup_9, columGroup_10, columGroup_11, columGroup_12];

    return listColumns;
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  exportPdf() {
    let imgBase64 = this.getBase64Logo();

    let dataExportExcel = this.getDataExportExcel(this.listVendorOrderReport);

    let statusName = '';
    if (this.listSelectedStatus.length == 0 || this.listSelectedStatus.length == this.listStatus.length) {
      statusName = 'Tất cả'
    } else {
      statusName = this.listSelectedStatus.map(e => e.description).join(', ');
    }

    let fromDate = this.fromDate ? moment(new Date(this.fromDate)).format('DD/MM/YYYY') : '';
    let toDate = this.toDate ? moment(new Date(this.toDate)).format('DD/MM/YYYY') : '';

    let dataRows: Array<any> = [];

    dataExportExcel.forEach(item => {
      let row = [
        { text: `${item.vendorOrderDate ? item.vendorOrderDate.toString() : ""}`, alignment: 'center' }, //1.ngay
        { text: `${item.vendorOrderCode ? item.vendorOrderCode.toString() : ""}`, alignment: 'center' },//2.Số
        { text: `${item.productName ? item.productName.toString() : ""}`, alignment: 'left' },//3.Vật tư
        { text: `${item.unitName ? item.unitName.toString() : ""}`, alignment: 'left' },//4.Đơn vị tính
        { text: `${item.priceWarehouse ? formatNumber(Number(item.priceWarehouse), 'en-US', '1.0-0') : ""}`, alignment: 'right' },//5.right giá
        { text: `${item.priceValueWarehouse ? formatNumber(Number(item.priceValueWarehouse), 'en-US', '1.0-0') : ""}`, alignment: 'right' },//6.Thành tiền
        { text: `${item.quantity}`, alignment: 'right' },//7.Số lượng đặt
        { text: `${item.quantity_1}`, alignment: 'right' },//8.Số lượng đã nhận
        { text: `${item.quantity_2}`, alignment: 'right' },//9.Số lượng hóa đơn
        { text: `${item.quantity_3}`, alignment: 'right' },//10.Số lượng trả lại
        { text: `${item.quantity_4}`, alignment: 'right' },//11.Số lượng còn lại
        { text: `${item.statusName ? item.statusName : ""}`, alignment: 'left' },//12.Trạng thái
      ];

      //nếu là summary row thì thêm font bold cho mỗi text
      if (item.isSummary) {
        row.forEach((cell: any) => {
          cell.bold = true;
        });
      }

      dataRows.push(row);
    });


    let documentDefinition: any = {
      pageSize: 'A4',
      pageMargins: [20, 20, 20, 20],
      content: [
        {
          table: {
            widths: ['*', '400'],
            body: [
              [
                {
                  stack: [
                    {
                      image: imgBase64, width: 100, height: 60
                    }
                  ],
                },
                {
                  stack: [
                    {
                      text: `CÔNG TY TNHH TRÍ NGHĨA`,
                      style: 'timer',
                      alignment: 'left'
                    },
                    {
                      text: '',
                      margin: [0, 2, 0, 2]
                    },
                    {
                      text: `Địa chỉ: Lô 10 BT2 Trung Văn, Nam Từ Liêm, Hà Nội`,
                      style: 'timer',
                      alignment: 'left'
                    },
                    {
                      text: '',
                      margin: [0, 2, 0, 2]
                    },
                    {
                      text: `Điện thoại: 09130166667`,
                      style: 'timer',
                      alignment: 'left'
                    },
                    {
                      text: '',
                      margin: [0, 2, 0, 2]
                    },
                    {
                      text: `Email: info@tringhiatech.vn`,
                      style: 'timer',
                      alignment: 'left'
                    },
                    {
                      text: '',
                      margin: [0, 2, 0, 2]
                    },
                    {
                      text: `Website dịch vụ:`,
                      style: 'timer',
                      alignment: 'left'
                    },
                  ],
                }
              ],
            ]
          },
          layout: {
            defaultBorder: false
          },
          lineHeight: 0.75
        },
        {
          text: '',
          margin: [0, 10, 0, 10]
        },
        {
          text: 'BÁO CÁO TÌNH TRẠNG ĐƠN HÀNG MUA',
          style: 'header',
          alignment: 'center'
        },
        {
          text: `Trạng thái: ${statusName}`,
          style: 'timer',
          alignment: 'center'
        },
        {
          text: `Từ ngày ${fromDate} - đến ngày ${toDate}`,
          style: 'timer',
          alignment: 'center'
        },
        {
          style: 'table',
          table: {
            headerRows: 0,
            widths: ['10%', '10%', '10%', '10%', '10%', '10%', '10%', '5%', '5%', '5%', '5%', '10%'],
            heights: 20,
            body: [
              [{ text: 'Số đơn hàng', colSpan: 2, alignment: 'center', bold: true },
                '',
              { text: 'Vật tư', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'Đơn vị tính', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'Đơn giá', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'Thành tiền', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'Số lượng đặt', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'Số lượng đã nhận', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'Số lượng hóa đơn', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'Số lượng trả lại', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'Số lượng còn lại', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'Trạng thái', rowSpan: 2, alignment: 'center', bold: true },
              ],
              [{ text: 'Ngày', alignment: 'center', bold: true }, { text: 'Số', alignment: 'center', bold: true }, '', '', '', '', '', '', '', '', '', ''],
              ...dataRows
            ]
          },
          layout: {
          }
        },
        {
          columns: [
            {
              width: '55%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'right'
            },
            {
              width: '45%',
              text: 'Ngày ........ tháng ........ năm .............',
              style: { fontSize: 10, bold: false },
              alignment: 'center'
            }
          ]
        },
        {
          columns: [
            {
              width: '55%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'right'
            },
            {
              width: '45%',
              text: 'NGƯỜI LẬP',
              style: { fontSize: 10, bold: false },
              alignment: 'center'
            }
          ]
        },
        {
          columns: [
            {
              width: '55%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'right'
            },
            {
              width: '45%',
              text: '(Ký, họ tên)',
              style: { fontSize: 10, bold: false },
              alignment: 'center'
            }
          ]
        },
      ],
      styles: {
        header: {
          fontSize: 18.5,
          bold: true
        },
        timer: {
          fontSize: 10,
          italics: true
        },
        table: {
          margin: [0, 15, 0, 15],
          wordBreak: 'break-word',
          fontSize: 8
        },
        tableHeader: {
          fontSize: 10,
          bold: true
        },
        tableLine: {
          fontSize: 10,
        },
        tableLines: {
          fontSize: 9,
        },
        tableLiness: {
          fontSize: 7,
        },
        StyleItalics: {
          italics: true
        },
      }
    }

    let currentDate = moment(new Date()).format('DD_MM_YYYY');
    let title = `BÁO CÁO TÌNH TRẠNG ĐƠN HÀNG MUA - ${currentDate}`;
    pdfMake.createPdf(documentDefinition).download(`${title}.pdf`);
  }
  
  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo?.systemValueString;
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

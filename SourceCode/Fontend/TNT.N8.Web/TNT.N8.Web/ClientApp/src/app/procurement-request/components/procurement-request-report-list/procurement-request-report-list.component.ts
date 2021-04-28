
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { OrganizationpopupComponent } from '../../../shared/components/organizationpopup/organizationpopup.component';
import { ProcurementRequestService } from '../../services/procurement-request.service';
import { Router } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { Table } from 'primeng/table';
import { MessageService, SortEvent } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { OrganizationDialogComponent } from '../../../shared/components/organization-dialog/organization-dialog.component';
import { TranslateService } from '@ngx-translate/core';
import { DatePipe } from '@angular/common';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import * as pdfMake from 'pdfmake/build/pdfmake.js';
import * as pdfFonts from 'pdfmake/build/vfs_fonts.js';
pdfMake.vfs = pdfFonts.pdfMake.vfs;
import * as moment from 'moment';

interface ProcurementRequest {
  procurementRequestId: string;
  procurementCode: string;
  procurementContent: string;
  requestEmployeeId: string;
  employeePhone: string;
  unit: string;
  approverId: string;
  approverPostion: string;
  explain: string;
  statusId: string;
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  requestEmployeeName: string;
  approverName: string;
  statusName: string;
  organizationName: string;
  totalMoney: Number;
  backgroundColorForStatus: string;
}

//model dùng để build template excel
class columnModel {
  column1: string;
  column2: string;
}

//model data xuất báo cáo excel - pdf
class dataExportModel {
  createdDate: string; //ngay tao
  procurementCode: string; // so
  productName: string; //vat tu
  unitName: string; //don vi tinh
  orderDate: string; //ngay yeu cau: tam thoi lay gia tri cua ngay tao
  sumQuantity: string; //so luong yeu cau
  sumQuantityApproval: string; //so luong duyet
  sumQuantityPO: string; //so luong dat hang
  statusName: string; //trang thai
  organizationName: string; //bo phan
  isSumary: boolean;
}

@Component({
  selector: 'app-procurement-request-report-list',
  templateUrl: './procurement-request-report-list.component.html',
  styleUrls: ['./procurement-request-report-list.component.css'],
  providers: [
    DatePipe,
  ]
})
export class ProcurementRequestReportListComponent implements OnInit {

  // Mảng chứa Procurement Request
  listProcurementRequest: Array<any> = [];
  filterForm: FormGroup;

  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  auth: any = JSON.parse(localStorage.getItem("auth"));


  actionAdd: boolean = true;
  actionDownload: boolean = true;
  loading: boolean = false;
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  isShowFilterUpTop: boolean = false;
  isShowFilterUpLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();
  employeeList: Array<any> = [];

  nowDate = new Date();
  filterGlobal: string;

  prcmCode = '';
  prcmContent = '';
  fromDate: Date = null;
  toDate: Date = null;
  prcmOrgId: string;
  prcmOrgName: string;
  selectedRequester: Array<any> = [];
  selectedVendor: Array<any> = [];
  selectedApproval: Array<any> = [];
  selectedStatus: Array<any> = [];
  selectedBudget: Array<any> = [];
  selectedProduct: Array<any> = [];
  selectedRequesterToSearch: Array<any> = [];

  @ViewChild('myTable') myTable: Table;
  colsList: any;
  selectedColumns: any[];
  frozenCols: any[];

  // Khai báo các mang/ biến chứa masterData
  listStatus: Array<any> = [];
  listEmp: Array<any> = [];
  listProduct: Array<any> = [];
  listVendor: Array<any> = [];
  listBudget: Array<any> = [];
  // filterEmp: Observable<any>;
  // Khai báo popup
  dialogOrg: MatDialogRef<OrganizationpopupComponent>;
  dateFieldFormat: string = 'DD/MM/YYYY';
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };

  messageTitle: string = '';
  message: string = '';

  constructor(
    private getPermission: GetPermission,
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private procurementRequestService: ProcurementRequestService,
    private router: Router,
    private messageService: MessageService,
    public dialogService: DialogService,
    private translate: TranslateService,
    private datePipe: DatePipe,
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "buy/procurement-request/list-report";
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
      this.procurementRequestService.getMasterDataSearchProcurementRequest().subscribe(response => {
        this.loading = true;
        let result: any = response;
        if (result.statusCode == 200) {
          this.listEmp = result.listEmployee;
          this.listStatus = result.listStatus;
          this.listProduct = result.listProduct;
          this.listVendor = result.listVendor;
          this.listBudget = result.listBudget;
          this.listEmp.forEach(item => {
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;
          });
          this.searchProcurementRequestData();
        } else {
          this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
          let msg = { severity: 'error', summary: this.messageTitle, detail: result.messageCode };
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
      { field: 'stt', header: 'STT', textAlign: 'center', display: 'table-cell', width: '90px' },
      { field: 'procurementCode', header: 'Mã phiếu', textAlign: 'left', display: 'table-cell', width: '130px' },
      { field: 'createdDate', header: 'Ngày để xuất', textAlign: 'right', display: 'table-cell', width: '130px' },
      { field: 'procurementContent', header: 'Nội dung yêu cầu', textAlign: 'left', display: 'table-cell', width: '180px' },
      { field: 'description', header: 'Diễn giải', textAlign: 'left', display: 'table-cell', width: '180px' },
      { field: 'productCode', header: 'Mã hàng', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'productName', header: 'Tên hàng', textAlign: 'left', display: 'table-cell', width: '180px' },
      { field: 'unitName', header: 'Đơn vị tính', textAlign: 'left', display: 'table-cell', width: '120px' },
      { field: 'sumQuantity', header: 'Số lượng yêu cầu', textAlign: 'right', display: 'table-cell', width: '100px' },
      { field: 'sumQuantityApproval', header: 'Số lượng duyệt', textAlign: 'right', display: 'table-cell', width: '100px' },
      { field: 'sumQuantityPO', header: 'Số lượng đặt hàng', textAlign: 'right', display: 'table-cell', width: '120px' },
      { field: 'organizationName', header: 'Bộ phận', textAlign: 'left', display: 'table-cell', width: '130px' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '150px' },
    ];
    this.selectedColumns = this.colsList;
  }

  leftColNumber: number = 12;
  rightColNumber: number = 0;

  // Search ProcurementRequest
  searchProcurementRequestData() {
    if (this.prcmCode) {
      this.prcmCode = this.prcmCode.trim();
    }
    if (this.prcmContent) {
      this.prcmContent = this.prcmContent.trim();
    }

    let listEmpIds: Array<string> = [];
    listEmpIds = this.selectedRequester.map(c => c.employeeId);

    let listApprovalIds: Array<string> = [];
    listApprovalIds = this.selectedApproval.map(c => c.employeeId);

    let listStatusIds: Array<string> = [];
    listStatusIds = this.selectedStatus.map(c => c.categoryId);

    let listBudgetIds: Array<string> = [];
    listBudgetIds = this.selectedBudget.map(c => c.procurementPlanId);

    let listVendorIds: Array<string> = [];
    listVendorIds = this.selectedVendor.map(c => c.vendorId);

    let listProductIds: Array<string> = [];
    listProductIds = this.selectedProduct.map(c => c.productId);

    let org: string = '';
    if (this.prcmOrgId) {
      org = this.prcmOrgId;
    }

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
    this.procurementRequestService.searchProcurementRequestReport(this.prcmCode, this.fromDate, this.toDate, listEmpIds, listStatusIds, org, this.auth.userId, listProductIds,
      this.prcmContent, listVendorIds, listApprovalIds, listBudgetIds).subscribe(response => {
        let result = <any>response;
        this.loading = false;
        this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
        if (result.statusCode == 200) {
          this.listProcurementRequest = result.listProcurementRequest;
          this.listProcurementRequest.forEach((item, index) => {
            item.stt = index + 1;
          });
          if (this.listProcurementRequest.length == 0) {
            this.translate.get('procurement-request.list.no_search_data').subscribe(value => { this.message = value; });
            let msg = { severity: 'warn', summary: this.messageTitle, detail: this.message };
            this.showMessage(msg);
          } else {
            this.listProcurementRequest.forEach(item => {
              item.createdDate = this.datePipe.transform(item.createdDate, 'dd/MM/yyyy');
            });
          }
        } else {
          let msg = { severity: 'error', summary: this.messageTitle, detail: result.messageCode };
          this.showMessage(msg);
        }
      });
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

  refreshFilter() {
    this.prcmCode = '';
    this.fromDate = null;
    this.toDate = null;
    this.selectedRequester = [];
    this.selectedStatus = [];
    this.prcmOrgName = '';
    this.prcmOrgId = '';
    this.filterGlobal = "";
    this.listProcurementRequest = [];

    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;

    this.searchProcurementRequestData();
  }

  openOrgPopup() {
    this.translate.get('procurement-request.list.unit').subscribe(value => { this.message = value; });
    let ref = this.dialogService.open(OrganizationDialogComponent, {
      data: {
        chooseFinancialIndependence: false
      },
      header: this.message,
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.prcmOrgId = result.selectedOrgId;
          this.prcmOrgName = result.selectedOrgName;
        }
      }
    });
  }

  gotoView(value: any) {
    this.router.navigate(['/procurement-request/view', { id: value.procurementRequestId }])
  }


  exportExcel() {
    let currentDate = moment(new Date()).format('DD_MM_YYYY');
    let title = `Báo cáo tình trạng đề xuất mua hàng - ${currentDate}`;

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
    worksheet.mergeCells(`C${2}:E${2}`);
    worksheet.mergeCells(`C${3}:E${3}`);
    worksheet.mergeCells(`C${4}:E${4}`);
    worksheet.mergeCells(`C${5}:E${5}`);

    let statusName = '';
    if (this.selectedStatus.length == 0 || this.selectedStatus.length == this.listStatus.length) {
      statusName = 'Tất cả';
    } else {
      statusName = this.selectedStatus.map(e => e.categoryName).join(', ');
    }

    let fromDate = this.fromDate ? moment(new Date(this.fromDate)).format('DD/MM/YYYY') : '';
    let toDate = this.toDate ? moment(new Date(this.toDate)).format('DD/MM/YYYY') : '';

    /* title */
    let dataHeaderMain = "Báo cáo tình trạng đề xuất mua hàng".toUpperCase();
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
    let dataExportExcel = this.getDataExportExcel(this.listProcurementRequest);

    dataExportExcel.forEach((item, index) => {
      let row: Array<any> = [];
      row[0] = item.createdDate;
      row[1] = item.procurementCode;
      row[2] = item.productName;
      row[3] = item.unitName;
      row[4] = item.orderDate;
      row[5] = item.sumQuantity;
      row[6] = item.sumQuantityApproval;
      row[7] = item.sumQuantityPO;
      row[8] = item.statusName;
      row[9] = item.organizationName;
      data.push(row);
    });

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };

      if (dataExportExcel[index].isSumary) row.font.bold = true;

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
      row.getCell(9).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(10).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(10).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

    });

    //sign reagion
    worksheet.addRow([]);

    let signData_1 = 'Ngày....... tháng ........năm ................';
    let signData_2 = 'NGƯỜI LẬP';
    let signData_3 = '(Ký, họ tên)';

    let signRow_1 = worksheet.addRow(['', '', '', '', '', '', '', signData_1]);
    let signRow_2 = worksheet.addRow(['', '', '', '', '', '', '', signData_2]);
    let signRow_3 = worksheet.addRow(['', '', '', '', '', '', '', signData_3]);

    signRow_1.getCell(8).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    signRow_2.getCell(8).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
    signRow_3.getCell(8).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    worksheet.mergeCells(`H${signRow_1.number}:J${signRow_1.number}`);
    worksheet.mergeCells(`H${signRow_2.number}:J${signRow_2.number}`);
    worksheet.mergeCells(`H${signRow_3.number}:J${signRow_3.number}`);

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

    this.exportToExel(workBook, title);
  }

  exportPdf() {
    let imgBase64 = this.getBase64Logo();

    let statusName = '';
    if (this.selectedStatus.length == 0 || this.selectedStatus.length == this.listStatus.length) {
      statusName = 'Tất cả';
    } else {
      statusName = this.selectedStatus.map(e => e.categoryName).join(', ');
    }

    let fromDate = this.fromDate ? moment(new Date(this.fromDate)).format('DD/MM/YYYY') : '';
    let toDate = this.toDate ? moment(new Date(this.toDate)).format('DD/MM/YYYY') : '';

    let dataRows: Array<any> = [];
    let dataExportExcel = this.getDataExportExcel(this.listProcurementRequest);

    dataExportExcel.forEach((item, index) => {
      let row = [
        { text: `${item.createdDate ? item.createdDate.toString() : ""}`, alignment: 'center' }, //1.ngay
        { text: `${item.procurementCode ? item.procurementCode.toString() : ""}`, alignment: 'center' },//2.Số
        { text: `${item.productName ? item.productName.toString() : ""}`, alignment: 'left' },//3.Vật tư
        { text: `${item.unitName ? item.unitName.toString() : ""}`, alignment: 'left' },//4.Đơn vị tính
        { text: `${item.orderDate ? item.orderDate.toString() : ""}`, alignment: 'right' },//5.ngay yeu cau
        { text: `${item.sumQuantity}`, alignment: 'right' },//6.sl yeu cau
        { text: `${item.sumQuantityApproval}`, alignment: 'right' },//7.Số lượng  duyet
        { text: `${item.sumQuantityPO}`, alignment: 'right' },//7.Số lượng  duyet
        { text: `${item.statusName}`, alignment: 'center' },//8.status
        { text: `${item.organizationName}`, alignment: 'left' },//9.bo phan
      ];

      //nếu là summary row thì thêm font bold cho mỗi text
      if (item.isSumary) {
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
          text: 'Báo cáo tình trạng đề xuất mua hàng'.toUpperCase(),
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
            widths: ['12.5%', '15%', '15%', '10%', '12.5%', '5%', '5%', '5%', '10%', '10%'],
            heights: 20,
            body: [
              [{ text: 'Phiếu đề xuất mua hàng', colSpan: 2, alignment: 'center', bold: true },
                '',
              { text: 'Vật tư', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'Đơn vị tính', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'Ngày yêu cầu', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'SL yêu cầu', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'SL duyệt', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'SL đặt hàng', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'Trạng thái', rowSpan: 2, alignment: 'center', bold: true },
              { text: 'Bộ phận', rowSpan: 2, alignment: 'center', bold: true },
              ],
              [{ text: 'Ngày', alignment: 'center', bold: true }, { text: 'Số', alignment: 'center', bold: true }, '', '', '', '', '', '', '', ''],
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
          margin: [0, 15, 0, 0],
          fontSize: 12,
          bold: true,
          wordBreak: 'break-word',
        },
        tableBody: {
          margin: [0, 0, 0, 15],
          fontSize: 10,
          wordBreak: 'break-word',
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
    let title = `BÁO CÁO TÌNH TRẠNG ĐỀ XUẤT MUA HÀNG - ${currentDate}`;
    pdfMake.createPdf(documentDefinition).download(`${title}.pdf`);
  }

  getDataExportExcel(listRequest) {
    listRequest.sort((a, b) => (a.procurementCode < b.procurementCode) ? 1 : ((b.procurementCode < a.procurementCode) ? -1 : 0));
    let uniqueRequestCodes = listRequest.map(item => item.procurementCode).filter((value, index, self) => self.indexOf(value) === index);

    let result = [];

    uniqueRequestCodes.forEach(procurementCode => {
      //danh sách đề xuất con
      let listDetailRequest = this.listProcurementRequest.filter(e => e.procurementCode == procurementCode);

      //thêm record sumary
      let sumaryRecord = new dataExportModel();
      sumaryRecord.createdDate = listDetailRequest[0].createdDate;
      sumaryRecord.procurementCode = procurementCode;
      sumaryRecord.productName = '';
      sumaryRecord.unitName = '';
      sumaryRecord.orderDate = '';
      sumaryRecord.sumQuantity = '';
      sumaryRecord.sumQuantityApproval = '';
      sumaryRecord.sumQuantityPO = '';
      sumaryRecord.statusName = '';
      sumaryRecord.organizationName = '';
      sumaryRecord.isSumary = true;

      result = [...result, sumaryRecord];

      listDetailRequest.forEach(detail => {
        //thêm record đơn hàng con
        let detailRecord = new dataExportModel();
        detailRecord.createdDate = "";
        detailRecord.procurementCode = "";
        detailRecord.productName = detail.productName || "";
        detailRecord.unitName = detail.unitName || "";
        detailRecord.orderDate = detail.createdDate;//TODO: replace this. tạm thời để = ngày tạo
        detailRecord.sumQuantity = detail.sumQuantity;
        detailRecord.sumQuantityApproval = detail.sumQuantityApproval;
        detailRecord.sumQuantityPO = detail.sumQuantityPO;
        detailRecord.statusName = detail.statusName || "";
        detailRecord.organizationName = detail.organizationName || "";
        detailRecord.isSumary = false;
        result = [...result, detailRecord];
      });
    });

    return result;
  }

  getListExcelColumns(): Array<columnModel> {
    let listColumns: Array<columnModel> = [];

    /* cột ngày */
    let columGroup_1 = new columnModel();
    columGroup_1.column1 = "Phiếu đề xuất mua hàng";
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
    columGroup_5.column1 = "Ngày yêu cầu";
    columGroup_5.column2 = "";

    let columGroup_6 = new columnModel();
    columGroup_6.column1 = "SL yêu cầu";
    columGroup_6.column2 = "";

    let columGroup_7 = new columnModel();
    columGroup_7.column1 = "SL duyệt";
    columGroup_7.column2 = "";

    let columGroup_8 = new columnModel();
    columGroup_8.column1 = "SL đặt hàng";
    columGroup_8.column2 = "";

    let columGroup_9 = new columnModel();
    columGroup_9.column1 = "Trạng thái";
    columGroup_9.column2 = "";

    let columGroup_10 = new columnModel();
    columGroup_10.column1 = "Bộ phận";
    columGroup_10.column2 = "";

    listColumns = [columGroup_1, columGroup_2, columGroup_3, columGroup_4, columGroup_5,
      columGroup_6, columGroup_7, columGroup_8, columGroup_9, columGroup_10];

    return listColumns;
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo?.systemValueString;
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};




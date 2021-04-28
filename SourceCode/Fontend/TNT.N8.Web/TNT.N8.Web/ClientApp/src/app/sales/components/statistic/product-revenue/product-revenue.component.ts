import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { OrganizationpopupComponent } from '../../../../shared/components/organizationpopup/organizationpopup.component';
import { Router } from '@angular/router';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { Table } from 'primeng/table';
import { MessageService, SortEvent } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { OrganizationDialogComponent } from '../../../../shared/components/organization-dialog/organization-dialog.component';
import { TranslateService } from '@ngx-translate/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import * as pdfMake from 'pdfmake/build/pdfmake.js';
import * as pdfFonts from 'pdfmake/build/vfs_fonts.js';
pdfMake.vfs = pdfFonts.pdfMake.vfs;
import * as moment from 'moment';
import { TopRevenueService } from '../services/top-revenue.service';

class Employee {
  public employeeId: string;
  public employeeName: string;
  public employeeCode: string;
  public active: boolean
}

class ProductCategory {
  productCategoryId: string;
  productCategoryName: string;
  productCategoryCode: string;
}

class WarehouseModel {
  warehouseId: string;
  warehouseName: string;
  warehouseCode: string;
}

class categoryModel {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

class customerModel {
  customerId: string;
  customerName: string;
  customerCode: string;
}

class InforExportExcel {
  companyName: string;
  address: string;
  phone: string;
  website: string;
  email: string;
  textTotalMoney: string
}

class filterModel {
  //basic filter
  ProductCode: string;
  ProductCategory: Array<string>;
  OrderFromDate: Date;
  OrderToDate: Date;
  SaleRevenueFrom: number;
  SaleRevenueTo: number;
  ProductInOrderCountFrom: number;
  ProductInOrderCountTo: number;
  OrderCountFrom: number;
  OrderCountTo: number;
  ProductRefundCountFrom: number;
  ProductRefundCountTo: number;
  //advanced filter
  Warehouse: Array<string>;
  Seller: string;
  CustomerGroup: Array<string>;
  CustomerType: Array<number>;
  Customer: Array<string>;
  constructor() {
    this.ProductCode = '';
    this.ProductCategory = [];
    this.OrderFromDate = null;
    this.OrderToDate = null;
    this.SaleRevenueFrom = null;
    this.SaleRevenueTo = null;
    this.ProductInOrderCountFrom = null;
    this.ProductInOrderCountTo = null;
    this.OrderCountFrom = null;
    this.OrderCountTo = null;
    this.ProductRefundCountFrom = null;
    this.ProductRefundCountTo = null;

    this.Warehouse = [];
    this.Seller = null;
    this.CustomerGroup = [];
    this.CustomerType = [];
    this.Customer = [];
  }
}

class productRevenueEntityModel {
  index: number;
  productCode: string;
  productName: string;
  unitName: string;
  orderCount: number;
  productInOrderCount: number;
  productRefundCount: number;
  saleRevenue: number;
  saleRevenueRefund: number;
  totalDiscount: number;
  totalPriceInitial: number;
  totalPriceInitialRefund: number;
  totalGrossProfit: number;
  totalProfitPerSaleRevenue: number;
  totalProfitPerPriceInitial: number;
}

class customerTypeModel {
  label: string;
  value: number;
}

@Component({
  selector: 'app-product-revenue',
  templateUrl: './product-revenue.component.html',
  styleUrls: ['./product-revenue.component.css'],
  providers: [DatePipe, DecimalPipe]
})
export class ProductRevenueComponent implements OnInit {
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  auth: any = JSON.parse(localStorage.getItem("auth"));
  dateFieldFormat: string = 'DD/MM/YYYY';

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

  /* table */
  @ViewChild('table') table: Table;
  colsList: any;
  selectedColumns: any[];
  frozenCols: any[];

  /* filter form */
  filterForm: FormGroup;
  advancedFilterForm: FormGroup;

  /* master data */
  listCustomerType: Array<customerTypeModel> = [{ label: "Doanh nghiệp", value: 1 }, { label: "Cá nhân", value: 2 }]
  listEmployee: Array<Employee> = [];
  listProductCategory: Array<ProductCategory> = [];
  listWarehouse: Array<WarehouseModel> = [];
  inforExportExcel: InforExportExcel;
  listCustomer: Array<customerModel> = [];
  listCustomerGroupCategory: Array<categoryModel> = [];

  /* summary */
  summaryOrderCount: number = 0;
  summaryProductInOrderCount: number = 0;
  summaryProductRefundCount: number = 0;
  summarySaleRevenue: number = 0;
  summarySaleRevenueRefund: number = 0;
  summaryTotalDiscount: number = 0;
  summaryTotalPriceInitial: number = 0;
  summaryTotalPriceInitialRefund: number = 0;
  summaryTotalGrossProfit: number = 0;
  summaryTotalProfitPerSaleRevenue: number = 0;
  summaryTotalProfitPerPriceInitial: number = 0;

  /* search data */
  listProductRevenue: Array<productRevenueEntityModel> = [];

  constructor(
    private getPermission: GetPermission,
    private router: Router,
    private messageService: MessageService,
    public dialogService: DialogService,
    private translate: TranslateService,
    private datePipe: DatePipe,
    private topRevenueService: TopRevenueService,
    private decimalPipe: DecimalPipe,
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit(): void {
    let resource = "sal/sales/product-revenue";
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
      this.initForm();
      this.getMasterdata();
    }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  initTable() {
    this.colsList = [
      { field: 'index', header: 'STT', textAlign: 'center', display: 'table-cell', width: '73px', excelWidth: 10, pdfWidth: 25 },
      { field: 'productCode', header: 'Mã hàng', textAlign: 'left', display: 'table-cell', width: '130px', excelWidth: 15, pdfWidth: 40 },
      { field: 'productName', header: 'Tên hàng', textAlign: 'left', display: 'table-cell', width: '210px', excelWidth: 20, pdfWidth: 70 },
      { field: 'unitName', header: 'Đơn vị tính', textAlign: 'left', display: 'table-cell', width: '210px', excelWidth: 10, pdfWidth: 25 },
      { field: 'orderCount', header: 'SL đơn hàng', textAlign: 'right', display: 'table-cell', width: '164px', excelWidth: 10, pdfWidth: 20 },
      { field: 'productInOrderCount', header: 'SL bán', textAlign: 'right', display: 'table-cell', width: '130px', excelWidth: 10, pdfWidth: 20 },
      { field: 'productRefundCount', header: 'SL trả lại', textAlign: 'right', display: 'table-cell', width: '145px', excelWidth: 10, pdfWidth: 20 },
      { field: 'saleRevenue', header: 'Doanh thu bán', textAlign: 'right', display: 'table-cell', width: '170px', excelWidth: 15, pdfWidth: 55 },
      { field: 'saleRevenueRefund', header: 'Doanh thu trả lại', textAlign: 'right', display: 'table-cell', width: '170px', excelWidth: 15, pdfWidth: 55 },
      { field: 'totalDiscount', header: 'Chiết khấu', textAlign: 'right', display: 'table-cell', width: '170px', excelWidth: 15, pdfWidth: 55 },
      { field: 'totalPriceInitial', header: 'Tiền vốn', textAlign: 'right', display: 'table-cell', width: '170px', excelWidth: 15, pdfWidth: 55 },
      { field: 'totalPriceInitialRefund', header: 'Tiền vốn trả lại', textAlign: 'right', display: 'table-cell', width: '170px', excelWidth: 15, pdfWidth: 55 },
      { field: 'totalGrossProfit', header: 'Lãi gộp', textAlign: 'right', display: 'table-cell', width: '170px', excelWidth: 15, pdfWidth: 55 },
      { field: 'totalProfitPerSaleRevenue', header: '% lãi/DT', textAlign: 'right', display: 'table-cell', width: '170px', excelWidth: 15, pdfWidth: 55 },
      { field: 'totalProfitPerPriceInitial', header: '% lãi/Tiền vốn', textAlign: 'right', display: 'table-cell', width: '170px', excelWidth: 15, pdfWidth: 55 },
    ];
    this.selectedColumns = this.colsList;
  }

  initForm() {
    this.filterForm = new FormGroup({
      'ProductCode': new FormControl(''),
      'ProductCategory': new FormControl([]),
      'OrderFromDate': new FormControl(null),
      'OrderToDate': new FormControl(null),
      'SaleRevenueFrom': new FormControl(null),
      'SaleRevenueTo': new FormControl(null),
      'ProductInOrderCountFrom': new FormControl(null),
      'ProductInOrderCountTo': new FormControl(null),
      'OrderCountFrom': new FormControl(null),
      'OrderCountTo': new FormControl(null),
      'ProductRefundCountFrom': new FormControl(null),
      'ProductRefundCountTo': new FormControl(null),
    });

    this.advancedFilterForm = new FormGroup({
      'Warehouse': new FormControl([]),
      'Seller': new FormControl(null),
      'CustomerGroup': new FormControl([]),
      'CustomerType': new FormControl([]),
      'Customer': new FormControl([]),
    });

  }

  async getMasterdata() {
    //default filter
    let defaultFilter = this.getDefaultFilter();
    this.patchInitFormData(defaultFilter);

    this.loading = true;
    let result: any = await this.topRevenueService.getDataSearchRevenueProduct(this.auth.UserId);
    let resultSearch: any = await this.topRevenueService.searchRevenueProduct(defaultFilter, this.auth.UserId);

    this.loading = false;
    if (result.statusCode === 200 && resultSearch.statusCode === 200) {
      this.listEmployee = result.listEmployee ?? [];
      this.listProductCategory = result.listProductCategory ?? [];
      this.listWarehouse = result.listWarehouse;
      this.inforExportExcel = result.inforExportExcel;
      this.listCustomer = result.listCustomer;
      this.listCustomerGroupCategory = result.listCustomerGroupCategory;

      this.listProductRevenue = resultSearch.listProductRevenue ?? [];
      this.handlerReportAfterSearch();
    } else {
      this.showToast('error', 'Thông báo', 'Lấy dữ liệu thất bại')
    }
  }

  handlerReportAfterSearch() {
    this.summaryOrderCount = 0;
    this.summaryProductInOrderCount = 0;
    this.summaryProductRefundCount = 0;
    this.summarySaleRevenue = 0;
    this.summarySaleRevenueRefund = 0;
    this.summaryTotalDiscount = 0;
    this.summaryTotalPriceInitial = 0;
    this.summaryTotalPriceInitialRefund = 0;
    this.summaryTotalGrossProfit = 0;
    this.summaryTotalProfitPerSaleRevenue = 0;
    this.summaryTotalProfitPerPriceInitial = 0;

    this.listProductRevenue.forEach((item, index) => {
      item.index = index + 1;
      this.summaryOrderCount += item.orderCount ? Number(item.orderCount) : 0;
      this.summaryProductInOrderCount += item.productInOrderCount ? Number(item.productInOrderCount) : 0;
      this.summaryProductRefundCount += item.productRefundCount ? Number(item.productRefundCount) : 0;
      this.summarySaleRevenue += item.saleRevenue ? Number(item.saleRevenue) : 0;
      this.summarySaleRevenueRefund += item.saleRevenueRefund ? Number(item.saleRevenueRefund) : 0;
      this.summaryTotalDiscount += item.totalDiscount ? Number(item.totalDiscount) : 0;
      this.summaryTotalPriceInitial += item.totalPriceInitial ? Number(item.totalPriceInitial) : 0;
      this.summaryTotalPriceInitialRefund += item.totalPriceInitialRefund ? Number(item.totalPriceInitialRefund) : 0;
      this.summaryTotalGrossProfit += item.totalGrossProfit ? Number(item.totalGrossProfit) : 0;
      this.summaryTotalProfitPerSaleRevenue += item.totalProfitPerSaleRevenue ? Number(item.totalProfitPerSaleRevenue) : 0;
      this.summaryTotalProfitPerPriceInitial += item.totalProfitPerPriceInitial ? Number(item.totalProfitPerPriceInitial) : 0;
    });
  }

  leftColNumber: number = 12;
  rightColNumber: number = 0;
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

  exportExcel() {
    let title = 'BÁO CÁO LỢI NHUẬN THEO MẶT HÀNG';
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('BAO_CAO');

    let imgBase64 = this.getBase64Logo();
    /* Image */
    var imgLogo = workbook.addImage({
      base64: imgBase64,
      extension: 'png',
    });
    worksheet.addImage(imgLogo, {
      tl: { col: 0, row: 0 },
      ext: { width: 155, height: 100 }
    });

    let dataRow1 = [];
    dataRow1[3] = this.inforExportExcel.companyName.toUpperCase();  //Tên công ty
    let row1 = worksheet.addRow(dataRow1);
    row1.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`C${row1.number}:H${row1.number}`);
    row1.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow2 = [];
    dataRow2[3] = 'Địa chỉ: ' + this.inforExportExcel.address;  //Địa chỉ
    let row2 = worksheet.addRow(dataRow2);
    row2.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`C${row2.number}:H${row2.number}`);
    row2.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow3 = [];
    dataRow3[3] = 'Điện thoại: ' + this.inforExportExcel.phone;  //Số điện thoại
    let row3 = worksheet.addRow(dataRow3);
    row3.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`C${row3.number}:H${row3.number}`);
    row3.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow4 = [];
    dataRow4[3] = 'Email: ' + this.inforExportExcel.email;
    let row4 = worksheet.addRow(dataRow4);
    row4.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`C${row4.number}:H${row4.number}`);
    row4.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow5 = [];
    dataRow5[3] = 'Website dịch vụ: ' + this.inforExportExcel.website;  //Địa chỉ website
    let row5 = worksheet.addRow(dataRow5);
    row5.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`C${row5.number}:H${row5.number}`);
    row5.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    worksheet.addRow([]);
    worksheet.addRow([]);
    worksheet.addRow([]);

    /* title */
    let titleRow = worksheet.addRow([title]);
    titleRow.font = { family: 4, size: 16, bold: true };
    titleRow.height = 25;
    titleRow.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A${titleRow.number}:O${titleRow.number}`);
    /* subtitle */
    let filterData = this.getDataFilterForm();
    let orderFromDate = filterData.OrderFromDate ? this.datePipe.transform(filterData.OrderFromDate, 'dd/MM/yyyy') : '';
    let orderToDate = filterData.OrderToDate ? this.datePipe.transform(filterData.OrderToDate, 'dd/MM/yyyy') : '';
    let subTitle = worksheet.addRow([`Từ ngày ${orderFromDate} đến ngày ${orderToDate}`]);
    subTitle.font = { family: 4, italic: true };
    subTitle.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A${subTitle.number}:O${subTitle.number}`);

    /* header */
    const header = this.colsList.map(e => e.header);
    let headerRow = worksheet.addRow(header);
    headerRow.font = { bold: true };
    header.forEach((item, index) => {
      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });

    /* data table */
    let data = this.listProductRevenue.map((item, index) => {
      let row = [];

      row.push(index + 1); //STT
      row.push(item.productName); /* Mã hàng */
      row.push(item.productCode); /* Tên hàng*/
      row.push(item.unitName); /* Đơn vị tính */
      row.push(item.orderCount ? this.decimalPipe.transform(item.orderCount) : '0'); /* Số lượng đơn hàng */
      row.push(item.productInOrderCount ? this.decimalPipe.transform(item.productInOrderCount) : 0); /* Số lượng bán */
      row.push(item.productRefundCount ? this.decimalPipe.transform(item.productRefundCount) : 0); /* Số lượng trả lại */
      row.push(item.saleRevenue ? this.decimalPipe.transform(item.saleRevenue) : 0); /* Doanh thu */
      row.push(item.saleRevenueRefund ? this.decimalPipe.transform(item.saleRevenueRefund) : 0); /* Doanh thu trả lại */
      row.push(item.totalDiscount ? this.decimalPipe.transform(item.totalDiscount) : 0); /* Chiết khấu */
      row.push(item.totalPriceInitial ? this.decimalPipe.transform(item.totalPriceInitial) : 0); /* Tiền vốn */
      row.push(item.totalPriceInitialRefund ? this.decimalPipe.transform(item.totalPriceInitialRefund) : 0); /* Tiền vốn trả lại */
      row.push(item.totalGrossProfit ? this.decimalPipe.transform(item.totalGrossProfit) : 0); /* Lãi gộp */
      row.push(item.totalProfitPerSaleRevenue ? this.decimalPipe.transform(item.totalProfitPerSaleRevenue) : 0); /* % lãi/DT */
      row.push(item.totalProfitPerPriceInitial ? this.decimalPipe.transform(item.totalProfitPerPriceInitial) : 0); /* % lãi/Giá vốn */

      return row;
    });

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      this.colsList.forEach((item, index) => {
        row.getCell(index + 1).alignment = { vertical: 'bottom', horizontal: item.textAlign, wrapText: true };
        row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      });
    });

    //get summary
    let summaryRow = [];
    summaryRow.push('Tổng cộng');
    summaryRow.push('');
    summaryRow.push('');
    summaryRow.push('');
    summaryRow.push(this.summaryOrderCount ? this.decimalPipe.transform(this.summaryOrderCount) : '0');
    summaryRow.push(this.summaryProductInOrderCount ? this.decimalPipe.transform(this.summaryProductInOrderCount) : '0');
    summaryRow.push(this.summaryProductRefundCount ? this.decimalPipe.transform(this.summaryProductRefundCount) : '0');
    summaryRow.push(this.summarySaleRevenue ? this.decimalPipe.transform(this.summarySaleRevenue) : '0');
    summaryRow.push(this.summarySaleRevenueRefund ? this.decimalPipe.transform(this.summarySaleRevenueRefund) : '0');
    summaryRow.push(this.summaryTotalDiscount ? this.decimalPipe.transform(this.summaryTotalDiscount) : '0');
    summaryRow.push(this.summaryTotalPriceInitial ? this.decimalPipe.transform(this.summaryTotalPriceInitial) : '0');
    summaryRow.push(this.summaryTotalPriceInitialRefund ? this.decimalPipe.transform(this.summaryTotalPriceInitialRefund) : '0');
    summaryRow.push(this.summaryTotalGrossProfit ? this.decimalPipe.transform(this.summaryTotalGrossProfit) : '0');
    summaryRow.push(this.summaryTotalProfitPerSaleRevenue ? this.decimalPipe.transform(this.summaryTotalProfitPerSaleRevenue) : '0');
    summaryRow.push(this.summaryTotalProfitPerPriceInitial ? this.decimalPipe.transform(this.summaryTotalProfitPerPriceInitial) : '0');

    let summaryData = [summaryRow];
    summaryData.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      worksheet.mergeCells(`A${row.number}:D${row.number}`);
      this.colsList.forEach((item, index) => {
        row.getCell(index + 1).alignment = { vertical: 'bottom', horizontal: 'right', wrapText: true };
        row.font = { bold: true };
        row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      });
    });

    /* set width */
    this.colsList.forEach((item, index) => {
      worksheet.getColumn(index + 1).width = item.excelWidth;
    });

    /* ký tên */

    worksheet.addRow([]);
    worksheet.addRow([]);

    let footer1 = [];
    footer1[10] = 'Ngày.....tháng.....năm..........';
    let rowFooter1 = worksheet.addRow(footer1);
    worksheet.mergeCells(`J${rowFooter1.number}:L${rowFooter1.number}`);
    rowFooter1.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    let footer2 = [];
    footer2[2] = 'NGƯỜI LẬP';
    footer2[10] = 'KẾ TOÁN TRƯỞNG';

    let rowFooter2 = worksheet.addRow(footer2);
    worksheet.mergeCells(`B${rowFooter2.number}:C${rowFooter2.number}`);
    worksheet.mergeCells(`J${rowFooter2.number}:L${rowFooter2.number}`);
    rowFooter2.font = { bold: true };
    rowFooter2.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    let footer3 = [];
    footer3[2] = '(Ký, họ tên)';
    footer3[10] = '(Ký, họ tên)';
    let rowFooter3 = worksheet.addRow(footer3);
    worksheet.mergeCells(`B${rowFooter3.number}:C${rowFooter3.number}`);
    worksheet.mergeCells(`J${rowFooter3.number}:L${rowFooter3.number}`);
    rowFooter3.font = { italic: true };
    rowFooter3.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    this.exportToExel(workbook, title);
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    });
  }

  exportPdf() {
    let title = 'BÁO CÁO TỔNG HỢP DOANH SỐ BÁN HÀNG THEO NHÂN VIÊN';
    let imgBase64 = this.getBase64Logo();
    let filterData = this.getDataFilterForm();
    let orderFromDate = filterData.OrderFromDate ? this.datePipe.transform(filterData.OrderFromDate, 'dd/MM/yyyy') : '';
    let orderToDate = filterData.OrderToDate ? this.datePipe.transform(filterData.OrderToDate, 'dd/MM/yyyy') : '';

    let documentDefinition: any = {
      pageSize: 'A4',
      pageOrientation: 'landscape',
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
                      text: "" + this.inforExportExcel.companyName.toUpperCase(),
                      style: {
                        fontSize: 10,
                        bold: true,
                      },
                      alignment: 'left'
                    },
                    {
                      text: '',
                      margin: [0, 2, 0, 2]
                    },
                    {
                      text: 'Địa chỉ: ' + this.inforExportExcel.address,
                      style: 'timer',
                      alignment: 'left'
                    },
                    {
                      text: '',
                      margin: [0, 2, 0, 2]
                    },
                    {
                      text: 'Điện thoại: ' + this.inforExportExcel.phone,
                      style: 'timer',
                      alignment: 'left'
                    },
                    {
                      text: '',
                      margin: [0, 2, 0, 2]
                    },
                    {
                      text: 'Email: ' + this.inforExportExcel.email,
                      style: 'timer',
                      alignment: 'left'
                    },
                    {
                      text: '',
                      margin: [0, 2, 0, 2]
                    },
                    {
                      text: 'Website dịch vụ: ' + this.inforExportExcel.website,
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
          margin: [0, 15, 0, 15]
        },
        {
          text: title,
          style: 'header',
          alignment: 'center'
        },
        {
          text: `Từ ngày ${orderFromDate} đến ngày ${orderToDate}`,
          fontSize: 10,
          alignment: 'center'
        },
        {
          style: 'table',
          table: {
            widths: this.colsList.map(e => e.pdfWidth),
            headerRows: 1,
            dontBreakRows: true,
            body: [
              this.colsList.map(col => ({
                text: col.header,
                style: 'tableHeader',
                alignment: 'center'
              }))
            ],
          },
          layout: {
            defaultBorder: true,
            paddingTop: function (i, node) { return 2; },
            paddingBottom: function (i, node) { return 2; }
          }
        },


        {
          columns: [
            {
              width: '50%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'right'
            },
            {
              width: '50%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'right'
            }
          ]
        },
        {
          columns: [
            {
              width: '50%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'right',
              margin: [0, 2, 0, 2]
            },
            {
              width: '50%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'right',
              margin: [0, 2, 0, 2]
            }
          ]
        },
        {
          columns: [
            {
              width: '50%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'right'
            },
            {
              width: '50%',
              text: 'Ngày.....tháng.....năm..........',
              style: { fontSize: 10, bold: false, italics: true },
              alignment: 'center'
            }
          ]
        },
        {
          columns: [
            {
              width: '50%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'center',
              margin: [0, 2, 0, 2]
            },
            {
              width: '50%',
              text: 'NGƯỜI LẬP',
              style: { fontSize: 10, bold: true },
              alignment: 'center',
              margin: [0, 2, 0, 2]
            }
          ]
        },
        {
          columns: [
            {
              width: '50%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'center',
              margin: [0, 2, 0, 2]
            },
            {
              width: '50%',
              text: '(Ký, họ tên)',
              style: { fontSize: 10, italics: true },
              alignment: 'center',
              margin: [0, 2, 0, 2]
            }
          ]
        },

      ],
      styles: {
        header: {
          fontSize: 13,
          bold: true
        },
        default: {
          fontSize: 10,
          bold: true
        },
        timer: {
          fontSize: 10,
          color: '#1F497D'
        },
        table: {
          margin: [0, 15, 0, 15]
        },
        tableHeader: {
          fontSize: 9,
          bold: true,
          fillColor: '#E3EDF8',
          margin: [0, 2, 0, 2],
        },
        tableLine: {
          fontSize: 10,
        },
        tableLines: {
          fontSize: 7,
        },
        summaryTableLines: {
          fontSize: 7,
          bold: true
        },
        tableLiness: {
          fontSize: 7,
        },
        StyleItalics: {
          italics: true
        }
      }
    };

    this.listProductRevenue.forEach((item, index) => {
      let option = [
        { text: index + 1, style: 'tableLines', alignment: 'center' },
        {
          text: item.productName,
          style: 'tableLines',
          alignment: 'center'
        },
        {
          text: item.productCode,
          style: 'tableLines',
          alignment: 'left'
        },
        {
          text: item.unitName,
          style: 'tableLines',
          alignment: 'left'
        },
        {
          text: item.orderCount ? this.decimalPipe.transform(item.orderCount) : '0',
          style: 'tableLines',
          alignment: 'right'
        },
        {
          text: item.productInOrderCount ? this.decimalPipe.transform(item.productInOrderCount) : '0',
          style: 'tableLines',
          alignment: 'right'
        },
        {
          text: item.productRefundCount ? this.decimalPipe.transform(item.productRefundCount) : '0',
          style: 'tableLines',
          alignment: 'right'
        },
        {
          text: item.saleRevenue ? this.decimalPipe.transform(item.saleRevenue) : '0',
          style: 'tableLines',
          alignment: 'right'
        },
        {
          text: item.saleRevenueRefund ? this.decimalPipe.transform(item.saleRevenueRefund) : '0',
          style: 'tableLines',
          alignment: 'right'
        },
        {
          text: item.totalDiscount ? this.decimalPipe.transform(item.totalDiscount) : '0',
          style: 'tableLines',
          alignment: 'right'
        },
        {
          text: item.totalPriceInitial ? this.decimalPipe.transform(item.totalPriceInitial) : '0',
          style: 'tableLines',
          alignment: 'right'
        },
        {
          text: item.totalPriceInitialRefund ? this.decimalPipe.transform(item.totalPriceInitialRefund) : '0',
          style: 'tableLines',
          alignment: 'right'
        },
        {
          text: item.totalGrossProfit ? this.decimalPipe.transform(item.totalGrossProfit) : '0',
          style: 'tableLines',
          alignment: 'right'
        },
        {
          text: item.totalProfitPerSaleRevenue ? this.decimalPipe.transform(item.totalProfitPerSaleRevenue) : '0',
          style: 'tableLines',
          alignment: 'right'
        },
        {
          text: item.totalProfitPerPriceInitial ? this.decimalPipe.transform(item.totalProfitPerPriceInitial) : '0',
          style: 'tableLines',
          alignment: 'right'
        },
      ];
      documentDefinition.content[4].table.body.push(option);
    });

    let summary = [
      { text: '', style: 'tableLines', alignment: 'center' },
      {
        text: '',
        style: 'tableLines',
        alignment: 'center'
      },
      {
        text: '',
        style: 'tableLines',
        alignment: 'center'
      },
      {
        text: 'Tổng số',
        style: 'summaryTableLines',
        alignment: 'right'
      },
      {
        text: this.summaryOrderCount ? this.decimalPipe.transform(this.summaryOrderCount) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },
      {
        text: this.summaryProductInOrderCount ? this.decimalPipe.transform(this.summaryProductInOrderCount) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },

      {
        text: this.summaryProductRefundCount ? this.decimalPipe.transform(this.summaryProductRefundCount) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },

      {
        text: this.summarySaleRevenue ? this.decimalPipe.transform(this.summarySaleRevenue) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },
      {
        text: this.summarySaleRevenueRefund ? this.decimalPipe.transform(this.summarySaleRevenueRefund) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },
      {
        text: this.summaryTotalDiscount ? this.decimalPipe.transform(this.summaryTotalDiscount) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },
      {
        text: this.summaryTotalPriceInitial ? this.decimalPipe.transform(this.summaryTotalPriceInitial) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },
      {
        text: this.summaryTotalPriceInitialRefund ? this.decimalPipe.transform(this.summaryTotalPriceInitialRefund) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },
      {
        text: this.summaryTotalGrossProfit ? this.decimalPipe.transform(this.summaryTotalGrossProfit) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },
      {
        text: this.summaryTotalProfitPerSaleRevenue ? this.decimalPipe.transform(this.summaryTotalProfitPerSaleRevenue) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },
      {
        text: this.summaryTotalProfitPerPriceInitial ? this.decimalPipe.transform(this.summaryTotalProfitPerPriceInitial) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },
    ];

    documentDefinition.content[4].table.body.push(summary);

    pdfMake.createPdf(documentDefinition).download(title + '.pdf');

  }

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo?.systemValueString;
  }

  getDataFilterForm(): filterModel {
    let productCode: string = this.filterForm.get('ProductCode').value;
    let productCategory: Array<ProductCategory> = this.filterForm.get('ProductCategory').value;
    let orderFromDate: Date = this.filterForm.get('OrderFromDate').value;
    let orderToDate: Date = this.filterForm.get('OrderToDate').value;

    let saleRevenueFrom: string = this.filterForm.get('SaleRevenueFrom').value;
    let saleRevenueTo: string = this.filterForm.get('SaleRevenueTo').value;

    let productInOrderCountFrom: string = this.filterForm.get('ProductInOrderCountFrom').value;
    let productInOrderCountTo: string = this.filterForm.get('ProductInOrderCountTo').value;

    let orderCountFrom: string = this.filterForm.get('OrderCountFrom').value;
    let orderCountTo: string = this.filterForm.get('OrderCountTo').value;

    let productRefundCountFrom: string = this.filterForm.get('ProductRefundCountFrom').value;
    let productRefundCountTo: string = this.filterForm.get('ProductRefundCountTo').value;

    if (orderFromDate) orderFromDate.setHours(0, 0, 0, 0);
    if (orderToDate) orderToDate.setHours(23, 59, 59, 999);
    //advanced filter

    let warehouse: Array<WarehouseModel> = this.advancedFilterForm.get('Warehouse').value;
    let seller: Employee = this.advancedFilterForm.get('Seller').value;
    let customerGroup: Array<categoryModel> = this.advancedFilterForm.get('CustomerGroup').value;
    let customerType: Array<customerTypeModel> = this.advancedFilterForm.get('CustomerType').value;
    let customer: Array<customerModel> = this.advancedFilterForm.get('Customer').value;

    let filterData = new filterModel();

    filterData.ProductCode = productCode ? productCode.toString().trim() : '';
    filterData.ProductCategory = productCategory.map(e => e.productCategoryId);

    filterData.OrderFromDate = orderFromDate ? convertToUTCTime(orderFromDate) : null;
    filterData.OrderToDate = orderToDate ? convertToUTCTime(orderToDate) : null;

    filterData.SaleRevenueFrom = saleRevenueFrom ? Number(saleRevenueFrom.replace(/,/g, "")) : null;
    filterData.SaleRevenueTo = saleRevenueTo ? Number(saleRevenueTo.replace(/,/g, "")) : null;

    filterData.ProductInOrderCountFrom = productInOrderCountFrom ? Number(productInOrderCountFrom.replace(/,/g, "")) : null;
    filterData.ProductInOrderCountTo = productInOrderCountTo ? Number(productInOrderCountTo.replace(/,/g, "")) : null;

    filterData.OrderCountFrom = orderCountFrom ? Number(orderCountFrom.replace(/,/g, "")) : null;
    filterData.OrderCountTo = orderCountTo ? Number(orderCountTo.replace(/,/g, "")) : null;

    filterData.ProductRefundCountFrom = productRefundCountFrom ? Number(productRefundCountFrom.replace(/,/g, "")) : null;
    filterData.ProductRefundCountTo = productRefundCountTo ? Number(productRefundCountTo.replace(/,/g, "")) : null;

    filterData.Warehouse = warehouse.map(e => e.warehouseId);
    filterData.Seller = seller ? seller.employeeId : null;
    filterData.CustomerGroup = customerGroup.map(e => e.categoryId);
    filterData.CustomerType = customerType.map(e => e.value);
    filterData.Customer = customer.map(e => e.customerId);

    return filterData;
  }

  async search() {
    let filterData = this.getDataFilterForm();
    this.loading = true;
    let result: any = await this.topRevenueService.searchRevenueProduct(filterData, this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listProductRevenue = result.listProductRevenue ?? [];
      this.handlerReportAfterSearch();
    } else {
      this.showToast('error', 'Thông báo', 'Lấy dữ liệu thất bại')
    }

  }

  resetTable() {
    if (!this.table) return;
    this.filterGlobal = '';
    this.table.sortField = '';
    this.table.reset();
  }

  refreshFilter() {
    this.resetTable();
    let defaultFilter = this.getDefaultFilter();
    this.patchInitFormData(defaultFilter);
    this.search();
  }

  getDefaultFilter(): filterModel {
    let currentDate = new Date();
    let currentYear = currentDate.getFullYear();
    let curretMonth = currentDate.getMonth();

    let fromDate = new Date(currentYear, curretMonth, 1);
    fromDate.setHours(0, 0, 0, 0);

    let toDate = currentDate;
    toDate.setHours(23, 59, 59, 999);

    let filterData = new filterModel();

    filterData.OrderFromDate = convertToUTCTime(fromDate);
    filterData.OrderToDate = convertToUTCTime(toDate);

    return filterData;
  }

  patchInitFormData(defaultFilter: filterModel) {
    let currentDate = new Date();
    let currentYear = currentDate.getFullYear();
    let curretMonth = currentDate.getMonth();
    let fromDate = new Date(currentYear, curretMonth, 1);
    fromDate.setHours(0, 0, 0, 0);
    let toDate = currentDate;
    toDate.setHours(23, 59, 59, 999);
    this.filterForm.get('ProductCode').patchValue('');
    this.filterForm.get('ProductCategory').patchValue([]);
    this.filterForm.get('OrderFromDate').patchValue(fromDate);
    this.filterForm.get('OrderToDate').patchValue(toDate);
    this.filterForm.get('SaleRevenueFrom').patchValue(null);
    this.filterForm.get('SaleRevenueTo').patchValue(null);
    this.filterForm.get('ProductInOrderCountFrom').patchValue(null);
    this.filterForm.get('ProductInOrderCountTo').patchValue(null);
    this.filterForm.get('OrderCountFrom').patchValue(null);
    this.filterForm.get('OrderCountTo').patchValue(null);
    this.filterForm.get('ProductRefundCountFrom').patchValue(null);
    this.filterForm.get('ProductRefundCountTo').patchValue(null);
    //advanced filter
    this.advancedFilterForm.get('Warehouse').patchValue([]);
    this.advancedFilterForm.get('Seller').patchValue(null);
    this.advancedFilterForm.get('CustomerGroup').patchValue([]);
    this.advancedFilterForm.get('CustomerType').patchValue([]);
    this.advancedFilterForm.get('Customer').patchValue([]);
  }

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

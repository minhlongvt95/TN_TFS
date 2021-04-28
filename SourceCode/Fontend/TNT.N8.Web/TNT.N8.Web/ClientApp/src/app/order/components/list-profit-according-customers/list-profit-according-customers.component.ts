import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';
import { CustomerOrderService } from '../../services/customer-order.service';
import { GetPermission } from '../../../shared/permission/get-permission';

import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DecimalPipe } from '@angular/common';
import * as pdfMake from 'pdfmake/build/pdfmake.js';
import * as pdfFonts from 'pdfmake/build/vfs_fonts.js';
import { FormControl, FormGroup } from '@angular/forms';

class customerTypeModel {
  label: string;
  value: number;
}

class InforExportExcel {
  companyName: string;
  address: string;
  phone: string;
  website: string;
  email: string;
  textTotalMoney: string
}

class contractModel {
  contractId: string;
  contractCode: string;
  amount: number;
}

class customerModel {
  customerId: string;
  customerName: string;
  customerCode: string;
}


class categoryModel {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

class productCategoryModel {
  productCategoryId: string;
  productCategoryName: string;
  productCategoryCode: string;
}

class filterModel {
  //basic filter
  ListCustomer: Array<string>;
  ListCustomerType: Array<number>;
  ListCustomerGroup: Array<string>;
  OrderFromDate: Date;
  OrderToDate: Date;
  SaleRevenueFrom: number;
  SaleRevenueTo: number;
  //advanced filter
  OrderCode: string;
  ListProductCategory: Array<string>;
  ListContract: Array<string>;
  ProductCode: string;
  QuoteCode: string;

  constructor() {
    this.ListCustomer = [];
    this.ListCustomerType = [];
    this.ListCustomerGroup = [];
    this.OrderFromDate = null;
    this.OrderToDate = null;
    this.SaleRevenueFrom = null;
    this.SaleRevenueTo = null;
    this.OrderCode = '';
    this.ListProductCategory = [];
    this.ListContract = [];
    this.ProductCode = '';
    this.QuoteCode = '';
  }
}

class profitCustomerModel {
  index: number;
  customerCode: string;
  customerName: string;
  saleRevenue: number;
  totalPriceInitial: number;
  totalGrossProfit: number;
  totalProfitPerSaleRevenue: number;
  totalProfitPerPriceInitial: number
}

@Component({
  selector: 'app-list-order',
  templateUrl: './list-profit-according-customers.component.html',
  styleUrls: ['./list-profit-according-customers.component.css'],
  providers: [
    DecimalPipe,
    DatePipe
  ]
})
export class ListProfitAccordingCustomerComponent implements OnInit {
  /*Check user permission*/
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
  inforExportExcel: InforExportExcel;
  listContract: Array<contractModel> = [];
  listCustomer: Array<customerModel> = [];
  listCustomerGroupCategory: Array<categoryModel> = [];
  listProductCategory: Array<productCategoryModel> = [];

  /* search response data */
  listProfitCustomer: Array<profitCustomerModel> = [];
  summarySaleRevenue: number = 0;
  summaryTotalPriceInitial: number = 0;
  summaryTotalGrossProfit: number = 0;
  // summaryTotalProfitPerSaleRevenue: number = 0;
  // summaryTotalProfitPerPriceInitial: number = 0;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private customerOrderService: CustomerOrderService,
    private messageService: MessageService,
    private decimalPipe: DecimalPipe,
    private confirmationService: ConfirmationService,
    private datePipe: DatePipe,
  ) {
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "sal/order/list-profit-according-customers";
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
      { field: 'index', header: 'STT', textAlign: 'center', display: 'table-cell', width: '75px', excelWidth: 10, pdfWidth: 30 },
      { field: 'customerCode', header: 'Mã khách hàng', textAlign: 'left', display: 'table-cell', width: '150px', excelWidth: 20, pdfWidth: 100 },
      { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left', display: 'table-cell', width: '170px', excelWidth: 30, pdfWidth: 120 },
      { field: 'saleRevenue', header: 'Doanh thu', textAlign: 'right', display: 'table-cell', width: '180px', excelWidth: 30, pdfWidth: 90 },
      { field: 'totalPriceInitial', header: 'Tiền vốn', textAlign: 'right', display: 'table-cell', width: '180px', excelWidth: 20, pdfWidth: 90 },
      { field: 'totalGrossProfit', header: 'Lãi gộp', textAlign: 'right', display: 'table-cell', width: '180px', excelWidth: 20, pdfWidth: 90 },
      { field: 'totalProfitPerSaleRevenue', header: '%lãi/DT', textAlign: 'right', display: 'table-cell', width: '180px', excelWidth: 30, pdfWidth: 90 },
      { field: 'totalProfitPerPriceInitial', header: '%lãi/Tiền vốn', textAlign: 'right', display: 'table-cell', width: '180px', excelWidth: 20, pdfWidth: 90 },
    ];
    this.selectedColumns = this.colsList;
  }

  initForm() {
    this.filterForm = new FormGroup({
      'Customer': new FormControl([]),
      'CustomerType': new FormControl([]),
      'CustomerGroup': new FormControl([]),
      'OrderFromDate': new FormControl(null),
      'OrderToDate': new FormControl(null),
      'SaleRevenueFrom': new FormControl(null),
      'SaleRevenueTo': new FormControl(null),
    });

    this.advancedFilterForm = new FormGroup({
      'OrderCode': new FormControl(''),
      'ProductCategory': new FormControl([]),
      'Contract': new FormControl([]),
      'ProductCode': new FormControl(''),
      'QuoteCode': new FormControl(''),
    });
  }

  async getMasterdata() {
    //default filter
    let defaultFilter = this.getDefaultFilter();
    this.patchInitFormData(defaultFilter);
    let filterData = this.getDataFilterForm();

    this.loading = true;
    let [result, searchResult]: any = await Promise.all([
      this.customerOrderService.getDataProfitByCustomer(this.auth.UserId),
      this.customerOrderService.searchProfitCustomer(filterData, this.auth.UserId)
    ]);
    this.loading = false;

    if (result.statusCode === 200 && searchResult.statusCode === 200) {
      /* master data */
      this.listProductCategory = result.listProductCategory ?? [];
      this.inforExportExcel = result.inforExportExcel;
      this.listCustomer = result.listCustomer;
      this.listCustomerGroupCategory = result.listCustomerGroupCategory ?? [];
      this.listContract = result.listContract ?? [];
      /* search data */
      this.listProfitCustomer = searchResult.listSearchProfitCustomer;
      this.handlerReportAfterSearch();
    } else {
      this.showToast('error', 'Thông báo', 'Lấy dữ liệu thất bại')
    }
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
    let title = 'BÁO CÁO LỢI NHUẬN THEO KHÁCH HÀNG';
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

    /* title */
    let titleRow = worksheet.addRow([title]);
    titleRow.font = { family: 4, size: 16, bold: true };
    titleRow.height = 25;
    titleRow.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A${titleRow.number}:H${titleRow.number}`);
    /* subtitle */
    let filterData = this.getDataFilterForm();
    let orderFromDate = filterData.OrderFromDate ? this.datePipe.transform(filterData.OrderFromDate, 'dd/MM/yyyy') : '';
    let orderToDate = filterData.OrderToDate ? this.datePipe.transform(filterData.OrderToDate, 'dd/MM/yyyy') : '';
    let subTitle = worksheet.addRow([`Từ ngày ${orderFromDate} đến ngày ${orderToDate}`]);
    subTitle.font = { family: 4, italic: true };
    subTitle.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A${subTitle.number}:H${subTitle.number}`);

    /* header */
    const header = this.colsList.map(e => e.header);
    let headerRow = worksheet.addRow(header);
    headerRow.font = { bold: true };
    header.forEach((item, index) => {
      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });

    /* data table */
    let data = this.listProfitCustomer.map((item, index) => {
      let row = [];

      row.push(index + 1); //STT
      row.push(item.customerCode); /* Mã khách hàng */
      row.push(item.customerName); /*Tên khách hàng */
      row.push(item.saleRevenue ? this.decimalPipe.transform(item.saleRevenue) : '0'); /* Doanh thu */
      row.push(item.totalPriceInitial ? this.decimalPipe.transform(item.totalPriceInitial) : 0); /* Tiền vốn */
      row.push(item.totalGrossProfit ? this.decimalPipe.transform(item.totalGrossProfit) : 0); /* Lãi gộp */
      row.push(item.totalProfitPerSaleRevenue ? this.decimalPipe.transform(item.totalProfitPerSaleRevenue) : 0); /* %lãi/DT */
      row.push(item.totalProfitPerPriceInitial ? this.decimalPipe.transform(item.totalProfitPerPriceInitial) : 0); /* %lãi/Giá vốn*/

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
    summaryRow.push(this.summarySaleRevenue ? this.decimalPipe.transform(this.summarySaleRevenue) : '0');
    summaryRow.push(this.summaryTotalPriceInitial ? this.decimalPipe.transform(this.summaryTotalPriceInitial) : '0');
    summaryRow.push(this.summaryTotalGrossProfit ? this.decimalPipe.transform(this.summaryTotalGrossProfit) : '0');

    let summaryData = [summaryRow];
    summaryData.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      worksheet.mergeCells(`A${row.number}:C${row.number}`);
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
    footer1[6] = 'Ngày.....tháng.....năm..........';
    let rowFooter1 = worksheet.addRow(footer1);
    worksheet.mergeCells(`F${rowFooter1.number}:H${rowFooter1.number}`);
    rowFooter1.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    let footer2 = [];
    footer2[1] = 'KẾ TOÁN TRƯỞNG';
    footer2[6] = 'NGƯỜI LẬP';

    let rowFooter2 = worksheet.addRow(footer2);
    worksheet.mergeCells(`A${rowFooter2.number}:B${rowFooter2.number}`);
    worksheet.mergeCells(`F${rowFooter2.number}:H${rowFooter2.number}`);
    rowFooter2.font = { bold: true };
    rowFooter2.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    let footer3 = [];
    footer3[1] = '(Ký, họ tên)';
    footer3[6] = '(Ký, họ tên)';
    let rowFooter3 = worksheet.addRow(footer3);
    worksheet.mergeCells(`A${rowFooter3.number}:B${rowFooter3.number}`);
    worksheet.mergeCells(`F${rowFooter3.number}:H${rowFooter3.number}`);
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
          fontSize: 10,
          bold: true,
          fillColor: '#E3EDF8',
          margin: [0, 2, 0, 2],
        },
        tableLine: {
          fontSize: 10,
        },
        tableLines: {
          fontSize: 10,
        },
        summaryTableLines: {
          fontSize: 10,
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

    this.listProfitCustomer.forEach((item, index) => {
      let option = [
        { text: index + 1, style: 'tableLines', alignment: 'center' },
        {
          text: item.customerCode,
          style: 'tableLines',
          alignment: 'center'
        },
        {
          text: item.customerName,
          style: 'tableLines',
          alignment: 'left'
        },
        {
          text: item.saleRevenue ? this.decimalPipe.transform(item.saleRevenue) : '0',
          style: 'tableLines',
          alignment: 'right'
        },
        {
          text: item.totalPriceInitial ? this.decimalPipe.transform(item.totalPriceInitial) : '0',
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
        text: 'Tổng số',
        style: 'summaryTableLines',
        alignment: 'right'
      },
      {
        text: this.summarySaleRevenue ? this.decimalPipe.transform(this.summarySaleRevenue) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },
      {
        text: this.summaryTotalPriceInitial ? this.decimalPipe.transform(this.summaryTotalPriceInitial) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },

      {
        text: this.summaryTotalGrossProfit ? this.decimalPipe.transform(this.summaryTotalGrossProfit) : '0',
        style: 'summaryTableLines',
        alignment: 'right'
      },
      { text: '', style: 'tableLines', alignment: 'center' },
      { text: '', style: 'tableLines', alignment: 'center' },
    ];

    documentDefinition.content[4].table.body.push(summary);

    pdfMake.createPdf(documentDefinition).download(title + '.pdf');

  }

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo?.systemValueString;
  }

  getDataFilterForm(): filterModel {
    //basic filter
    let customer: Array<customerModel> = this.filterForm.get('Customer').value;
    let customerType: Array<customerTypeModel> = this.filterForm.get('CustomerType').value;
    let customerGroup: Array<categoryModel> = this.filterForm.get('CustomerGroup').value;
    let orderFromDate: Date = this.filterForm.get('OrderFromDate').value;
    let orderToDate: Date = this.filterForm.get('OrderToDate').value;
    let saleRevenueFrom: string = this.filterForm.get('SaleRevenueFrom').value;
    let saleRevenueTo: string = this.filterForm.get('SaleRevenueTo').value;
    if (orderFromDate) orderFromDate.setHours(0, 0, 0, 0);
    if (orderToDate) orderToDate.setHours(23, 59, 59, 999);
    //advanced filter
    let orderCode: string = this.advancedFilterForm.get('OrderCode').value;
    let productCategory: Array<productCategoryModel> = this.advancedFilterForm.get('ProductCategory').value;
    let contract: Array<contractModel> = this.advancedFilterForm.get('Contract').value;
    let productCode: string = this.advancedFilterForm.get('ProductCode').value;
    let quoteCode: string = this.advancedFilterForm.get('QuoteCode').value;

    let filterData = new filterModel();
    //basic filter
    filterData.ListCustomer = customer.map(e => e.customerId);
    filterData.ListCustomerType = customerType.map(e => e.value);
    filterData.ListCustomerGroup = customerGroup.map(e => e.categoryId);
    filterData.OrderFromDate = orderFromDate ? convertToUTCTime(orderFromDate) : null;
    filterData.OrderToDate = orderToDate ? convertToUTCTime(orderToDate) : null;
    filterData.SaleRevenueFrom = saleRevenueFrom ? Number(saleRevenueFrom.replace(/,/g, "")) : null;
    filterData.SaleRevenueTo = saleRevenueTo ? Number(saleRevenueTo.replace(/,/g, "")) : null;
    //advanced filter
    filterData.OrderCode = orderCode ? orderCode.toString().trim() : '';
    filterData.ListProductCategory = productCategory.map(e => e.productCategoryId);
    filterData.ListContract = contract.map(e => e.contractId);
    filterData.ProductCode = productCode ? productCode.toString().trim() : '';
    filterData.QuoteCode = quoteCode ? quoteCode.toString().trim() : '';

    return filterData;
  }

  async search() {
    let filterData = this.getDataFilterForm();
    this.loading = true;
    let result: any = await this.customerOrderService.searchProfitCustomer(filterData, this.auth.UserId);

    this.loading = false;
    if (result.statusCode === 200) {
      this.listProfitCustomer = result.listSearchProfitCustomer;
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

  handlerReportAfterSearch() {
    this.summarySaleRevenue = 0;
    this.summaryTotalPriceInitial = 0;
    this.summaryTotalGrossProfit = 0;
    // this.summaryTotalProfitPerSaleRevenue = 0;
    // this.summaryTotalProfitPerPriceInitial = 0;
    this.listProfitCustomer.forEach((item, index) => {
      item.index = index + 1
      this.summarySaleRevenue += item.saleRevenue ? Number(item.saleRevenue) : 0;
      this.summaryTotalPriceInitial += item.totalPriceInitial ? Number(item.totalPriceInitial) : 0;
      this.summaryTotalGrossProfit += item.totalGrossProfit ? Number(item.totalGrossProfit) : 0;
      // this.summaryTotalProfitPerSaleRevenue += item.totalProfitPerSaleRevenue ? Number(item.totalProfitPerSaleRevenue) : 0;
      // this.summaryTotalProfitPerPriceInitial += item.totalProfitPerPriceInitial ? Number(item.totalProfitPerPriceInitial) : 0;
    });
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
    //basic filter
    this.filterForm.get('Customer').patchValue([]);
    this.filterForm.get('CustomerType').patchValue([]);
    this.filterForm.get('CustomerGroup').patchValue([]);
    this.filterForm.get('OrderFromDate').patchValue(fromDate);
    this.filterForm.get('OrderToDate').patchValue(toDate);
    this.filterForm.get('SaleRevenueFrom').patchValue(null);
    this.filterForm.get('SaleRevenueTo').patchValue(null);
    //advanced filter
    this.advancedFilterForm.get('OrderCode').patchValue('');
    this.advancedFilterForm.get('ProductCategory').patchValue([]);
    this.advancedFilterForm.get('Contract').patchValue([]);
    this.advancedFilterForm.get('ProductCode').patchValue('');
    this.advancedFilterForm.get('QuoteCode').patchValue('');
  }

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

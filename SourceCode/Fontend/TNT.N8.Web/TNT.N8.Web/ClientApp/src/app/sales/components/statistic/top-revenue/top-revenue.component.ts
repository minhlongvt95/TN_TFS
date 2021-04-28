import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { OrganizationpopupComponent } from '../../../../shared/components/organizationpopup/organizationpopup.component';
import { Router } from '@angular/router';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { Table } from 'primeng/table';
import { MessageService, SortEvent } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { OrganizationDialogComponent } from '../../../../shared/components/organization-dialog/organization-dialog.component';
import { DialogOrganizationPermissionsionComponent } from '../../../../shared/components/dialog-organization-permissionsion/dialog-organization-permissionsion.component';
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

class customerTypeModel {
  label: string;
  value: number;
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

class topRevenueModel {
  index: number;
  employeeCode: string;
  employeeName: string;
  amount: number;
  totalProductInOrder: number;
  totalOrder: number;
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
  ListSellerId: Array<string>; //nhan vien ban hang
  OrganizationId: string;
  OrderFromDate: Date;
  OrderToDate: Date;
  AmountFrom: number;
  AmountTo: number;
  TotalProductInOrderFrom: number;
  TotalProductInOrderTo: number;
  TotalOrderFrom: number;
  TotalOrderTo: number;
  //advanced filter
  ListCustomerType: Array<number>;
  ListCustomer: Array<string>;
  ListCustomerGroup: Array<string>;
  ListProductGroup: Array<string>;
  ProductCode: string;

  constructor() {
    this.OrderFromDate = null;
    this.OrderToDate = null;
    this.AmountFrom = null;
    this.AmountTo = null;
    this.TotalProductInOrderFrom = null;
    this.TotalProductInOrderTo = null;
    this.TotalOrderFrom = null;
    this.TotalOrderTo = null;

    this.ListSellerId = [];
    this.ListCustomerType = [];
    this.ListCustomer = [];
    this.ListCustomerGroup = [];
    this.ListProductGroup = [];
  }
}

class organization {
  organizationId: string;
  organizationName: string;
}

@Component({
  selector: 'app-top-revenue',
  templateUrl: './top-revenue.component.html',
  styleUrls: ['./top-revenue.component.css'],
  providers: [DatePipe, DecimalPipe]
})

export class TopRevenueComponent implements OnInit {
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
  listEmployee: Array<Employee> = [];
  listCustomerType: Array<customerTypeModel> = [{ label: "Doanh nghiệp", value: 1 }, { label: "Cá nhân", value: 2 }]
  listCustomer: Array<customerModel> = [];
  listCustomerGroupCategory: Array<categoryModel> = [];
  listProductCategory: Array<productCategoryModel> = [];
  currentOrganization: organization;
  inforExportExcel: InforExportExcel;

  /* dialog */
  selectedOrgId: string = null;
  selectedOrgName: string = ''

  /* search response data */
  listTopEmployeeRevenue: Array<topRevenueModel> = [];
  summaryAmount: number = 0;
  summaryTotalProductInOrder: number = 0;
  summaryTotalOrder: number = 0;

  constructor(
    private getPermission: GetPermission,
    private router: Router,
    private messageService: MessageService,
    public dialogService: DialogService,
    private translate: TranslateService,
    private datePipe: DatePipe,
    private decimalPipe: DecimalPipe,
    private topRevenueService: TopRevenueService,
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    let resource = "sal/sales/top-revenue";
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

  initTable() {
    this.colsList = [
      { field: 'index', header: 'STT', textAlign: 'center', display: 'table-cell', width: '73px', excelWidth: 10 },
      { field: 'employeeCode', header: 'Mã nhân viên', textAlign: 'left', display: 'table-cell', width: '120px', excelWidth: 20 },
      { field: 'employeeName', header: 'Tên nhân viên', textAlign: 'left', display: 'table-cell', width: '210px', excelWidth: 30 },
      { field: 'amount', header: 'Doanh thu', textAlign: 'right', display: 'table-cell', width: '180px', excelWidth: 30 },
      { field: 'totalProductInOrder', header: 'SL bán', textAlign: 'right', display: 'table-cell', width: '120px', excelWidth: 20 },
      { field: 'totalOrder', header: 'SL đơn hàng', textAlign: 'right', display: 'table-cell', width: '120px', excelWidth: 20 },
    ];
    this.selectedColumns = this.colsList;
  }

  initForm() {
    this.filterForm = new FormGroup({
      'Seller': new FormControl(null),
      'OrderFromDate': new FormControl(null),
      'OrderToDate': new FormControl(null),
      'AmountFrom': new FormControl(null),
      'AmountTo': new FormControl(null),
      'TotalProductInOrderFrom': new FormControl(null),
      'TotalProductInOrderTo': new FormControl(null),
      'TotalOrderFrom': new FormControl(null),
      'TotalOrderTo': new FormControl(null),
    });

    this.advancedFilterForm = new FormGroup({
      'CustomerType': new FormControl([]),
      'Customer': new FormControl([]),
      'CustomerGroup': new FormControl([]),
      'ProductGroup': new FormControl([]),
      'ProductCode': new FormControl(''),
    })
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.topRevenueService.getDataSearchTopReVenue(this.auth.UserId);
    if (result.statusCode === 200) {
      this.listEmployee = result.listEmployee ?? [];
      this.currentOrganization = result.currentOrganization;
      this.listCustomer = result.listCustomer;
      this.listCustomerGroupCategory = result.listCustomerGroupCategory;
      this.listProductCategory = result.listProductCategory;
      this.inforExportExcel = result.inforExportExcel;

      let defaultFilter = this.getDefaultFilter();
      this.patchInitFormData(defaultFilter);

      let resultSearch: any = await this.topRevenueService.searchTopReVenue(defaultFilter, this.auth.UserId);
      this.listTopEmployeeRevenue = resultSearch.listTopEmployeeRevenue ?? [];
      this.handleReportAfterSearch();

    } else {
      this.showToast('error', 'Thông báo', 'Lấy dữ liệu thất bại')
    }

    this.loading = false;
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
    filterData.OrganizationId = this.selectedOrgId;
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

    this.filterForm.get('Seller').patchValue(null);
    this.filterForm.get('OrderFromDate').patchValue(fromDate);
    this.filterForm.get('OrderToDate').patchValue(toDate);
    this.selectedOrgId = this.currentOrganization.organizationId;
    this.selectedOrgName = this.currentOrganization.organizationName;
    this.filterForm.get('AmountFrom').patchValue(null);
    this.filterForm.get('AmountTo').patchValue(null);
    this.filterForm.get('TotalProductInOrderFrom').patchValue(null);
    this.filterForm.get('TotalProductInOrderTo').patchValue(null);
    this.filterForm.get('TotalOrderFrom').patchValue(null);
    this.filterForm.get('TotalOrderTo').patchValue(null);

    this.advancedFilterForm.get('CustomerType').patchValue([]);
    this.advancedFilterForm.get('Customer').patchValue([]);
    this.advancedFilterForm.get('CustomerGroup').patchValue([]);
    this.advancedFilterForm.get('ProductGroup').patchValue([]);
    this.advancedFilterForm.get('ProductCode').patchValue('');
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

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  async search() {
    this.resetTable();
    let filterData = this.getDataFilterForm();
    this.loading = true;
    let result: any = await this.topRevenueService.searchTopReVenue(filterData, this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listTopEmployeeRevenue = result.listTopEmployeeRevenue ?? [];
      this.handleReportAfterSearch();
    } else {
      this.showToast('error', 'Thông báo', 'Lấy dữ liệu thất bại')
    }

  }

  getDataFilterForm() {
    let seller: Employee = this.filterForm.get('Seller').value;
    let orderFromDate = this.filterForm.get('OrderFromDate').value;
    let orderToDate = this.filterForm.get('OrderToDate').value;
    let amountFrom = this.filterForm.get('AmountFrom').value;
    let amountTo = this.filterForm.get('AmountTo').value;
    let totalProductInOrderFrom = this.filterForm.get('TotalProductInOrderFrom').value;
    let totalProductInOrderTo = this.filterForm.get('TotalProductInOrderTo').value;
    let totalOrderFrom = this.filterForm.get('TotalOrderFrom').value;
    let totalOrderTo = this.filterForm.get('TotalOrderTo').value;

    let customerType: Array<customerTypeModel> = this.advancedFilterForm.get('CustomerType').value;
    let customer: Array<customerModel> = this.advancedFilterForm.get('Customer').value;
    let customerGroup: Array<categoryModel> = this.advancedFilterForm.get('CustomerGroup').value;
    let productGroup: Array<productCategoryModel> = this.advancedFilterForm.get('ProductGroup').value;
    let productCode: string = this.advancedFilterForm.get('ProductCode').value;

    if (orderFromDate) orderFromDate.setHours(0, 0, 0, 0);
    if (orderToDate) orderToDate.setHours(23, 59, 59, 999);

    let filterData = new filterModel();
    if (seller != null) {
      filterData.ListSellerId.push(seller.employeeId);
    }
    //basic filter
    filterData.OrganizationId = this.selectedOrgId;
    filterData.OrderFromDate = orderFromDate ? convertToUTCTime(orderFromDate) : null;
    filterData.OrderToDate = orderToDate ? convertToUTCTime(orderToDate) : null;
    filterData.AmountFrom = amountFrom ? Number(amountFrom.replace(/,/g, "")) : null;
    filterData.AmountTo = amountTo ? Number(amountTo.replace(/,/g, "")) : null;
    filterData.TotalProductInOrderFrom = totalProductInOrderFrom ? Number(totalProductInOrderFrom.replace(/,/g, "")) : null;
    filterData.TotalProductInOrderTo = totalProductInOrderTo ? Number(totalProductInOrderTo.replace(/,/g, "")) : null;
    filterData.TotalOrderFrom = totalOrderFrom ? Number(totalOrderFrom.replace(/,/g, "")) : null;
    filterData.TotalOrderTo = totalOrderTo ? Number(totalOrderTo.replace(/,/g, "")) : null;
    //advanced filter
    filterData.ListCustomerType = customerType.map(e => e.value);
    filterData.ListCustomer = customer.map(e => e.customerId);
    filterData.ListCustomerGroup = customerGroup.map(e => e.categoryId);
    filterData.ListProductGroup = productGroup.map(e => e.productCategoryId);
    filterData.ProductCode = productCode ? productCode.trim() : '';

    return filterData;
  }

  handleReportAfterSearch() {
    this.listTopEmployeeRevenue.forEach((e, index) => e.index = index + 1);

    this.summaryAmount = 0;
    this.summaryTotalProductInOrder = 0;
    this.summaryTotalOrder = 0;

    this.listTopEmployeeRevenue.forEach(e => {
      this.summaryAmount += e.amount ? Number(e.amount) : 0;
      this.summaryTotalProductInOrder += e.totalProductInOrder ? Number(e.totalProductInOrder) : 0;
      this.summaryTotalOrder += e.totalOrder ? Number(e.totalOrder) : 0;
    });

  }

  openOrgPopup() {
    let ref = this.dialogService.open(DialogOrganizationPermissionsionComponent, {
      data: {
        chooseFinancialIndependence: false
      },
      header: 'Chọn phòng ban',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        'overflow-x': 'hidden'
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          let selectedOrganization: organization = result.selectedOrganization;
          this.selectedOrgId = selectedOrganization.organizationId;
          this.selectedOrgName = selectedOrganization.organizationName;
        }
      }
    });
  }

  exportExcel() {
    //BÁO CÁO TỔNG HỢP DOANH SỐ BÁN HÀNG THEO NHÂN VIÊN
    let title = 'BÁO CÁO TỔNG HỢP DOANH SỐ BÁN HÀNG THEO NHÂN VIÊN';
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
    worksheet.mergeCells(`A${titleRow.number}:F${titleRow.number}`);
    /* subtitle */
    let filterData = this.getDataFilterForm();
    let orderFromDate = filterData.OrderFromDate ? this.datePipe.transform(filterData.OrderFromDate, 'dd/MM/yyyy') : '';
    let orderToDate = filterData.OrderToDate ? this.datePipe.transform(filterData.OrderToDate, 'dd/MM/yyyy') : '';
    let subTitle = worksheet.addRow([`Từ ngày ${orderFromDate} đến ngày ${orderToDate}`]);
    subTitle.font = { family: 4, italic: true };
    subTitle.alignment = { vertical: 'middle', horizontal: 'center' };
    worksheet.mergeCells(`A${subTitle.number}:F${subTitle.number}`);
    /* header */
    const header = this.colsList.map(e => e.header);
    let headerRow = worksheet.addRow(header);
    headerRow.font = { bold: true };
    header.forEach((item, index) => {
      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
    });

    /* data table */
    let data = this.listTopEmployeeRevenue.map((item, index) => {
      let row = [];

      row.push(index + 1);
      row.push(item.employeeCode);
      row.push(item.employeeName);
      row.push(item.amount ? this.decimalPipe.transform(item.amount) : '0');
      row.push(item.totalProductInOrder ? this.decimalPipe.transform(item.totalProductInOrder) : '0');
      row.push(item.totalOrder ? this.decimalPipe.transform(item.totalOrder) : 0);

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
    summaryRow.push('');
    summaryRow.push('');
    summaryRow.push('Tổng cộng');
    summaryRow.push(this.summaryAmount ? this.decimalPipe.transform(this.summaryAmount) : '0');
    summaryRow.push(this.summaryTotalProductInOrder ? this.decimalPipe.transform(this.summaryTotalProductInOrder) : '0');
    summaryRow.push(this.summaryTotalOrder ? this.decimalPipe.transform(this.summaryTotalOrder) : '0');

    let summaryData = [summaryRow];
    summaryData.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      this.colsList.forEach((item, index) => {
        row.getCell(index + 1).alignment = { vertical: 'bottom', horizontal: 'right', wrapText: true };
        row.font = { bold: true };
        //    row.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      });
    });

    /* set width */
    this.colsList.forEach((item, index) => {
      worksheet.getColumn(index + 1).width = item.excelWidth;
    });

    /* ký tên */
    let footer1 = [];
    footer1[4] = 'Ngày.....tháng.....năm..........';
    let rowFooter1 = worksheet.addRow(footer1);
    worksheet.mergeCells(`D${rowFooter1.number}:F${rowFooter1.number}`);
    rowFooter1.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    let footer2 = [];
    footer2[4] = 'NGƯỜI LẬP';
    let rowFooter2 = worksheet.addRow(footer2);
    worksheet.mergeCells(`D${rowFooter2.number}:F${rowFooter2.number}`);
    rowFooter2.font = { bold: true };
    rowFooter2.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

    let footer3 = [];
    footer3[4] = '(Ký, họ tên)';
    let rowFooter3 = worksheet.addRow(footer3);
    worksheet.mergeCells(`D${rowFooter3.number}:F${rowFooter3.number}`);
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
            widths: [25, 70, 120, 100, 60, 60],
            headerRows: 1,
            dontBreakRows: true,
            body: [
              [
                { text: 'STT', style: 'tableHeader', alignment: 'center' },
                { text: 'Mã nhân viên', style: 'tableHeader', alignment: 'center' },
                { text: 'Tên nhân viên', style: 'tableHeader', alignment: 'center' },
                { text: 'Doanh thu', style: 'tableHeader', alignment: 'center' },
                { text: 'SL bán', style: 'tableHeader', alignment: 'center' },
                { text: 'SL đơn hàng', style: 'tableHeader', alignment: 'center' },
              ],
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
          fontSize: 18.5,
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
          fontSize: 9,
        },
        tableLiness: {
          fontSize: 7,
        },
        StyleItalics: {
          italics: true
        }
      }
    };

    this.listTopEmployeeRevenue.forEach((item, index) => {
      let option = [
        { text: index + 1, style: 'tableLines', alignment: 'center' },
        {
          text: item.employeeCode,
          style: 'tableLines',
          alignment: 'center'
        },
        {
          text: item.employeeName,
          style: 'tableLines',
          alignment: 'left'
        },
        {
          text: item.amount ? this.decimalPipe.transform(item.amount) : '0',
          style: 'tableLines',
          alignment: 'right'
        },
        {
          text: item.totalProductInOrder ? this.decimalPipe.transform(item.totalProductInOrder) : '0',
          style: 'tableLines',
          alignment: 'right'
        },

        {
          text: item.totalOrder ? this.decimalPipe.transform(item.totalOrder) : '0',
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
        // style: 'tableLines',
        style: {
          fontSize: 10,
          bold: true,
        },
        alignment: 'right'
      },
      {
        text: this.summaryAmount ? this.decimalPipe.transform(this.summaryAmount) : '0',
        // style: 'tableLines',
        style: {
          fontSize: 10,
          bold: true,
        },
        alignment: 'right'
      },
      {
        text: this.summaryTotalProductInOrder ? this.decimalPipe.transform(this.summaryTotalProductInOrder) : '0',
        //style: 'tableLines',
        style: {
          fontSize: 10,
          bold: true,
        },
        alignment: 'right'
      },

      {
        text: this.summaryTotalOrder ? this.decimalPipe.transform(this.summaryTotalOrder) : '0',
        //style: 'tableLines',
        style: {
          fontSize: 10,
          bold: true,
        },
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
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};


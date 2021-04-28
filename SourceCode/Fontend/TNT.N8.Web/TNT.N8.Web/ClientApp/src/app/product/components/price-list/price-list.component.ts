import * as $ from 'jquery';
import { Component, OnInit, ViewChild, ElementRef, HostListener, Renderer2 } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { PriceProductModel } from '../../models/product.model';

import * as XLSX from 'xlsx';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { type } from 'os';


interface PriceProduct {
  priceProductId: string,
  productId: string,
  productCode: string,
  productName: string,
  effectiveDate: Date,
  priceVnd: string,
  minQuantity: string,
  priceForeignMoney: string,
  customerGroupCategory: string,
  customerGroupCategoryName: string,
  createdById: string,
  createdDate: Date,
}

interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string,
}

@Component({
  selector: 'app-price-list',
  templateUrl: './price-list.component.html',
  styleUrls: ['./price-list.component.css'],
  providers: [
    DatePipe,
  ]
})
export class PriceListComponent implements OnInit {
  fixed: boolean = false;
  withFiexd: string = "";

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;

  auth: any = JSON.parse(localStorage.getItem('auth'));
  //get system parameter
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  loading: boolean = false;
  awaitResult: boolean = false;// khóa nút lưu
  cols: any[];
  selectedColumns: any[];
  listPrice: Array<PriceProduct> = [];
  listPriceForUpdate: Array<PriceProduct> = [];
  listStatus: Array<Category> = [];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  priveListId: string;
  isUpdate: boolean = false;
  isDisplayName: boolean = false;
  priceProductId: string;

  listProduct: Array<any> = [];
  listGroupCustomer: Array<Category> = [];

  /*START : FORM PriceList*/
  priceProductForm: FormGroup;
  productControl: FormControl;
  effectiveDateControl: FormControl;
  priceVNDControl: FormControl;
  MinimumQuantityControl: FormControl;
  priceForeignMoneyControl: FormControl;
  customerGroupCategoryControl: FormControl;
  /*END : FORM PriceList*/

  isUpdateAI: boolean = false;

  productName: string;
  productId: string;
  effectiveDate: string;
  priceVND: string;
  priceForeignMoney: string;
  customerGroupCategory: string;

  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();

  /*Import*/
  displayDialog: boolean = false;
  importFileExcel: any = null;
  messErrFile: any = [];
  cellErrFile: any = [];
  fileName: string = '';
  listPriceProductImport: Array<PriceProductModel> = [];

  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('save') save: ElementRef;

  constructor(
    private route: ActivatedRoute,
    private translate: TranslateService,
    private getPermission: GetPermission,
    private productService: ProductService,
    private el: ElementRef,
    private renderer: Renderer2,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    public dialogService: DialogService,
    private router: Router,
  ) {

    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (!this.toggleButton.nativeElement.contains(e.target) &&
          !this.notifi.nativeElement.contains(e.target) &&
          !this.save.nativeElement.contains(e.target)) {
          this.isOpenNotifiError = false;
        }
      }
    });
  }

  ngOnInit() {
    let resource = "sal/product/price-list/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    } else {
      this.setForm();
      this.setTable();
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      this.getMasterData();
    }
  }
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  setTable() {
    this.cols = [
      { field: 'productCode', header: 'Mã hàng hóa', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'productName', header: 'Tên hàng hóa', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'priceVnd', header: 'Giá bán', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'minQuantity', header: 'Số lượng tối thiểu', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'priceForeignMoney', header: 'Giá bán ngoại tệ', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'effectiveDate', header: 'Ngày hiệu lực', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'customerGroupCategoryName', header: 'Nhóm khách hàng', width: '50px', textAlign: 'left', color: '#f44336' },
    ];

    this.selectedColumns = this.cols.filter(e => e.field == "productCode" || e.field == "productName" || e.field == "priceVnd"
      || e.field == "minQuantity" || e.field == "effectiveDate");
  }

  createPrice() {
    if (!this.priceProductForm.valid) {
      Object.keys(this.priceProductForm.controls).forEach(key => {
        if (this.priceProductForm.controls[key].valid == false) {
          this.priceProductForm.controls[key].markAsTouched();
        }
      });
      this.isInvalidForm = true;
      this.isOpenNotifiError = true;
      this.emitStatusChangeForm = this.priceProductForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } else {
      let newPriceProduct: PriceProductModel = this.mappingPiceProductFormCreate();
      this.productId = this.priceProductForm.controls['productControl'].value.productId;
      // Lấy giá trị để truyền xuống db
      let check = this.listPrice.find(x => x.productId == this.productId &&
        this.calcDaysDiff(new Date(x.effectiveDate), newPriceProduct.EffectiveDate) === 0 &&
        x.customerGroupCategory == newPriceProduct.CustomerGroupCategory);
      if (check) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Đã tồn tại bản ghi trong hệ thống!' };
        this.showMessage(msg);
      } else {
        this.savePrice(newPriceProduct);
      }
    }
  }

  mappingPiceProductFormCreate(): PriceProductModel {
    let newPriceProduct = new PriceProductModel();

    newPriceProduct.ProductId = this.priceProductForm.controls['productControl'].value.productId;
    let efectiveDate = this.priceProductForm.controls['effectiveDateControl'].value;
    newPriceProduct.EffectiveDate = convertToUTCTime(efectiveDate);
    newPriceProduct.PriceVnd = this.priceProductForm.controls['priceVNDControl'].value;
    newPriceProduct.MinQuantity = this.MinimumQuantityControl.value;
    newPriceProduct.PriceForeignMoney = this.priceProductForm.controls['priceForeignMoneyControl'].value;
    let customerGroup = this.priceProductForm.controls['customerGroupCategoryControl'].value;
    newPriceProduct.CustomerGroupCategory = customerGroup === null ? null : customerGroup.categoryId;

    return newPriceProduct;
  }

  mappingPiceProductFormUpdate(): PriceProductModel {
    let newPriceProduct = new PriceProductModel();
    newPriceProduct.PriceProductId = this.priceProductId;
    newPriceProduct.ProductId = this.priceProductForm.controls['productControl'].value.productId;
    let efectiveDate = this.priceProductForm.controls['effectiveDateControl'].value;
    newPriceProduct.EffectiveDate = convertToUTCTime(efectiveDate);
    newPriceProduct.PriceVnd = this.priceProductForm.controls['priceVNDControl'].value;
    newPriceProduct.MinQuantity = this.MinimumQuantityControl.value;
    newPriceProduct.PriceForeignMoney = this.priceProductForm.controls['priceForeignMoneyControl'].value;
    let customerGroup = this.priceProductForm.controls['customerGroupCategoryControl'].value;
    newPriceProduct.CustomerGroupCategory = customerGroup === null ? null : customerGroup.categoryId;

    return newPriceProduct;
  }

  savePrice(priceProduct: PriceProductModel) {
    this.awaitResult = true;
    this.loading = true;
    this.productService.createOrUpdatePriceProduct(priceProduct).subscribe(response => {
      var result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listPrice = [];
        // this.listPrice = result.listPrice;
        result.listPrice.forEach(item => {
          let priceProduct: PriceProduct = {
            priceProductId: item.priceProductId,
            productCode: item.productCode,
            productName: item.productName,
            productId: item.productId,
            effectiveDate: item.effectiveDate,
            priceVnd: item.priceVnd,
            minQuantity: item.minQuantity,
            priceForeignMoney: item.priceForeignMoney,
            customerGroupCategory: item.customerGroupCategory,
            customerGroupCategoryName: item.customerGroupCategoryName,
            createdById: item.createdById,
            createdDate: item.createdDate
          }
          this.listPrice.push(priceProduct);
        });
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: 'Thêm chi phí thành công' };
        this.showMessage(mgs);
        this.awaitResult = false;
        this.resetForm();
        if (this.emitStatusChangeForm) {
          this.emitStatusChangeForm.unsubscribe();
          this.isInvalidForm = false; //Ẩn icon-warning-active
        }
      } else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
        this.awaitResult = false;
      }

    }, error => { });
  }

  updatePrice() {
    if (!this.priceProductForm.valid) {
      Object.keys(this.priceProductForm.controls).forEach(key => {
        if (this.priceProductForm.controls[key].valid == false) {
          this.priceProductForm.controls[key].markAsTouched();
        }
      });
      this.isInvalidForm = true;
      this.isOpenNotifiError = true;
      this.emitStatusChangeForm = this.priceProductForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } else {
      // Lấy giá trị để truyền xuống db
      let newPriceProduct: PriceProductModel = this.mappingPiceProductFormUpdate();
      let newProductId = this.priceProductForm.controls['productControl'].value.productId;
      // Lấy giá trị để truyền xuống db
      let check: PriceProduct;

      check = this.listPriceForUpdate.find(x => x.productId == newProductId && this.calcDaysDiff(new Date(x.effectiveDate), newPriceProduct.EffectiveDate) === 0 && x.customerGroupCategory == newPriceProduct.CustomerGroupCategory);
      if (check) {
        //Nếu tồn tại rồi thì không cho thêm và hiển thị cảnh báo
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Đã tồn tại bản ghi trong hệ thống!' };
        this.showMessage(msg);
      } else {
        this.awaitResult = true;
        this.loading = true;
        this.productService.createOrUpdatePriceProduct(newPriceProduct).subscribe(response => {
          var result = <any>response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.listPrice = [];
            // this.listPrice = result.listPrice;
            result.listPrice.forEach(item => {
              let priceProduct: PriceProduct = {
                priceProductId: item.priceProductId,
                productCode: item.productCode,
                productName: item.productName,
                productId: item.productId,
                effectiveDate: item.effectiveDate,
                priceVnd: item.priceVnd,
                minQuantity: item.minQuantity,
                priceForeignMoney: item.priceForeignMoney,
                customerGroupCategory: item.customerGroupCategory,
                customerGroupCategoryName: item.customerGroupCategoryName,
                createdById: item.createdById,
                createdDate: item.createdDate
              }
              this.listPrice.push(priceProduct);
            });
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật chi phí thành công' };
            this.showMessage(mgs);
            this.awaitResult = false;
            if (this.emitStatusChangeForm) {
              this.emitStatusChangeForm.unsubscribe();
              this.isInvalidForm = false; //Ẩn icon-warning-active
            }
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
            this.awaitResult = false;
          }
        }, error => { });
      }
    }
  }

  delPriceProduct(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.productService.deletePriceProduct(rowData.priceProductId).subscribe(response => {
          var result = <any>response;
          if (result.statusCode == 200) {
            this.listPrice = this.listPrice.filter(c => c != rowData);
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        });
      }
    });
  }

  cancel() {
    this.isUpdate = false;
    this.resetForm();
  }

  /*Hiển thị lại thông tin bổ sung*/
  reShowNote(event: any) {
    this.listPriceForUpdate = [];
    let rowData: PriceProduct = event.data;
    this.isUpdate = true;
    this.priceProductId = rowData.priceProductId;
    this.productId = rowData.productId;
    let product = this.listProduct.find(c => c.productId == rowData.productId);
    let customerGroup = this.listGroupCustomer.find(c => c.categoryId == rowData.customerGroupCategory);
    customerGroup = customerGroup === undefined ? null : customerGroup;
    this.productName = product.productName;
    this.isDisplayName = true;
    this.priceProductForm.controls['productControl'].setValue(product);
    this.priceProductForm.controls['effectiveDateControl'].setValue(new Date(rowData.effectiveDate));
    this.priceProductForm.controls['priceVNDControl'].setValue(rowData.priceVnd);
    this.priceProductForm.controls['MinimumQuantityControl'].setValue(rowData.minQuantity);
    this.priceProductForm.controls['priceForeignMoneyControl'].setValue(rowData.priceForeignMoney);
    this.priceProductForm.controls['customerGroupCategoryControl'].setValue(customerGroup);

    this.priceProductForm.controls['productControl'].updateValueAndValidity();
    this.priceProductForm.controls['effectiveDateControl'].updateValueAndValidity();
    this.priceProductForm.controls['priceVNDControl'].updateValueAndValidity();
    this.priceProductForm.controls['priceForeignMoneyControl'].updateValueAndValidity();
    this.priceProductForm.controls['customerGroupCategoryControl'].updateValueAndValidity();

    this.listPriceForUpdate = this.listPrice.filter(c => c != rowData);
  }

  getMasterData() {
    this.loading = true;
    this.productService.getMasterDataPriceList().subscribe(response => {
      var result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listPrice = result.listPrice;
        this.listProduct = result.listProduct;
        this.listProduct.forEach(item => {
          item.productCodeName = item.productCode + ' - ' + item.productName;
        });

        this.listGroupCustomer = result.listCategory;
      }
    });
  }

  resetForm() {
    this.priceProductForm.controls['productControl'].reset();
    this.priceProductForm.controls['effectiveDateControl'].reset();
    this.priceProductForm.controls['priceVNDControl'].setValue('0');
    this.priceProductForm.controls['MinimumQuantityControl'].setValue('1');
    this.priceProductForm.controls['priceForeignMoneyControl'].reset();
    this.priceProductForm.controls['customerGroupCategoryControl'].setValue(null);
    this.isDisplayName = false;
    this.productId = '';
    this.productName = '';
    this.isInvalidForm = false;
    this.isOpenNotifiError = false;
    this.listPriceForUpdate = [];
  }

  changeProduct(event: any) {
    if (event.value === null) {
      this.productId = '';
      return;
    }
    this.productName = event.value.productName;
    this.isDisplayName = true;
  }

  setForm() {
    this.productControl = new FormControl(null, [Validators.required]);
    this.effectiveDateControl = new FormControl(new Date(), [Validators.required]);
    this.priceVNDControl = new FormControl('0', [Validators.required]);
    this.MinimumQuantityControl = new FormControl('1', [Validators.required]);
    this.priceForeignMoneyControl = new FormControl('');
    this.customerGroupCategoryControl = new FormControl(null);

    this.priceProductForm = new FormGroup({
      productControl: this.productControl,
      effectiveDateControl: this.effectiveDateControl,
      priceVNDControl: this.priceVNDControl,
      MinimumQuantityControl: this.MinimumQuantityControl,
      priceForeignMoneyControl: this.priceForeignMoneyControl,
      customerGroupCategoryControl: this.customerGroupCategoryControl
    });
  }

  calcDaysDiff(dateFrom, dateTo): number {
    let currentDate = new Date(dateTo);
    let dateSent = new Date(dateFrom);

    return Math.floor((Date.UTC(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate()) - Date.UTC(dateSent.getFullYear(), dateSent.getMonth(), dateSent.getDate())) / (1000 * 60 * 60 * 24));
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  showDialogImport() {
    this.displayDialog = true;
  }

  downloadTemplateExcel() {
    let dateUTC = new Date();
    let title = "Template_Import_Bng_Gia";
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Sheet1');
    worksheet.pageSetup.margins = {
      left: 0.25, right: 0.25,
      top: 0.75, bottom: 0.75,
      header: 0.3, footer: 0.3
    };
    worksheet.pageSetup.paperSize = 9;  //A4 : 9


    let dataRow1 = [];
    dataRow1[1] = `Template import Bảng giá bán`
    let row1 = worksheet.addRow(dataRow1);
    row1.font = { name: 'Arial', size: 15, bold: true };
    worksheet.mergeCells(`A${row1.number}:F${row1.number}`);
    row1.alignment = { vertical: 'bottom', horizontal: 'center', wrapText: true };
    worksheet.addRow([]);

    // let dataRow2 = [];
    // dataRow2[1] = `    `
    // let row2 = worksheet.addRow(dataRow2);
    // row2.font = { name: 'Arial', size: 18, bold: true };
    // row2.alignment = { vertical: 'bottom', horizontal: 'center', wrapText: true };

    let dataRow3 = [];
    dataRow3[2] = `- Các cột màu đỏ là các cột bắt buộc nhập.
    - Các cột có ký hiệu (*) là các cột bắt buộc nhập theo điều kiện.
    - Mã sản phẩm, ngày hiệu lực và Mã nhóm khách hàng không được trùng nhau hoàn toàn giữa 2 bản ghi.`
    let row3 = worksheet.addRow(dataRow3);
    row3.font = { name: 'Arial', size: 11, color: { argb: 'ff0000' } };
    worksheet.mergeCells(`B${row3.number}:F${row3.number}`);
    row3.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow4 = [];
    dataRow4[2] = `- Tất cả dữ liệu nhập cần trùng với dữ liệu trong hệ thống.
    - Các trường dữ liệu là kiểu số không nhập dữ liệu cần để mặc định là 0.`
    let row4 = worksheet.addRow(dataRow4);
    row4.font = { name: 'Arial', size: 11, color: { argb: 'ff0000' } };
    worksheet.mergeCells(`B${row4.number}:F${row4.number}`);
    row4.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataHeaderRow = ['Mã sản phẩm*', 'Ngày hiệu lực*', 'Giá bán VND*', 'Số lượng tối thiểu*', 'Giá bán ngoại tệ', 'Nhóm khách hàng'];
    let headerRow = worksheet.addRow(dataHeaderRow);
    headerRow.font = { name: 'Arial', size: 10, bold: true, color: { argb: '0D0904' } };
    dataHeaderRow.forEach((item, index) => {
      if (index !== 4 && index !== 5) {
        headerRow.getCell(index + 1).font = { name: 'Arial', size: 10, bold: true, color: { argb: 'ff0000' } };
      }
      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'C6E2FF' }
      };
    });
    headerRow.height = 25;

    let dataHeaderRow2 = ['BKAV-ANTIVIRUS', '11-04-2021', '300000', '1', '0', 'UTA'];
    let headerRow2 = worksheet.addRow(dataHeaderRow2);
    headerRow2.font = { name: 'Arial', size: 10, bold: false };
    dataHeaderRow2.forEach((item, index) => {
      headerRow2.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow2.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'FFFFFF' }
      };
      if (index == 0 || index == 5) {
        headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      } else {
        headerRow2.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      }

    });
    headerRow2.height = 18;

    worksheet.addRow([]);
    worksheet.getRow(3).height = 50;
    worksheet.getRow(4).height = 50;
    worksheet.getColumn(1).width = 30;
    worksheet.getColumn(2).width = 30;
    worksheet.getColumn(3).width = 30;
    worksheet.getColumn(4).width = 30;
    worksheet.getColumn(5).width = 30;
    worksheet.getColumn(6).width = 30;

    this.exportToExel(workbook, title);
    // this.productService.downloadPriceProductTemplate().subscribe(response => {
    //   this.loading = false;
    //   const result = <any>response;
    //   if (result.templateExcel != null && result.statusCode === 200) {
    //     const binaryString = window.atob(result.templateExcel);
    //     const binaryLen = binaryString.length;
    //     const bytes = new Uint8Array(binaryLen);
    //     for (let idx = 0; idx < binaryLen; idx++) {
    //       const ascii = binaryString.charCodeAt(idx);
    //       bytes[idx] = ascii;
    //     }
    //     const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    //     const link = document.createElement('a');
    //     link.href = window.URL.createObjectURL(blob);
    //     const fileName = result.fileName + ".xls";
    //     //const fileName = result.nameFile  + ".xlsx";
    //     link.download = fileName;
    //     link.click();
    //   }
    // }, error => { this.loading = false; });
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  chooseFile(event: any) {
    this.fileName = event.target.files[0].name;
    this.importFileExcel = event.target;
  }

  cancelFile() {
    $("#importFilePriceProduct").val("")
    this.fileName = "";
  }

  validateFile(data) {
    this.messErrFile = [];
    this.cellErrFile = [];

    data.forEach((row, i) => {
      if (i > 3) {
        if ((row[0] === null || row[0] === undefined || row[0].toString().trim() == "")) {
          this.messErrFile.push('Dòng { ' + (i + 2) + ' } chưa nhập Mã sp hoặc Tên sản phẩm!');
        }
        if (row[1] === null || row[1] === undefined || row[1] == "") {
          this.messErrFile.push('Ngày hiệu lực tại dòng ' + (i + 2) + ' không được để trống');
        }
        // if (typeof (row[1]) !== 'number') {
        //   this.messErrFile.push('Ngày hiệu lực tại dòng ' + (i + 2) + ' sai định dạng');
        // }
        if (new Date(row[1].replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")).toString() == "Invalid Date") {
          this.messErrFile.push('Ngày hiệu lực từ tại dòng { ' + (i + 2) + ' } không đúng format');
        }
        if (row[2] === null || row[2] === undefined || row[2] == "") {
          this.messErrFile.push('Giá bán tại dòng ' + (i + 2) + ' không được để trống');
        }
        if (parseFloat(row[2]) == undefined || parseFloat(row[2]).toString() == "NaN" || parseFloat(row[2]) == null) {
          this.messErrFile.push('Gái bán tại dòng ' + (i + 2) + ' sai định dạng');
        }
        if (row[3] === null || row[3] === undefined || row[3] == "") {
          this.messErrFile.push('Số lượng tối thiểu tại dòng ' + (i + 2) + ' không được để trống');
        }
        if (parseFloat(row[3]) == undefined || parseFloat(row[3]).toString() == "NaN" || parseFloat(row[3]) == null) {
          this.messErrFile.push('Số lượng tối thiểu tại dòng ' + (i + 2) + ' sai định dạng');
        }
      }
    });
    if (this.messErrFile.length != 0) return true;
    else return false;
  }

  importExcel() {
    if (this.fileName == "") {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Chưa chọn file cần nhập" };
      this.showMessage(mgs);
    } else {
      const targetFiles: DataTransfer = <DataTransfer>(this.importFileExcel);
      const reader: FileReader = new FileReader();
      reader.readAsBinaryString(targetFiles.files[0]);

      reader.onload = (e: any) => {

        /* read workbook */
        const bstr: string = e.target.result;

        const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });

        // kiểm tra form value và file excel có khớp mã với nhau hay không
        let code = 'Sheet1';
        if (workbook.Sheets[code] === undefined) {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
          this.showMessage(mgs);
          return;
        }
        //lấy data từ file excel của product
        const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[code];
        /* save data */
        let dataPriceProduct: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
        dataPriceProduct.shift();
        let productCodeList: string[] = [];
        let cusGroupCodeList: string[] = [];
        let isValidation = this.validateFile(dataPriceProduct);
        if (isValidation) {
          this.isInvalidForm = true;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = true;  //Hiển thị message lỗi
        } else {
          var messProCodeErr = [];
          var messCusGroupErr = [];
          var messErrExist = [];
          var messErrExistExcel = [];

          this.isInvalidForm = false;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = false;  //Hiển thị message lỗi
          dataPriceProduct.forEach((row, i) => {
            if (i > 3 && row.length != 0) {
              if (row[0] !== null || row[0] !== undefined && row[0].trim() != "") {
                let isProducCode = productCodeList.find(c => c.trim() == row[0].trim());
                if (isProducCode == null || isProducCode == undefined) {
                  productCodeList.push(row[0]);
                  let check = this.listProduct.find(c => c.productCode.toLowerCase().trim() == row[0].trim().toLowerCase());
                  if (check === undefined || check === null) {
                    messProCodeErr.push(i + 2);
                  }
                }
              }
              if (row[5]) {
                if (row[5] !== null || row[5] !== undefined && row[5].trim() != "") {
                  let isCusGroup = cusGroupCodeList.find(c => c.trim() == row[5].trim());
                  if (isCusGroup == null || isCusGroup == undefined) {
                    cusGroupCodeList.push(row[5]);
                    let check = this.listGroupCustomer.find(c => c.categoryCode.toLowerCase().trim() == row[5].trim().toLowerCase());
                    if (check === undefined || check === null) {
                      messCusGroupErr.push(i + 2);
                    }

                  }
                }
              }

            }
          });

          let countProCode = this.listProduct.filter(c => productCodeList.includes(c.productCode));
          let countCusGroup = this.listGroupCustomer.filter(c => cusGroupCodeList.includes(c.categoryCode));
          if (countProCode.length == productCodeList.length && countCusGroup.length == cusGroupCodeList.length) {
            this.listPriceProductImport = [];
            dataPriceProduct.forEach((row, i) => {
              if (i > 3 && row.length != 0) {
                let priceProduct = new PriceProductModel();
                let _product = this.listProduct.find(c => c.productCode == row[0].trim());
                if (_product) {
                  priceProduct.ProductId = _product.productId;
                }
                // let _time = ExcelDateToJSDate(row[1]);
                let date = new Date(row[1].replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
                priceProduct.EffectiveDate = convertToUTCTime(date);
                priceProduct.PriceVnd = row[2];
                priceProduct.MinQuantity = row[3];
                priceProduct.PriceForeignMoney = row[4];
                priceProduct.CustomerGroupCategory = this.listGroupCustomer.find(c => c.categoryCode == row[5])?.categoryId;

                // Kiểm tra đã tồn tại trong hệ thống
                let check = this.listPrice.find(x => x.productId == priceProduct.ProductId &&
                  this.calcDaysDiff(new Date(x.effectiveDate), priceProduct.EffectiveDate) === 0 &&
                  x.customerGroupCategory == priceProduct.CustomerGroupCategory);
                if (check) {
                  messErrExist.push(i + 2);
                } else {
                  this.listPriceProductImport.push(priceProduct);

                  // Kiểm tra trùng lặp trong file excel
                  let checkExcel = this.listPriceProductImport.filter(x => x.ProductId == priceProduct.ProductId &&
                    this.calcDaysDiff(new Date(x.EffectiveDate), priceProduct.EffectiveDate) === 0 &&
                    x.CustomerGroupCategory == priceProduct.CustomerGroupCategory);
                  if (checkExcel.length > 1) {
                    messErrExistExcel.push(i + 2);
                  }
                }
              }
            });
            if (messErrExist.length > 0) {
              this.isInvalidForm = true;  //Hiển thị icon-warning-active
              this.isOpenNotifiError = true;  //Hiển thị message lỗi
              messErrExist.forEach(item => {
                this.messErrFile.push('Giá bán tại dòng ' + item + ' đã tồn tại trong hệ thống')
              });
            } else if (messErrExistExcel.length > 0) {
              this.isInvalidForm = true;  //Hiển thị icon-warning-active
              this.isOpenNotifiError = true;  //Hiển thị message lỗi
              messErrExistExcel.forEach(item => {
                this.messErrFile.push('Giá bán tại dòng ' + item + ' bị trùng lặp trong file excel');
              });
            } else {
              if (this.listPriceProductImport.length > 0) {
                this.productService.importPriceProduct(this.listPriceProductImport).subscribe(response => {
                  var result = <any>response;
                  if (result.statusCode == 200) {
                    let mgs = { severity: 'success', summary: 'Thông báo', detail: "Nhập excel giá sản phẩm thành công!" };
                    this.showMessage(mgs);
                    this.getMasterData();
                  } else {
                    let mgs = { severity: 'warn', summary: 'Thông báo', detail: result.messageCode };
                    this.showMessage(mgs);
                  }
                });
              }
            }
          }

          if (countProCode.length != productCodeList.length) {
            this.isInvalidForm = true;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = true;  //Hiển thị message lỗi
            messProCodeErr.forEach(item => {
              this.messErrFile.push('Sản phẩm tại dòng ' + item + ' không tồn tại trong hệ thống')
            });
          }
          if (countCusGroup.length != cusGroupCodeList.length) {
            this.isInvalidForm = true;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = true;  //Hiển thị message lỗi
            messCusGroupErr.forEach(item => {
              this.messErrFile.push('Nhóm KH tại dòng ' + item + ' không tồn tại trong hệ thống')
            });
          }
        }
        this.displayDialog = false;
      }
    }
  }

}
function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};


function ExcelDateToJSDate(serial) {
  var utc_days = Math.floor(serial - 25569);
  var utc_value = utc_days * 86400;
  var date_info = new Date(utc_value * 1000);

  var fractional_day = serial - Math.floor(serial) + 0.0000001;

  var total_seconds = Math.floor(86400 * fractional_day);

  var seconds = total_seconds % 60;

  total_seconds -= seconds;

  var hours = Math.floor(total_seconds / (60 * 60));
  var minutes = Math.floor(total_seconds / 60) % 60;

  return new Date(date_info.getFullYear(), date_info.getMonth(), date_info.getDate(), hours, minutes, seconds);
}

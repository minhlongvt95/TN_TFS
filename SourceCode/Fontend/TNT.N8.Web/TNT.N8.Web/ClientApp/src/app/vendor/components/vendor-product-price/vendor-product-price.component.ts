import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MenuItem, MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { SortEvent } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import * as moment from 'moment';
import 'moment/locale/pt-br';

import { VendorService } from "../../services/vendor.service";
import { ProductVendorMappingModel } from '../../models/vendor.model';
import { AddVendorPriceDialogComponent } from '../add-vendor-price-dialog/add-vendor-price-dialog.component';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import * as $ from 'jquery';
import * as XLSX from 'xlsx';

import { DatePipe, DecimalPipe, formatDate } from '@angular/common';
import { ProductService } from '../../../product/services/product.service';



interface ProductVendorMapping {
  productVendorMappingId: string;
  vendorId: string;
  vendorName: string;
  vendorCode: string;
  productCode: string;
  productId: string;
  productName: string;
  productUnitName: string;
  vendorProductName: string;
  miniumQuantity: number;
  price: number;
  moneyUnitId: string;
  moneyUnitName: string;
  fromDate: Date;
  toDate: Date;
  createdById: string;
  createdDate: Date;
  listSuggestedSupplierQuoteId: Array<string>
}

class category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefauld: boolean;
}

class vendor {
  vendorId: string;
  vendorCode: string;
  vendorName: string;
}

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  // productVendorMappingModel: ProductVendorMappingModel,
  message: string;
}

@Component({
  selector: 'app-vendor-product-price',
  templateUrl: './vendor-product-price.component.html',
  styleUrls: ['./vendor-product-price.component.css'],
  providers: [
    DecimalPipe,
    DatePipe
  ]
})
export class VendorProductPriceComponent implements OnInit {
  innerWidth: number = 0; //number window size first
  emitParamUrl: any;
  @ViewChild('myTable') myTable: Table;

  /*START: Biến điều kiện*/
  loading: boolean = false;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  today = new Date();
  minYear: number = 2015;
  currentYear: number = (new Date()).getFullYear();
  isGlobalFilter: string = '';
  startDate: Date = null;
  maxEndDate: Date = new Date();
  endDate: Date = null;
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  auth = JSON.parse(localStorage.getItem('auth'));
  /*END: Biến điều kiện*/
  actionEdit: boolean = true;
  /*Table*/
  cols: any;
  selectedColumns: any[];
  actions: MenuItem[] = [];
  /*Biến lưu giá trị trả về*/
  listVendorProductPrice: Array<ProductVendorMapping> = [];
  productVendorMapping = new ProductVendorMappingModel();
  /*Action*/
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  actionDelete: boolean = true;
  displayDialog: boolean = false;
  importFileExcel: any = null;
  messErrFile: any = [];
  cellErrFile: any = [];
  fileName: string = '';
  listUnitMoney: Array<category> = [];
  listVendor: Array<vendor> = [];
  listProduct: Array<any> = [];
  listVendorProdcutPriceImport: Array<ProductVendorMappingModel> = [];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  regexDate: string = '/^([0-9]{1,2})\/([0-9]{1,2})\/([0-9]{4})$/';
  // Thông báo lỗi
  isOpenNotifiError: boolean = false;
  isInvalidForm: boolean = false;
  /* Value Search */
  productName: string = '';
  vendorName: string = '';
  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private vendorService: VendorService,
    private dialogService: DialogService,
    private decimalPipe: DecimalPipe,
    private datePipe: DatePipe,
    private productService: ProductService
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    let resource = "buy/vendor/list-vendor-price/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      await this.getMasterdata();
      this.setTable();
      this.searchVendorProductPrice();
    }
  }

  setTable() {
    this.cols = [
      { field: 'vendorName', header: 'Nhà cung cấp', textAlign: 'left', display: 'table-cell', width: '220px' },
      { field: 'productCode', header: 'Mã sản phẩm', textAlign: 'left', display: 'table-cell', width: '140px' },
      { field: 'productName', header: 'Tên sản phẩm', textAlign: 'left', display: 'table-cell' },
      { field: 'miniumQuantity', header: 'Số lượng tối thiểu', textAlign: 'right', display: 'table-cell', width: '160px' },
      { field: 'productUnitName', header: 'Đơn vị tính', textAlign: 'left', display: 'table-cell', width: '120px' },
      { field: 'price', header: 'Giá  ', textAlign: 'right', display: 'table-cell', width: '120px' },
      { field: 'moneyUnitName', header: 'Tiền tệ', textAlign: 'left', display: 'table-cell', width: '130px' },
      { field: 'fromDate', header: 'Ngày hiệu lực từ', textAlign: 'right', display: 'table-cell', width: '160px' },
      { field: 'toDate', header: 'Ngày hiệu lực đến', textAlign: 'right', display: 'table-cell', width: '160px' },
    ];
    this.selectedColumns = this.cols.filter(e => e.field == "vendorName" || e.field == "productName" || e.field == "miniumQuantity"
      || e.field == "price" || e.field == "moneyUnitName" || e.field == "fromDate" || e.field == "toDate");
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.productService.getMasterDataVendorDialog();
    this.loading = false;
    if (result.statusCode == 200) {
      this.listUnitMoney = result.listProductMoneyUnit;
      this.listVendor = result.listVendor;
      this.listProduct = result.listProduct;
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  searchVendorProductPrice() {
    if (this.vendorName) {
      this.vendorName = this.vendorName.trim();
    }
    if (this.productName) {
      this.productName = this.productName.trim();
    }

    this.loading = true;
    this.vendorService.searchVendorProductPrice(this.productName, this.vendorName).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listVendorProductPrice = result.listVendorProductPrice;
      } else {

      }
    });
  }

  leftColNumber: number = 12;
  rightColNumber: number = 0;

  refreshFilter() {
    this.productName = '';
    this.vendorName = '';
    this.isGlobalFilter = '';
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.listVendorProductPrice = [];
    this.isShowFilterLeft = false;

    this.searchVendorProductPrice();
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 9;
        this.rightColNumber = 3;
        this.selectedColumns = this.cols.filter(e => e.field == "vendorName" || e.field == "productName" || e.field == "miniumQuantity"
          || e.field == "price" || e.field == "fromDate");
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
        this.selectedColumns = this.cols.filter(e => e.field == "vendorName" || e.field == "productName" || e.field == "miniumQuantity"
          || e.field == "price" || e.field == "moneyUnitName" || e.field == "fromDate");
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

  addVendorProductDetail() {
    let ref = this.dialogService.open(AddVendorPriceDialogComponent, {
      data: {
        isCreate: true,
        listVendorProductPrice: this.listVendorProductPrice
      },
      header: 'Thêm bảng giá nhà cung cấp',
      width: '70%',
      baseZIndex: 9999,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow-y": "scroll",
      }
    });

    ref.onClose.subscribe((result: ResultDialog) => {
      if (result) {
        if (result.status == true) {
          let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.message };
          this.showMessage(mgs);
          this.searchVendorProductPrice();
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.message };
          this.showMessage(mgs);
        }
      }
    });
  }

  editVendorProductPrice(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
    if (this.actionEdit) {
      var index = this.listVendorProductPrice.indexOf(dataRow);
      var OldArray = this.listVendorProductPrice[index];

      let ref = this.dialogService.open(AddVendorPriceDialogComponent, {
        data: {
          isCreate: false,
          productVendorMappingModel: OldArray,
          listVendorProductPrice: this.listVendorProductPrice
        },
        header: "Sửa bảng giá nhà cung cấp",
        width: '70%',
        baseZIndex: 9999,
        contentStyle: {
          "min-height": "280px",
          "max-height": "600px",
          "overflow-y": "scroll",
        },
      });

      ref.onClose.subscribe((result: ResultDialog) => {
        if (result) {
          if (result.status == true) {
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.message };
            this.showMessage(mgs);
            this.searchVendorProductPrice();
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.message };
            this.showMessage(mgs);
          }
        }
      });
    }
  }

  deleteVendorProductPrice(productVendorMappingId: string) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn xóa bản ghi này?',
      accept: () => {
        this.loading = true;
        this.vendorService.deleteVendorProductPrice(productVendorMappingId).subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode == 200) {
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
            this.searchVendorProductPrice();
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        });
      }
    });
  }

  goToDetailProduct(event: any) {
    this.router.navigate(['/product/detail', { productId: event.productId }]);
  }

  goToDetailVendor(event: any) {
    this.router.navigate(['/vendor/detail', { vendorId: event.vendorId }]);
  }

  onChangeAction(rowData: any) {
    this.actions = [];
    let editVendorProductPrice: MenuItem = {
      id: '1', label: 'Sửa giá NCC', icon: 'pi pi-comment', command: () => {
        this.editVendorProductPrice(rowData);
      }
    }
    let deleteVendorProductPrice: MenuItem = {
      id: '2', label: 'Xóa giá NCC', icon: 'pi pi-trash', command: () => {
        this.deleteVendorProductPrice(rowData.productVendorMappingId);
      }
    }

    if (this.actionEdit === true) {
      this.actions.push(editVendorProductPrice);
    }
    if (this.actionDelete === true) {
      this.actions.push(deleteVendorProductPrice);
    }
  }

  showDialogImport() {
    this.displayDialog = true;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }
  clear() {
    this.messageService.clear();
  }

  validateFile(data) {
    this.messErrFile = [];
    this.cellErrFile = [];

    data.forEach((row, i) => {
      if (i > 3) {
        if ((row[0] === null || row[0] === undefined || row[0].toString().trim() == "") && (row[1] === null || row[1] === undefined || row[1].toString().trim() == "")) {
          this.messErrFile.push('Dòng { ' + (i + 2) + ' } chưa nhập Mã sp hoặc Tên nhà cung cấp!');
        }
        if (parseFloat(row[4]) == undefined || parseFloat(row[4]).toString() == "NaN" || parseFloat(row[4]) == null) {
          this.messErrFile.push('Số lượng tối thiểu tại dòng { ' + (i + 2) + ' } sai định dạng');
        }
        if (parseFloat(row[5]) == undefined || parseFloat(row[5]).toString() == "NaN" || parseFloat(row[5]) == null) {
          this.messErrFile.push('Đơn giá tại dòng { ' + (i + 2) + ' } sai định dạng');
        }
        if (row[7] === null || row[7] === undefined || row[7] == "") {
          this.messErrFile.push('Ngày hiệu lực từ tại dòng { ' + (i + 2) + ' } không được để trống');
        }

        if (new Date(row[7].replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")).toString() == "Invalid Date") {
          this.messErrFile.push('Ngày hiệu lực từ tại dòng { ' + (i + 2) + ' } không đúng format');
        }
        if (row[8] != null || row[8] != undefined) {
          if (Date.parse(row[8]) == undefined || Date.parse(row[8]).toString() == "NaN" || Date.parse(row[8]) == null) {
            this.messErrFile.push('Ngày hiệu lực đến tại dòng { ' + (i + 2) + ' } sai định dạng');
          }
        }
      }
    });
    if (this.messErrFile.length != 0) return true;
    else return false;
  }

  async importExcel() {
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
        //lấy data từ file excel
        const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[code];
        /* save data */
        let dataVendorProductPrice: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
        //remove header row
        dataVendorProductPrice.shift();
        let productCodeList: string[] = [];
        let vendorNameList: string[] = [];
        let moneyUnitList: string[] = [];
        let isValidation = this.validateFile(dataVendorProductPrice);
        if (isValidation) {
          this.isInvalidForm = true;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = true;  //Hiển thị message lỗi       
        } else {
          var messProductErr = [];
          var messVendorErr = [];
          var messMoneyUnitErr = [];
          var messDuplicateInDataErr = [];

          this.isInvalidForm = false;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = false;  //Hiển thị message lỗi
          dataVendorProductPrice.forEach((row, i) => {
            if (i > 3 && row.length != 0) {
              if (row[0] !== null || row[0] !== undefined && row[0].trim() != "") {
                let isProduct = productCodeList.find(c => c.trim() == row[0].trim());
                if (isProduct == null || isProduct == undefined) {
                  productCodeList.push(row[0]);
                  let check = this.listProduct.find(c => c.productCode.toLowerCase().trim() == row[0].trim().toLowerCase());
                  if (check === undefined || check === null) {
                    messProductErr.push(i + 2);
                  }
                }
              }
              if (row[1] !== null || row[1] !== undefined && row[1].trim() != "") {
                let isVendor = vendorNameList.find(c => c.trim() == row[1].trim());
                if (isVendor == null || isVendor == undefined) {
                  vendorNameList.push(row[1]);
                  let check = this.listVendor.find(c => c.vendorName.toLowerCase().trim() == row[1].trim().toLowerCase());
                  if (check === undefined || check === null) {
                    messVendorErr.push(i + 2);
                  }
                }
              }
              if (row[6] !== null || row[6] !== undefined && row[6].trim() != "") {
                let isMoneyUnit = moneyUnitList.find(c => c.trim() == row[6].trim());
                if (isMoneyUnit == null || isMoneyUnit == undefined) {
                  moneyUnitList.push(row[6]);
                  let check = this.listUnitMoney.find(c => c.categoryName.toLowerCase().trim() == row[6].trim().toLowerCase());
                  if (check === undefined || check === null) {
                    messMoneyUnitErr.push(i + 2);
                  }
                }
              }
            }
          });
          let countProduct = this.listProduct.filter(c => productCodeList.includes(c.productCode));
          let countVendor = this.listVendor.filter(c => vendorNameList.includes(c.vendorName));
          let countMoneyUnit = this.listUnitMoney.filter(c => moneyUnitList.includes(c.categoryName));

          if (countProduct.length == productCodeList.length && countVendor.length == vendorNameList.length && countMoneyUnit.length == countMoneyUnit.length) {
            this.listVendorProdcutPriceImport = [];
            dataVendorProductPrice.forEach((row, i) => {
              if (i > 3 && row.length != 0) {
                let vendorProductVendorPrice = new ProductVendorMappingModel();
                vendorProductVendorPrice.ProductVendorMappingId = this.emptyGuid;
                let product = this.listProduct.find(c => c.productCode.toLowerCase() == row[0].trim().toLowerCase());
                vendorProductVendorPrice.ProductId = product.productId;
                let vendor = this.listVendor.find(c => c.vendorName.toLowerCase() == row[1].toLowerCase().trim());
                vendorProductVendorPrice.VendorId = vendor.vendorId;
                vendorProductVendorPrice.VendorProductCode = row[2] == null ? row[2] : row[2].trim();
                vendorProductVendorPrice.VendorProductName = row[3] == null ? row[3] : row[3].trim();
                vendorProductVendorPrice.MiniumQuantity = (row[4] === null || row[4] === undefined || row[4] == "") ? 0 : row[4];;
                vendorProductVendorPrice.Price = (row[5] === null || row[5] === undefined || row[5] == "") ? 0 : row[5];
                let moneyUnit = this.listUnitMoney.find(c => c.categoryName.toLowerCase() == row[6].toLowerCase().trim());
                vendorProductVendorPrice.MoneyUnitId = moneyUnit.categoryId;

                let date = new Date(row[7].replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
                vendorProductVendorPrice.FromDate = convertToUTCTime(date);

                if (row[8] != null || row[8] != undefined) {
                  vendorProductVendorPrice.ToDate = new Date(row[8].replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"))
                }

                this.listVendorProdcutPriceImport.push(vendorProductVendorPrice);
                let check = this.listVendorProductPrice.find(c => c.productId == vendorProductVendorPrice.ProductId && c.vendorId == vendorProductVendorPrice.VendorId
                  && c.miniumQuantity == vendorProductVendorPrice.MiniumQuantity && c.moneyUnitId == vendorProductVendorPrice.MoneyUnitId &&
                  this.calcDaysDiff(new Date(c.fromDate), new Date(vendorProductVendorPrice.FromDate)) == 0);
                if (check) {
                  messDuplicateInDataErr.push('Giá NCC tại dòng { ' + (i + 2) + ' } đã tồn tại trong hệ thống!');
                }
              }
            });
            if (this.listVendorProdcutPriceImport.length > 0 && messDuplicateInDataErr.length == 0) {
              this.vendorService.importVendorProductPrice(this.listVendorProdcutPriceImport, this.auth.UserId).subscribe(response => {
                var result = <any>response;
                if (result.statusCode == 200) {
                  let mgs = { severity: 'success', summary: 'Thông báo', detail: result.messageCode };
                  this.showMessage(mgs);
                  this.searchVendorProductPrice();
                } else {
                  let mgs = { severity: 'warn', summary: 'Thông báo', detail: result.messageCode };
                  this.showMessage(mgs);
                }
              });
            }
          }

          if (messDuplicateInDataErr.length != 0) {
            this.isInvalidForm = true;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = true;  //Hiển thị message lỗi 
            messDuplicateInDataErr.forEach(item => {
              this.messErrFile.push(item)
            });
          }

          if (countProduct.length != productCodeList.length) {
            this.isInvalidForm = true;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = true;  //Hiển thị message lỗi 
            messProductErr.forEach(item => {
              this.messErrFile.push('Sản phẩm tại dòng ' + item + ' không tồn tại trong hệ thống')
            });
          }
          if (countVendor.length != vendorNameList.length) {
            this.isInvalidForm = true;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = true;  //Hiển thị message lỗi 
            messVendorErr.forEach(item => {
              this.messErrFile.push('Nhà cung cấp tại dòng ' + item + ' không tồn tại trong hệ thống')
            });
          }
          if (countMoneyUnit.length != moneyUnitList.length) {
            this.isInvalidForm = true;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = true;  //Hiển thị message lỗi 
            messMoneyUnitErr.forEach(item => {
              this.messErrFile.push('Đơn vị tiền tại dòng ' + item + ' không tồn tại trong hệ thống')
            });
          }
        }
        this.displayDialog = false;
      }
    }
  }

  exportExcel() {
    let dateUTC = new Date();
    // getMonth() trả về index trong mảng nên cần cộng thêm 1
    let title = "Danh sách bảng giá nhà cung cấp " + dateUTC.getDate() + '_' + (dateUTC.getMonth() + 1) + '_' + dateUTC.getUTCFullYear();
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Sheet1');
    worksheet.pageSetup.margins = {
      left: 0.25, right: 0.25,
      top: 0.75, bottom: 0.75,
      header: 0.3, footer: 0.3
    };
    worksheet.pageSetup.paperSize = 9;  //A4 : 9

    let dataRow1 = [];
    dataRow1[1] = `    `
    let row1 = worksheet.addRow(dataRow1);
    row1.font = { name: 'Arial', size: 18, bold: true };
    row1.alignment = { vertical: 'bottom', horizontal: 'center', wrapText: true };

    let dataRow2 = [];
    dataRow2[1] = `    `
    dataRow2[3] = `Danh sách bảng giá nhà cung cấp`
    let row2 = worksheet.addRow(dataRow2);
    row2.font = { name: 'Arial', size: 18, bold: true };
    worksheet.mergeCells(`C${row2.number}:D${row2.number}`);
    row2.alignment = { vertical: 'bottom', horizontal: 'center', wrapText: true };

    worksheet.addRow([]);
    /* Header row */
    let dataHeaderRow = ['STT', 'Nhà cung cấp', 'Mã sản phẩm', 'Tên sản phẩm', 'Số lượng tối thiểu', 'Giá', 'Tiền tệ', 'Đơn vị tính', 'Ngày bắt đầu', 'Ngày kết thúc'];
    let headerRow = worksheet.addRow(dataHeaderRow);
    headerRow.font = { name: 'Arial', size: 10, bold: true };
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

    this.listVendorProductPrice.forEach((item, index) => {
      let vendorName = item.vendorName;
      let productCode = item.productCode;
      let productName = item.productName;
      let miniumQuantity = this.decimalPipe.transform(item.miniumQuantity).toString();
      let price = this.decimalPipe.transform(item.price).toString();
      let moneyUnitName = item.moneyUnitName;
      let productUnitName = item.productUnitName;
      let fromDate = this.datePipe.transform(item.fromDate, 'dd/MM/yyyy').toString();
      let toDate = '';
      if (item.toDate) {
        toDate = this.datePipe.transform(item.toDate, 'dd/MM/yyyy').toString();
      }

      /* Header row */
      let dataHeaderRowIndex = [index + 1, vendorName, productCode, productName, miniumQuantity, price, moneyUnitName, productUnitName, fromDate, toDate];
      let headerRowIndex = worksheet.addRow(dataHeaderRowIndex);
      headerRowIndex.font = { name: 'Arial', size: 10 };
      dataHeaderRowIndex.forEach((item, index) => {
        headerRowIndex.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
        if (index == 4 || index == 5 || index == 8 || index == 9) {
          headerRowIndex.getCell(index + 1).alignment = { vertical: 'bottom', horizontal: 'right' };
        } else {
          headerRowIndex.getCell(index + 1).alignment = { vertical: 'bottom', horizontal: 'left' };
        }
      });
      // headerRowIndex.height = 40;     
    })

    worksheet.addRow([]);
    worksheet.getRow(2).height = 47;
    worksheet.getRow(4).height = 30;
    worksheet.getColumn(1).width = 5;
    worksheet.getColumn(2).width = 40;
    worksheet.getColumn(3).width = 30;
    worksheet.getColumn(4).width = 50;
    worksheet.getColumn(5).width = 15;
    worksheet.getColumn(6).width = 15;
    worksheet.getColumn(7).width = 15;
    worksheet.getColumn(8).width = 20;
    worksheet.getColumn(9).width = 25;
    worksheet.getColumn(10).width = 25;
    this.exportToExel(workbook, title);
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  downloadTemplateExcel() {
    this.vendorService.downloadTemplateVendorProductPrice(this.auth.UserId).subscribe(response => {
      this.loading = false;
      const result = <any>response;
      if (result.templateExcel != null && result.statusCode === 202 || result.statusCode === 200) {
        const binaryString = window.atob(result.templateExcel);
        const binaryLen = binaryString.length;
        const bytes = new Uint8Array(binaryLen);
        for (let idx = 0; idx < binaryLen; idx++) {
          const ascii = binaryString.charCodeAt(idx);
          bytes[idx] = ascii;
        }
        const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        const fileName = result.fileName + ".xls";
        //const fileName = result.nameFile  + ".xlsx";
        link.download = fileName;
        link.click();
      }
    }, error => { this.loading = false; });
  }

  chooseFile(event: any) {
    this.fileName = event.target.files[0].name;
    this.importFileExcel = event.target;

    // this.getInforDetailQuote();
  }

  calcDaysDiff(dateFrom, dateTo): number {
    let currentDate = new Date(dateTo);
    let dateSent = new Date(dateFrom);

    return Math.floor((Date.UTC(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate()) - Date.UTC(dateSent.getFullYear(), dateSent.getMonth(), dateSent.getDate())) / (1000 * 60 * 60 * 24));
  }

  cancelFile() {
    $("#importFileProduct").val("")
    this.fileName = "";
  }

  closeDialogImport() {
    this.cancelFile();
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function getDate(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate()))
}



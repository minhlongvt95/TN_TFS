import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';

import { ProductCategoryService } from "../../../admin/components/product-category/services/product-category.service";
import { CategoryService } from "../../../shared/services/category.service";
import { ProductService } from '../../../product/services/product.service';
import { WarehouseService } from "../../services/warehouse.service";
import { MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-add-product',
  templateUrl: './add-product.component.html',
  styleUrls: ['./add-product.component.css']
})
export class AddProductComponent implements OnInit {

  listProductCategory: Array<any> = [];
  selectedProductCategory: Array<any> = [];
  selectedProductName: Array<any> = [];
  productCode: string;
  cols: any[];
  frozenCols: any[];
  listVendorOrderProduct: Array<any> = [];
  unitmoney: Array<any> = [];
  DiscountTypeList: Array<any> = [{ "name": "Theo %", "value": true }, { "name": "Số tiền", "value": false }];
  listunit: Array<any> = [];
  listProductNameCode: Array<any> = [];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  dataObject: any;



  findProductForm: FormGroup;
  productCategoryControl: FormControl;
  productNameControl: FormControl;
  nameProductCodeControl: FormControl;
  rowDataNoteControl: FormControl;
  rowDataQuantityControl: FormControl;
  noteContentControl: FormControl;
  rowPriceControl: FormControl;
  rowExchangeRateControl: FormControl;
  rowVatControl: FormControl;
  rowNoteControl: FormControl;
  rowDataUnitNameControl: FormControl;
  rowDataDiscountTypeListControl: FormControl;
  rowDataUnitMoneyControl: FormControl;
  rowDiscountValueControl: FormControl;

  constructor(
    private productCategoryService: ProductCategoryService,
    private productService: ProductService,
    private categoryService: CategoryService,
    private warehouseService: WarehouseService,
    public messageService: MessageService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private confirmationService: ConfirmationService,
  ) { }

  async ngOnInit() {
    this.dataObject = this.config.data.object;
    this.getFormControl();
    this.getMasterData();
    if (this.dataObject == 1) {
      this.cols = [
        { field: 'productCode', header: 'Mã SP', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'productName', header: 'Diễn giải', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'unitName', header: 'Đơn vị tính', width: '7%', textAlign: 'left', color: '#f44336' },
        { field: 'quantityRequire', header: 'Số lượng nhập', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'price', header: 'Giá nhập', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'exchangeRate', header: 'Tỷ giá', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'vat', header: 'VAT(%)', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'discountValueType', header: 'Loại chiết khấu', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'discountValue', header: 'Chiết khấu', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'sumAmount', header: 'Thành tiền', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'note', header: 'Ghi chú', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'box', header: '', width: '1em', textAlign: 'center', color: '#f44336' },
        //{ field: 'totalSerial', header: 'Số serial' }
      ];
    } else {
      this.cols = [
        { field: 'productCode', header: 'Mã SP', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'productName', header: 'Diễn giải', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'unitName', header: 'Đơn vị tính', width: '7%', textAlign: 'left', color: '#f44336' },
        { field: 'quantityRequire', header: 'Số lượng xuất', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'price', header: 'Giá nhập', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'exchangeRate', header: 'Tỷ giá', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'vat', header: 'VAT(%)', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'discountValueType', header: 'Loại chiết khấu', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'discountValue', header: 'Chiết khấu', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'sumAmount', header: 'Thành tiền', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'note', header: 'Ghi chú', width: '7%', textAlign: 'center', color: '#f44336' },
        { field: 'box', header: '', width: '1em', textAlign: 'center', color: '#f44336' },
        //{ field: 'totalSerial', header: 'Số serial' }
      ];

    }
    this.frozenCols = [
      { field: 'vendorOrderCode', header: 'Phiếu mua hàng' },
    ];

  }

  getFormControl() {
    this.productCategoryControl = new FormControl();
    this.nameProductCodeControl = new FormControl();
    this.rowDataNoteControl = new FormControl();
    this.rowDataQuantityControl = new FormControl();
    this.noteContentControl = new FormControl();
    this.rowPriceControl = new FormControl();
    this.rowExchangeRateControl = new FormControl();
    this.rowVatControl = new FormControl();
    this.rowNoteControl = new FormControl();
    this.rowDataUnitNameControl = new FormControl();
    this.rowDataDiscountTypeListControl = new FormControl();
    this.rowDataUnitMoneyControl = new FormControl();
    this.rowDiscountValueControl = new FormControl();
    this.productNameControl = new FormControl();

    this.findProductForm = new FormGroup({
      productCategoryControl: this.productCategoryControl,
      productNameControl: this.productNameControl,
      nameProductCodeControl: this.nameProductCodeControl,
      rowDataNoteControl: this.rowDataNoteControl,
      rowDataQuantityControl: this.rowDataQuantityControl,
      noteContentControl: this.noteContentControl,
      rowPriceControl: this.rowPriceControl,
      rowExchangeRateControl: this.rowExchangeRateControl,
      rowVatControl: this.rowVatControl,
      rowNoteControl: this.rowNoteControl,
      rowDataUnitNameControl: this.rowDataUnitNameControl,
      rowDataDiscountTypeListControl: this.rowDataDiscountTypeListControl,
      rowDataUnitMoneyControl: this.rowDataUnitMoneyControl,
      rowDiscountValueControl: this.rowDiscountValueControl,
    });

  }

  async getMasterData() {

    var result: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc('DTI');
    this.unitmoney = result.category;

    this.productCategoryService.getAllProductCategory().subscribe(response => {
      let result = <any>response;
      if (result.productCategoryList != null) {
        this.listProductCategory = result.productCategoryList.filter(f => f.productCategoryLevel==0);

        this.listProductCategory.sort((a: any, b: any) => {
          while (a.productCategoryName != null && b.productCategoryName != null) {
            let x = a.productCategoryName.toLowerCase().trim();
            let y = b.productCategoryName.toLowerCase().trim();
            return (x.localeCompare(y) === -1 ? -1 : 1);
          }
        });
      }
    }, error => { });

    this.categoryService.getAllCategoryByCategoryTypeCode('DNH').subscribe(response => {
      let result = <any>response;
      this.listunit = result.category;
    }, error => { });

    this.warehouseService.getProductNameAndProductCode(null).subscribe(response => {
      let result = <any>response;
      this.listProductNameCode = result.productList;
    }, error => { });
  }

  searchProduct() {
    var selectedProductCategoryList = this.selectedProductCategory.map(item => item.productCategoryId);
    this.productService.searchProduct('', this.productCode, selectedProductCategoryList, []).subscribe(response => {
      let result = <any>response;
      if (result.productList.length > 0) {
        this.listVendorOrderProduct = [];
        for (var i = 0; i < result.productList.length; ++i) {
          var objectPush = {
            inventoryReceivingVoucherMappingId: this.emptyGuid, vendorOrderId: this.emptyGuid, vendorOrderDetailId: this.emptyGuid,
            vendorOrderCode: '', productId: result.productList[i].productId, productName: result.productList[i].productName, productCode: result.productList[i].productCode, unitId: '',
            unitName: '', quantityRequire: 0, quantity: 0, price: 0, wareHouseId: '',
            wareHouseName: '', note: '', listSerial: [], totalSerial:0, currencyUnit: '',
            exchangeRate: 1, vat: null, discountType: true, discountValue: 0, nameMoneyUnit: '',
            sumAmount: 0, error: false
          }
          this.listVendorOrderProduct.push(objectPush);
        }

      }
    }, error => { });
  }

  closePanelProductCategory() {
    if (this.selectedProductCategory.length == 0) return;
    var selectedProductCategoryList = this.selectedProductCategory.map(item => item.productCategoryId);
    var selectedProductId = this.selectedProductName.map(item => item.productId);
    this.warehouseService.filterProduct(selectedProductCategoryList, selectedProductId).subscribe(response => {
      let result = <any>response;
      if (result.productList.length > 0) {
        let ResultReturn = result.productList;
        if (this.listVendorOrderProduct == null) { this.listVendorOrderProduct = []; }
        for (var i = 0; i < result.productList.length; ++i) {
          let checkexist = this.listVendorOrderProduct.findIndex(f => f.productId == result.productList[i].productId);
          if (checkexist >= 0) {
            var itemArray = this.listVendorOrderProduct[checkexist];
            this.listVendorOrderProduct.splice(checkexist, 1);
            this.listVendorOrderProduct.unshift(itemArray);
          }
          else {
            let findNameMoney = this.unitmoney.find(f => f.categoryId == result.productList[i].productMoneyUnitId);
            let findNameUnit = this.listunit.find(f => f.categoryId == result.productList[i].productUnitId);
            var objectPush = {
              inventoryReceivingVoucherMappingId: this.emptyGuid, vendorOrderId: this.emptyGuid, vendorOrderDetailId: this.emptyGuid,
              vendorOrderCode: '', productId: result.productList[i].productId, productName: result.productList[i].productName, productCode: result.productList[i].productCode,
              unitId: result.productList[i].productUnitId,
              unitName: findNameUnit != null ? findNameUnit.categoryName : '', quantityRequire: 0, quantity: 0, price: formatNumber(result.productList[i].price1), wareHouseId: '',
              wareHouseName: '', note: '', listSerial: [], totalSerial: 0, currencyUnit: result.productList[i].productMoneyUnitId,
              exchangeRate: 1, vat: null, discountType: true, discountValue: 0, nameMoneyUnit: findNameMoney != null ? findNameMoney.categoryName : '',
              sumAmount: 0, error: false
            }
            this.listVendorOrderProduct.unshift(objectPush);
          }
        }

      }
    }, error => { });

  }

  closePanelProductName() {
    var selectedProductCategoryList = this.selectedProductCategory.map(item => item.productCategoryId);
    var selectedProductId = this.selectedProductName.map(item => item.productId);
    this.warehouseService.filterProduct(selectedProductCategoryList, selectedProductId).subscribe(response => {
      let result = <any>response;
      if (result.productList.length > 0) {
        let ResultReturn = result.productList;
        if (this.listVendorOrderProduct == null) { this.listVendorOrderProduct = []; }
        for (var i = 0; i < result.productList.length; ++i) {
          let checkexist = this.listVendorOrderProduct.findIndex(f => f.productId == result.productList[i].productId);
          if (checkexist >= 0) {
            var itemArray = this.listVendorOrderProduct[checkexist];
            this.listVendorOrderProduct.splice(checkexist, 1);
            this.listVendorOrderProduct.unshift(itemArray);
          }
          else {
            let findNameMoney = this.unitmoney.find(f => f.categoryId == result.productList[i].productMoneyUnitId);
            let findNameUnit = this.listunit.find(f => f.categoryId == result.productList[i].productUnitId);
            var objectPush = {
              inventoryReceivingVoucherMappingId: this.emptyGuid, vendorOrderId: this.emptyGuid, vendorOrderDetailId: this.emptyGuid,
              vendorOrderCode: '', productId: result.productList[i].productId, productName: result.productList[i].productName, productCode: result.productList[i].productCode,
              unitId: result.productList[i].productUnitId,
              unitName: findNameUnit != null ? findNameUnit.categoryName : '', quantityRequire: 0, quantity: 0, price: formatNumber(result.productList[i].price1), wareHouseId: '',
              wareHouseName: '', note: '', listSerial: [], totalSerial: 0, currencyUnit: result.productList[i].productMoneyUnitId,
              exchangeRate: 1, vat: null, discountType: true, discountValue: 0, nameMoneyUnit: findNameMoney != null ? findNameMoney.categoryName : '',
              sumAmount: 0
            }
            this.listVendorOrderProduct.unshift(objectPush);
          }
        }

      }
    }, error => { });
  }

  onCancelClick() {
    this.confirmationService.confirm({
      message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác ,bạn có chắc chắn muốn hủy?',
      header: 'Thông báo',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.ref.close();
      },
      reject: () => {
        return;
      }
    });

  }
  onSaveClick() {
    if (this.listVendorOrderProduct.length == 0 || this.listVendorOrderProduct.length == null) {
      this.messageService.clear();
      this.messageService.add({ key: 'error', severity: 'error', summary: "Cần chọn ít nhất một sản phẩm", detail: 'Danh sách sản phẩm' });
      return;
    } else {
      var isError = false;
      for (let i = 0; i < this.listVendorOrderProduct.length; i++) {
        if (this.listVendorOrderProduct[i].error == true) {
          isError = true;
          break;
        }
      }
      if (isError == true) {
        this.messageService.clear();
        this.messageService.add({ key: 'error', severity: 'error', summary: "Có lỗi trong danh sách sản phẩm", detail: 'Danh sách sản phẩm' });
        return;
      }
      this.ref.close({ 'listVendorOrderProduct': this.listVendorOrderProduct });
    }
  }

  changeunitName(event: any, rowdata: any) {
    rowdata.unitName = event.value.categoryName;
    rowdata.unitId = event.value.categoryId;
  }

  changeCurrency(event: any, rowdata: any) {
    rowdata.nameMoneyUnit = event.value.categoryName;
    rowdata.currencyUnit = event.value.categoryId;
  }
  changediscountValueType(event: any, rowdata: any) {
    //rowdata.nameMoneyUnit = event.value.name;
    rowdata.discountType = event.value.value;
  }

  sumTotal(rowdata: any) {
    rowdata.error = false;
    let quantity = 0;
    let price = 0;
    let exchangeRate = 0;
    if (rowdata.quantityRequire <= 0 || rowdata.price <= 0
      || rowdata.quantityRequire == null || rowdata.price == null
    ) {
      rowdata.error = true;
      this.messageService.clear();
      this.messageService.add({ key: 'error', severity: 'error', summary: "Số lượng xuất không được để trống hoặc bằng 0", detail: 'Danh sách ' });
      return;
    }
    //product Amount
    quantity = parseFloat(rowdata.quantityRequire.replace(/,/g, ''));
    price = parseFloat(rowdata.price.replace(/,/g, ''));

    if (quantity <= 0 || price <= 0) {
      rowdata.error = true;
      this.messageService.clear();
      this.messageService.add({ key: 'error', severity: 'error', summary: "Số lượng xuất không được để trống hoặc bằng 0", detail: 'Danh sách ' });
      return;
    }

    if (rowdata.exchangeRate == null || rowdata.exchangeRate == '') {
      rowdata.error = true;
      this.messageService.clear();
      this.messageService.add({ key: 'error', severity: 'error', summary: "Tỷ giá không được để trống", detail: 'Danh sách ' });
      return;
      //exchangeRate = 1;
    } else {
      exchangeRate = parseFloat(rowdata.exchangeRate);
      if (exchangeRate <= 0) {
        rowdata.error = true;
        this.messageService.clear();
        this.messageService.add({ key: 'error', severity: 'error', summary: "Tỷ giá cần phải lớn hơn 0", detail: 'Danh sách ' });
        return;
        //exchangeRate = 1;
      }
    }


    //var productAmount = rowdata.quantity * rowdata.price * rowdata.exchangeRate;
   var  productAmount = quantity * price * exchangeRate;
    //Vat
    var Vat = 0;
    if (rowdata.vat !== null && rowdata.vat !== '') {
      let vat = parseFloat(rowdata.vat);
      if (vat > 100) {
        rowdata.error = true;
        this.messageService.clear();
        this.messageService.add({ key: 'error', severity: 'error', summary: "Vat cần lớn hơn 0 và nhỏ hơn bằng 100", detail: 'Danh sách ' });
        return;
      }
      if (vat > 0) {
        Vat = (productAmount * vat) / 100;
      }
    }
    var discountAcmount = 0;
    //Discount
    if (rowdata.discountType == true) {
      if (rowdata.discountValue !== null) {
        let discountValue = parseFloat(rowdata.discountValue);
        if (discountValue > 0) {
          discountAcmount = (productAmount * discountValue) / 100;
        }

      }
    }
    else {
      if (rowdata.discountValue !== null) {
        let discountValue = parseFloat(rowdata.discountValue.replace(/,/g, ''));
        discountAcmount = discountValue;
      }
    }
    rowdata.sumAmount = productAmount + Vat - discountAcmount;
  }

  cancelRow(rowData: any) {
    var index = this.listVendorOrderProduct.findIndex(f => f.productId == rowData.productId);
    this.listVendorOrderProduct.splice(index, 1);
  }
  clearAllData() {
    this.listVendorOrderProduct = [];
  }
  filterProductNameMultiple(event: any) {
    let query = event.query;
    this.warehouseService.getProductNameAndProductCode(query).subscribe(response => {
      let result = <any>response;
      this.listProductNameCode = [];
      if (this.listVendorOrderProduct == null || this.listVendorOrderProduct.length == 0) {
        this.listProductNameCode = result.productList; return;
      } else {
        for (var i = 0; i < result.productList.length; ++i) {
          let checkexist = this.listVendorOrderProduct.findIndex(f => f.productId == result.productList[i].productId);
          if (checkexist < 0) {
            this.listProductNameCode.push(result.productList[i]);
          } 
        }
      }
    }, error => { });

  }
  onSelect(evt: any) {
    var selectedProductCategoryList = this.selectedProductCategory.map(item => item.productCategoryId);
    var selectedProductId = this.selectedProductName.map(item => item.productId);
    this.warehouseService.filterProduct(selectedProductCategoryList, selectedProductId).subscribe(response => {
      let result = <any>response;
      if (result.productList.length > 0) {
        let ResultReturn = result.productList;
        if (this.listVendorOrderProduct == null) { this.listVendorOrderProduct = []; }
        for (var i = 0; i < result.productList.length; ++i) {
          let checkexist = this.listVendorOrderProduct.findIndex(f => f.productId == result.productList[i].productId);
          if (checkexist >= 0) {
            var itemArray = this.listVendorOrderProduct[checkexist];
            this.listVendorOrderProduct.splice(checkexist, 1);
            this.listVendorOrderProduct.unshift(itemArray);
          }
          else {
            let findNameMoney = this.unitmoney.find(f => f.categoryId == result.productList[i].productMoneyUnitId);
            let findNameUnit = this.listunit.find(f => f.categoryId == result.productList[i].productUnitId);
            var objectPush = {
              inventoryReceivingVoucherMappingId: this.emptyGuid, vendorOrderId: this.emptyGuid, vendorOrderDetailId: this.emptyGuid,
              vendorOrderCode: '', productId: result.productList[i].productId, productName: result.productList[i].productName, productCode: result.productList[i].productCode,
              unitId: result.productList[i].productUnitId,
              unitName: findNameUnit != null ? findNameUnit.categoryName : '', quantityRequire: 0, quantity: 0, price: formatNumber(result.productList[i].price1), wareHouseId: '',
              wareHouseName: '', note: '', listSerial: [], totalSerial: 0, currencyUnit: result.productList[i].productMoneyUnitId,
              exchangeRate: 1, vat: null, discountType: true, discountValue: 0, nameMoneyUnit: findNameMoney != null ? findNameMoney.categoryName : '',
              sumAmount: 0, error: false
            }
            this.listVendorOrderProduct.unshift(objectPush);
          }
        }

      }
    }, error => { });

  }
  onUnselect(evt: any) {
    let checkexist = this.listVendorOrderProduct.findIndex(f => f.productId == evt.productId);
    if (checkexist >= 0) {
      this.listVendorOrderProduct.splice(checkexist, 1);
    }
  }
  checklengthNote(rowdata: any) {
    if (rowdata.note.length > 200) {
      rowdata.error = true;
      this.messageService.clear();
      this.messageService.add({ key: 'error', severity: 'error', summary: "Không được nhập quá 200 ký tự", detail: 'Danh sách ' });
      return;
    }
    else {
      rowdata.note = rowdata.note.trim();
    }
  }

}
function formatNumber(num) {
  return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
}

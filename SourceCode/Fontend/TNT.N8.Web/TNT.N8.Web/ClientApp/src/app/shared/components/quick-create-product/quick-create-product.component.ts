import * as $ from 'jquery';
import { Component, OnInit, ViewChild, ElementRef, HostListener, ViewRef, OnDestroy, Renderer2 } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';

import { DialogService } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService } from 'primeng/api';

import { TreeProductCategoryComponent } from '../../../product/components/tree-product-category/tree-product-category.component';

import { ProductService } from '../../services/product.service';

class productUnitModel {
  public categoryId: string;
  public categoryCode: string;
  public categoryName: string;
  public isDefauld: string;
}

class productMoneyUnitModel {
  public categoryId: string;
  public categoryCode: string;
  public categoryName: string;
  public isDefauld: string;
}

class productCategoryModel {
  public productCategoryId: string;
  public productCategoryCode: string;
  public productCategoryName: string;
  public productCategoryLevel: string;
  public hasChildren: boolean;
  public productCategoryNameByLevel: string; //tên theo phân cấp
  public productCategoryListNameByLevel: Array<string>;
  constructor() {
    this.productCategoryListNameByLevel = [];
  }
}

class productModel {
  productId: string;
  productCode: string;
  productName: string;
  productCodeName: string;
  productUnitId: string;
  productUnitName: string;
  productMoneyUnitId: string;
  price1: number;
  listProductAttributeCategory: Array<any>;
  constructor() {
    this.listProductAttributeCategory = [];
  }
}

class productRequestModel {
  ProductId: string;
  ProductCategoryId: string;
  ProductName: string;
  ProductCode: string;
  Price1: number;
  Price2: number;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: Boolean;
  Quantity: number;
  ProductUnitId: string;
  ProductDescription: string;
  Vat: Number;
  MinimumInventoryQuantity: Number;
  ProductMoneyUnitId: string;
  GuaranteeTime: Number;
  ProductCategoryName: string;
  ListVendorName: string;
  ExWarehousePrice: number;
  CountProductInformation: number;
  CalculateInventoryPricesId: string;
  PropertyId: string;
  WarehouseAccountId: string;
  RevenueAccountId: string;
  PayableAccountId: string;
  ImportTax: number;
  CostPriceAccountId: string;
  AccountReturnsId: string;
  FolowInventory: boolean;
  ManagerSerialNumber: boolean;

  constructor() {
    this.CountProductInformation = 0;
  }
}

@Component({
  selector: 'app-quick-create-product',
  templateUrl: './quick-create-product.component.html',
  styleUrls: ['./quick-create-product.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})

export class QuickCreateProductComponent implements OnInit {
  loading: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  //form controls
  productForm: FormGroup;
  selectedProductCategory: productCategoryModel;
  //master data
  listProductUnit: Array<productUnitModel> = [];
  listPriceInventory: Array<productMoneyUnitModel> = [];
  listProperty: Array<productMoneyUnitModel> = [];
  listProductCode: Array<string> = [];
  constructor(
    public dialogService: DialogService,
    private ref: DynamicDialogRef,
    private messageService: MessageService,
    private productService: ProductService
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.getMasterdata();
  }

  initForm() {
    this.productForm = new FormGroup({
      'FolowInventory': new FormControl(true),
      'ManagerSerialNumber': new FormControl(false),
      'ProductCategory': new FormControl(null, Validators.required),
      'ProductCode': new FormControl('', [Validators.required, checkBlankString()]),
      'ProductName': new FormControl('', [Validators.required, checkBlankString()]),

      'ProductUnit': new FormControl(null, Validators.required),
      'InventoryPrices': new FormControl(null, Validators.required),
      'Property': new FormControl(null, Validators.required),
    });
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.productService.getDataQuickCreateProduct();
    this.loading = false;
    if (result.statusCode == 200) {
      this.listPriceInventory = result.listPriceInventory;
      this.listProductUnit = result.listProductUnit;
      this.listProperty = result.listProperty;
      this.listProductCode = result.listProductCode;

      this.productForm.get('ProductCode').setValidators([Validators.required, checkDuplicateCode(this.listProductCode), checkBlankString()]);
      this.productForm.updateValueAndValidity();
    }
  }

  isDisplayRequired: boolean = true;
  changeFollowInventory(event: any) {
    if (event.checked == true) {
      this.isDisplayRequired = true;
      this.productForm.controls['InventoryPrices'].setValidators(Validators.required);
      this.productForm.controls['InventoryPrices'].updateValueAndValidity();
    } else {
      this.isDisplayRequired = false;
      this.productForm.controls['InventoryPrices'].setValidators(null);
      this.productForm.controls['InventoryPrices'].updateValueAndValidity();
    }
  }

  openProductCategoryDialog() {
    let ref = this.dialogService.open(TreeProductCategoryComponent, {
      header: 'Thông tin danh mục',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        'overflow-x': 'hidden'
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.selectedProductCategory = result.productCategory;
          this.productForm.get('ProductCategory').setValue(this.selectedProductCategory.productCategoryNameByLevel)
        }
      }
    });
  }

  async quickCreateProduct() {
    if (!this.productForm.valid) {
      Object.keys(this.productForm.controls).forEach(key => {
        if (!this.productForm.controls[key].valid) {
          this.productForm.controls[key].markAsTouched();
        }
      });
      return;
    }

    let product = this.mappingProductForm();

    this.loading = true;
    let result: any = await this.productService.createProductAsync(product, [], [], [], [], this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      let productUnitName = this.listProductUnit.find(e => e.categoryId == product.ProductUnitId)?.categoryName ?? '';
      let productCodeName = result.newProduct.productCode + ' - ' + result.newProduct.productName;
      let productUnitId = product.ProductUnitId;
      let resultPopup = {
        newProduct: { ...product, ...result.newProduct, productUnitName, productCodeName, productUnitId},
      }
      this.ref.close(resultPopup);
    }
  }

  mappingProductForm(): productRequestModel {
    let newProduct = new productRequestModel();
    newProduct.ProductId = this.emptyGuid;
    newProduct.ProductCategoryId = this.selectedProductCategory.productCategoryId;
    newProduct.ProductName = this.productForm.get('ProductName').value;
    newProduct.ProductCode = this.productForm.get('ProductCode').value;
    newProduct.Price2 = 0; //không dùng trường này
    newProduct.ProductUnitId = this.productForm.get('ProductUnit').value.categoryId;
    newProduct.Active = true;
    newProduct.Quantity = 0; //Không dùng trường này
    newProduct.MinimumInventoryQuantity = 0;
    newProduct.CalculateInventoryPricesId = this.productForm.get('InventoryPrices').value ? this.productForm.get('InventoryPrices').value.categoryId : null;
    newProduct.PropertyId = this.productForm.get('Property').value ? this.productForm.get('Property').value.categoryId : null;
    newProduct.FolowInventory = this.productForm.get('FolowInventory').value;
    newProduct.ManagerSerialNumber = this.productForm.get('ManagerSerialNumber').value;
    newProduct.CreatedById = this.emptyGuid;
    newProduct.CreatedDate = new Date();
    newProduct.UpdatedById = null;
    newProduct.UpdatedDate = null;

    return newProduct
  }

  cancel() {
    this.ref.close();
  }

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}

function checkBlankString(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() === "") {
        return { 'blankString': true };
      }
    }
    return null;
  }
}

function ValidationMaxValue(max: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      let value = ParseStringToFloat(control.value);
      if (value > max) return { 'maxValue': true }
    }
    return null;
  }
}

function checkDuplicateCode(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() !== "") {
        let duplicateCode = array.find(e => e.trim().toLowerCase() === control.value.trim().toLowerCase());
        if (duplicateCode !== undefined) {
          return { 'duplicateCode': true };
        }
      }
    }
    return null;
  }
}

function toCamel(o) {
  var newO, origKey, newKey, value;
  if (o instanceof Array) {
    return o.map(function(value) {
        if (typeof value === "object") {
          value = toCamel(value)
        }
        return value
    })
  } else {
    newO = {}
    for (origKey in o) {
      if (o.hasOwnProperty(origKey)) {
        newKey = (origKey.charAt(0).toLowerCase() + origKey.slice(1) || origKey).toString()
        value = o[origKey]
        if (value instanceof Array || (value !== null && value.constructor === Object)) {
          value = toCamel(value)
        }
        newO[newKey] = value
      }
    }
  }
  return newO
}

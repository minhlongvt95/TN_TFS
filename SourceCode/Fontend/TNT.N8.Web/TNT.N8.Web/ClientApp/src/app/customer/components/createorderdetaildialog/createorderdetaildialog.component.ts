import { Component, OnInit, ElementRef, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

import { MatRadioChange  } from '@angular/material/radio';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

import { FormControl, Validators, FormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { CategoryService } from "../../../shared/services/category.service";
import { VendorService } from "../../../vendor/services/vendor.service";
import { ProductService } from '../../../product/services/product.service';


import { QuoteDetail } from '../../models/quote-detail.model';
import { ProductAttributeCategoryModel, ProductAttributeCategoryValueModel } from '../../../product/models/productAttributeCategory.model';
import { QuoteProductDetailProductAttributeValue } from '../../models/quote-product-detail-product-attribute-value.model';

import * as $ from 'jquery';

export interface IDialogData {
  productValue: string;
  customerOrderDetailModel: QuoteDetail;  //Data của sản phẩm dịch vụ được trả về cho component (thêm mới hoặc sửa)
  customerOrderDetailModelOutPut: QuoteDetail;  //Data của một sản phẩm dịch vụ từ component truyền vào (Trong trường hợp edit một sản phẩm dịch vụ)
  isTrue: Boolean;
  isEdit: string;
}

@Component({
  selector: 'app-createorderdetaildialog',
  templateUrl: './createorderdetaildialog.component.html',
  styleUrls: ['./createorderdetaildialog.component.css']
})
export class CreateorderdetaildialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  incuredUseCurrencyMask = false;
  UseCurrencyMask = false;
  //get system parameter
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  //defaultNumberType = this.getDefaultNumberType();
  /*Declare Form */
  vendorIdControl: FormControl;
  productIdControl: FormControl;
  qualityControl: FormControl;
  priceControl: FormControl;
  unitControl: FormControl;
  moneyUnitControl: FormControl;
  exchangeRateControl: FormControl;
  discountTypeControl: FormControl;
  discountValueControl: FormControl;
  vatControl: FormControl;
  descriptionProductControl: FormControl;
  qualityIncuredControl: FormControl;
  priceIncuredControl: FormControl;
  vatIncuredControl: FormControl;
  unitIncuredControl: FormControl;
  moneyIncuredUnitControl: FormControl;
  exchangeRateIncuredControl: FormControl;
  discountTypeIncuredControl: FormControl;
  discountValueIncuredControl: FormControl;

  informationProductForm: FormGroup;
  checkProductForm: FormGroup;
  checkIncuredForm: FormGroup;
  /*Declare Model*/
  customerOrderDetailEditModel = new QuoteDetail();

  readonly OldData = new QuoteDetail();

  displayedAttributeColumns = ['nameattribute', 'value',];
  DiscountTypeList: Array<any> = [{ "name": "Theo %", "value": true }, { "name": "Số tiền", "value": false }];
  /*Declare */
  listunit: Array<any> = [];
  listPaymentMethod: Array<any> = [];
  listCategoryId: Array<string> = [];
  listVendor: Array<any> = [];
  listProductByVendor: Array<any> = [];
  unitmoney: Array<any> = [];
  //Declare  
  ProductAmount: number = 0;
  ProductVAT: number = 0;
  ProductDiscount: number = 0;
  ProductMoneyUnit: string = 'VND'; //null

  IncuredAmount: number = 0;
  IncuredVAT: number = 0;
  IncuredDiscount: number = 0;
  IncuredMoneyUnit: string = null;
  IsVND: boolean;
  CustomerOrderDiscount: number = 0;

  filteredVendorOptions: Observable<string[]>;
  filteredProductOptions: Observable<string[]>;
  filteredUnitProduct: Observable<string[]>;

  arrayProductAttributeCategoryModel: Array<ProductAttributeCategoryModel> = [];
  arrayOrderProductDetailProductAttributeValueModel: Array<QuoteProductDetailProductAttributeValue> = [];
  arrayProductAttributeCategoryValue: Array<ProductAttributeCategoryValueModel> = [];

  customerValue: any = "customer_none";
  productValue: any = "supplied_product";
  paymentValue: any = -1;

  dataSourceProductAttributeCategory: MatTableDataSource<ProductAttributeCategoryModel>;
  dataSourceCustomerOrderDetail: MatTableDataSource<QuoteDetail>;

  constructor(private el: ElementRef,
    private translate: TranslateService,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    private categoryService: CategoryService,
    private productService: ProductService,
    private vendorSevice: VendorService,
    public dialogRef: MatDialogRef<CreateorderdetaildialogComponent>,
    public snackBar: MatSnackBar,
  ) {
    translate.setDefaultLang('vi');
  }

  // /*Xóa dữ liệu ô Nhà cung cấp*/
  // clearDataVendor() {
  //   this.vendorIdControl.reset();
  //   this.customerOrderDetailEditModel.VendorId = null;
  //   this.customerOrderDetailEditModel.NameVendor = "";
  //   this.filteredProductOptions = this.productIdControl.valueChanges.pipe(
  //     startWith(null),
  //     map((item: string) => item ? this._filterProduct(item, []) : [].slice()));
  //   this.clearDataProduct();
  // }
  // /*End*/

  // /*Xóa dữ liệu ô Sản phẩm/dịch vụ cấp*/
  // clearDataProduct() {
  //   //reset form
  //   this.productIdControl.reset();
  //   this.unitControl.reset();
  //   this.qualityControl.reset();
  //   this.priceControl.reset();
  //   this.moneyUnitControl.reset();
  //   this.exchangeRateControl.reset();
  //   //this.guaranteeTimeControl.reset();
  //   this.vatControl.reset();
  //   this.discountTypeControl.reset();
  //   this.discountValueControl.reset();

  //   //set default value form
  //   this.moneyUnitControl.setValue(this.unitmoney.find(x => x.categoryCode == "VND").categoryId);
  //   this.IsVND = true;
  //   this.exchangeRateControl.setValue(1);
  //   //this.guaranteeTimeControl.setValue(0);
  //   //this.GuaranteeDatetime = '';
  //   this.vatControl.setValue(0);
  //   this.ProductVAT = 0;
  //   this.discountTypeControl.setValue(true);
  //   this.discountValueControl.setValue(0);
  //   this.ProductDiscount = 0;

  //   this.ProductAmount = 0;
  //   //reset model
  //   this.customerOrderDetailEditModel.ProductId = null;
  //   this.customerOrderDetailEditModel.UnitId = null;
  //   this.customerOrderDetailEditModel.Quantity = null;
  //   this.customerOrderDetailEditModel.UnitPrice = null;
  //   this.customerOrderDetailEditModel.CurrencyUnit = this.unitmoney.find(x => x.categoryCode == "VND").categoryId;
  //   this.customerOrderDetailEditModel.NameMoneyUnit = "VND";
  //   this.customerOrderDetailEditModel.ExchangeRate = 1;
  //   //this.customerOrderDetailEditModel.GuaranteeTime = 0;
  //   this.customerOrderDetailEditModel.Vat = 0;
  //   this.customerOrderDetailEditModel.DiscountType = true;
  //   this.customerOrderDetailEditModel.DiscountValue = 0;

  //   this.arrayProductAttributeCategoryModel = []; //list thuộc tính sản phẩm
  //   this.dataSourceProductAttributeCategory = new MatTableDataSource<ProductAttributeCategoryModel>(this.arrayProductAttributeCategoryModel);
  // }
  // /*End*/

  async ngOnInit() {
    // this.setUpControlOnFormGroup();
    // this.getMasterData();
    // this.getListVendor();
    // await this.getListMoneyUnit();

    // if (this.data.isEdit == 'edit') {
    //   this.OldData.QuoteDetailId = this.data.customerOrderDetailModelOutPut.QuoteDetailId;
    //   this.OldData.VendorId = this.data.customerOrderDetailModelOutPut.VendorId;
    //   this.OldData.QuoteId = this.data.customerOrderDetailModelOutPut.QuoteId;
    //   this.OldData.ProductId = this.data.customerOrderDetailModelOutPut.ProductId;
    //   this.OldData.Quantity = this.data.customerOrderDetailModelOutPut.Quantity;
    //   this.OldData.UnitPrice = this.data.customerOrderDetailModelOutPut.UnitPrice;
    //   this.OldData.CurrencyUnit = this.data.customerOrderDetailModelOutPut.CurrencyUnit;
    //   this.OldData.ExchangeRate = this.data.customerOrderDetailModelOutPut.ExchangeRate;
    //   this.OldData.Vat = this.data.customerOrderDetailModelOutPut.Vat;
    //   this.OldData.DiscountType = this.data.customerOrderDetailModelOutPut.DiscountType;
    //   this.OldData.DiscountValue = this.data.customerOrderDetailModelOutPut.DiscountValue;
    //   this.OldData.Description = this.data.customerOrderDetailModelOutPut.Description;
    //   this.OldData.OrderDetailType = this.data.customerOrderDetailModelOutPut.OrderDetailType;
    //   this.OldData.CreatedById = this.data.customerOrderDetailModelOutPut.CreatedById;
    //   this.OldData.CreatedDate = this.data.customerOrderDetailModelOutPut.CreatedDate;
    //   this.OldData.UpdatedById = this.data.customerOrderDetailModelOutPut.UpdatedById;
    //   this.OldData.UpdatedDate = this.data.customerOrderDetailModelOutPut.UpdatedDate;
    //   this.OldData.Active = this.data.customerOrderDetailModelOutPut.Active;
    //   this.OldData.UnitId = this.data.customerOrderDetailModelOutPut.UnitId;
    //   this.OldData.IncurredUnit = this.data.customerOrderDetailModelOutPut.IncurredUnit;
    //   this.OldData.QuoteProductDetailProductAttributeValue = this.data.customerOrderDetailModelOutPut.QuoteProductDetailProductAttributeValue;
    //   this.OldData.ExplainStr = this.data.customerOrderDetailModelOutPut.ExplainStr;
    //   this.OldData.NameVendor = this.data.customerOrderDetailModelOutPut.NameVendor;
    //   this.OldData.ProductNameUnit = this.data.customerOrderDetailModelOutPut.ProductNameUnit;
    //   this.OldData.NameMoneyUnit = this.data.customerOrderDetailModelOutPut.NameMoneyUnit;
    //   this.OldData.SumAmount = this.data.customerOrderDetailModelOutPut.SumAmount;
    //   //let CV = this.data.customerOrderDetailModelOutPut;
    //   //this.data.customerOrderDetailModelOutPut
    //   this.customerOrderDetailEditModel = this.OldData;

    //   if (this.data.customerOrderDetailModelOutPut.OrderDetailType == 0) {
    //     this.vendorIdControl.setValue(this.data.customerOrderDetailModelOutPut.NameVendor);
    //     this.getListProduct(this.data.customerOrderDetailModelOutPut.VendorId);
    //     this.productValue = 'supplied_product';
    //     this.productCaculateAmount();
    //     this.productCaculateVAT();
    //     this.productCaculateDiscount();
    //   }
    //   else {
    //     this.productValue = 'incured_product';
    //     this.IncuredAmountCaculateAmount();
    //     this.IncuredAmountCaculateVAT();
    //     this.IncuredAmountCaculateDiscount();
    //   }
    //   this.checkProductForm.get('moneyUnitControl').setValue(this.data.customerOrderDetailModelOutPut.CurrencyUnit);
    //   this.checkIncuredForm.get('moneyIncuredUnitControl').setValue(this.data.customerOrderDetailModelOutPut.CurrencyUnit);
    //   if (this.data.customerOrderDetailModelOutPut.NameMoneyUnit == 'VND') {
    //     this.IsVND = true;
    //     this.exchangeRateControl.disable();
    //     this.exchangeRateIncuredControl.disable();

    //   }
    //   else {
    //     this.IsVND = false;
    //     this.exchangeRateControl.enable();
    //     this.exchangeRateIncuredControl.enable();
    //   }
    // } else {
    //   this.setDefaultVaule();
    // }
  }

  // setUpControlOnFormGroup() {
  //   this.vendorIdControl = new FormControl('', [Validators.required]);
  //   this.productIdControl = new FormControl('', [Validators.required]);
  //   this.qualityControl = new FormControl('', [Validators.required, Validators.min(0.00000000001)]);
  //   this.priceControl = new FormControl('', [Validators.required, Validators.min(0)]);
  //   this.unitControl = new FormControl('', [Validators.required]);
  //   this.moneyUnitControl = new FormControl('', [Validators.required]);
  //   this.exchangeRateControl = new FormControl('', [Validators.required, Validators.min(0.00000000001)]);
  //   this.discountTypeControl = new FormControl('');
  //   this.discountValueControl = new FormControl('', [Validators.min(0)]);
  //   this.vatControl = new FormControl('', [Validators.min(0)]);

  //   this.descriptionProductControl = new FormControl('', [Validators.required]);
  //   this.qualityIncuredControl = new FormControl('', [Validators.required, Validators.min(0.00000000001)]);
  //   this.priceIncuredControl = new FormControl('', [Validators.required, Validators.min(0)]);
  //   this.vatIncuredControl = new FormControl('', [Validators.min(0)]);
  //   this.unitIncuredControl = new FormControl('');
  //   this.moneyIncuredUnitControl = new FormControl('', [Validators.required]);
  //   this.exchangeRateIncuredControl = new FormControl('', [Validators.required, Validators.min(0.00000000001)]);
  //   this.discountTypeIncuredControl = new FormControl('');
  //   this.discountValueIncuredControl = new FormControl('', [Validators.min(0)]);

  //   this.informationProductForm = new FormGroup({
  //   });
  //   this.checkProductForm = new FormGroup({
  //     vendorIdControl: this.vendorIdControl,
  //     productIdControl: this.productIdControl,
  //     qualityControl: this.qualityControl,
  //     priceControl: this.priceControl,
  //     unitControl: this.unitControl,
  //     moneyUnitControl: this.moneyUnitControl,
  //     exchangeRateControl: this.exchangeRateControl,
  //     discountTypeControl: this.discountTypeControl,
  //     discountValueControl: this.discountValueControl,
  //     vatControl: this.vatControl,
  //   });

  //   this.checkIncuredForm = new FormGroup({
  //     descriptionProductControl: this.descriptionProductControl,
  //     qualityIncuredControl: this.qualityIncuredControl,
  //     priceIncuredControl: this.priceIncuredControl,
  //     vatIncuredControl: this.vatIncuredControl,
  //     unitIncuredControl: this.unitIncuredControl,
  //     moneyIncuredUnitControl: this.moneyIncuredUnitControl,
  //     exchangeRateIncuredControl: this.exchangeRateIncuredControl,
  //     discountTypeIncuredControl: this.discountTypeIncuredControl,
  //     discountValueIncuredControl: this.discountValueIncuredControl
  //   });
  // }

  // getMasterData() {
  //   /**Get Product Unit */
  //   this.categoryService.getAllCategoryByCategoryTypeCode('DNH').subscribe(response => {
  //     let result = <any>response;
  //     this.listunit = result.category;
  //   }, error => { });
  //   this.categoryService.getAllCategoryByCategoryTypeCode('PTO').subscribe(response => {
  //     let result = <any>response;
  //     this.listPaymentMethod = result.category;

  //   }, error => { });

  // }

  // setDefaultVaule() {
  //   /*Unit Money */
  //   const toSelectMoneyUnit = this.unitmoney.find(c => c.categoryCode == 'VND');
  //   this.checkProductForm.get('moneyUnitControl').setValue(toSelectMoneyUnit.categoryId);
  //   this.checkIncuredForm.get('moneyIncuredUnitControl').setValue(toSelectMoneyUnit.categoryId);
  //   this.customerOrderDetailEditModel.NameMoneyUnit = 'VND';
  //   this.exchangeRateControl.disable();
  //   this.exchangeRateIncuredControl.disable();
  //   this.IsVND = true;
  // }
  // /*Money Unit */
  // async getListMoneyUnit() {
  //   var result: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc('DTI');
  //   this.unitmoney = result.category;
  // }

  // //Autocomplete VEndor
  // selectedVendorFn(event: MatAutocompleteSelectedEvent): void {
  //   // Disappear sp/dv khi thay doi nha cung cap
  //   if (event.option.value !== this.customerOrderDetailEditModel.VendorId) {
  //     this.productIdControl.setValue(null);
  //   }
  //   this.vendorIdControl.setValue(event.option.viewValue);
  //   this.customerOrderDetailEditModel.VendorId = event.option.value;
  //   this.customerOrderDetailEditModel.NameVendor = event.option.viewValue;
  //   this.getListProduct(event.option.value);
  // }

  // private _filterVendor(value: any, array: any): string[] {
  //   return array.filter(vendor =>
  //     vendor.vendorId.toLowerCase().indexOf(value.toLowerCase()) >= 0 || vendor.vendorName.toLowerCase().indexOf(value.toLowerCase()) >= 0);
  // }
  // getListVendor() {
  //   this.vendorSevice.searchVendor("", "", this.listCategoryId, this.auth.UserId).subscribe(response => {
  //     let result = <any>response;
  //     this.listVendor = result.vendorList;
  //     this.filteredVendorOptions = this.vendorIdControl.valueChanges.pipe(
  //       startWith(null),
  //       map((item: string) => item ? this._filterVendor(item, this.listVendor) : this.listVendor.slice()));

  //   }, error => { });
  // }
  // //End Autocomplete Vendor
  // //AutoComplete Product Unit
  // private _filterProductUnit(value: string, array: any) {
  //   return array.filter(state =>
  //     state.name.toLowerCase().indexOf(value.toLowerCase()) >= 0 || state.categoryId.toLowerCase().indexOf(value.toLowerCase()) >= 0);
  // }
  // //End Product Unit
  // //Autocomplete Product
  // selectedProductFn(event: MatAutocompleteSelectedEvent): void {
  //   this.productIdControl.setValue(event.option.viewValue);
  //   this.customerOrderDetailEditModel.ProductId = event.option.value;
  //   var productObject = this.listProductByVendor.filter(product => product.productId.toLowerCase().indexOf(event.option.value.toLowerCase()) >= 0);
  //   this.priceControl.setValue(productObject[0].price1);
  //   this.qualityControl.setValue(null);
  //   this.customerOrderDetailEditModel.Quantity = null;
  //   //Set up for Unit Product
  //   //this.customerOrderDetailModel.UnitId = productObject[0].productUnitId;
  //   var unitObject = this.listunit.filter(unit => unit.categoryId.toLowerCase().indexOf(productObject[0].productUnitId.toLowerCase()) >= 0);
  //   this.unitControl.setValue(unitObject[0].categoryId);
  //   this.customerOrderDetailEditModel.ProductNameUnit = unitObject[0].categoryName;
  //   this.customerOrderDetailEditModel.ExplainStr = event.option.viewValue + '()';
  //   //End Unit Product
  //   //Set up Grid Product Attribute
  //   this.arrayProductAttributeCategoryModel = [];
  //   this.arrayOrderProductDetailProductAttributeValueModel = [];
  //   this.productService.getProductAttributeByProductID(event.option.value).subscribe(response => {
  //     let result = <any>response;
  //     if (result.lstProductAttributeCategory !== null) {
  //       for (var x = 0; x < result.lstProductAttributeCategory.length; x++) {

  //         for (var j = 0; j < result.lstProductAttributeCategory[x].productAttributeCategoryValue.length; j++) {
  //           var objectPush = result.lstProductAttributeCategory[x].productAttributeCategoryValue[j];

  //           var item: ProductAttributeCategoryValueModel = {
  //             ProductAttributeCategoryValueId: objectPush.productAttributeCategoryValueId,
  //             ProductAttributeCategoryValue1: objectPush.productAttributeCategoryValue1,
  //             CreatedById: objectPush.createdById,
  //             CreatedDate: objectPush.createdDate,
  //             ProductAttributeCategoryId: objectPush.productAttributeCategoryId,
  //             UpdatedById: objectPush.updatedById,
  //             UpdatedDate: objectPush.updatedDate,
  //             Active: objectPush.active
  //           };
  //           this.arrayProductAttributeCategoryValue.push(item);
  //         }
  //         var productAttributeCategory: ProductAttributeCategoryModel = {
  //           ProductAttributeCategoryId: result.lstProductAttributeCategory[x].productAttributeCategoryId,
  //           ProductAttributeCategoryName: result.lstProductAttributeCategory[x].productAttributeCategoryName,
  //           ValueString: '',
  //           CreatedById: result.lstProductAttributeCategory[x].createdById,
  //           CreatedDate: result.lstProductAttributeCategory[x].createdDate,
  //           ProductAttributeCategoryValue: this.arrayProductAttributeCategoryValue,
  //           UpdatedDate: result.lstProductAttributeCategory[x].updatedDate,
  //           UpdatedById: result.lstProductAttributeCategory[x].updatedById,
  //           Active: result.lstProductAttributeCategory[x].active,
  //           DefaultValue: this.emptyGuid
  //         }
  //         var orderProductDetailProductAttributeValue: QuoteProductDetailProductAttributeValue = {
  //           QuoteDetailId: this.emptyGuid,
  //           QuoteProductDetailProductAttributeValueId: this.emptyGuid,
  //           ProductAttributeCategoryId: result.lstProductAttributeCategory[x].productAttributeCategoryId,
  //           ProductAttributeCategoryValueId: this.emptyGuid,
  //           ProductId: this.customerOrderDetailEditModel.ProductId,
  //         };
  //         this.arrayProductAttributeCategoryModel.push(productAttributeCategory);
  //         this.arrayOrderProductDetailProductAttributeValueModel.push(orderProductDetailProductAttributeValue);
  //         this.arrayProductAttributeCategoryValue = [];
  //       };

  //     }
  //     this.dataSourceProductAttributeCategory = new MatTableDataSource<ProductAttributeCategoryModel>(this.arrayProductAttributeCategoryModel);

  //   }, error => { });
  //   //End Grid Product Attribute
  // }

  // private _filterProduct(value: any, array: any): string[] {
  //   return array.filter(vendor =>
  //     vendor.productId.toLowerCase().indexOf(value.toLowerCase()) >= 0 || vendor.productName.toLowerCase().indexOf(value.toLowerCase()) >= 0);
  // }

  // getListProduct(VendorID: string) {
  //   this.productService.getProductByVendorID(VendorID).subscribe(response => {
  //     let result = <any>response;
  //     this.listProductByVendor = result.lstProduct;
  //     if (this.data.isEdit == 'edit') {
  //       const toSelectProduct = this.listProductByVendor.find(c => c.productId == this.data.customerOrderDetailModelOutPut.ProductId);
  //       if (typeof toSelectProduct !== "undefined") {
  //         if (toSelectProduct != null) {
  //           this.productIdControl.setValue(toSelectProduct.productName);
  //         }
  //       }
  //       this.productService.getProductAttributeByProductID(this.data.customerOrderDetailModelOutPut.ProductId).subscribe(response => {
  //         let result = <any>response;
  //         if (result.lstProductAttributeCategory != null) {
  //           for (var x = 0; x < result.lstProductAttributeCategory.length; x++) {
  //             for (var j = 0; j < result.lstProductAttributeCategory[x].productAttributeCategoryValue.length; j++) {
  //               var objectPush = result.lstProductAttributeCategory[x].productAttributeCategoryValue[j];

  //               var item: ProductAttributeCategoryValueModel = {
  //                 ProductAttributeCategoryValueId: objectPush.productAttributeCategoryValueId,
  //                 ProductAttributeCategoryValue1: objectPush.productAttributeCategoryValue1,
  //                 CreatedById: objectPush.createdById,
  //                 CreatedDate: objectPush.createdDate,
  //                 ProductAttributeCategoryId: objectPush.productAttributeCategoryId,
  //                 UpdatedById: objectPush.updatedById,
  //                 UpdatedDate: objectPush.updatedDate,
  //                 Active: objectPush.active
  //               };
  //               this.arrayProductAttributeCategoryValue.push(item);
  //             }
  //             var findDefaultValueAttribute = this.data.customerOrderDetailModelOutPut.QuoteProductDetailProductAttributeValue.find(item => item.ProductAttributeCategoryId == result.lstProductAttributeCategory[x].productAttributeCategoryId);
  //             var productAttributeCategory: ProductAttributeCategoryModel = {
  //               ProductAttributeCategoryId: result.lstProductAttributeCategory[x].productAttributeCategoryId,
  //               ProductAttributeCategoryName: result.lstProductAttributeCategory[x].productAttributeCategoryName,
  //               ValueString: '',
  //               CreatedById: result.lstProductAttributeCategory[x].createdById,
  //               CreatedDate: result.lstProductAttributeCategory[x].createdDate,
  //               ProductAttributeCategoryValue: this.arrayProductAttributeCategoryValue,
  //               UpdatedDate: result.lstProductAttributeCategory[x].updatedDate,
  //               UpdatedById: result.lstProductAttributeCategory[x].updatedById,
  //               Active: result.lstProductAttributeCategory[x].active,
  //               DefaultValue: findDefaultValueAttribute.ProductAttributeCategoryValueId,
  //             }
  //             var orderProductDetailProductAttributeValue: QuoteProductDetailProductAttributeValue = {
  //               QuoteDetailId: this.emptyGuid,
  //               QuoteProductDetailProductAttributeValueId: this.emptyGuid,
  //               ProductAttributeCategoryId: result.lstProductAttributeCategory[x].productAttributeCategoryId,
  //               ProductAttributeCategoryValueId: findDefaultValueAttribute.ProductAttributeCategoryValueId,
  //               ProductId: this.customerOrderDetailEditModel.ProductId,
  //             };
  //             this.arrayProductAttributeCategoryModel.push(productAttributeCategory);
  //             this.arrayOrderProductDetailProductAttributeValueModel.push(orderProductDetailProductAttributeValue);
  //             this.arrayProductAttributeCategoryValue = [];
  //           };
  //         }
  //         this.dataSourceProductAttributeCategory = new MatTableDataSource<ProductAttributeCategoryModel>(this.arrayProductAttributeCategoryModel);

  //       }, error => { });
  //     }
  //     this.filteredProductOptions = this.productIdControl.valueChanges.pipe(
  //       startWith(null),
  //       map((item: string) => item ? this._filterProduct(item, this.listProductByVendor) : this.listProductByVendor.slice()));

  //   }, error => { });
  // }
  // //End Autocomplete Product
  // changeTablist(event: MatRadioChange, code: any) {
  //   this.incuredUseCurrencyMask = false;
  //   this.UseCurrencyMask = false;
  //   if (code === "product") {
  //     this.checkProductForm.reset();
  //     this.checkIncuredForm.reset();
  //     this.productValue = event.value;
  //   }
  //   else {
  //     this.checkIncuredForm.reset();
  //     this.checkProductForm.reset();
  //     this.customerValue = event.value;
  //   }

  //   this.restartCustomerOrderDetailModel();
  //   this.arrayOrderProductDetailProductAttributeValueModel = [];
  //   this.arrayProductAttributeCategoryModel = [];

  //   this.dataSourceProductAttributeCategory = new MatTableDataSource<ProductAttributeCategoryModel>(this.arrayProductAttributeCategoryModel);

  //   this.checkProductForm.get("vendorIdControl").setValue('');
  //   this.checkProductForm.get("productIdControl").setValue('');
  //   this.checkProductForm.get("unitControl").setValue('');

  //   this.ProductAmount = 0;
  //   this.ProductVAT = 0;
  //   this.ProductDiscount = 0;
  //   //this.ProductMoneyUnit = null;
  //   this.IncuredAmount = 0;
  //   this.IncuredVAT = 0;
  //   this.IncuredDiscount = 0;
  //   this.IncuredMoneyUnit = null;
  //   if (code === "product") {
  //     const toSelectMoneyUnit = this.unitmoney.find(c => c.categoryCode == 'VND');
  //     this.checkProductForm.get('moneyUnitControl').setValue(toSelectMoneyUnit.categoryId);
  //     this.checkIncuredForm.get('moneyIncuredUnitControl').setValue(toSelectMoneyUnit.categoryId);
  //     this.customerOrderDetailEditModel.NameMoneyUnit = 'VND';
  //     this.IsVND = true;
  //   }
  //   this.customerOrderDetailEditModel.ExchangeRate = 1;
  // }

  // restartCustomerOrderDetailModel() {
  //   this.customerOrderDetailEditModel = new QuoteDetail();
  // }

  // selectedUnitmoneyFn(event: any) {
  //   let target = event.source.selected._element.nativeElement;
  //   let selectedData = {
  //     value: event.value,
  //     text: target.innerText.trim()
  //   };
  //   //this.ProductMoneyUnit = 'VND';
  //   this.customerOrderDetailEditModel.NameMoneyUnit = selectedData.text;
  //   if (selectedData.text === 'VND') {
  //     this.customerOrderDetailEditModel.ExchangeRate = 1;
  //     this.exchangeRateControl.disable();
  //     this.exchangeRateIncuredControl.disable();
  //     this.IsVND = true;
  //   }
  //   else {
  //     this.exchangeRateControl.enable();
  //     this.exchangeRateIncuredControl.enable();
  //     this.IsVND = false;
  //   }
  // }
  // //Product Amount
  // productCaculateAmount() {
  //   this.ProductAmount = this.customerOrderDetailEditModel.UnitPrice * this.customerOrderDetailEditModel.Quantity * this.customerOrderDetailEditModel.ExchangeRate;
  //   this.customerOrderDetailEditModel.SumAmount = this.ProductAmount;
  //   if (this.customerOrderDetailEditModel.Vat !== null && this.customerOrderDetailEditModel.Vat !== 0) {
  //     this.ProductVAT = (this.ProductAmount * this.customerOrderDetailEditModel.Vat) / 100;
  //   }
  // }
  // productCaculateExchangerate() {
  //   this.ProductAmount = this.customerOrderDetailEditModel.UnitPrice * this.customerOrderDetailEditModel.Quantity * this.customerOrderDetailEditModel.ExchangeRate;
  //   this.customerOrderDetailEditModel.SumAmount = this.ProductAmount;
  //   if (this.customerOrderDetailEditModel.Vat !== null && this.customerOrderDetailEditModel.Vat !== 0) {
  //     this.ProductVAT = (this.ProductAmount * this.customerOrderDetailEditModel.Vat) / 100;
  //   }
  // }
  // productCaculateVAT() {
  //   this.ProductVAT = (this.ProductAmount * this.customerOrderDetailEditModel.Vat) / 100;
  // }
  // productCaculateDiscount() {
  //   if (this.customerOrderDetailEditModel.DiscountType) {
  //     if (this.ProductAmount != null && this.ProductAmount > 0) {
  //       this.ProductDiscount = (this.ProductAmount * <number>this.customerOrderDetailEditModel.DiscountValue) / 100;
  //     }
  //   }
  //   else {
  //     this.ProductDiscount = <number>this.customerOrderDetailEditModel.DiscountValue;
  //   }
  // }
  // //IncuredAmount
  // IncuredAmountCaculateAmount() {
  //   this.IncuredAmount = this.customerOrderDetailEditModel.UnitPrice * this.customerOrderDetailEditModel.Quantity * this.customerOrderDetailEditModel.ExchangeRate;
  //   this.customerOrderDetailEditModel.SumAmount = this.IncuredAmount;
  //   if (this.customerOrderDetailEditModel.Vat !== null && this.customerOrderDetailEditModel.Vat !== 0) {
  //     this.IncuredVAT = (this.IncuredAmount * this.customerOrderDetailEditModel.Vat) / 100;
  //   }
  // }
  // IncuredCaculateExchangerate() {
  //   this.IncuredAmount = this.customerOrderDetailEditModel.UnitPrice * this.customerOrderDetailEditModel.Quantity * this.customerOrderDetailEditModel.ExchangeRate;
  //   this.customerOrderDetailEditModel.SumAmount = this.IncuredAmount;
  //   if (this.customerOrderDetailEditModel.Vat !== null && this.customerOrderDetailEditModel.Vat !== 0) {
  //     this.IncuredVAT = (this.IncuredAmount * this.customerOrderDetailEditModel.Vat) / 100;
  //   }
  // }
  // IncuredAmountCaculateVAT() {
  //   this.IncuredVAT = (this.IncuredAmount * this.customerOrderDetailEditModel.Vat) / 100;
  // }

  // IncuredAmountCaculateExchangerate() {

  //   this.IncuredAmount = this.customerOrderDetailEditModel.UnitPrice * this.customerOrderDetailEditModel.Quantity * this.customerOrderDetailEditModel.ExchangeRate;
  // }
  // IncuredAmountCaculateDiscount() {
  //   if (this.customerOrderDetailEditModel.DiscountType) {
  //     this.IncuredDiscount = (this.IncuredAmount * this.customerOrderDetailEditModel.DiscountValue) / 100;
  //   }
  //   else {
  //     this.IncuredDiscount = this.customerOrderDetailEditModel.DiscountValue;
  //   }
  // }
  // selectedDiscountTypeList(event: any) {
  //   this.UseCurrencyMask = !event.value;
  //   var value = event.value;
  //   this.customerOrderDetailEditModel.DiscountValue =0;
  //   if (this.customerOrderDetailEditModel.DiscountValue != null && this.customerOrderDetailEditModel.DiscountValue != 0) {
  //     if (value) {
  //       this.ProductDiscount = (this.ProductAmount * <number>this.customerOrderDetailEditModel.DiscountValue) / 100;
  //     } else {
  //       this.ProductDiscount = <number>this.customerOrderDetailEditModel.DiscountValue;
  //     }
  //   }
  // }
  // selectedIncuredDiscountTypeList(event: any) {
  //   this.incuredUseCurrencyMask = !event.value;
  //   var value = event.value;
  //   this.customerOrderDetailEditModel.DiscountValue = 0;
  //   if (this.customerOrderDetailEditModel.DiscountValue != null && this.customerOrderDetailEditModel.DiscountValue != 0) {
  //     if (value) {
  //       this.IncuredDiscount = (this.ProductAmount * <number>this.customerOrderDetailEditModel.DiscountValue) / 100;
  //     } else {
  //       this.IncuredDiscount = <number>this.customerOrderDetailEditModel.DiscountValue;
  //     }
  //   }
  // }
  // pushCustomerOrderDetail() {

  //   if (this.productValue === 'supplied_product') {

  //     if (!this.checkProductForm.valid) {
  //       Object.keys(this.checkProductForm.controls).forEach(key => {
  //         if (this.checkProductForm.controls[key].valid === false) {
  //           this.checkProductForm.controls[key].markAsTouched();
  //         }
  //       });
  //       let target;
  //       target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

  //       if (target) {
  //         $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
  //         target.focus();
  //       }
  //     } else {

  //       //if (this.ProductAmount !== null) {

  //       var filterEmptyGuid = this.arrayOrderProductDetailProductAttributeValueModel.filter(item => item.ProductAttributeCategoryValueId.toLocaleLowerCase().indexOf(this.emptyGuid.toLocaleLowerCase()) >= 0);
  //       if (filterEmptyGuid.length == 0) {
  //         this.customerOrderDetailEditModel.QuoteProductDetailProductAttributeValue = this.arrayOrderProductDetailProductAttributeValueModel;
  //         this.customerOrderDetailEditModel.OrderDetailType = 0;
  //         //TInh tong tien cho tung chi tiet don hang
  //         this.customerOrderDetailEditModel.SumAmount = this.ProductAmount + this.ProductVAT - this.ProductDiscount;

  //         this.data.productValue = 'supplied_product';
  //         this.data.customerOrderDetailModel = this.customerOrderDetailEditModel;
  //         this.data.isTrue = true;

  //         this.checkProductForm.markAsPristine();
  //         this.checkProductForm.markAsUntouched();

  //         this.restartCustomerOrderDetailModel();

  //         this.arrayOrderProductDetailProductAttributeValueModel = [];
  //         this.arrayProductAttributeCategoryModel = [];

  //         this.dataSourceProductAttributeCategory = new MatTableDataSource<ProductAttributeCategoryModel>(this.arrayProductAttributeCategoryModel);

  //         this.checkProductForm.get("vendorIdControl").setValue('');
  //         this.checkProductForm.get("productIdControl").setValue('');
  //         this.checkProductForm.get("unitControl").setValue('');

  //         this.ProductAmount = 0;
  //         this.ProductVAT = 0;
  //         this.ProductDiscount = 0;
  //         //this.ProductMoneyUnit = null;
  //         this.IncuredAmount = 0;
  //         this.IncuredVAT = 0;
  //         this.IncuredDiscount = 0;
  //         this.IncuredMoneyUnit = null;
  //         /*Unit Money */
  //         const toSelectMoneyUnit = this.unitmoney.find(c => c.categoryCode == 'VND');
  //         this.checkProductForm.get('moneyUnitControl').setValue(toSelectMoneyUnit.categoryId);
  //         this.checkIncuredForm.get('moneyIncuredUnitControl').setValue(toSelectMoneyUnit.categoryId);
  //         this.customerOrderDetailEditModel.NameMoneyUnit = 'VND';
  //         this.dialogRef.close(this.data);
  //       }
  //       else {
  //         var id = '#' + filterEmptyGuid[0].ProductAttributeCategoryId;
  //         $(id).css({
  //           "border-color": "#f44336",
  //           "border-width": "1px",
  //           "border-style": "solid"
  //         });
  //       }
  //       //}
  //     }
  //   }
  //   else {
  //     if (!this.checkIncuredForm.valid) {
  //       Object.keys(this.checkIncuredForm.controls).forEach(key => {
  //         if (this.checkIncuredForm.controls[key].valid === false) {
  //           this.checkIncuredForm.controls[key].markAsTouched();
  //         }
  //       });
  //       let target;
  //       target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

  //       if (target) {
  //         $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
  //         target.focus();
  //       }
  //     } else {

  //       if (this.IncuredAmount !== null) {
  //         this.customerOrderDetailEditModel.OrderDetailType = 1;
  //         //TInh tong tien cho tung chi tiet don hang
  //         this.customerOrderDetailEditModel.SumAmount = this.IncuredAmount + this.IncuredVAT - this.IncuredDiscount;

  //         this.data.productValue = 'incured_product';
  //         this.data.customerOrderDetailModel = this.customerOrderDetailEditModel;
  //         this.data.isTrue = true;

  //         this.checkIncuredForm.markAsPristine();
  //         this.checkIncuredForm.markAsUntouched();

  //         this.restartCustomerOrderDetailModel();
  //         this.arrayOrderProductDetailProductAttributeValueModel = [];
  //         this.arrayProductAttributeCategoryModel = [];

  //         this.dataSourceProductAttributeCategory = new MatTableDataSource<ProductAttributeCategoryModel>(this.arrayProductAttributeCategoryModel);
  //         this.checkProductForm.get("vendorIdControl").setValue('');
  //         this.checkProductForm.get("productIdControl").setValue('');
  //         this.checkProductForm.get("unitControl").setValue('');

  //         this.ProductAmount = 0;
  //         this.ProductVAT = 0;
  //         this.ProductDiscount = 0;
  //         //this.ProductMoneyUnit = null;
  //         this.IncuredAmount = 0;
  //         this.IncuredVAT = 0;
  //         this.IncuredDiscount = 0;
  //         this.IncuredMoneyUnit = null;
  //         /*Unit Money */
  //         const toSelectMoneyUnit = this.unitmoney.find(c => c.categoryCode == 'VND');
  //         this.checkProductForm.get('moneyUnitControl').setValue(toSelectMoneyUnit.categoryId);
  //         this.checkIncuredForm.get('moneyIncuredUnitControl').setValue(toSelectMoneyUnit.categoryId);
  //         this.customerOrderDetailEditModel.NameMoneyUnit = 'VND';
  //         this.dialogRef.close(this.data);
  //       }
  //     }
  //   }
  // }

  // selectedProductAttributeCategoryValueFn(event: any, productAttributeCategoryId: any) {
  //   let target = event.source.selected._element.nativeElement;
  //   let selectedData = {
  //     value: event.value,
  //     text: target.innerText.trim()
  //   };

  //   var filter = this.arrayOrderProductDetailProductAttributeValueModel.filter(item => item.ProductAttributeCategoryId.toLocaleLowerCase().indexOf(productAttributeCategoryId.toLocaleLowerCase()) >= 0);
  //   filter[0].ProductAttributeCategoryValueId = event.value;
  //   this.customerOrderDetailEditModel.ExplainStr = this.customerOrderDetailEditModel.ExplainStr.replace(')', selectedData.text + ';)');

  //   var id = '#' + productAttributeCategoryId;
  //   $(id).css({
  //     "border-color": "none",
  //     "border-width": "none",
  //     "border-style": "none"
  //   });
  // }

  // onCancelClick() {
  //   this.data.isTrue = false;
  //   if (this.data.isEdit === 'edit') {
  //     this.data.customerOrderDetailModel = this.OldData;
  //     //this.data.customerOrderDetailModelOutPut = this.OldData;
  //   }
  //   this.dialogRef.close(this.data);
  // }
  // changeUnit(event: any) {
  //   let target = event.source.selected.viewValue;
  //   let selectedData = {
  //     value: event.value,
  //     text: target
  //   };
  //   this.customerOrderDetailEditModel.ProductNameUnit = selectedData.text;
  // }

  // getDefaultNumberType() {
  //   return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  // }

  // onKeyPress(event: any) {
  //   const pattern = /^[0-9\.]$/;
  //   const inputChar = String.fromCharCode(event.charCode);
  //   if (!pattern.test(inputChar)) {
  //     event.preventDefault();
  //   }
  // }

}

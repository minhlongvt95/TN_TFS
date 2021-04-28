import { Component, OnInit, ViewChild, ElementRef, Inject, AfterViewInit, AfterContentChecked, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { MatRadioChange } from '@angular/material/radio';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

import { FormControl, Validators, FormGroup } from '@angular/forms';
import * as $ from 'jquery';
import { EmployeeService } from '../../../employee/services/employee.service';
import { CategoryService } from "../../../shared/services/category.service";
import { VendorOrderDetailModel } from '../../models/vendorOrderDetail.model';
import { VendorOrderProductDetailProductAttributeValueModel } from '../../models/vendorOrderProductDetailProductAttributeValue.model';
import { ProductAttributeCategoryModel, ProductAttributeCategoryValueModel } from '../../../product/models/productAttributeCategory.model';
import { VendorService } from "../../services/vendor.service";
import { CustomerOrderService } from '../../../order/services/customer-order.service';
import { ProductService } from '../../../product/services/product.service';
import { SelectionModel } from '@angular/cdk/collections';
import { ContactService } from '../../../shared/services/contact.service';
import { OrderstatusService } from '../../../shared/services/orderstatus.service';

export interface IDialogData {
  title: string;
  vendorId: string;
  vendorName: string;
  ok: boolean;
  mode: string;
  vendorOrderDetail: VendorOrderDetailModel;
  productVAT: number;
  productDiscount: number;
  productAmount: number;
  arrayProductAttr: Array<VendorOrderProductDetailProductAttributeValueModel>;
  oldAmount: number;
}

@Component({
  selector: 'app-vendor-order-product',
  templateUrl: './vendor-order-product.component.html',
  styleUrls: ['./vendor-order-product.component.css']
})
export class VendorOrderProductComponent implements OnInit, AfterContentChecked {
  //get system parameter
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();

  useCurrencyMask = false;
  useCurrencyMaskNo = false;
  informationProductForm: FormGroup;
  productIdControl: FormControl;
  unitControl: FormControl;
  moneyUnitControl: FormControl;
  quantityControl: FormControl;
  priceControl: FormControl;
  exchangeRateControl: FormControl;
  discountTypeControl: FormControl;
  discountValueControl: FormControl;
  vatControl: FormControl;
  descriptionProductControl: FormControl;
  quantityIncuredControl: FormControl;
  priceIncuredControl: FormControl;
  vatIncuredControl: FormControl;
  unitIncuredControl: FormControl;
  moneyIncuredUnitControl: FormControl;
  exchangeRateIncuredControl: FormControl;
  discountTypeIncuredControl: FormControl;
  discountValueIncuredControl: FormControl;

  internalProductForm: FormGroup;

  isVnd: boolean = true;
  DiscountTypeList: Array<any> = [{ "name": "Theo %", "value": true }, { "name": "Số tiền", "value": false }];
  displayedAttributeColumns = ['nameattribute', 'value'];
  productValue: any = "supplied_product";
  vendorValue: any = "vendor_none";
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  ProductAmount: number = 0;
  ProductVAT: number = 0;
  ProductDiscount: number = 0;
  ProductMoneyUnit: string = null;
  //unitName: string = '';

  IncuredAmount: number = 0;
  IncuredVAT: number = 0;
  IncuredDiscount: number = 0;
  IncuredMoneyUnit: string = null;

  listProductByVendor: Array<any> = [];
  listunit: Array<any> = [];
  unitmoney: Array<any> = [];
  readonly oldData: VendorOrderDetailModel = {
    id: 0,
    vendorOrderDetailId: this.emptyGuid,
    vendorId: this.data.vendorId,
    vendorOrderId: this.emptyGuid,
    productId: this.emptyGuid,
    quantity: null,
    unitPrice: null,
    currencyUnit: '',
    exchangeRate: null,
    vat: 0,
    discountType: true,
    discountValue: null,
    description: '',
    orderDetailType: 1,
    createdById: this.auth.UserId,
    createdDate: new Date(),
    updatedById: '',
    updatedDate: null,
    active: true,
    unitId: '',
    vendorOrderProductDetailProductAttributeValue: null,
    explainStr: '',
    nameVendor: '',
    productNameUnit: '',
    nameMoneyUnit: 'VND',
    sumAmount: 0,
    incurredUnit: '',
    cost: 0,
    currencyUnitName: '',
    isReshow: null,
    listVendorOrderProductDetailProductAttributeValue: [],
    priceValueWarehouse: 0,
    priceWarehouse: 0,
    procurementCode: '',
    procurementRequestId: '',
    procurementRequestItemId: '',
    productName: '',
    productUnitId: '',
    selectedAttributeName: '',
    sumAmountDiscount: 0,
    sumAmountVat: 0,
    unitName: '',
    vendorName: '',
    isEditCost: null,
    isInvalidItemRequest: false,
    orderNumber: 0,
  };
  oldAmount: number;
  vendorOrderDetailModel: VendorOrderDetailModel = {
    id: 0,
    vendorOrderDetailId: this.emptyGuid,
    vendorId: this.data.vendorId,
    vendorOrderId: this.emptyGuid,
    productId: this.emptyGuid,
    quantity: null,
    unitPrice: null,
    currencyUnit: '',
    exchangeRate: 1,
    vat: 0,
    discountType: true,
    discountValue: 0,
    description: '',
    orderDetailType: 1,
    createdById: this.auth.UserId,
    createdDate: new Date(),
    updatedById: '',
    updatedDate: null,
    active: true,
    unitId: '',
    vendorOrderProductDetailProductAttributeValue: null,
    explainStr: '',
    nameVendor: '',
    productNameUnit: '',
    nameMoneyUnit: 'VND',
    sumAmount: 0,
    incurredUnit: '',
    cost: 0,
    currencyUnitName: '',
    isReshow: null,
    listVendorOrderProductDetailProductAttributeValue: [],
    priceValueWarehouse: 0,
    priceWarehouse: 0,
    procurementCode: '',
    procurementRequestId: '',
    procurementRequestItemId: '',
    productName: '',
    productUnitId: '',
    selectedAttributeName: '',
    sumAmountDiscount: 0,
    sumAmountVat: 0,
    unitName: '',
    vendorName: '',
    isEditCost: null,
    isInvalidItemRequest: false,
    orderNumber: 0,
  };
  vendorOrderProductDetailProductAttributeValueModel: VendorOrderProductDetailProductAttributeValueModel = {
    vendorOrderDetailId: this.emptyGuid,
    productId: '',
    productAttributeCategoryId: '',
    productAttributeCategoryValueId: '',
    vendorOrderProductDetailProductAttributeValueId: this.emptyGuid,
    orderProductDetailProductAttributeValueId: '',
    productAttributeCategoryName: '',
    productAttributeCategoryValue: []
  };
  arrayVendorOrderDetailModel: Array<VendorOrderDetailModel> = [];
  dataSourceProductAttributeCategory: MatTableDataSource<ProductAttributeCategoryModel>;
  selection: SelectionModel<ProductAttributeCategoryModel>;
  arrayProductAttributeCategoryValue: Array<ProductAttributeCategoryValueModel> = [];
  arrayProductAttributeCategoryModel: Array<ProductAttributeCategoryModel> = [];
  arrayVendorOrderProductDetailProductAttributeValueModel: Array<VendorOrderProductDetailProductAttributeValueModel> = [];

  filteredProductOptions: Observable<string[]>;
  productList: Array<any> = ["pr1", "pr2"];

  @ViewChild('nameit') private elementRef: ElementRef;
  @ViewChild('priceControl') priceRef: ElementRef;

  constructor(
    private ref: ChangeDetectorRef,
    private router: Router,
    private employeeService: EmployeeService,
    private vendorService: VendorService,
    private categoryService: CategoryService,
    private productService: ProductService,
    private route: ActivatedRoute,
    private orderstatusService: OrderstatusService,
    private contactService: ContactService,
    private customerOrderService: CustomerOrderService,
    private el: ElementRef,
    public snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<VendorOrderProductComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData) 
  { }

  async ngOnInit() {
    this.data.ok = false;
    this.setupFormControl();
    await this.getMasterData();
    this.setDefaultValueForm();

    if (this.data.mode === 'edit') {
      this.oldData.id = this.data.vendorOrderDetail.id;
      this.oldData.vendorOrderDetailId = this.data.vendorOrderDetail.vendorOrderDetailId;
      this.oldData.vendorId = this.data.vendorOrderDetail.vendorId;
      this.oldData.vendorOrderId = this.data.vendorOrderDetail.vendorOrderId;
      this.oldData.productId = this.data.vendorOrderDetail.productId;
      this.oldData.quantity = this.data.vendorOrderDetail.quantity;
      this.oldData.unitPrice = this.data.vendorOrderDetail.unitPrice;
      this.oldData.currencyUnit = this.data.vendorOrderDetail.currencyUnit;
      this.oldData.exchangeRate = this.data.vendorOrderDetail.exchangeRate;
      this.oldData.vat = this.data.vendorOrderDetail.vat;
      this.oldData.discountType = this.data.vendorOrderDetail.discountType;
      this.oldData.discountValue = this.data.vendorOrderDetail.discountValue;
      this.oldData.description = this.data.vendorOrderDetail.description;
      this.oldData.orderDetailType = this.data.vendorOrderDetail.orderDetailType;
      this.oldData.createdById = this.data.vendorOrderDetail.createdById;
      this.oldData.createdDate = this.data.vendorOrderDetail.createdDate;
      this.oldData.updatedById = this.data.vendorOrderDetail.updatedById;
      this.oldData.updatedDate = this.data.vendorOrderDetail.updatedDate;
      this.oldData.active = this.data.vendorOrderDetail.active;
      this.oldData.unitId = this.data.vendorOrderDetail.unitId;
      this.oldData.vendorOrderProductDetailProductAttributeValue = [];
      this.oldData.explainStr = this.data.vendorOrderDetail.explainStr;
      this.oldData.nameVendor = this.data.vendorOrderDetail.nameVendor;
      this.oldData.productNameUnit = this.data.vendorOrderDetail.productNameUnit;
      this.oldData.nameMoneyUnit = this.data.vendorOrderDetail.nameMoneyUnit;
      this.oldData.sumAmount = this.data.vendorOrderDetail.sumAmount;
      this.oldAmount = this.oldData.sumAmount;

      this.vendorOrderDetailModel = this.oldData;
      this.arrayVendorOrderProductDetailProductAttributeValueModel = this.vendorOrderDetailModel.vendorOrderProductDetailProductAttributeValue;
      
      /*Set lại form*/
      if (this.vendorOrderDetailModel.orderDetailType == 1) {
        var productObject = this.listProductByVendor.filter(product => product.productId.toLowerCase().indexOf(this.data.vendorOrderDetail.productId.toLowerCase()) >= 0);
        //Set up for Unit Product
        this.vendorOrderDetailModel.unitId = productObject[0].productUnitId;
        var unitObject = this.listunit.filter(unit => unit.categoryId.toLowerCase().indexOf(productObject[0].productUnitId.toLowerCase()) >= 0);
        this.unitControl.setValue(unitObject[0].categoryName);
        this.vendorOrderDetailModel.productNameUnit = unitObject[0].categoryName;
        this.productIdControl.setValue(productObject[0].productName);
        this.getProductAttr(productObject[0].productId);
      }

      this.moneyUnitControl.setValue(this.vendorOrderDetailModel.currencyUnit);
      let currentcyUnitCode = this.unitmoney.find(x => x.categoryId == this.vendorOrderDetailModel.currencyUnit).categoryCode;
      
      if (currentcyUnitCode == "USD") 
      {
        this.isVnd = false;
        this.ProductMoneyUnit = "USD";
      }
      else if (currentcyUnitCode == "VND")
      {
        this.isVnd = true;
        this.ProductMoneyUnit = "VND";
      }
      this.calculatorMoney();
      /*End*/

      if (this.data.mode === 'edit') {
        //focus vào ô đơn giá khi mở popup lên
        this.elementRef.nativeElement.focus();
      }
    }
  }

  ngAfterContentChecked() : void {
    this.ref.detectChanges();
  }

  calculatorMoney() {
    let quantity = this.vendorOrderDetailModel.quantity ? this.vendorOrderDetailModel.quantity : 0;  //Số lượng
    let price = this.vendorOrderDetailModel.unitPrice ? this.vendorOrderDetailModel.unitPrice : 0;  //Đơn giá
    let exchangeRate = this.vendorOrderDetailModel.exchangeRate ? this.vendorOrderDetailModel.exchangeRate : 0; //Tỷ giá
    let VAT = this.vendorOrderDetailModel.vat ? this.vendorOrderDetailModel.vat : 0;  //Giá trị Vat(%)
    let discountType = this.vendorOrderDetailModel.discountType;  //Loại chiết khấu(% hoặc tiền)
    let discountValue = this.vendorOrderDetailModel.discountValue ? this.vendorOrderDetailModel.discountValue : 0;  //Giá trị chiết khấu (tùy theo Loại chiết khấu)
    
    if (this.isVnd) {
      exchangeRate = 1;
    }
    //Thành tiền:
    this.ProductAmount = quantity * price * exchangeRate;
    //Thành tiền thuế VAT:
    this.ProductVAT = (this.ProductAmount * VAT) / 100;
    //Thành tiền Chiết khấu:
    if (discountType == true)
    {
      //Nếu là chiết khấu theo %
      this.ProductDiscount = (this.ProductAmount * discountValue) / 100;
    }
    else
    {
      //Nếu là chiết khấu theo số tiền
      this.ProductDiscount = discountValue;
    }

    this.vendorOrderDetailModel.sumAmount = this.ProductAmount;
  }

  onCancelClick(): void {
    this.data.ok = false;
    this.data.vendorOrderDetail = this.oldData;
    this.dialogRef.close(this.data);
  }

  onOkClick(): void {
    if (this.vendorOrderDetailModel.orderDetailType == 1) {
      if (!this.informationProductForm.valid) {
        Object.keys(this.informationProductForm.controls).forEach(key => {
          if (this.informationProductForm.controls[key].valid === false) {
            this.informationProductForm.controls[key].markAsTouched();
          }
        });
        let target;
        target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

        if (target) {
          $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
          target.focus();
        }
      } else {
        if(this.arrayVendorOrderProductDetailProductAttributeValueModel.length > 0) 
        {
          //Nếu sản phẩm có thuộc tính thì kiểm tra đã chọn đủ thuộc tính hay chưa
          let countEmptyValue = this.arrayVendorOrderProductDetailProductAttributeValueModel.filter(x => x.productAttributeCategoryValueId == this.emptyGuid).length;
          if (countEmptyValue > 0) 
          {
            //Nếu chưa chọn đủ thuộc tính
            this.arrayVendorOrderProductDetailProductAttributeValueModel.filter(x => x.productAttributeCategoryValueId == this.emptyGuid).forEach(item => {
              let id = '#' + item.productAttributeCategoryId;
              $(id).css({
                "border-color": "#f44336",
                "border-width": "1px",
                "border-style": "solid"
              });
            });
          }
          else if (countEmptyValue == 0)
          {
            //Nếu đã chọn đủ thuộc tính
            this.data.ok = true;
            this.data.vendorOrderDetail = this.vendorOrderDetailModel;
            this.data.productAmount = this.ProductAmount;
            this.data.productDiscount = this.ProductDiscount;
            this.data.productVAT = this.ProductVAT;
            this.data.arrayProductAttr = this.arrayVendorOrderProductDetailProductAttributeValueModel;
            this.dialogRef.close(this.data);
          }
        }
        else 
        {
          //Nếu sản phẩm không có thuộc tính thì cho đóng dialog
          this.data.ok = true;
          this.data.vendorOrderDetail = this.vendorOrderDetailModel;
          this.data.productAmount = this.ProductAmount;
          this.data.productDiscount = this.ProductDiscount;
          this.data.productVAT = this.ProductVAT;
          this.data.arrayProductAttr = this.arrayVendorOrderProductDetailProductAttributeValueModel;
          this.dialogRef.close(this.data);
        }
      }
    } else {
      if (!this.internalProductForm.valid) {
        Object.keys(this.internalProductForm.controls).forEach(key => {
          if (this.internalProductForm.controls[key].valid === false) {
            this.internalProductForm.controls[key].markAsTouched();
          }
        });
        let target;
        target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

        if (target) {
          $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
          target.focus();
        }
      } else {
        this.data.ok = true;
        this.data.vendorOrderDetail = this.vendorOrderDetailModel;
        this.data.productAmount = this.ProductAmount;
        this.data.productDiscount = this.ProductDiscount;
        this.data.productVAT = this.ProductVAT;
        this.data.arrayProductAttr = this.arrayVendorOrderProductDetailProductAttributeValueModel;
        this.dialogRef.close(this.data);
      }
    }
  }

  changeTablist(event: MatRadioChange, code: any) {
    this.productValue = event.value;
    if (code !== "product") {
      this.vendorOrderDetailModel.orderDetailType = 2;
    } else {
      this.vendorOrderDetailModel.orderDetailType = 1;
    }
    this.resetForm();
  }

  resetForm() {
    const toSelectMoneyUnit = this.unitmoney.find(c => c.categoryCode === 'VND');
    /*Form Mua sản phẩm/dịch vụ theo đơn đặt hàng của khách hàng*/
    this.informationProductForm.reset();
    this.quantityControl.setValue(0);
    this.priceControl.setValue(0);
    this.moneyUnitControl.setValue(toSelectMoneyUnit.categoryId);
    this.exchangeRateControl.setValue(1);
    this.vatControl.setValue(0);
    this.arrayProductAttributeCategoryModel = []; //list các thuộc tính của sản phẩm
    this.dataSourceProductAttributeCategory = new MatTableDataSource<ProductAttributeCategoryModel>(this.arrayProductAttributeCategoryModel); //reset lại table
    this.arrayVendorOrderProductDetailProductAttributeValueModel = [];
    this.discountTypeControl.setValue(true);
    this.discountValueControl.setValue(0);
    /*End*/

    /*Form Chi phí khác*/
    this.internalProductForm.reset();
    this.descriptionProductControl.setValue("");
    this.quantityIncuredControl.setValue(0);
    this.priceIncuredControl.setValue(0);
    this.moneyIncuredUnitControl.setValue(toSelectMoneyUnit.categoryId);
    this.exchangeRateIncuredControl.setValue(1);
    this.discountTypeIncuredControl.setValue(true);
    this.discountValueIncuredControl.setValue(0);
    /*End*/

    this.isVnd = true;

    this.ProductMoneyUnit = 'VND';
    this.ProductAmount = 0; //Thành tiền
    this.ProductDiscount = 0; //Thành tiền chiết khấu
    this.ProductVAT = 0;  //Thành tiền thuế GTGT

    /*Reset Model*/
    this.vendorOrderDetailModel.productId = this.emptyGuid;
    this.vendorOrderDetailModel.unitId = this.emptyGuid;
    this.vendorOrderDetailModel.explainStr = ""; //Diễn giải
    this.vendorOrderDetailModel.nameVendor = ""; //Tên nhà cung cấp
    this.vendorOrderDetailModel.productNameUnit = ""; //Tên đơn vị tính của sản phẩm/dịch vụ
    this.vendorOrderDetailModel.nameMoneyUnit = "VND";
    this.vendorOrderDetailModel.sumAmount = 0;  //Tổng tiền đơn hàng = Số lượng * Đơn giá * Tỷ giá
    this.vendorOrderDetailModel.description = ""; //Trường Diễn giải bên form Chi phí khác
    this.vendorOrderDetailModel.vendorOrderProductDetailProductAttributeValue = [];
    /*End*/
  }

  async getMasterData() {
    /**Get Money Unit */
    let resultUnitMoney: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc('DTI');
    this.unitmoney = resultUnitMoney.category;
    //this.setDefaultCurrency();

    /**Get Product Unit */
    let resultProductUnit: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc('DNH');
    this.listunit = resultProductUnit.category;

    /**Get List Product By Vendor*/
    let resultProductList: any = await this.productService.getProductByVendorIDAsync(this.data.vendorId);
    this.listProductByVendor = resultProductList.lstProduct;
  }

  setDefaultValueForm() {
    const toSelectMoneyUnit = this.unitmoney.find(c => c.categoryCode === 'VND');
    this.ProductMoneyUnit = 'VND';
    this.exchangeRateControl.setValue(1);
    this.moneyUnitControl.setValue(toSelectMoneyUnit.categoryId);
    this.filteredProductOptions = this.productIdControl.valueChanges.pipe(
      startWith(null),
      map((item: string) => item ? this._filterProduct(item, this.listProductByVendor) : this.listProductByVendor.slice()));
  }

  private _filterProduct(value: any, array: any): string[] {
    return array.filter(vendor =>
      vendor.productId.toLowerCase().indexOf(value.toLowerCase()) >= 0 || vendor.productName.toLowerCase().indexOf(value.toLowerCase()) >= 0);
  }

  setupFormControl() {
    var patternInt = '^[0-9]*$';
    this.productIdControl = new FormControl('');
    this.quantityControl = new FormControl('', [Validators.required, Validators.min(0.000001)]);
    this.priceControl = new FormControl('');
    this.unitControl = new FormControl('');
    this.moneyUnitControl = new FormControl('');
    this.exchangeRateControl = new FormControl('', [Validators.required, Validators.min(0.000001)]);
    this.discountTypeControl = new FormControl('');
    this.discountValueControl = new FormControl('');
    this.vatControl = new FormControl('');
    this.descriptionProductControl = new FormControl('');
    this.quantityIncuredControl = new FormControl('', [Validators.required, Validators.min(0.000001)]);
    this.priceIncuredControl = new FormControl('');
    this.vatIncuredControl = new FormControl('');
    this.unitIncuredControl = new FormControl('');
    this.moneyIncuredUnitControl = new FormControl('');
    this.exchangeRateIncuredControl = new FormControl('', [Validators.required, Validators.min(0.000001)]);
    this.discountTypeIncuredControl = new FormControl('');
    this.discountValueIncuredControl = new FormControl('');

    this.informationProductForm = new FormGroup({
      productIdControl: this.productIdControl,
      quantityControl: this.quantityControl,
      priceControl: this.priceControl,
      unitControl: this.unitControl,
      moneyUnitControl: this.moneyUnitControl,
      exchangeRateControl: this.exchangeRateControl,
      discountTypeControl: this.discountTypeControl,
      discountValueControl: this.discountValueControl,
      vatControl: this.vatControl,
    });

    this.internalProductForm = new FormGroup({
      descriptionProductControl: this.descriptionProductControl,
      quantityIncuredControl: this.quantityIncuredControl,
      priceIncuredControl: this.priceIncuredControl,
      vatIncuredControl: this.vatIncuredControl,
      moneyIncuredUnitControl: this.moneyIncuredUnitControl,
      exchangeRateIncuredControl: this.exchangeRateIncuredControl,
      discountTypeIncuredControl: this.discountTypeIncuredControl,
      discountValueIncuredControl: this.discountValueIncuredControl
    });
  }

  setDefaultCurrency() {
    const toSelectMoneyUnit = this.unitmoney.find(c => c.categoryCode === 'VND');
    this.ProductMoneyUnit = 'VND';

    if (this.vendorOrderDetailModel.orderDetailType == 1) {
      //Form Mua sản phẩm/dịch vụ theo đơn đặt hàng của khách hàng
      this.exchangeRateControl.setValue(1);
      this.moneyUnitControl.setValue(toSelectMoneyUnit.categoryId);
    } else if (this.vendorOrderDetailModel.orderDetailType == 2) {
      //Form Chi phí khác
      this.exchangeRateIncuredControl.setValue(1);
      this.moneyIncuredUnitControl.setValue(toSelectMoneyUnit.categoryId);
    }
  }

  selectedProductFn(event: MatAutocompleteSelectedEvent): void {
    this.productIdControl.setValue(event.option.viewValue);
    this.vendorOrderDetailModel.productId = event.option.value;
    var productObject = this.listProductByVendor.filter(product => product.productId.toLowerCase().indexOf(event.option.value.toLowerCase()) >= 0);
    var unitObject = this.listunit.filter(unit => unit.categoryId.toLowerCase().indexOf(productObject[0].productUnitId.toLowerCase()) >= 0);
     /*Set value form*/
    this.unitControl.setValue(unitObject[0].categoryName);
    this.priceControl.setValue(productObject[0].price1);
    this.quantityControl.setValue(0);
    this.moneyUnitControl.setValue(this.unitmoney.find(x => x.categoryId == productObject[0].productMoneyUnitId).categoryId);
    this.isVnd = this.unitmoney.find(x => x.categoryId == productObject[0].productMoneyUnitId).categoryCode == "VND" ? true : false;
    if (!this.isVnd) {
      this.exchangeRateControl.enable();
    } else {
      this.exchangeRateControl.disable();
    }
    this.exchangeRateControl.setValue(1);
    this.vatControl.setValue(productObject[0].vat);
    /*end*/

    /*Set up for Unit Product*/
    this.vendorOrderDetailModel.unitId = productObject[0].productUnitId;
    this.vendorOrderDetailModel.productNameUnit = unitObject[0].categoryName;
    this.vendorOrderDetailModel.quantity = 0;
    this.vendorOrderDetailModel.currencyUnit = this.unitmoney.find(x => x.categoryId == productObject[0].productMoneyUnitId).categoryId;
    this.vendorOrderDetailModel.nameMoneyUnit = this.unitmoney.find(x => x.categoryId == productObject[0].productMoneyUnitId).categoryCode;
    this.vendorOrderDetailModel.exchangeRate = 1;
    this.vendorOrderDetailModel.vat = productObject[0].vat;
    this.vendorOrderDetailModel.explainStr = event.option.viewValue + '()';
    /*end*/

    //Set up Grid Product Attribute
    this.arrayProductAttributeCategoryModel = [];
    this.arrayVendorOrderProductDetailProductAttributeValueModel = [];
    this.getProductAttr(event.option.value);
    //End Grid Product Attribute

    this.calculatorMoney();
  }

  getProductAttr(productId: string) {
    this.productService.getProductAttributeByProductID(productId).subscribe(response => {
      let result = <any>response;
      for (var x = 0; x < result.lstProductAttributeCategory.length; x++) {
        for (var j = 0; j < result.lstProductAttributeCategory[x].productAttributeCategoryValue.length; j++) {
          var objectPush = result.lstProductAttributeCategory[x].productAttributeCategoryValue[j];

          var item: ProductAttributeCategoryValueModel = {
            ProductAttributeCategoryValueId: objectPush.productAttributeCategoryValueId,
            ProductAttributeCategoryValue1: objectPush.productAttributeCategoryValue1,
            CreatedById: objectPush.createdById,
            CreatedDate: objectPush.createdDate,
            ProductAttributeCategoryId: objectPush.productAttributeCategoryId,
            UpdatedById: objectPush.updatedById,
            UpdatedDate: objectPush.updatedDate,
            Active: objectPush.active
          };
          this.arrayProductAttributeCategoryValue.push(item);
        }
        var findDefaultValueAttribute = this.data.vendorOrderDetail === null ? null : this.data.vendorOrderDetail.vendorOrderProductDetailProductAttributeValue.find(item => item.productAttributeCategoryId == result.lstProductAttributeCategory[x].productAttributeCategoryId);
        if (findDefaultValueAttribute !== null && findDefaultValueAttribute !== undefined) {
          var productAttributeCategory: ProductAttributeCategoryModel = {
            ProductAttributeCategoryId: result.lstProductAttributeCategory[x].productAttributeCategoryId,
            ProductAttributeCategoryName: result.lstProductAttributeCategory[x].productAttributeCategoryName,
            ValueString: '',
            CreatedById: result.lstProductAttributeCategory[x].createdById,
            CreatedDate: result.lstProductAttributeCategory[x].createdDate,
            ProductAttributeCategoryValue: this.arrayProductAttributeCategoryValue,
            UpdatedDate: result.lstProductAttributeCategory[x].updatedDate,
            UpdatedById: result.lstProductAttributeCategory[x].updatedById,
            Active: result.lstProductAttributeCategory[x].active,
            DefaultValue: findDefaultValueAttribute.productAttributeCategoryValueId,
          }
          var orderProductDetailProductAttributeValue: VendorOrderProductDetailProductAttributeValueModel = {
            vendorOrderDetailId: this.emptyGuid,
            vendorOrderProductDetailProductAttributeValueId: this.emptyGuid,
            productAttributeCategoryId: result.lstProductAttributeCategory[x].productAttributeCategoryId,
            productAttributeCategoryValueId: findDefaultValueAttribute.productAttributeCategoryValueId,
            productId: this.vendorOrderDetailModel.productId,

            //Add by Giang: cần review lại
            orderProductDetailProductAttributeValueId: '',
            productAttributeCategoryName: '',
            productAttributeCategoryValue: []
          };
          this.arrayProductAttributeCategoryModel.push(productAttributeCategory);
          this.arrayVendorOrderProductDetailProductAttributeValueModel.push(orderProductDetailProductAttributeValue);
          this.arrayProductAttributeCategoryValue = [];
        }
        else {
          var productAttributeCategory: ProductAttributeCategoryModel = {
            ProductAttributeCategoryId: result.lstProductAttributeCategory[x].productAttributeCategoryId,
            ProductAttributeCategoryName: result.lstProductAttributeCategory[x].productAttributeCategoryName,
            ValueString: '',
            CreatedById: result.lstProductAttributeCategory[x].createdById,
            CreatedDate: result.lstProductAttributeCategory[x].createdDate,
            ProductAttributeCategoryValue: this.arrayProductAttributeCategoryValue,
            UpdatedDate: result.lstProductAttributeCategory[x].updatedDate,
            UpdatedById: result.lstProductAttributeCategory[x].updatedById,
            Active: result.lstProductAttributeCategory[x].active,
            DefaultValue: this.emptyGuid,
          }
          var orderProductDetailProductAttributeValue: VendorOrderProductDetailProductAttributeValueModel = {
            vendorOrderDetailId: this.emptyGuid,
            vendorOrderProductDetailProductAttributeValueId: this.emptyGuid,
            productAttributeCategoryId: result.lstProductAttributeCategory[x].productAttributeCategoryId,
            productAttributeCategoryValueId: this.emptyGuid,
            productId: this.vendorOrderDetailModel.productId,

            //Add by Giang: cần review lại
            orderProductDetailProductAttributeValueId: '',
            productAttributeCategoryName: '',
            productAttributeCategoryValue: []
          };
          this.arrayProductAttributeCategoryModel.push(productAttributeCategory);
          this.arrayVendorOrderProductDetailProductAttributeValueModel.push(orderProductDetailProductAttributeValue);
          this.arrayProductAttributeCategoryValue = [];
        }
      };
      this.dataSourceProductAttributeCategory = new MatTableDataSource<ProductAttributeCategoryModel>(this.arrayProductAttributeCategoryModel);
    }, error => { });
  }

  selectedUnitmoneyFn(event: any) {
    let target = event.source.selected._element.nativeElement;
    let selectedData = {
      value: event.value,
      text: target.innerText.trim()
    };
    this.ProductMoneyUnit = selectedData.text;
    this.vendorOrderDetailModel.nameMoneyUnit = selectedData.text;
    this.isVnd = selectedData.text === 'VND';
    if (this.isVnd) {
      this.vendorOrderDetailModel.exchangeRate = 1;
    }
    if (this.vendorOrderDetailModel.orderDetailType == 1) {
      if (this.isVnd) {
        this.exchangeRateControl.disable();
      } else {
        this.exchangeRateControl.enable();
      }
    } else if (this.vendorOrderDetailModel.orderDetailType == 2) {
      if (this.isVnd) {
        this.exchangeRateIncuredControl.disable();
      } else {
        this.exchangeRateIncuredControl.enable();
      }
    }
    this.calculatorMoney();
  }

  selectedProductAttributeCategoryValueFn(event: any, productAttributeCategoryId: any) {
    let target = event.source.selected._element.nativeElement;
    let selectedData = {
      value: event.value,
      text: target.innerText.trim()
    };
    //this.ProductMoneyUnit = selectedData.text;
    //this.vendorOrderDetailModel.NameMoneyUnit = selectedData.text;

    var filter = this.arrayVendorOrderProductDetailProductAttributeValueModel.filter(item => item.productAttributeCategoryId.toLocaleLowerCase().indexOf(productAttributeCategoryId.toLocaleLowerCase()) >= 0);
    filter[0].productAttributeCategoryValueId = event.value;
    this.vendorOrderDetailModel.explainStr = this.vendorOrderDetailModel.explainStr.replace(')', selectedData.text + ';)');

    var id = '#' + productAttributeCategoryId;
    $(id).css({
      "border-color": "none",
      "border-width": "none",
      "border-style": "none"
    });
  }

  productCaculateAmount() {
    this.ProductAmount = this.vendorOrderDetailModel.unitPrice * this.vendorOrderDetailModel.quantity;
    this.vendorOrderDetailModel.sumAmount = this.ProductAmount;
  }
  productCaculateExchangerate() {
    this.ProductAmount = this.vendorOrderDetailModel.unitPrice * this.vendorOrderDetailModel.quantity * this.vendorOrderDetailModel.exchangeRate;
    this.vendorOrderDetailModel.sumAmount = this.ProductAmount;
  }
  productCaculateVAT() {
    this.ProductVAT = (this.ProductAmount * this.vendorOrderDetailModel.vat) / 100;
  }

  selectedDiscountTypeList(event: any) {
    this.vendorOrderDetailModel.discountValue = 0;
    this.calculatorMoney();
  }

  changeVendorOrderDetailDiscountType(event: any) {
    this.vendorOrderDetailModel.discountValue = 0;
    this.calculatorMoney();
  }

  productCaculateDiscount() {
    if (this.vendorOrderDetailModel.discountType) {
      if (this.ProductAmount != null && this.ProductAmount > 0) {
        this.ProductDiscount = (this.ProductAmount * <number>this.vendorOrderDetailModel.discountValue) / 100;
      }
    }
    else {
      this.ProductDiscount = <number>this.vendorOrderDetailModel.discountValue;
    }
  }

  onKeyPress(event: any) {
    const pattern = /^[0-9\.]$/;
    const inputChar = String.fromCharCode(event.charCode);
    if (!pattern.test(inputChar)) {
      event.preventDefault();
    }
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }
}

import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { } from 'primeng/api';

import { QuoteService } from '../../services/quote.service';
import { MessageService } from 'primeng/api';

import { QuoteDetail } from '../../models/quote-detail.model';
import { QuoteProductDetailProductAttributeValue } from '../../models/quote-product-detail-product-attribute-value.model';
import { QuickCreateProductComponent } from '../../../shared/components/quick-create-product/quick-create-product.component';

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  quoteDetailModel: QuoteDetail,
}

interface Category {
  categoryId: string,
  categoryCode: string,
  categoryName: string,
  isDefault: boolean;
}

interface Vendor {
  vendorId: string,
  vendorCode: string,
  vendorName: string
}

interface Product {
  productId: string,
  productCode: string,
  productName: string,
  productUnitId: string,
  productCodeName: string,
  price1: number,
}

interface DiscountType {
  name: string;
  code: string;
  value: boolean;
}

interface ObjectAttrNameProduct {
  productAttributeCategoryId: string,
  productAttributeCategoryName: string  //Màu sắc
}

interface ObjectAttrValueProduct {
  productAttributeCategoryValueId: string,
  productAttributeCategoryValue: string, //Đỏ, Vàng, Xanh, Trắng...
  productAttributeCategoryId: string
}

interface GroupAttrProduct {
  AttrName: ObjectAttrNameProduct,
  AttrValue: Array<ObjectAttrValueProduct>,
  SelectedAttrValue: ObjectAttrValueProduct
}

@Component({
  selector: 'app-add-edit-product-dialog',
  templateUrl: './add-edit-product-dialog.component.html',
  styleUrls: ['./add-edit-product-dialog.component.css'],
  providers: [DialogService]
})
export class AddEditProductDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;

  /*Các biến điều kiện*/
  isCreate: boolean = true; //true: Tạo mới sản phẩm dịch vụ(hoặc chi phí phát sinh), false: Sửa sản phẩm dịch vụ(hoặc chi phí phát sinh)
  cusGroupId: string; //Nhóm khách hàng
  selectedOrderDetailType: number = 0;  //0: Sản phẩm dịch vụ, 1: Chi phí phát sinh
  isShowRadioProduct: boolean = true;
  isShowRadioOC: boolean = true;
  /*End*/

  /*Các biến nhận giá trị trả về*/
  orderDate: Date;
  listUnitMoney: Array<Category> = [];
  unitMoneyLabel: string = 'VND';
  unitMoneyOCLabel: string = 'VND';
  listUnitProduct: Array<Category> = [];
  listVendor: Array<Vendor> = [];
  listProduct: Array<Product> = [];
  amountProduct: number = 0;  //amountProduct = quantityProduct * priceProduct (sản phẩm dịch vụ)
  amountVatProduct: number = 0; //tiền thuế GTGT (sản phẩm dịch vụ)
  amountPriceInitialProduct: number = 0; //tiền thuế GTGT (sản phẩm dịch vụ)
  amountDiscountProduct: number = 0; //tiền chiết khấu (sản phẩm dịch vụ)
  amountOC: number = 0;
  amountVatOC: number = 0;
  amountDiscountOC: number = 0;
  unitPriceVon: number = 0;
  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];
  cols: any[];
  listObjectAttrNameProduct: Array<ObjectAttrNameProduct> = [];
  listObjectAttrValueProduct: Array<ObjectAttrValueProduct> = [];
  listAttrProduct: Array<GroupAttrProduct> = [];
  quoteDetailModel = new QuoteDetail();
  IsPriceInitial: boolean = false;
  /*End*/

  /*Form sản phẩm dịch vụ*/
  productForm: FormGroup;
  productControl: FormControl;
  unitProductControl: FormControl;
  vendorControl: FormControl;
  productNameControl: FormControl;
  quantityProductControl: FormControl;
  priceProductControl: FormControl;
  unitMoneyProductControl: FormControl;
  priceInitialControl: FormControl;
  exchangeRateProductControl: FormControl;
  vatProductControl: FormControl;
  discountTypeProductControl: FormControl;
  discountValueProductControl: FormControl;
  /*End*/

  /*Form chi phí khác*/
  otherCostsForm: FormGroup; //otherCosts sẽ ký hiệu tắt là OC và đc thêm vào hậu tố của control
  descriptionOCControl: FormControl;
  unitOCControl: FormControl;
  quantityOCControl: FormControl;
  priceOCControl: FormControl;
  unitMoneyOCControl: FormControl;
  exchangeRateOCControl: FormControl;
  vatOCControl: FormControl;
  discountTypeOCControl: FormControl;
  discountValueOCControl: FormControl;
  /*End*/

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private quoteService: QuoteService,
    private messageService: MessageService,
    public dialogService: DialogService,
  ) {
    this.isCreate = this.config.data.isCreate;
    this.cusGroupId = this.config.data.cusGroupId;
    this.orderDate = this.config.data.orderDate;
    if (!this.isCreate) {
      //Nếu là sửa
      this.quoteDetailModel = this.config.data.quoteDetailModel;
      this.selectedOrderDetailType = this.quoteDetailModel.orderDetailType;
      if (this.selectedOrderDetailType == 0) {
        //ẩn button radio Sản phẩm dịch vụ
        this.isShowRadioOC = false;
      } else if (this.selectedOrderDetailType == 1) {
        //ẩn button radio Chi phí khác
        this.isShowRadioProduct = false;
      }
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  ngOnInit() {
    this.setForm();
    this.loading = true;
    this.quoteService.getDataQuoteAddEditProductDialog().subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listProduct = result.listProduct;
        this.listUnitMoney = result.listUnitMoney;
        this.listUnitProduct = result.listUnitProduct;

        this.setDefaultValueForm();

        this.setTable();
      } else {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setForm() {
    /*Form Sản phẩm dịch vụ*/
    this.productControl = new FormControl(null, [Validators.required]);
    this.unitProductControl = new FormControl({ value: '', disabled: true });
    this.vendorControl = new FormControl(null); //, [Validators.required]);
    this.productNameControl = new FormControl(null); //, [Validators.required]);
    this.quantityProductControl = new FormControl('0');
    this.priceProductControl = new FormControl('0');
    this.priceInitialControl = new FormControl('0');
    this.unitMoneyProductControl = new FormControl(null);
    this.exchangeRateProductControl = new FormControl('1');
    this.vatProductControl = new FormControl('0');
    this.discountTypeProductControl = new FormControl(null);
    this.discountValueProductControl = new FormControl('0');

    this.productForm = new FormGroup({
      productControl: this.productControl,
      unitProductControl: this.unitProductControl,
      vendorControl: this.vendorControl,
      productNameControl: this.productNameControl,
      quantityProductControl: this.quantityProductControl,
      priceProductControl: this.priceProductControl,
      priceInitialControl: this.priceInitialControl,
      unitMoneyProductControl: this.unitMoneyProductControl,
      exchangeRateProductControl: this.exchangeRateProductControl,
      vatProductControl: this.vatProductControl,
      discountTypeProductControl: this.discountTypeProductControl,
      discountValueProductControl: this.discountValueProductControl
    });
    /*End*/

    /*Form Chi phí khác*/
    this.descriptionOCControl = new FormControl('', [Validators.required, forbiddenSpaceText]);
    this.unitOCControl = new FormControl('', [Validators.maxLength(50)]);
    this.quantityOCControl = new FormControl('0');
    this.priceOCControl = new FormControl('0');
    this.unitMoneyOCControl = new FormControl(null);
    this.exchangeRateOCControl = new FormControl('1');
    this.vatOCControl = new FormControl('0');
    this.discountTypeOCControl = new FormControl(null);
    this.discountValueOCControl = new FormControl('0');

    this.otherCostsForm = new FormGroup({
      descriptionOCControl: this.descriptionOCControl,
      unitOCControl: this.unitOCControl,
      quantityOCControl: this.quantityOCControl,
      priceOCControl: this.priceOCControl,
      unitMoneyOCControl: this.unitMoneyOCControl,
      exchangeRateOCControl: this.exchangeRateOCControl,
      vatOCControl: this.vatOCControl,
      discountTypeOCControl: this.discountTypeOCControl,
      discountValueOCControl: this.discountValueOCControl
    });
    /*End*/
  }

  setTable() {
    this.cols = [
      { field: 'AttrName', header: 'Tên thuộc tính', width: '50%', textAlign: 'left', color: '#f44336' },
      { field: 'AttrValue', header: 'Giá trị', width: '50%', textAlign: 'left', color: '#f44336' },
    ];
  }

  /*Event set giá trị mặc định cho các control*/
  setDefaultValueForm() {
    if (this.isCreate) {
      /*Form Sản phẩm dịch vụ*/

      //Đơn vị tiền
      let toSelectUnitMoneyProduct = this.listUnitMoney.find(x => x.isDefault == true);
      this.unitMoneyProductControl.setValue(toSelectUnitMoneyProduct);
      this.unitMoneyLabel = toSelectUnitMoneyProduct.categoryCode;

      //Loại chiết khấu
      let toSelectDiscountTypeProduct = this.discountTypeList.find(x => x.code == 'PT');
      this.discountTypeProductControl.setValue(toSelectDiscountTypeProduct);

      /*End: Form Sản phẩm dịch vụ*/

      /*Form Chi phí khác (OC)*/

      //Đơn vị tiền
      let toSelectUnitMoneyOC = this.listUnitMoney.find(x => x.isDefault == true);
      this.unitMoneyOCControl.setValue(toSelectUnitMoneyOC);
      this.unitMoneyOCLabel = toSelectUnitMoneyOC.categoryCode;

      //Loại chiết khấu
      let toSelectDiscountTypeOC = this.discountTypeList.find(x => x.code == 'PT');
      this.discountTypeOCControl.setValue(toSelectDiscountTypeOC);

      /*End: Form Chi phí khác*/
    } else {
      if (this.selectedOrderDetailType == 0) {
        let toSelectProduct = this.listProduct.find(x => x.productId == this.quoteDetailModel.productId);
        this.productControl.setValue(toSelectProduct);
        let productUnitName = this.listUnitProduct.find(x => x.categoryId == toSelectProduct.productUnitId).categoryName;
        this.unitProductControl.setValue(productUnitName);
        this.productNameControl.setValue(this.quoteDetailModel.productName);

        this.IsPriceInitial = this.quoteDetailModel.isPriceInitial;
        this.priceInitialControl.setValue(this.quoteDetailModel.priceInitial);
        this.amountPriceInitialProduct = this.quoteDetailModel.priceInitial * this.quoteDetailModel.quantity * this.quoteDetailModel.exchangeRate;
        //Lấy list nhà cung cấp
        this.quoteService.getVendorByProductId(toSelectProduct.productId).subscribe(response => {
          let result: any = response;

          if (result.statusCode == 200) {
            this.listVendor = result.listVendor;
            this.listObjectAttrNameProduct = result.listObjectAttributeNameProduct;
            this.listObjectAttrValueProduct = result.listObjectAttributeValueProduct;

            /*Nhóm các thuộc tính của sản phẩm lại và map data*/
            this.listObjectAttrNameProduct.forEach(objectAttrName => {
              let objectGroup: GroupAttrProduct = {
                AttrName: {
                  productAttributeCategoryId: '',
                  productAttributeCategoryName: ''
                },
                AttrValue: [],
                SelectedAttrValue: {
                  productAttributeCategoryId: '',
                  productAttributeCategoryValueId: '',
                  productAttributeCategoryValue: '',
                }
              };
              objectGroup.AttrName = objectAttrName;
              objectGroup.AttrValue = this.listObjectAttrValueProduct.filter(x => x.productAttributeCategoryId == objectAttrName.productAttributeCategoryId);
              this.listAttrProduct.push(objectGroup);
            });

            //map data
            this.listAttrProduct.forEach(item => {
              item.SelectedAttrValue.productAttributeCategoryId = item.AttrName.productAttributeCategoryId;
              item.SelectedAttrValue.productAttributeCategoryValueId = this.quoteDetailModel
                .quoteProductDetailProductAttributeValue
                .find(x => x.productAttributeCategoryId == item.AttrName.productAttributeCategoryId)
                .productAttributeCategoryValueId;
              item.SelectedAttrValue.productAttributeCategoryValue = this.listObjectAttrValueProduct
                .find(x => x.productAttributeCategoryValueId == item.SelectedAttrValue.productAttributeCategoryValueId)
                .productAttributeCategoryValue;
            });
            /*End*/

            /*map data vendor*/
            if (this.listVendor.length >= 1) {
              let toSelectVendor = this.listVendor.find(x => x.vendorId == this.quoteDetailModel.vendorId);
              this.vendorControl.setValue(toSelectVendor);
            }
            /*End*/
          } else {
            let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });

        this.quantityProductControl.setValue(this.quoteDetailModel.quantity.toString());
        this.priceProductControl.setValue(this.quoteDetailModel.unitPrice.toString());
        this.unitPriceVon = this.quoteDetailModel.unitPrice;
        this.unitMoneyProductControl.setValue(this.listUnitMoney.find(x => x.categoryId == this.quoteDetailModel.currencyUnit));
        let toSelectExchangeRate = this.quoteDetailModel.exchangeRate != null ? this.quoteDetailModel.exchangeRate.toString() : '0';
        this.exchangeRateProductControl.setValue(toSelectExchangeRate);
        this.unitMoneyLabel = this.listUnitMoney.find(x => x.categoryId == this.quoteDetailModel.currencyUnit).categoryCode;
        let toSelectVat = this.quoteDetailModel.vat != null ? this.quoteDetailModel.vat.toString() : '0';
        this.vatProductControl.setValue(toSelectVat);
        this.discountTypeProductControl.setValue(this.discountTypeList.find(x => x.value == this.quoteDetailModel.discountType));
        let discountValue = this.quoteDetailModel.discountValue != null ? this.quoteDetailModel.discountValue.toString() : '0';
        this.discountValueProductControl.setValue(discountValue);
        /*Tắt validator kiểm tra Chiết khấu*/
        this.discountValueProductControl.setValidators(null);
        this.discountValueProductControl.updateValueAndValidity();
        /*End*/

        //Gán giá trị lại cho các biến lưu số thành tiền
        this.calculatorAmountProduct();
      } else if (this.selectedOrderDetailType == 1) {
        let description = this.quoteDetailModel.description != null ? this.quoteDetailModel.description.trim() : '';
        this.descriptionOCControl.setValue(description);
        let unitOC = this.quoteDetailModel.incurredUnit != null ? this.quoteDetailModel.incurredUnit.trim() : '';
        this.unitOCControl.setValue(unitOC);
        this.quantityOCControl.setValue(this.quoteDetailModel.quantity.toString());
        this.priceOCControl.setValue(this.quoteDetailModel.unitPrice.toString());
        this.unitMoneyOCControl.setValue(this.listUnitMoney.find(x => x.categoryId == this.quoteDetailModel.currencyUnit));
        let toSelectExchangeRate = this.quoteDetailModel.exchangeRate != null ? this.quoteDetailModel.exchangeRate.toString() : '0';
        this.exchangeRateOCControl.setValue(toSelectExchangeRate);
        let toSelectVat = this.quoteDetailModel.vat != null ? this.quoteDetailModel.vat.toString() : '0';
        this.vatOCControl.setValue(toSelectVat);
        this.discountTypeOCControl.setValue(this.discountTypeList.find(x => x.value == this.quoteDetailModel.discountType));
        let discountValue = this.quoteDetailModel.discountValue != null ? this.quoteDetailModel.discountValue.toString() : '0';
        this.discountValueOCControl.setValue(discountValue);
        /*Tắt validator kiểm tra Chiết khấu*/
        this.discountValueOCControl.setValidators(null);
        this.discountValueOCControl.updateValueAndValidity();
        /*End*/

        //gán giá trị mặc định cho đơn vị tiền
        let toSelectUnitMoneyOC = this.listUnitMoney.find(x => x.categoryId == this.quoteDetailModel.currencyUnit);
        this.unitMoneyOCLabel = toSelectUnitMoneyOC.categoryCode;

        //Gán giá trị lại cho các biến lưu số thành tiền
        this.calculatorAmountOC();
      }
    }
  }
  /*End*/

  /*Event khi thay đổi OrderDetailType (Sản phẩm dịch vụ hoặc Chi phí phát sinh)*/
  changeOrderDetailType(orderDetailType: number) {
    this.selectedOrderDetailType = orderDetailType; //thay đổi kiểu dữ liệu từ text => number
  }
  /*End*/

  /*Event khi thay đổi sản phẩm dịch vụ*/
  changeProduct(product: Product) {
    /*reset và setValue các control còn lại*/
    this.unitProductControl.setValue('');
    this.vendorControl.reset();
    this.listVendor = [];
    this.quantityProductControl.setValue('0');
    this.priceProductControl.setValue('0');
    this.unitMoneyLabel = this.listUnitMoney.find(x => x.isDefault == true).categoryCode;
    this.amountProduct = 0;
    this.vatProductControl.setValue('0');
    this.amountVatProduct = 0;
    this.discountTypeProductControl.setValue(this.discountTypeList.find(x => x.code == 'PT'));
    this.discountValueProductControl.setValue('0');
    /*Tắt validator kiểm tra Chiết khấu*/
    this.discountValueProductControl.setValidators(null);
    this.discountValueProductControl.updateValueAndValidity();
    /*End*/
    this.amountDiscountProduct = 0;
    this.listAttrProduct = [];
    /*End*/

    if (product) {
      //Lấy đơn vị tính
      let productUnitId = product.productUnitId;
      let productUnitName = this.listUnitProduct.find(x => x.categoryId == productUnitId).categoryName;
      this.unitProductControl.setValue(productUnitName);
      this.productNameControl.setValue(product.productName);

      //Lấy list nhà cung cấp
      let orderDate = new Date();
      if (this.orderDate) {
        orderDate = convertToUTCTime(this.orderDate);
      }
      this.quoteService.getVendorByProductId(product.productId, orderDate, this.cusGroupId).subscribe(response => {
        let result: any = response;
        if (result.statusCode == 200) {
          this.listVendor = result.listVendor;
          this.priceProductControl.setValue(result.priceProduct);
          this.listObjectAttrNameProduct = result.listObjectAttributeNameProduct;
          this.listObjectAttrValueProduct = result.listObjectAttributeValueProduct;

          /*Nhóm các thuộc tính của sản phẩm lại*/
          this.listObjectAttrNameProduct.forEach(objectAttrName => {
            let objectGroup: GroupAttrProduct = {
              AttrName: {
                productAttributeCategoryId: '',
                productAttributeCategoryName: ''
              },
              AttrValue: [],
              SelectedAttrValue: null
            };
            objectGroup.AttrName = objectAttrName;
            objectGroup.AttrValue = this.listObjectAttrValueProduct.filter(x => x.productAttributeCategoryId == objectAttrName.productAttributeCategoryId);
            this.listAttrProduct.push(objectGroup);
          });
          /*End*/

          /*Nếu listVendor chỉ có 1 giá trị duy nhất thì lấy luôn giá trị đó làm default value*/
          if (this.listVendor.length == 1) {
            let toSelectVendor = this.listVendor[0];
            this.vendorControl.setValue(toSelectVendor);
          }
          /*End*/
        } else {
          let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }
  /*End*/

  changePriceInitial() {
    if (!this.IsPriceInitial) {
      this.priceInitialControl.setValue("0");
      this.amountPriceInitialProduct = 0;
    }
    else {
      let priceInitial = this.priceInitialControl.value;
      if (!priceInitial) {
        this.priceInitialControl.setValue('0');
      }
      else {
        this.amountPriceInitialProduct = parseFloat(this.priceInitialControl.value.replace(/,/g, '')) * parseFloat(this.quantityProductControl.value.replace(/,/g, '')) * parseFloat(this.exchangeRateProductControl.value.replace(/,/g, ''));
      }
    }
  }
  /*Event khi thay đổi Số lượng*/
  changeQuantityProduct() {
    let quantity = this.quantityProductControl.value;
    if (!quantity) {
      this.quantityProductControl.setValue('0');
    }
    this.amountPriceInitialProduct = parseFloat(this.priceInitialControl.value.replace(/,/g, '')) * parseFloat(this.quantityProductControl.value.replace(/,/g, '')) * parseFloat(this.exchangeRateProductControl.value.replace(/,/g, ''));
    this.calculatorAmountProduct();
  }
  /*End*/

  /*Event khi thay đổi Đơn giá*/
  changePriceProduct() {
    let price = this.priceProductControl.value;
    if (!price) {
      this.priceProductControl.setValue('0');
    }

    this.calculatorAmountProduct();
  }
  /*End*/

  /*Event khi thay đổi loại đơn vị tiền: VND, USD,..v..v..*/
  changeUnitMoneyProduct(unitMoney: Category) {
    this.unitMoneyLabel = unitMoney.categoryCode;
    this.exchangeRateProductControl.setValue('1');

    this.calculatorAmountProduct();
  }
  /*End*/

  /*Event khi thay đổi tỷ giá*/
  changeExchangeRateProduct() {
    let exchangeRate = this.exchangeRateProductControl.value;
    if (!exchangeRate) {
      this.exchangeRateProductControl.setValue('1');
    }
    this.amountPriceInitialProduct = parseFloat(this.priceInitialControl.value.replace(/,/g, '')) * parseFloat(this.quantityProductControl.value.replace(/,/g, '')) * parseFloat(this.exchangeRateProductControl.value.replace(/,/g, ''));

    this.calculatorAmountProduct();
  }
  /*End*/

  /*Event khi thay đổi giá trị thuế GTGT*/
  changeVatProduct() {
    let vat = this.vatProductControl.value;
    if (!vat) {
      this.vatProductControl.setValue('0');
    }

    this.calculatorAmountProduct();
  }
  /*End*/

  /*Event khi thay đổi loại Chiết khấu (theo %, theo Số tiền)*/
  changeDiscountTypeProduct(discountType: DiscountType) {
    this.discountValueProductControl.setValue('0');

    if (discountType.code == 'PT') {
      /*Tắt validator kiểm tra Chiết khấu*/
      this.discountValueProductControl.setValidators(null);
      this.discountValueProductControl.updateValueAndValidity();
      /*End*/
    }

    this.calculatorAmountProduct();
  }
  /*End*/

  /*Event khi thay đổi giá trị Chiết khấu*/
  changeDiscountValueProduct() {
    let discountValue = 0;
    if (this.discountValueProductControl.value.trim() == '') {
      discountValue = 0;
      this.discountValueProductControl.setValue('0');
    } else {
      discountValue = parseFloat(this.discountValueProductControl.value.replace(/,/g, ''));
    }

    let discountType: DiscountType = this.discountTypeProductControl.value;
    if (discountType.code == 'PT') {
      if (discountValue > 100) {
        discountValue = 100;
        this.discountValueProductControl.setValue('100');
      }

      /*Tắt validator kiểm tra Chiết khấu*/
      this.discountValueProductControl.setValidators(null);
      this.discountValueProductControl.updateValueAndValidity();
      /*End*/
    } else if (discountType.code == 'ST') {
      /*Bật validator kiểm tra Chiết khấu*/
      this.discountValueProductControl.setValidators([compareNumberValidator(this.amountProduct)]);
      this.discountValueProductControl.updateValueAndValidity();
      /*End*/
    }

    this.calculatorAmountProduct();
  }
  /*End*/

  /*Tính các giá trị: amountProduct, amountVatProduct, amountDiscountProduct*/
  calculatorAmountProduct() {
    let quantity: number = parseFloat(this.quantityProductControl.value.replace(/,/g, ''));
    let price: number = parseFloat(this.priceProductControl.value.replace(/,/g, ''));
    let exchangeRate: number = parseFloat(this.exchangeRateProductControl.value.replace(/,/g, ''));
    let vat: number = parseFloat(this.vatProductControl.value.replace(/,/g, ''));
    let discountType: DiscountType = this.discountTypeProductControl.value;
    let discountValue: number = parseFloat(this.discountValueProductControl.value.replace(/,/g, ''));

    this.amountProduct = this.roundNumber((quantity * price * exchangeRate), parseInt(this.defaultNumberType, 10));

    if (discountType.code == 'PT') {
      this.amountDiscountProduct = this.roundNumber(((discountValue * quantity * price * exchangeRate) / 100), parseInt(this.defaultNumberType, 10));
      this.amountVatProduct = this.roundNumber((((quantity * price * exchangeRate) - ((discountValue * quantity * price * exchangeRate) / 100)) * (vat / 100)), parseInt(this.defaultNumberType, 10));
      /*Tắt validator kiểm tra Chiết khấu*/
      this.discountValueProductControl.setValidators(null);
      this.discountValueProductControl.updateValueAndValidity();
      /*End*/
    } else if (discountType.code == 'ST') {
      this.amountDiscountProduct = discountValue;
      this.amountVatProduct = this.roundNumber((((quantity * price * exchangeRate) - (discountValue)) * (vat / 100)), parseInt(this.defaultNumberType, 10));
      /*Bật validator kiểm tra Chiết khấu*/
      this.discountValueProductControl.setValidators([compareNumberValidator(this.amountProduct)]);
      this.discountValueProductControl.updateValueAndValidity();
      /*End*/
    }

    this.amountProduct = this.roundNumber((quantity * price * exchangeRate - this.amountDiscountProduct + this.amountVatProduct), parseInt(this.defaultNumberType, 10));
  }
  /*End*/

  /*Event thay đổi số lượng (OC)*/
  changeQuantityOC() {
    let quantity = this.quantityOCControl.value;
    if (!quantity) {
      this.quantityOCControl.setValue('0');
    }

    this.calculatorAmountOC();
  }
  /*End*/

  /*Event thay đổi đơn giá (OC)*/
  changePriceOC() {
    let price = this.priceOCControl.value;
    if (!price) {
      this.priceOCControl.setValue('0');
    }

    this.calculatorAmountOC();
  }
  /*End*/

  /*Event thay đổi giá trị thuế VAT (OC)*/
  changeVatOC() {
    let vat = this.vatOCControl.value;
    if (!vat) {
      this.vatOCControl.setValue('0');
    }

    this.calculatorAmountOC();
  }
  /*End*/

  changeDiscountTypeOC(discountType: DiscountType) {
    this.discountValueOCControl.setValue('0');

    if (discountType.code == 'PT') {
      /*Tắt validator kiểm tra Chiết khấu*/
      this.discountValueOCControl.setValidators(null);
      this.discountValueOCControl.updateValueAndValidity();
      /*End*/
    }

    this.calculatorAmountOC();
  }

  changeDiscountValueOC() {
    let discountValue = 0;
    if (this.discountValueOCControl.value.trim() == '') {
      discountValue = 0;
      this.discountValueOCControl.setValue('0');
    } else {
      discountValue = parseFloat(this.discountValueOCControl.value.replace(/,/g, ''));
    }

    let discountType: DiscountType = this.discountTypeOCControl.value;
    if (discountType.code == 'PT') {
      if (discountValue > 100) {
        discountValue = 100;
        this.discountValueOCControl.setValue('100');
      }

      /*Tắt validator kiểm tra Chiết khấu*/
      this.discountValueOCControl.setValidators(null);
      this.discountValueOCControl.updateValueAndValidity();
      /*End*/
    } else if (discountType.code == 'ST') {
      /*Bật validator kiểm tra Chiết khấu*/
      this.discountValueOCControl.setValidators([compareNumberValidator(this.amountOC)]);
      this.discountValueOCControl.updateValueAndValidity();
      /*End*/
    }

    this.calculatorAmountOC();
  }

  calculatorAmountOC() {
    let quantity: number = parseFloat(this.quantityOCControl.value.replace(/,/g, ''));
    let price: number = parseFloat(this.priceOCControl.value.replace(/,/g, ''));
    let exchangeRate: number = parseFloat(this.exchangeRateOCControl.value.replace(/,/g, ''));
    let vat: number = parseFloat(this.vatOCControl.value.replace(/,/g, ''));
    let discountType: DiscountType = this.discountTypeOCControl.value;
    let discountValue: number = parseFloat(this.discountValueOCControl.value.replace(/,/g, ''));

    this.amountOC = this.roundNumber((quantity * price * exchangeRate), parseInt(this.defaultNumberType, 10));

    if (discountType.code == 'PT') {
      this.amountDiscountOC = this.roundNumber(((discountValue * quantity * price * exchangeRate) / 100), parseInt(this.defaultNumberType, 10));
      this.amountVatOC = this.roundNumber((((quantity * price * exchangeRate) - ((discountValue * quantity * price * exchangeRate) / 100)) * (vat / 100)), parseInt(this.defaultNumberType, 10));

      /*Tắt validator kiểm tra Chiết khấu*/
      this.discountValueOCControl.setValidators(null);
      this.discountValueOCControl.updateValueAndValidity();
      /*End*/
    } else if (discountType.code == 'ST') {
      this.amountDiscountOC = discountValue;
      this.amountVatOC = this.roundNumber((((quantity * price * exchangeRate) - (discountValue)) * (vat / 100)), parseInt(this.defaultNumberType, 10));

      /*Bật validator kiểm tra Chiết khấu*/
      this.discountValueOCControl.setValidators([compareNumberValidator(this.amountOC)]);
      this.discountValueOCControl.updateValueAndValidity();
      /*End*/
    }
  }

  /*Event Hủy dialog*/
  cancel() {
    let result: ResultDialog = {
      status: false,  //Hủy
      quoteDetailModel: null
    };
    this.ref.close(result);
  }
  /*End*/

  /*Event Lưu dialog*/
  save() {
    let result: ResultDialog = {
      status: true,  //Lưu
      quoteDetailModel: new QuoteDetail()
    };

    let quantityProduct: number = parseFloat(this.quantityProductControl.value.replace(/,/g, ''));
    let priceProduct: number = parseFloat(this.priceProductControl.value.replace(/,/g, ''));
    let quantityOC: number = parseFloat(this.quantityOCControl.value.replace(/,/g, ''));
    let priceOC: number = parseFloat(this.priceOCControl.value.replace(/,/g, ''));

    if (priceProduct > 999000000000) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không được lơn hơn 999,000,000,000 VND' };
      this.showMessage(msg);
    }
    else if (priceOC > 999000000000) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không được lơn hơn 999,000,000,000 VND' };
      this.showMessage(msg);
    }
    else if (quantityProduct > 999999) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Số lượng không được lơn hơn 999,999' };
      this.showMessage(msg);
    }
    else if (quantityOC > 999999) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Số lượng không được lơn hơn 999,999' };
      this.showMessage(msg);
    }
    else if (this.amountProduct > 999000000000) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Thành tiền (VND) không được lơn hơn 999,000,000,000 VND' };
      this.showMessage(msg);
    }
    else if (this.amountOC > 999000000000) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Thành tiền (VND) không được lơn hơn 999,000,000,000 VND' };
      this.showMessage(msg);
    }
    else {
      if (this.selectedOrderDetailType == 0) {
        /*Nếu là thêm Sản phẩm*/

        //reset Form Chi phí khác
        this.resetOtherCostsForm();

        //Tổng số thuộc tính:
        let countTotalAttrProduct = this.listAttrProduct.length;
        //Tổng số thuộc tính đã điền giá trị:
        let countCurrentAttrProduct = this.listAttrProduct.filter(x => x.SelectedAttrValue != null);
        //Số lượng sản phẩm
        let quantity = parseFloat(this.quantityProductControl.value.replace(/,/g, ''));

        let checkErr = false;
        countCurrentAttrProduct.forEach(item => {
          if (!checkErr) {
            if (item.SelectedAttrValue.productAttributeCategoryValueId === "" || item.SelectedAttrValue.productAttributeCategoryValueId === null || item.SelectedAttrValue.productAttributeCategoryValueId === undefined) {
              checkErr = true;
            }
            if (item.SelectedAttrValue.productAttributeCategoryValue === "" || item.SelectedAttrValue.productAttributeCategoryValue === null || item.SelectedAttrValue.productAttributeCategoryValue === undefined) {
              checkErr = true;
            }
          }
        })

        if (countCurrentAttrProduct.length < countTotalAttrProduct || checkErr) {
          let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Bạn phải chọn đầy đủ thuộc tính cho sản phẩm/dịch vụ' };
          this.showMessage(msg);
        }
        else if (!this.productForm.valid) {
          Object.keys(this.productForm.controls).forEach(key => {
            if (this.productForm.controls[key].valid == false) {
              this.productForm.controls[key].markAsTouched();
            }
          });
        } else if (quantity <= 0) {
          let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Bạn chưa nhập số lượng' };
          this.showMessage(msg);
        } else {
          let currencyUnit: Category = this.unitMoneyProductControl.value
          result.quoteDetailModel.currencyUnit = currencyUnit.categoryId; //Đơn vị tiền

          result.quoteDetailModel.description = '';
          let discountType: DiscountType = this.discountTypeProductControl.value;
          result.quoteDetailModel.discountType = discountType.value;  //Loại chiết khấu

          let discountValue = parseFloat(this.discountValueProductControl.value.replace(/,/g, ''));
          result.quoteDetailModel.discountValue = discountValue;  //Gía trị chiết khấu

          let exchangeRate = parseFloat(this.exchangeRateProductControl.value.replace(/,/g, ''));
          result.quoteDetailModel.exchangeRate = exchangeRate;  //Tỷ giá

          let product: Product = this.productControl.value;
          let productAllAttrName: string = '';
          if (this.listAttrProduct.length > 0) {
            this.listAttrProduct.forEach(item => {
              productAllAttrName += item.SelectedAttrValue.productAttributeCategoryValue + ';'
            });
            productAllAttrName = '(' + productAllAttrName + ')';
          }
          result.quoteDetailModel.productCode = product.productCode;

          result.quoteDetailModel.incurredUnit = 'CCCCC';
          result.quoteDetailModel.nameMoneyUnit = currencyUnit.categoryCode;
          let vendor: Vendor = this.vendorControl.value;
          if (vendor !== null && vendor !== undefined) {
            result.quoteDetailModel.nameVendor = vendor.vendorCode + ' - ' + vendor.vendorName;
            result.quoteDetailModel.vendorId = vendor.vendorId;
          }
          result.quoteDetailModel.orderDetailType = this.selectedOrderDetailType;
          result.quoteDetailModel.productId = product.productId;
          result.quoteDetailModel.productNameUnit = this.unitProductControl.value;
          result.quoteDetailModel.quantity = parseFloat(this.quantityProductControl.value.replace(/,/g, ''));

          result.quoteDetailModel.quoteProductDetailProductAttributeValue = [];
          let listObjectSelectedProductAttr = this.listAttrProduct.map(x => x.SelectedAttrValue);
          listObjectSelectedProductAttr.forEach(item => {
            var option = new QuoteProductDetailProductAttributeValue();
            option.productId = product.productId;
            option.productAttributeCategoryId = item.productAttributeCategoryId;
            option.productAttributeCategoryValueId = item.productAttributeCategoryValueId;

            result.quoteDetailModel.quoteProductDetailProductAttributeValue.push(option);
          });

          result.quoteDetailModel.sumAmount = this.roundNumber((this.amountProduct), 2);
          result.quoteDetailModel.unitId = product.productUnitId;
          result.quoteDetailModel.unitPrice = (this.priceProductControl.value === null || this.priceProductControl.value === undefined) ? 0 : parseFloat(this.priceProductControl.value.replace(/,/g, ''));
          result.quoteDetailModel.vat = (this.vatProductControl.value === null || this.vatProductControl.value === undefined) ? 0 : parseFloat(this.vatProductControl.value.replace(/,/g, ''));
          result.quoteDetailModel.isPriceInitial = this.IsPriceInitial;
          result.quoteDetailModel.priceInitial = this.priceInitialControl.value ? ParseStringToFloat(this.priceInitialControl.value.toString()) : 0;
          result.quoteDetailModel.amountDiscount = this.amountDiscountProduct;
          result.quoteDetailModel.productName = this.productNameControl.value === null ? '' : this.productNameControl.value.trim();

          if (!this.isCreate) {
            result.quoteDetailModel.orderNumber = this.quoteDetailModel.orderNumber;
          }

          this.ref.close(result);
        }
      } else if (this.selectedOrderDetailType == 1) {
        /*Nếu là thêm Chi phí khác*/

        //reset Form Sản phẩm dịch vụ
        this.resetProductForm();

        //Số lượng
        let quantity = parseFloat(this.quantityOCControl.value.replace(/,/g, ''));

        if (!this.otherCostsForm.valid) {
          Object.keys(this.otherCostsForm.controls).forEach(key => {
            if (this.otherCostsForm.controls[key].valid == false) {
              this.otherCostsForm.controls[key].markAsTouched();
            }
          });
        } else if (quantity <= 0) {
          let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Bạn chưa nhập số lượng' };
          this.showMessage(msg);
        } else {
          let currencyUnit: Category = this.unitMoneyOCControl.value
          result.quoteDetailModel.currencyUnit = currencyUnit.categoryId; //Đơn vị tiền
          result.quoteDetailModel.description = this.descriptionOCControl.value.trim();

          let discountType: DiscountType = this.discountTypeOCControl.value;
          result.quoteDetailModel.discountType = discountType.value;  //Loại chiết khấu

          let discountValue = parseFloat(this.discountValueOCControl.value.replace(/,/g, ''));
          result.quoteDetailModel.discountValue = discountValue;  //Gía trị chiết khấu

          let exchangeRate = parseFloat(this.exchangeRateOCControl.value.replace(/,/g, ''));
          result.quoteDetailModel.exchangeRate = exchangeRate;  //Tỷ giá

          result.quoteDetailModel.explainStr = "";
          result.quoteDetailModel.incurredUnit = this.unitOCControl.value != null ? this.unitOCControl.value.trim() : '';
          result.quoteDetailModel.nameMoneyUnit = currencyUnit.categoryCode;
          result.quoteDetailModel.nameVendor = "";
          result.quoteDetailModel.orderDetailType = this.selectedOrderDetailType;
          result.quoteDetailModel.productId = null;
          result.quoteDetailModel.productNameUnit = "";
          result.quoteDetailModel.quantity = parseFloat(this.quantityOCControl.value.replace(/,/g, ''));
          result.quoteDetailModel.quoteProductDetailProductAttributeValue = [];
          result.quoteDetailModel.sumAmount = this.roundNumber((this.amountOC + this.amountVatOC - this.amountDiscountOC), 2);
          result.quoteDetailModel.unitId = null;
          result.quoteDetailModel.unitPrice = parseFloat(this.priceOCControl.value.replace(/,/g, ''));
          result.quoteDetailModel.vat = parseFloat(this.vatOCControl.value.replace(/,/g, ''));
          result.quoteDetailModel.vendorId = null;
          result.quoteDetailModel.amountDiscount = this.amountDiscountProduct;
          result.quoteDetailModel.priceInitial = 0;
          result.quoteDetailModel.isPriceInitial = false;
          result.quoteDetailModel.productName = this.descriptionOCControl.value.trim();

          if (!this.isCreate) {
            result.quoteDetailModel.orderNumber = this.quoteDetailModel.orderNumber;
          }

          this.ref.close(result);
        }
      }
    }
  }
  /*End*/

  /*Reset form chi phí khác*/
  resetOtherCostsForm() {
    this.descriptionOCControl.reset();
    this.unitOCControl.setValue('');
    this.quantityOCControl.setValue('0');
    this.priceOCControl.setValue('0');
    this.vatOCControl.setValue('0');
    this.discountTypeOCControl.setValue(null);
    this.discountValueOCControl.setValue('0');
    /*Tắt validator kiểm tra Chiết khấu*/
    this.discountValueOCControl.setValidators(null);
    this.discountValueOCControl.updateValueAndValidity();
    /*End*/

    //gán giá trị mặc định cho đơn vị tiền
    let toSelectUnitMoneyOC = this.listUnitMoney.find(x => x.isDefault == true);
    this.unitMoneyOCControl.setValue(toSelectUnitMoneyOC);
    this.unitMoneyOCLabel = toSelectUnitMoneyOC.categoryCode;

    //gán giá trị mặc định cho loại chiết khấu
    let toSelectDiscountTypeOC = this.discountTypeList.find(x => x.code == 'PT');
    this.discountTypeOCControl.setValue(toSelectDiscountTypeOC);

    //Gán giá trị lại cho các biến lưu số thành tiền
    this.amountOC = 0;
    this.amountVatOC = 0;
    this.amountDiscountOC = 0;
  }
  /*End*/

  /*Reset form sản phẩm dịch vụ*/
  resetProductForm() {
    this.productControl.reset();
    this.unitProductControl.setValue('');
    this.vendorControl.reset();
    this.listVendor = [];
    this.quantityProductControl.setValue('0');
    this.priceProductControl.setValue('0');
    let toSelectUnitMoney = this.listUnitMoney.find(x => x.isDefault == true);
    this.unitMoneyProductControl.setValue(toSelectUnitMoney);
    this.unitMoneyLabel = this.listUnitMoney.find(x => x.isDefault == true).categoryCode;
    this.vatProductControl.setValue('0');
    this.discountTypeProductControl.setValue(this.discountTypeList.find(x => x.code == 'PT'));
    this.discountValueProductControl.setValue('0');
    /*Tắt validator kiểm tra Chiết khấu*/
    this.discountValueProductControl.setValidators(null);
    this.discountValueProductControl.updateValueAndValidity();
    /*End*/
    this.listAttrProduct = [];  //list group thuộc tính của sản phẩm

    //Gán giá trị lại cho các biến lưu số thành tiền
    this.amountProduct = 0;
    this.amountVatProduct = 0;
    this.amountDiscountProduct = 0;
  }

  /* Mở popup Tạo nhanh sản phẩm */
  openQuickCreProductDialog() {
    let ref = this.dialogService.open(QuickCreateProductComponent, {
      data: {},
      header: 'Tạo nhanh sản phẩm',
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
        let newProduct: Product = result.newProduct;
        this.listProduct = [newProduct, ...this.listProduct];
        this.productControl.setValue(newProduct ? newProduct : null);
        this.changeProduct(newProduct ? newProduct : null);
      }
    });
  }

  /*End*/

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case 0: {
        result = result;
        break;
      }
      case 1: {
        result = Math.round(number * 10) / 10;
        break;
      }
      case 2: {
        result = Math.round(number * 100) / 100;
        break;
      }
      case 3: {
        result = Math.round(number * 1000) / 1000;
        break;
      }
      case 4: {
        result = Math.round(number * 10000) / 10000;
        break;
      }
      default: {
        result = result;
        break;
      }
    }
    return result;
  }

}

//So sánh giá trị nhập vào với một giá trị xác định
function compareNumberValidator(number: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value !== undefined && (parseFloat(control.value.replace(/,/g, '')) > number)) {
      return { 'numberInvalid': true };
    }
    return null;
  };
}

//Không được nhập chỉ có khoảng trắng
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

//So sánh giá trị nhập vào có thuộc khoảng xác định hay không?
function ageRangeValidator(min: number, max: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value !== undefined && (isNaN(control.value) || control.value < min || control.value > max)) {
      return { 'ageRange': true };
    }
    return null;
  };
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
};

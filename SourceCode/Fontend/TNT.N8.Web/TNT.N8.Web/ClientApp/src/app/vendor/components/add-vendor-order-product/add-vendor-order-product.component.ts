import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService } from 'primeng/api';

import { VendorService } from '../../services/vendor.service';

import { VendorOrderDetailModel } from '../../models/vendorOrderDetail.model';
import { VendorOrderProductDetailProductAttributeValueModel } from '../../models/vendorOrderProductDetailProductAttributeValue.model';

import { QuickCreateProductComponent } from '../../../shared/components/quick-create-product/quick-create-product.component';

interface ConfigDialogOrderDetail {
  isCreate: boolean;
  vendor: vendorCreateOrderModel;
  vendorOrderDetail: VendorOrderDetailModel;
  isProcurementRequestItem: boolean;
  quantityApproval: number;
}

class vendorCreateOrderModel {
  fullAddressVendor: string;
  paymentId: string;
  vendorEmail: string;
  vendorId: string;
  vendorName: string;
  vendorPhone: string;
}

interface ResultDialog {
  status: boolean,
  vendorOrderDetail: VendorOrderDetailModel
}

class DiscountType {
  name: string;
  code: string;
  value: boolean;
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
  listProductAttributeCategory: Array<productAttributeCategory>;
  constructor() {
    this.listProductAttributeCategory = [];
  }
}

class productAttributeCategory {
  productAttributeCategoryId: string;
  productAttributeCategoryName: string;
  productAttributeCategoryValue: Array<productAttributeCategoryValueModel>;
  selectedProductAttributeCategory: productAttributeCategoryValueModel;
  constructor() {
    this.productAttributeCategoryValue = [];
  }
}

class productAttributeCategoryValueModel {
  productAttributeCategoryValueId: string;
  productAttributeCategoryValue1: string;
}

class moneyUnit {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefault: boolean;
}


@Component({
  selector: 'app-add-vendor-order-product',
  templateUrl: './add-vendor-order-product.component.html',
  styleUrls: ['./add-vendor-order-product.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class AddVendorOrderProductComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  /* routing varriable */
  configData: ConfigDialogOrderDetail;
  /* master data */
  listUnitMoney: Array<moneyUnit> = [];
  listProduct: Array<productModel> = [];
  /*Các biến điều kiện*/
  isCreate: boolean = true; //true: Tạo mới sản phẩm dịch vụ(hoặc chi phí phát sinh), false: Sửa sản phẩm dịch vụ(hoặc chi phí phát sinh)
  selectedOrderDetailType: number = 0;  //0: Sản phẩm dịch vụ, 1: Chi phí phát sinh
  isShowRadioProduct: boolean = true;
  isShowRadioOC: boolean = true;
  /*End*/
  /*Các biến nhận giá trị trả về*/
  unitMoneyLabel: string = 'VND';
  unitMoneyOCLabel: string = 'VND';
  listAttrProduct: Array<productAttributeCategory> = [];

  // listProduct: Array<Product> = [];
  amountProduct: number = 0;  //amountProduct = quantityProduct * priceProduct (sản phẩm dịch vụ)
  amountVatProduct: number = 0; //tiền thuế GTGT (sản phẩm dịch vụ)
  amountDiscountProduct: number = 0; //tiền chiết khấu (sản phẩm dịch vụ)
  amountOC: number = 0;
  amountVatOC: number = 0;
  amountDiscountOC: number = 0;
  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];
  cols: any[];
  // customerOrderDetailModel = new CustomerOrderDetail();
  // guaranteeDatetime: Date = null;
  /*End*/

  /*Form sản phẩm dịch vụ*/
  productForm: FormGroup;
  vendorControl: FormControl;
  productControl: FormControl;
  unitProductControl: FormControl;
  quantityProductControl: FormControl;
  priceProductControl: FormControl;
  unitMoneyProductControl: FormControl;
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
    private messageService: MessageService,
    private vendorService: VendorService,
    public dialogService: DialogService,
  ) {
    if (this.config) {
      this.configData = this.config.data;
    }
  }

  ngOnInit() {
    this.setForm();
    this.setTable();
    this.getMasterdata();
  }

  setForm() {
    /*Form Sản phẩm dịch vụ*/

    //Nếu sản phẩm từ Phiếu đề xuất
    if (this.configData.isProcurementRequestItem) {
      this.productControl = new FormControl({ value: null, disabled: true }, [Validators.required]);
      this.quantityProductControl = new FormControl('1', [compareWhitZeroValidator(), Validators.max(this.configData.quantityApproval)]);
    }
    //Nếu sản phẩm không từ Phiếu đề xuất
    else {
      this.productControl = new FormControl(null, [Validators.required]);
      this.quantityProductControl = new FormControl('1', [compareWhitZeroValidator()]);
    }

    this.unitProductControl = new FormControl({ value: '', disabled: true });
    // this.vendorControl = new FormControl(null, [Validators.required]);
    this.priceProductControl = new FormControl('0');
    this.unitMoneyProductControl = new FormControl(null);
    this.exchangeRateProductControl = new FormControl('1');
    this.vatProductControl = new FormControl('0');
    this.discountTypeProductControl = new FormControl(null);
    this.discountValueProductControl = new FormControl('0');

    this.productForm = new FormGroup({
      productControl: this.productControl,
      unitProductControl: this.unitProductControl,
      // vendorControl: this.vendorControl,
      quantityProductControl: this.quantityProductControl,
      priceProductControl: this.priceProductControl,
      unitMoneyProductControl: this.unitMoneyProductControl,
      exchangeRateProductControl: this.exchangeRateProductControl,
      vatProductControl: this.vatProductControl,
      discountTypeProductControl: this.discountTypeProductControl,
      discountValueProductControl: this.discountValueProductControl
    });
    /*End*/

    /*Form Chi phí khác*/
    this.descriptionOCControl = new FormControl('', [Validators.required, forbiddenSpaceText]);
    this.unitOCControl = new FormControl('', [Validators.required, Validators.maxLength(50), forbiddenSpaceText]);
    this.quantityOCControl = new FormControl('1', [compareWhitZeroValidator()]);
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
      { field: 'productAttributeCategoryName', header: 'Tên thuộc tính', width: '50%', textAlign: 'left', color: '#f44336' },
      { field: 'selectedProductAttributeCategory', header: 'Giá trị', width: '50%', textAlign: 'left', color: '#f44336' },
    ];
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.vendorService.getDataAddVendorOrderDetail(this.auth.UserId);
    this.loading = false;
    if (result.statusCode == 200) {
      this.listUnitMoney = result.listMoneyUnit;
      this.listProduct = result.listProductByVendorId;
      this.listProduct.forEach(item => {
        item.productCodeName = item.productCode + ' - ' + item.productName;
      });

      this.setDefaultValueForm();
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  /*Event set giá trị mặc định cho các control*/
  setDefaultValueForm() {
    // this.vendorControl.setValue(this.configData.vendor.vendorName);
    /*Form Sản phẩm dịch vụ*/

    //Đơn vị tiền
    let toSelectUnitMoneyProduct = this.listUnitMoney.find(x => x.isDefault == true);
    if (toSelectUnitMoneyProduct) {
      this.unitMoneyProductControl.setValue(toSelectUnitMoneyProduct);
      this.unitMoneyLabel = toSelectUnitMoneyProduct.categoryCode;
    } else {
      this.unitMoneyProductControl.setValue(this.listUnitMoney[0]);
      this.unitMoneyLabel = this.listUnitMoney[0].categoryCode;
    }

    //Loại chiết khấu
    let toSelectDiscountTypeProduct = this.discountTypeList.find(x => x.code == 'PT');
    this.discountTypeProductControl.setValue(toSelectDiscountTypeProduct);

    /*End: Form Sản phẩm dịch vụ*/

    /*Form Chi phí khác (OC)*/

    //Đơn vị tiền
    let toSelectUnitMoneyOC = this.listUnitMoney.find(x => x.isDefault == true);
    if (toSelectUnitMoneyOC) {
      this.unitMoneyOCControl.setValue(toSelectUnitMoneyOC);
      this.unitMoneyOCLabel = toSelectUnitMoneyOC.categoryCode;
    } else {
      this.unitMoneyOCControl.setValue(this.listUnitMoney[0]);
      this.unitMoneyOCLabel = this.listUnitMoney[0].categoryCode;
    }

    //Loại chiết khấu
    let toSelectDiscountTypeOC = this.discountTypeList.find(x => x.code == 'PT');
    this.discountTypeOCControl.setValue(toSelectDiscountTypeOC);

    /*End: Form Chi phí khác*/

    if (this.configData.isCreate == false) {
      /* CHINH SUA */
      let editModel: VendorOrderDetailModel = this.configData.vendorOrderDetail;
      this.selectedOrderDetailType = editModel.orderDetailType;
      if (editModel.orderDetailType == 0) {
        /* sản phẩm dịch vụ */
        let toSelectProduct = this.listProduct.find(x => x.productId == editModel.productId);
        this.productControl.setValue(toSelectProduct);
        //đơn vị tính
        this.unitProductControl.setValue(editModel.unitName);
        //đơn vị tiền
        this.quantityProductControl.setValue(editModel.quantity.toString());
        this.priceProductControl.setValue(editModel.unitPrice.toString());
        this.unitMoneyProductControl.setValue(this.listUnitMoney.find(x => x.categoryId == editModel.currencyUnit));
        let toSelectExchangeRate = editModel.exchangeRate != null ? editModel.exchangeRate.toString() : '1';
        this.exchangeRateProductControl.setValue(toSelectExchangeRate);
        let toSelectVat = editModel.vat != null ? editModel.vat.toString() : '0';
        this.vatProductControl.setValue(toSelectVat);
        this.discountTypeProductControl.setValue(this.discountTypeList.find(x => x.value == editModel.discountType));
        let discountValue = editModel.discountValue != null ? editModel.discountValue.toString() : '0';
        this.discountValueProductControl.setValue(discountValue);
        //   /*Tắt validator kiểm tra Chiết khấu*/
        this.discountValueProductControl.setValidators(null);
        this.discountValueProductControl.updateValueAndValidity();
        this.calculatorAmountProduct();

        //map table thuộc tính
        this.listAttrProduct = [];

        editModel.listVendorOrderProductDetailProductAttributeValue.forEach(e => {
          let newItem: productAttributeCategory = new productAttributeCategory();
          newItem.productAttributeCategoryId = e.productAttributeCategoryId; //id thuộc tính
          newItem.productAttributeCategoryName = e.productAttributeCategoryName; //tên thuộc tính
          newItem.productAttributeCategoryValue = e.productAttributeCategoryValue;
          // newItem.selectedProductAttributeCategory = e.productAttributeCategoryValue
          let selectedAttribute = newItem.productAttributeCategoryValue.find(attr => attr.productAttributeCategoryValueId == e.productAttributeCategoryValueId);
          newItem.selectedProductAttributeCategory = selectedAttribute ? selectedAttribute : null;

          this.listAttrProduct = [...this.listAttrProduct, newItem];
        });
      } else {
        // chi phí khác

        let description = editModel.description != null ? editModel.description.trim() : '';
        this.descriptionOCControl.setValue(description);

        let unitOC = editModel.incurredUnit != null ? editModel.incurredUnit.trim() : '';
        this.unitOCControl.setValue(unitOC);

        this.quantityOCControl.setValue(editModel.quantity.toString());

        this.priceOCControl.setValue(editModel.unitPrice.toString());

        this.unitMoneyOCControl.setValue(this.listUnitMoney.find(x => x.categoryId == editModel.currencyUnit));

        let toSelectExchangeRate = editModel.exchangeRate != null ? editModel.exchangeRate.toString() : '1';
        this.exchangeRateOCControl.setValue(toSelectExchangeRate);

        let toSelectVat = editModel.vat != null ? editModel.vat.toString() : '0';
        this.vatOCControl.setValue(toSelectVat);

        this.discountTypeOCControl.setValue(this.discountTypeList.find(x => x.value == editModel.discountType));

        let discountValue = editModel.discountValue != null ? editModel.discountValue.toString() : '0';
        this.discountValueOCControl.setValue(discountValue);
        /*Tắt validator kiểm tra Chiết khấu*/
        this.discountValueOCControl.setValidators(null);
        this.discountValueOCControl.updateValueAndValidity();
        /*End*/

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
  changeProduct(event: any) {
    /*reset và setValue các control còn lại*/
    this.unitProductControl.setValue('');
    this.quantityProductControl.setValue('1');
    this.priceProductControl.setValue('0');
    this.unitMoneyProductControl.setValue(this.listUnitMoney.find(x => x.isDefault == true));
    this.exchangeRateProductControl.setValue('1');
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

    let product: productModel = event.value;
    if (product) {
      //Lấy đơn vị tính
      this.unitProductControl.setValue(product.productUnitName);

      //set giá trị cho Đơn giá nếu sản phẩm có giá trị đơn giá(price1)
      if (product.price1 != null) {
        this.priceProductControl.setValue(product.price1.toString());
      }

      product.listProductAttributeCategory.forEach(e => {
        let newRecord: productAttributeCategory = new productAttributeCategory();
        newRecord.productAttributeCategoryName = e.productAttributeCategoryName; //name
        newRecord.productAttributeCategoryValue = e.productAttributeCategoryValue; //list option
        newRecord.productAttributeCategoryId = e.productAttributeCategoryId; //id
        this.listAttrProduct = [...this.listAttrProduct, newRecord];
      });
    }
  }
  /*End*/

  /*Event khi thay đổi Số lượng*/
  changeQuantityProduct() {
    let quantity = this.quantityProductControl.value;
    if (!quantity) {
      this.quantityProductControl.setValue('1');
    }

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
  changeUnitMoneyProduct(unitMoney: moneyUnit) {
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
    } else {
      exchangeRate = parseFloat(this.exchangeRateProductControl.value.replace(/,/g, ''));
      if (exchangeRate == 0) {
        this.exchangeRateProductControl.setValue('1');
      }
    }

    this.calculatorAmountProduct();
  }
  /*End*/

  /*Event khi thay đổi giá trị thuế GTGT*/
  changeVatProduct() {
    let vat = this.vatProductControl.value;
    if (!vat) {
      this.vatProductControl.setValue('0');
    } else {
      vat = parseFloat(this.vatProductControl.value.replace(/,/g, ''));
      if (vat > 100) {
        this.vatProductControl.setValue('100');
      }
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
  }
  /*End*/

  /*Event thay đổi số lượng (OC)*/
  changeQuantityOC() {
    let quantity = this.quantityOCControl.value;
    if (!quantity) {
      this.quantityOCControl.setValue('1');
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

  /*Event thay đổi đơn vị tiền (OC)*/
  changeUnitMoneyOC(unitMoney: moneyUnit) {
    this.unitMoneyOCLabel = unitMoney.categoryCode;
    this.exchangeRateOCControl.setValue('1');

    this.calculatorAmountOC();
  }
  /*End*/

  /*Event thay đổi tỷ giá (OC)*/
  changeExchangeRateOC() {
    let exchangeRate = this.exchangeRateOCControl.value;
    if (!exchangeRate) {
      this.exchangeRateOCControl.setValue('1');
    } else {
      exchangeRate = parseFloat(this.exchangeRateOCControl.value.replace(/,/g, ''));
      if (exchangeRate == 0) {
        this.exchangeRateOCControl.setValue('1');
      }
    }

    this.calculatorAmountOC();
  }
  /*End*/

  /*Event thay đổi giá trị thuế VAT (OC)*/
  changeVatOC() {
    let vat = this.vatOCControl.value;
    if (!vat) {
      this.vatOCControl.setValue('0');
    } else {
      vat = parseFloat(this.vatOCControl.value.replace(/,/g, ''));
      if (vat > 100) {
        this.vatOCControl.setValue('100');
      }
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
        let newProduct: productModel = result.newProduct;
        this.listProduct = [newProduct, ...this.listProduct];
        this.productControl.setValue(newProduct ? newProduct : null);
        let event = {
          value: newProduct ? newProduct : null
        }
        this.changeProduct(event);
      }
    });
  }

  /*Event Hủy dialog*/
  cancel() {
    let result: ResultDialog = {
      status: false,  //Hủy
      vendorOrderDetail: null
    };
    this.ref.close(result);
  }
  /*End*/

  /*Event Lưu dialog*/
  save() {
    if (this.selectedOrderDetailType == 0) {
      /*Nếu là thêm Sản phẩm dịch vụ*/

      //reset Form Chi phí khác
      this.resetOtherCostsForm();

      //Tổng số thuộc tính:
      let countTotalAttrProduct = this.listAttrProduct.length;
      //Tổng số thuộc tính đã điền giá trị:
      let countCurrentAttrProduct = this.listAttrProduct.filter(x => x.selectedProductAttributeCategory != null).length;
      // Đơn giá 
      let price = parseFloat(this.priceProductControl.value.replace(/,/g, ''));
      // số lượng
      let quantity = parseFloat(this.quantityProductControl.value.replace(/,/g, ''));


      if (countCurrentAttrProduct < countTotalAttrProduct) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Bạn phải chọn đầy đủ thuộc tính cho sản phẩm/dịch vụ' };
        this.showMessage(msg);
      } else if (quantity > 999999) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Số lượng không được lơn hơn 999,999' };
        this.showMessage(msg);
      } else if (price > 999000000000) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không được lơn hơn 999,000,000,000 VND' };
        this.showMessage(msg);
      } else if (this.amountProduct > 999000000000) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Thành tiền (VND) không được lơn hơn 999,000,000,000 VND' };
        this.showMessage(msg);
      } else if (!this.productForm.valid) {
        Object.keys(this.productForm.controls).forEach(key => {
          if (this.productForm.controls[key].valid == false) {
            this.productForm.controls[key].markAsTouched();
          }
        });
      } else {
        let productOrderModel = this.mapProductFormToModel();
        let result: ResultDialog = {
          status: true,
          vendorOrderDetail: productOrderModel
        }
        this.ref.close(result);
      }
    } else if (this.selectedOrderDetailType == 1) {
      /*Nếu là thêm Chi phí khác*/

      //reset Form Sản phẩm dịch vụ
      this.resetProductForm();

      // Đơn giá 
      let price = parseFloat(this.priceOCControl.value.replace(/,/g, ''));
      // số lượng
      let quantity = parseFloat(this.quantityOCControl.value.replace(/,/g, ''));

      if (!this.otherCostsForm.valid) {
        Object.keys(this.otherCostsForm.controls).forEach(key => {
          if (this.otherCostsForm.controls[key].valid == false) {
            this.otherCostsForm.controls[key].markAsTouched();
          }
        });
      } else if (quantity > 999999) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Số lượng không được lơn hơn 999,999' };
        this.showMessage(msg);
      } else if (price > 999000000000) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không được lơn hơn 999,000,000,000 VND' };
        this.showMessage(msg);
      } else if (this.amountOC > 999000000000) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Thành tiền (VND) không được lơn hơn 999,000,000,000 VND' };
        this.showMessage(msg);
      } else {
        let otherCostDetailModel: VendorOrderDetailModel = new VendorOrderDetailModel();
        otherCostDetailModel.description = this.descriptionOCControl.value.trim();
        let discountType: DiscountType = this.discountTypeOCControl.value;
        otherCostDetailModel.discountType = discountType.value;  //Loại chiết khấu
        let discountValue = parseFloat(this.discountValueOCControl.value.replace(/,/g, ''));
        otherCostDetailModel.discountValue = discountValue;  //Gía trị chiết khấu
        let exchangeRate = parseFloat(this.exchangeRateOCControl.value.replace(/,/g, ''));
        otherCostDetailModel.exchangeRate = exchangeRate;  //Tỷ giá
        otherCostDetailModel.incurredUnit = this.unitOCControl.value != null ? this.unitOCControl.value.trim() : '';
        otherCostDetailModel.unitName = otherCostDetailModel.incurredUnit;
        let currency: moneyUnit = this.unitMoneyOCControl.value;
        otherCostDetailModel.currencyUnit = currency.categoryId; //currency.categoryCode;
        otherCostDetailModel.currencyUnitName = currency.categoryName;
        otherCostDetailModel.vendorId = this.configData.vendor == null ? null : this.configData.vendor.vendorId;
        otherCostDetailModel.vendorName = this.configData.vendor == null ? null : this.configData.vendor.vendorName;
        otherCostDetailModel.orderDetailType = this.selectedOrderDetailType;
        otherCostDetailModel.productId = null;
        otherCostDetailModel.quantity = parseFloat(this.quantityOCControl.value.replace(/,/g, ''));
        otherCostDetailModel.sumAmount = this.roundNumber((this.amountOC + this.amountVatOC - this.amountDiscountOC), parseInt(this.defaultNumberType, 10));
        otherCostDetailModel.productUnitId = null;
        otherCostDetailModel.unitPrice = parseFloat(this.priceOCControl.value.replace(/,/g, ''));
        otherCostDetailModel.vat = parseFloat(this.vatOCControl.value.replace(/,/g, ''));
        otherCostDetailModel.sumAmountDiscount = this.amountDiscountProduct;
        otherCostDetailModel.procurementRequestItemId = null;
        otherCostDetailModel.procurementRequestId = null;
        otherCostDetailModel.procurementCode = null;

        if (this.configData.isCreate) {
          otherCostDetailModel.isReshow = false;
        } else {
          otherCostDetailModel.isReshow = this.configData.vendorOrderDetail.isReshow;
        }

        let result: ResultDialog = {
          status: true,
          vendorOrderDetail: otherCostDetailModel
        }
        this.ref.close(result);
      }
    }
  }
  /*End*/

  /*  Map form sản phẩm/ dịch vụ */
  mapProductFormToModel(): VendorOrderDetailModel {
    let vendorOrderDetail = new VendorOrderDetailModel();
    vendorOrderDetail.vendorOrderDetailId = this.emptyGuid;
    vendorOrderDetail.vendorOrderId = this.emptyGuid;

    //loại sp/dv
    vendorOrderDetail.orderDetailType = this.selectedOrderDetailType;

    let _product: productModel = this.productControl.value;
    vendorOrderDetail.productId = _product.productId;
    vendorOrderDetail.productUnitId = _product.productUnitId;
    vendorOrderDetail.description = this.buildDescription();
    vendorOrderDetail.vendorId = this.configData.vendor == null ? null : this.configData.vendor.vendorId;
    vendorOrderDetail.vendorName = this.configData.vendor == null ? null : this.configData.vendor.vendorName;
    vendorOrderDetail.quantity = parseFloat(this.quantityProductControl.value.replace(/,/g, ''));
    vendorOrderDetail.unitName = this.unitProductControl.value;
    vendorOrderDetail.unitPrice = parseFloat(this.priceProductControl.value.replace(/,/g, ''));

    let currency: moneyUnit = this.unitMoneyProductControl.value;
    vendorOrderDetail.currencyUnit = currency.categoryId;
    vendorOrderDetail.currencyUnitName = currency.categoryName;

    vendorOrderDetail.exchangeRate = parseFloat(this.exchangeRateProductControl.value.replace(/,/g, ''));

    let discountType: DiscountType = this.discountTypeProductControl.value;
    vendorOrderDetail.discountType = discountType.value;

    let discountValue = parseFloat(this.discountValueProductControl.value.replace(/,/g, ''));
    vendorOrderDetail.discountValue = discountValue;  //Gía trị chiết khấu

    let exchangeRate = parseFloat(this.exchangeRateProductControl.value.replace(/,/g, ''));
    vendorOrderDetail.exchangeRate = exchangeRate;  //Tỷ giá

    vendorOrderDetail.sumAmountDiscount = this.amountDiscountProduct;
    vendorOrderDetail.sumAmountVat = this.amountVatProduct;
    vendorOrderDetail.vat = parseFloat(this.vatProductControl.value.replace(/,/g, ''));
    vendorOrderDetail.sumAmount = this.roundNumber((this.amountProduct + this.amountVatProduct - this.amountDiscountProduct), parseInt(this.defaultNumberType, 10));

    if (this.configData.isCreate) {
      vendorOrderDetail.isReshow = false;
      vendorOrderDetail.procurementRequestItemId = null;
      vendorOrderDetail.procurementRequestId = null;
      vendorOrderDetail.procurementCode = null;
    } else {
      vendorOrderDetail.isReshow = this.configData.vendorOrderDetail.isReshow;
      vendorOrderDetail.procurementRequestItemId = this.configData.vendorOrderDetail.procurementRequestItemId;
      vendorOrderDetail.procurementRequestId = this.configData.vendorOrderDetail.procurementRequestId;
      vendorOrderDetail.procurementCode = this.configData.vendorOrderDetail.procurementCode;
    }

    vendorOrderDetail.listVendorOrderProductDetailProductAttributeValue = [];

    this.listAttrProduct.forEach(e => {
      let item: VendorOrderProductDetailProductAttributeValueModel = new VendorOrderProductDetailProductAttributeValueModel();
      item.orderProductDetailProductAttributeValueId = this.emptyGuid;
      item.vendorOrderDetailId = this.emptyGuid;
      item.productId = _product.productId;

      item.productAttributeCategoryId = e.productAttributeCategoryId; //tên thuộc tính
      item.productAttributeCategoryName = e.productAttributeCategoryName; //id thuộc tính
      item.productAttributeCategoryValue = e.productAttributeCategoryValue; //danh sách giá trị thuộc tính
      item.productAttributeCategoryValueId = e.selectedProductAttributeCategory.productAttributeCategoryValueId //thuộc tính đã được chọn

      vendorOrderDetail.listVendorOrderProductDetailProductAttributeValue.push(item);
    });
    return vendorOrderDetail;
  }

  buildDescription(): string {
    let _product: productModel = this.productControl.value;
    let _productName = _product ? _product.productName : '';

    let listAttrName: Array<string> = [];
    this.listAttrProduct.forEach(e => {
      let newAttrName = e.productAttributeCategoryName + ":" + e.selectedProductAttributeCategory.productAttributeCategoryValue1;
      listAttrName.push(newAttrName);
    });

    let attrName = '';
    if (listAttrName.length > 0) {
      attrName = "(" + listAttrName.join(', ') + ")";
    }
    let result = _productName + " " + attrName;

    return result;
  }


  /*Reset form chi phí khác*/
  resetOtherCostsForm() {
    this.descriptionOCControl.reset();
    this.unitOCControl.setValue('');
    this.quantityOCControl.setValue('1');
    this.priceOCControl.setValue('0');
    this.vatOCControl.setValue('0');
    this.discountTypeOCControl.setValue(null);
    this.discountValueOCControl.setValue('0');
    /*Tắt validator kiểm tra Chiết khấu*/
    this.discountValueOCControl.setValidators(null);
    this.discountValueOCControl.updateValueAndValidity();
    /*End*/
    let toSelectUnitMoneyOC = this.listUnitMoney.find(x => x.isDefault == true);
    if (toSelectUnitMoneyOC === undefined) toSelectUnitMoneyOC = this.listUnitMoney[0];

    this.unitMoneyOCControl.setValue(toSelectUnitMoneyOC);
    this.unitMoneyOCLabel = toSelectUnitMoneyOC.categoryCode;
    this.exchangeRateOCControl.setValue('1');

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
    // this.vendorControl.reset();
    this.quantityProductControl.setValue('1');
    this.priceProductControl.setValue('0');
    let defaultMoney = this.listUnitMoney.find(x => x.isDefault == true);
    this.unitMoneyProductControl.setValue(defaultMoney ? defaultMoney : null);
    this.exchangeRateProductControl.setValue('1');
    // this.unitMoneyLabel = this.listUnitMoney.find(x => x.isDefault == true).categoryCode;
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
  /*End*/

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case -1: {
        result = result;
        break;
      }
      case 0: {
        result = Math.round(number);
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

//Valid when is zero
function compareWhitZeroValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value !== undefined && (parseFloat(control.value.replace(/,/g, '')) == 0)) {
      return { 'zeroInvalid': true };
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

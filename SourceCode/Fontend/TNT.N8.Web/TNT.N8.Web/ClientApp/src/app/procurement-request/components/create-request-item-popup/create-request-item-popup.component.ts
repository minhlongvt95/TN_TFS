import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { NumberToStringPipe } from '../../../shared/ConvertMoneyToString/numberToString.pipe';
import { ProcurementRequestService } from '../../services/procurement-request.service';

/* Primeng API */
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';
/* end PrimeNg API */

import { QuickCreateVendorComponent } from '../../../shared/components/quick-create-vendor/quick-create-vendor.component';
import { QuickCreateProductComponent } from '../../../shared/components/quick-create-product/quick-create-product.component';
/* data đẩy vào dialog */
interface ConfigData {
  isEdit: boolean;
  prItemEdit: procurementRequestItemModel
}

/* data trả về */
interface DialogResult {
  status: boolean;
  prItem: procurementRequestItemModel;
}

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

// data đơn vị tiền
class moneyUnit {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class procurementPlan {
  procurementPlanId: string;
  procurementPlanCode: string;
  productCategoryId: string;
}

class productModel {
  productId: string;
  productName: string;
  productCode: string;
  productCodeName: string;
  price1: number;
  productMoneyUnitId: string;
  productUnitId: string;
  productUnitName: string;
  productCategoryId: string;
}

class vendorModel {
  vendorId: string;
  vendorName: string;
  vendorCode: string;
  listProductId: Array<string>;
  constructor() {
    this.listProductId = [];
  }
}

class procurementRequestItemModel {
  procurementRequestItemId: string;
  productId: string;
  vendorId: string;
  quantity: number;
  unitPrice: number;
  currencyUnit: string;
  exchangeRate: number;
  procurementRequestId: string;
  procurementPlanId: string;
  createdById: string;
  createdDate: Date;
  //label name
  vendorName: string;
  productName: string;
  productCode: string;
  procurementPlanCode: string;
  productUnit: string;
  amount: number;
  description: string;
  orderDetailType: string;
  discountType: boolean;
  discountValue: number;
  vat: number;
  unitId: string;
  nameMoneyUnit: string;
  incurredUnit: string;
}

class vendorCreateOrderModel {
  fullAddressVendor: string;
  listVendorContact: Array<vendorContact>;
  paymentId: string;
  vendorEmail: string;
  vendorId: string;
  vendorName: string;
  vendorPhone: string;
  constructor() {
    this.listVendorContact = [];
  }
}

class vendorContact {
  contactId: string;
  fullName: string;
  phone: string;
  email: string;
}

interface DiscountType {
  name: string;
  code: string;
  value: boolean;
}

@Component({
  selector: 'app-create-request-item-popup',
  templateUrl: './create-request-item-popup.component.html',
  styleUrls: ['./create-request-item-popup.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class CreateRequestItemPopupComponent implements OnInit {
  loading: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  auth: any = JSON.parse(localStorage.getItem("auth"));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;  //Số chữ số thập phân sau dấu phẩy
  //master data
  listMoneyUnit: Array<moneyUnit> = [];
  listProcurementPlan: Array<procurementPlan> = []; //mã dự toán
  listProduct: Array<productModel> = [];
  listVendor: Array<vendorModel> = [];
  listCurrentVendor: Array<vendorModel> = [];
  listCurrentProduct: Array<productModel> = [];
  listCurrentProcurementPlan: Array<procurementPlan> = [];
  //form sản phẩm
  createPRItemForm: FormGroup;

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

  isVND: boolean = true;
  messageTitle: string = '';
  message: string = '';

  listVendorCreateOrderModel: Array<vendorCreateOrderModel> = [];
  isShowRadioProduct: boolean = true;
  isShowRadioOC: boolean = true;
  selectedOrderDetailType: number = 0;
  unitMoneyOCLabel: string = 'VND';
  amountOC: number = 0;
  amountVatOC: number = 0;
  amountDiscountOC: number = 0;
  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];
  isCreate: boolean = true; //true: Tạo mới sản phẩm dịch vụ(hoặc chi phí phát sinh), false: Sửa sản phẩm dịch vụ(hoặc chi phí phát sinh)

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private translate: TranslateService,
    private procurementRequestService: ProcurementRequestService,
    private dialogService: DialogService,
  ) {
    this.isCreate = this.config.data.isCreate;
  }

  ngOnInit() {
    this.initForm();
    this.getMasterdata();
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  changeVendor(event: any) {
    this.createPRItemForm.get('Quantity').reset();
    if (event.value === null) {
      return false;
    }
    return false;
  }

  changeProduct(event: any) {
    this.createPRItemForm.get('Price').reset();
    this.createPRItemForm.get('Quantity').reset();
    this.createPRItemForm.get('ProductUnit').reset();
    this.createPRItemForm.get('ProductCode').reset();
    // this.setDefaultMoneyUnit();
    this.isVND = true;
    if (event.value === null) {
      return false
    };

    let currentProduct: productModel = event.value;
    /* reset đơn vị tính */
    this.createPRItemForm.get('ProductUnit').patchValue(currentProduct.productUnitName);
    /* loại tiền */
    // let _moneyUnit = this.listMoneyUnit.find(e => e.categoryId == currentProduct.productMoneyUnitId);
    // this.createPRItemForm.get('PriceUnit').patchValue(_moneyUnit ? _moneyUnit : null);
    // if (_moneyUnit === null || _moneyUnit === undefined || _moneyUnit.categoryCode === "VND") {
    //   this.isVND = true;
    //   // this.setDefaultMoneyUnit();
    // } else {
    //   this.isVND = false;
    // }
    /* đơn giá */
    this.createPRItemForm.get('Price').patchValue((currentProduct.price1 === null || currentProduct.price1 === undefined) ? 0 : currentProduct.price1);
    /* mã sản phẩm */
    this.createPRItemForm.get('ProductCode').patchValue(currentProduct.productCode);
    /* mã dự toán */
    this.listCurrentProcurementPlan = this.listProcurementPlan.filter(e => e.productCategoryId == currentProduct.productCategoryId);
    if (this.listCurrentProcurementPlan.length === 0) {
      this.clearToast();
      this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
      this.translate.get('procurement-request.messages_error.no_MDT').subscribe(value => { this.message = value; });
      this.showToast('warn', this.messageTitle, this.message);
    }

    return false;
  }

  initForm() {
    this.createPRItemForm = new FormGroup({
      'Vendor': new FormControl(null), //, [Validators.required]),
      'Description': new FormControl(null),
      'Product': new FormControl(null, [Validators.required]),
      'ProductCode': new FormControl(null, [Validators.required]),
      'ProductUnit': new FormControl(null, [Validators.required]),// đơn vị tính
      'PriceUnit': new FormControl(null, [Validators.required]), //đơn vị tiền
      'Price': new FormControl('0', [Validators.required]), //đơn giá
      'ProcurementPlan': new FormControl(''),//, [Validators.required]), //mã dự toán
      'Quantity': new FormControl('0', [Validators.required, Validators.min(1)]),
      'AmountNumber': new FormControl('0'),
      'AmountText': new FormControl(''),
      'ExchangeRate': new FormControl('1', [Validators.required])
    });

    this.createPRItemForm.get('ProductUnit').disable();
    this.createPRItemForm.get('ProductCode').disable();
    this.createPRItemForm.get('AmountNumber').disable();
    this.createPRItemForm.get('AmountText').disable();

    /*Form Chi phí khác*/
    this.descriptionOCControl = new FormControl('', [Validators.required, forbiddenSpaceText]);
    this.unitOCControl = new FormControl('', [Validators.required, Validators.maxLength(50), forbiddenSpaceText]);
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

  setDefaultValueForm() {
    let defaultMoneyUnit = this.listMoneyUnit.find(e => e.isDefault == true);
    this.createPRItemForm.get('PriceUnit').setValue(defaultMoneyUnit ? defaultMoneyUnit : this.listMoneyUnit[0]);
    this.unitMoneyOCControl.setValue(defaultMoneyUnit ? defaultMoneyUnit : this.listMoneyUnit[0]);
    this.unitMoneyOCLabel = defaultMoneyUnit.categoryCode;

    //Loại chiết khấu
    let toSelectDiscountTypeOC = this.discountTypeList.find(x => x.code == 'PT');
    this.discountTypeOCControl.setValue(toSelectDiscountTypeOC);
  }

  /* thay đổi đơn giá */
  changeUnitPrice() {
    this.calculateAmount();
  }

  /* thay đổi số lượng */
  changeQuantity() {
    // var today = new Date();
    var quantity = parseFloat(this.createPRItemForm.get('Quantity').value.replace(/,/g, ''));
    var productId = this.createPRItemForm.get('Product').value?.productId;
    var vendorId = this.createPRItemForm.get('Vendor').value?.vendorId;
    /* #region  Bắt đầu lấy giá NCC */
    if (this.createPRItemForm.get('Product').value != null) {
      this.procurementRequestService.searchVendorProductPrice(productId, vendorId, quantity).subscribe(response => {
        let result = <any>response;
        this.loading = false;
        if (result.statusCode == 200) {
          var vendorProductPrice = result.vendorProductPrice
          if (vendorProductPrice) {
            this.createPRItemForm.get('Price').setValue(vendorProductPrice.price);
          } else {
            this.createPRItemForm.get('Price').setValue(0);
          }
          this.calculateAmount();
        }
      });
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', 'Phải chọn sản phẩm trước');
    }

    /* #endregion */

  }

  /* thay đổi đơn vị tiền */
  changePriceUnit(event: any) {
    if (event.value === null) return false;
    let currentPriceUnit: moneyUnit = event.value;
    if (currentPriceUnit.categoryCode === "VND") {
      this.createPRItemForm.get('ExchangeRate').reset();
      this.createPRItemForm.get('ExchangeRate').setValue('1');
      this.isVND = true;
    } else {
      this.createPRItemForm.get('ExchangeRate').reset();
      this.createPRItemForm.get('ExchangeRate').setValue('1');
      this.isVND = false;
    }

    this.calculateAmount();
    return false;
  }

  /* thay đổi tỷ giá */
  changeExchangeRate() {
    this.calculateAmount();
  }

  calculateAmount(): number {
    let result: number = 0;
    let unitPrice = ParseStringToFloat(this.createPRItemForm.get('Price').value);
    let quantity = ParseStringToFloat(this.createPRItemForm.get('Quantity').value);
    let exchange = ParseStringToFloat(this.createPRItemForm.get('ExchangeRate').value);
    //tính tổng giá trị
    result = this.roundNumber((unitPrice * quantity * exchange), parseInt(this.defaultNumberType, 10));
    this.createPRItemForm.get('AmountNumber').patchValue(result);
    //chuyển thành text
    const moneyPipe = new NumberToStringPipe();
    var amountText = moneyPipe.transform(result, this.defaultNumberType);
    this.createPRItemForm.get('AmountText').patchValue(amountText);
    return result;
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.procurementRequestService.getDataCreateProcurementRequestItem(this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listMoneyUnit = result.listMoneyUnit;
      this.listProcurementPlan = result.listProcurementPlan;
      this.listVendor = result.listVendor;
      this.listProduct = result.listProduct;
      this.listProduct.forEach(item => {
        item.productCodeName = item.productCode + ' - ' + item.productName;
      });

      //set mac dinh data
      this.setDefaultValueForm();
      //patch data nếu là edit mode
      if (this.config.data) {
        let configData: ConfigData = this.config.data;
        if (configData.isEdit === true) this.patchDataToForm(configData.prItemEdit);
      }

    } else {
      this.clearToast();
      this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
      this.showToast('error', this.messageTitle, result.messageCode);
    }
  }

  patchDataToForm(prItemEdit: procurementRequestItemModel) {
    if (prItemEdit.orderDetailType == "0" || prItemEdit.orderDetailType == null || prItemEdit.orderDetailType == undefined){
      let _product = this.listProduct.find(e => e.productId == prItemEdit.productId);
      this.listCurrentProcurementPlan = this.listProcurementPlan.filter(e => e.productCategoryId == _product.productCategoryId);
      //nhà cung cấp
      let _vendor = this.listVendor.find(e => e.vendorId == prItemEdit.vendorId);
      this.createPRItemForm.get('Vendor').patchValue(_vendor ? _vendor : null);
      //sản phẩm
      this.createPRItemForm.get('Product').patchValue(_product ? _product : null);
      this.createPRItemForm.get('ProductCode').patchValue(_product ? _product.productCode : '');
      this.createPRItemForm.get('ProductUnit').patchValue(_product ? _product.productUnitName : '');
      //đơn vị tiền
      let _currencyUnit = this.listMoneyUnit.find(e => e.categoryId == prItemEdit.currencyUnit);
      //nếu đơn vị tiền khác VND thì hiện tỷ giá
      // if (_currencyUnit.categoryCode !== "VND") {
      //   this.isVND = false;
      // } else {
      //   this.isVND = true;
      // }
      this.createPRItemForm.get('Description').patchValue(prItemEdit.description);
      this.createPRItemForm.get('PriceUnit').patchValue(_currencyUnit ? _currencyUnit : null);
      //tỷ giá
      this.createPRItemForm.get('ExchangeRate').patchValue(prItemEdit.exchangeRate ? prItemEdit.exchangeRate : '1');
      //đơn giá
      this.createPRItemForm.get('Price').patchValue(prItemEdit.unitPrice ? prItemEdit.unitPrice : '0');
      // mã dự toán
      var _procurementPlan = this.listProcurementPlan.find(e => e.procurementPlanId == prItemEdit.procurementPlanId);
      this.createPRItemForm.get('ProcurementPlan').patchValue(_procurementPlan ? _procurementPlan : null);
      //số lượng
      this.createPRItemForm.get('Quantity').patchValue(prItemEdit.quantity ? prItemEdit.quantity : '0');
      //tính thành tineef
      this.calculateAmount();
    }
    else if (prItemEdit.orderDetailType == '1'){
      
      // type 
      this.selectedOrderDetailType = 1;

      // số lượng
      this.quantityOCControl.setValue(prItemEdit.quantity);

      // đơn vị tính
      this.unitOCControl.setValue(prItemEdit.incurredUnit);

      //đơn vị tiền
      let _currencyUnit = this.listMoneyUnit.find(e => e.categoryId == prItemEdit.currencyUnit);
      this.unitMoneyOCControl.setValue(_currencyUnit);
      this.unitMoneyOCLabel = _currencyUnit.categoryCode;

      // đơn giá
      this.priceOCControl.setValue(prItemEdit.unitPrice);

      // tỷ giá
      this.exchangeRateOCControl.setValue(prItemEdit.exchangeRate ? prItemEdit.exchangeRate : null)

      // thành tiền
      this.amountOC = prItemEdit.amount

      // Diễn giải
      this.descriptionOCControl.setValue(prItemEdit.description);

      //thuế
      this.vatOCControl.setValue(prItemEdit.vat ? prItemEdit.vat : 0);

      // loại chiết khấu
      let discountType = this.discountTypeList.find(x => x.value == prItemEdit.discountType);
      this.discountTypeOCControl.setValue(discountType)

      // CK
      this.discountValueOCControl.setValue(prItemEdit.discountValue)

      // thành tiền chiết khấu và thuế
      let quantity: number = this.quantityOCControl.value;
      let price: number = this.priceOCControl.value;
      let exchangeRate: number = this.exchangeRateOCControl.value;
      let vat: number = this.vatOCControl.value;
      let discountValue: number = this.discountValueOCControl.value;

      this.amountOC = this.roundNumber((quantity * price * exchangeRate), parseInt(this.defaultNumberType, 10));

      if (discountType) {
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
    }

  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case -1: {
        result = result;
        break;
      }
      case 0: {
        result = Math.round(result);
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

  cancel() {
    this.ref.close();
  }

  save() {
    if (this.selectedOrderDetailType == 0) {
      if (!this.createPRItemForm.valid) {
        Object.keys(this.createPRItemForm.controls).forEach(key => {
          if (!this.createPRItemForm.controls[key].valid) {
            this.createPRItemForm.controls[key].markAsTouched();
          }
        });
      } else {
        let price = ParseStringToFloat(this.createPRItemForm.get('Price').value);
        let quantity = ParseStringToFloat(this.createPRItemForm.get('Quantity').value);
        let exchange = ParseStringToFloat(this.createPRItemForm.get('ExchangeRate').value);
        let amount = ParseStringToFloat(this.createPRItemForm.get('AmountNumber').value);

        // if (price <= 0) {
        //   this.clearToast();
        //   this.showToast('warn', this.messageTitle, 'Đơn giá cần lớn hơn 0');
        //   return;
        // }
        this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
        if (quantity <= 0) {
          this.clearToast();
          this.translate.get('procurement-request.messages_error.quatity_min0').subscribe(value => { this.message = value; });
          this.showToast('warn', this.messageTitle, this.message);
          return;
        }

        if (exchange <= 0) {
          this.clearToast();
          this.translate.get('procurement-request.messages_error.exchange_min0').subscribe(value => { this.message = value; });
          this.showToast('warn', this.messageTitle, this.message);
          return;
        }

        if (quantity > 999999) {
          this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: 'Số lượng không được lơn hơn 999,999' });
        } else if (price > 999000000000) {
          this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không được lơn hơn 999,000,000,000 VND' });
        } else if (amount > 999000000000) {
          this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: 'Thành tiền không được lơn hơn 999,000,000,000 VND' });
        } else {
          //valid form
          let prRequestItem: procurementRequestItemModel = this.mapFormToPRModel();
          let result: DialogResult = {
            status: true,
            prItem: prRequestItem
          }
          this.ref.close(result);
        }
      }
    }
    else if (this.selectedOrderDetailType == 1) {
      if (!this.otherCostsForm.valid) {
        Object.keys(this.otherCostsForm.controls).forEach(key => {
          if (!this.otherCostsForm.controls[key].valid) {
            this.otherCostsForm.controls[key].markAsTouched();
          }
        });
      } else {
        let price = ParseStringToFloat(this.priceOCControl.value);
        let quantity = ParseStringToFloat(this.quantityOCControl.value);
        let exchange = ParseStringToFloat(this.exchangeRateOCControl.value);
        let amount = this.amountOC;

        // if (price <= 0) {
        //   this.clearToast();
        //   this.showToast('warn', this.messageTitle, 'Đơn giá cần lớn hơn 0');
        //   return;
        // }
        this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
        if (quantity <= 0) {
          this.clearToast();
          this.translate.get('procurement-request.messages_error.quatity_min0').subscribe(value => { this.message = value; });
          this.showToast('warn', this.messageTitle, this.message);
          return;
        }

        if (exchange <= 0) {
          this.clearToast();
          this.translate.get('procurement-request.messages_error.exchange_min0').subscribe(value => { this.message = value; });
          this.showToast('warn', this.messageTitle, this.message);
          return;
        }

        if (quantity > 999999) {
          this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: 'Số lượng không được lơn hơn 999,999' });
        } else if (price > 999000000000) {
          this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không được lơn hơn 999,000,000,000 VND' });
        } else if (amount > 999000000000) {
          this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: 'Thành tiền không được lơn hơn 999,000,000,000 VND' });
        } else {
          //valid form
          let prRequestItem: procurementRequestItemModel = this.mapFormToPRModelOC();
          let result: DialogResult = {
            status: true,
            prItem: prRequestItem
          }
          this.ref.close(result);
        }
      }
    }
  }

  mapFormToPRModelOC(): procurementRequestItemModel {
    let newPR = new procurementRequestItemModel();
    newPR.procurementRequestItemId = this.emptyGuid;

    //map lại Id cũ nếu là edit
    if (this.config.data) {
      let configData: ConfigData = this.config.data;
      if (configData.isEdit === true) newPR.procurementRequestItemId = configData.prItemEdit.procurementRequestItemId;
    }

    // đơn vị tiền
    let currencyUnit: moneyUnit = this.unitMoneyOCControl.value
    newPR.currencyUnit = currencyUnit.categoryId; //Đơn vị tiền
    newPR.nameMoneyUnit = currencyUnit.categoryName;

    newPR.description = this.descriptionOCControl.value.trim();

    let discountType: DiscountType = this.discountTypeOCControl.value;
    newPR.discountType = discountType.value;  //Loại chiết khấu

    let discountValue = parseFloat(this.discountValueOCControl.value.replace(/,/g, ''));
    newPR.discountValue = discountValue;  //Gía trị chiết khấu

    let exchangeRate = this.exchangeRateOCControl.value;
    newPR.exchangeRate = ParseStringToFloat(exchangeRate);  //Tỷ giá

    // newPR.ExplainStr = newPRDescription;
    newPR.incurredUnit = this.unitOCControl.value != null ? this.unitOCControl.value.trim() : '';
    newPR.nameMoneyUnit = currencyUnit.categoryCode;
    // newPR.NameVendor = "";
    // newPR.OrderDetailType = this.selectedOrderDetailType;
    // newPR.ProductId = null;
    // newPR.ProductNameUnit = "";
    newPR.productUnit = this.unitOCControl.value != null ? this.unitOCControl.value.trim() : '';
    newPR.quantity = parseFloat(this.quantityOCControl.value.replace(/,/g, ''));
    // newPR.OrderProductDetailProductAttributeValue = [];
    newPR.amount = this.roundNumber((this.amountOC + this.amountVatOC - this.amountDiscountOC), 2);
    newPR.unitId = null;
    newPR.unitPrice = parseFloat(this.priceOCControl.value.replace(/,/g, ''));
    newPR.vat = parseFloat(this.vatOCControl.value.replace(/,/g, ''));
    newPR.vendorId = null;
    // newPR.discountValue = this.amountDiscountProduct;
    // newPR.IsPriceInitial = false;
    // newPR.PriceInitial = 0;
    // newPR.WarehouseId = null;
    // newPR.ActualInventory = 0;
    // newPR.BusinessInventory = 0;
    newPR.productName = this.descriptionOCControl.value.trim();
    // newPR.GuaranteeTime = null;
    newPR.orderDetailType = '1';

    return newPR;
  }

  mapFormToPRModel(): procurementRequestItemModel {
    let newPR = new procurementRequestItemModel();
    newPR.procurementRequestItemId = this.emptyGuid;

    //map lại Id cũ nếu là edit
    if (this.config.data) {
      let configData: ConfigData = this.config.data;
      if (configData.isEdit === true) newPR.procurementRequestItemId = configData.prItemEdit.procurementRequestItemId;
    }

    let _product: productModel = this.createPRItemForm.get('Product').value;
    if (_product) {
      newPR.productId = _product.productId;
      newPR.productName = _product.productName;
      newPR.productCode = _product.productCode;
    }

    let _vendor: vendorModel = this.createPRItemForm.get('Vendor').value;
    if (_vendor) {
      newPR.vendorId = _vendor.vendorId;
      newPR.vendorName = _vendor.vendorName;
    }

    let _quantity: number = ParseStringToFloat(this.createPRItemForm.get('Quantity').value);
    newPR.quantity = _quantity;

    let _productUnit = this.createPRItemForm.get('ProductUnit').value;
    newPR.productUnit = _productUnit;

    let _currencyUnit: moneyUnit = this.createPRItemForm.get('PriceUnit').value;
    if (_currencyUnit) {
      newPR.currencyUnit = _currencyUnit.categoryId;
      newPR.nameMoneyUnit = _currencyUnit.categoryName;
    }

    let _exchangeRate: string = this.createPRItemForm.get('ExchangeRate').value;
    newPR.exchangeRate = ParseStringToFloat(_exchangeRate);

    let _unitPrice: number = ParseStringToFloat(this.createPRItemForm.get('Price').value);
    newPR.unitPrice = _unitPrice;

    let _PRPlan: procurementPlan = this.createPRItemForm.get('ProcurementPlan').value;
    if (_PRPlan) {
      newPR.procurementPlanId = _PRPlan.procurementPlanId;
      newPR.procurementPlanCode = _PRPlan.procurementPlanCode;
    }

    let _amount: number = ParseStringToFloat(this.createPRItemForm.get('AmountNumber').value);
    newPR.amount = _amount;

    let description: string = this.createPRItemForm.get('Description').value;
    if (description) {
      newPR.description = description.trim();
    }
    // newPR.orderDetailType =
    newPR.createdById = this.auth.UserId;
    newPR.createdDate = new Date();
    newPR.procurementRequestId = this.emptyGuid;
    newPR.orderDetailType = '0';

    return newPR;
  }

  openQuickCreVendorModal() {
    let ref = this.dialogService.open(QuickCreateVendorComponent, {
      data: {},
      header: 'Tạo nhanh nhà cung cấp',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        this.listVendorCreateOrderModel = result.listVendor;

        if (this.listVendorCreateOrderModel.length > 0) {
          this.listVendor = [];
          this.listVendorCreateOrderModel.forEach(_vendor => {
            let vendor = new vendorModel;
            vendor.vendorId = _vendor.vendorId;
            vendor.vendorName = _vendor.vendorName;

            this.listVendor = [...this.listVendor, vendor];
          });

          //set luôn nhà cung cấp vừa tạo cho control nhà cung cấp
          let newVendor = this.listVendor.find(x => x.vendorId == result.newVendorId);
          this.createPRItemForm.get('Vendor').setValue(newVendor ? newVendor : null);

          //Hiển thị lại thông tin của nhà cung cấp vừa tạo
          let event = {
            value: newVendor ? newVendor : null
          }
          this.changeVendor(event);
        }
      }
    });
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
        // this.productControl.setValue(newProduct ? newProduct : null);
        // this.changeProduct(newProduct ? newProduct : null);
        this.createPRItemForm.get('Product').patchValue(newProduct ? newProduct : null);
        let event = {
          value: newProduct
        }
        this.changeProduct(event)
      }
    });
  }

  /*Event khi thay đổi OrderDetailType (Sản phẩm dịch vụ hoặc Chi phí phát sinh)*/
  changeOrderDetailType(orderDetailType: number) {
    this.selectedOrderDetailType = orderDetailType;
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

    if (discountType) {
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

function ParseStringToFloat(str: string) {
  if (str == '') return 0;
  str = str.toString().replace(/,/g, '');
  return parseFloat(str);
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



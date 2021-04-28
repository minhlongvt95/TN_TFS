import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';

import { CustomerOrderDetail } from '../../models/customer-order-detail.model';
import { OrderProductDetailProductAttributeValue } from '../../models/order-product-detail-product-attribute-value.model';

import { CustomerOrderService } from '../../services/customer-order.service';
import { QuickCreateProductComponent } from '../../../shared/components/quick-create-product/quick-create-product.component';

interface ResultDialog {
  status: boolean,
  customerOrderDetailModel: CustomerOrderDetail,
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
  price1: number,
  guaranteeTime: number
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
  selector: 'app-order-detail-dialog',
  templateUrl: './order-detail-dialog.component.html',
  styleUrls: ['./order-detail-dialog.component.css'],
  providers: [DialogService]
})
export class OrderDetailDialogComponent implements OnInit {
  @ViewChild('priceInitial') priceInitialElement: ElementRef;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  dateOrder: Date;
  cusGroupId: string;

  /*Các biến điều kiện*/
  isCreate: boolean = true; //true: Tạo mới sản phẩm dịch vụ(hoặc chi phí phát sinh), false: Sửa sản phẩm dịch vụ(hoặc chi phí phát sinh)
  selectedOrderDetailType: number = 0;  //0: Sản phẩm dịch vụ, 1: Chi phí phát sinh
  isShowRadioProduct: boolean = true;
  isShowRadioOC: boolean = true;
  readOnly: boolean = false; //Chỉ đc xem không được lưu
  /*End*/

  /*Các biến nhận giá trị trả về*/
  listUnitMoney: Array<Category> = [];
  unitMoneyLabel: string = 'VND';
  unitMoneyOCLabel: string = 'VND';
  listUnitProduct: Array<Category> = [];
  listVendor: Array<Vendor> = [];
  listProduct: Array<Product> = [];
  listWarehouse: Array<any> = [];
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
  listObjectAttrNameProduct: Array<ObjectAttrNameProduct> = [];
  listObjectAttrValueProduct: Array<ObjectAttrValueProduct> = [];
  listAttrProduct: Array<GroupAttrProduct> = [];
  customerOrderDetailModel = new CustomerOrderDetail();
  guaranteeDatetime: Date = null;
  IsPriceInitial: boolean = false;
  amountPriceInitialProduct: number = 0; //tiền thuế GTGT (sản phẩm dịch vụ)
  warehouse: any = null;
  /*End*/

  /*Form sản phẩm dịch vụ*/
  productForm: FormGroup;
  productControl: FormControl;
  productNameControl: FormControl;
  unitProductControl: FormControl;
  vendorControl: FormControl;
  quantityProductControl: FormControl;
  priceProductControl: FormControl;
  unitMoneyProductControl: FormControl;
  exchangeRateProductControl: FormControl;
  // guaranteeTimeProductControl: FormControl;
  expirationDateProductControl: FormControl;
  vatProductControl: FormControl;
  discountTypeProductControl: FormControl;
  discountValueProductControl: FormControl;
  priceInitialControl: FormControl;
  warrantyPeriodControl: FormControl;
  warehouseIdControl: FormControl;
  actualInventoryControl: FormControl;
  businessInventoryControl: FormControl;
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
    private customerOrderService: CustomerOrderService,
    public dialogService: DialogService,
  ) { 
    this.isCreate = this.config.data.isCreate;
    this.warehouse = this.config.data.warehouse;
    this.dateOrder = this.config.data.dateOrder;
    this.cusGroupId = this.config.data.cusGroupId;
    this.readOnly = this.config.data.readOnly;
    if (!this.isCreate) {
      //Nếu là sửa
      this.customerOrderDetailModel = this.config.data.customerOrderDetailModel;
      this.selectedOrderDetailType = this.customerOrderDetailModel.OrderDetailType;
      if (this.selectedOrderDetailType == 0) {
        //ẩn button radio Sản phẩm dịch vụ
        this.isShowRadioOC = false;
      } else if (this.selectedOrderDetailType == 1) {
        //ẩn button radio Chi phí khác
        this.isShowRadioProduct = false;
      }
    }
  }

  ngOnInit() {
    this.setForm();
    this.setTable();
    this.loading = true;
    this.customerOrderService.getMasterDataOrderDetailDialog().subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listProduct = result.listProduct;
        this.listUnitMoney = result.listUnitMoney;
        this.listUnitProduct = result.listUnitProduct;
        this.listWarehouse = result.listWareHouse;

        this.setDefaultValueForm();
      } else {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setForm() {
    /*Form Sản phẩm dịch vụ*/
    this.productControl = new FormControl(null, [Validators.required]);
    this.productNameControl = new FormControl(null);
    this.unitProductControl = new FormControl({ value: '', disabled: true });
    this.vendorControl = new FormControl(null);
    this.priceInitialControl = new FormControl('0');
    this.quantityProductControl = new FormControl('0');
    this.warrantyPeriodControl = new FormControl(null);
    this.priceProductControl = new FormControl('0');
    this.warehouseIdControl = new FormControl(null);
    this.actualInventoryControl = new FormControl(null);
    this.businessInventoryControl = new FormControl(null);
    this.unitMoneyProductControl = new FormControl(null);
    this.exchangeRateProductControl = new FormControl('1');
    // this.guaranteeTimeProductControl = new FormControl(null);
    this.expirationDateProductControl = new FormControl(null);
    this.vatProductControl = new FormControl('0');
    this.discountTypeProductControl = new FormControl(null);
    this.discountValueProductControl = new FormControl('0');

    this.productForm = new FormGroup({
      productControl: this.productControl,
      productNameControl: this.productNameControl,
      unitProductControl: this.unitProductControl,
      vendorControl: this.vendorControl,
      priceInitialControl: this.priceInitialControl,
      warrantyPeriodControl: this.warrantyPeriodControl,
      warehouseIdControl: this.warehouseIdControl,
      actualInventoryControl: this.actualInventoryControl,
      businessInventoryControl: this.businessInventoryControl,
      quantityProductControl: this.quantityProductControl,
      priceProductControl: this.priceProductControl,
      unitMoneyProductControl: this.unitMoneyProductControl,
      exchangeRateProductControl: this.exchangeRateProductControl,
      // guaranteeTimeProductControl: this.guaranteeTimeProductControl,
      expirationDateProductControl: this.expirationDateProductControl,
      vatProductControl: this.vatProductControl,
      discountTypeProductControl: this.discountTypeProductControl,
      discountValueProductControl: this.discountValueProductControl
    });
    /*End*/

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
      this.priceInitialControl.setValue("0");
      if (this.warehouse) {
        let wareHouseid = this.listWarehouse.find(c => c.warehouseId == this.warehouse);
        this.warehouseIdControl.setValue(wareHouseid);
      }
      /*End: Form Chi phí khác*/
    } else {
      if (this.selectedOrderDetailType == 0) {
        this.productNameControl.setValue(this.customerOrderDetailModel.ProductName);
        let toSelectProduct = this.listProduct.find(x => x.productId == this.customerOrderDetailModel.ProductId);
        this.productControl.setValue(toSelectProduct);
        let productUnitName = this.listUnitProduct.find(x => x.categoryId == toSelectProduct.productUnitId).categoryName;
        this.unitProductControl.setValue(productUnitName);

        let wareHouse = this.listWarehouse.find(c => c.warehouseId == this.customerOrderDetailModel.WarehouseId);
        this.warehouseIdControl.setValue(wareHouse);
        this.actualInventoryControl.setValue(this.customerOrderDetailModel.ActualInventory);
        this.businessInventoryControl.setValue(this.customerOrderDetailModel.BusinessInventory);
        this.IsPriceInitial = this.customerOrderDetailModel.IsPriceInitial;
        let priceInitial = this.customerOrderDetailModel.PriceInitial ? this.customerOrderDetailModel.PriceInitial.toString() : "0";
        this.priceInitialControl.setValue(priceInitial);
        this.warrantyPeriodControl.setValue(this.customerOrderDetailModel.WarrantyPeriod);

        this.amountPriceInitialProduct = this.customerOrderDetailModel.PriceInitial * this.customerOrderDetailModel.Quantity * this.customerOrderDetailModel.ExchangeRate;
        if (this.warehouse) {
          let wareHouseid = this.listWarehouse.find(c => c.warehouseId == this.warehouse);
          this.warehouseIdControl.setValue(wareHouseid);
        }
        //Lấy list nhà cung cấp
        this.customerOrderService.getVendorByProductId(toSelectProduct.productId, this.cusGroupId, this.dateOrder).subscribe(response => {
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
              item.SelectedAttrValue.productAttributeCategoryValueId = this.customerOrderDetailModel
                .OrderProductDetailProductAttributeValue
                .find(x => x.ProductAttributeCategoryId == item.AttrName.productAttributeCategoryId)
                .ProductAttributeCategoryValueId;
              item.SelectedAttrValue.productAttributeCategoryValue = this.listObjectAttrValueProduct
                .find(x => x.productAttributeCategoryValueId == item.SelectedAttrValue.productAttributeCategoryValueId)
                .productAttributeCategoryValue;
            });
            /*End*/

            /*map data vendor*/
            if (this.listVendor.length >= 1) {
              let toSelectVendor = this.listVendor.find(x => x.vendorId == this.customerOrderDetailModel.VendorId);
              this.vendorControl.setValue(toSelectVendor);
            }
            /*End*/
          } else {
            let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });

        this.quantityProductControl.setValue(this.customerOrderDetailModel.Quantity.toString());
        this.priceProductControl.setValue(this.customerOrderDetailModel.UnitPrice.toString());
        this.unitMoneyProductControl.setValue(this.listUnitMoney.find(x => x.categoryId == this.customerOrderDetailModel.CurrencyUnit));
        let toSelectExchangeRate = this.customerOrderDetailModel.ExchangeRate != null ? this.customerOrderDetailModel.ExchangeRate.toString() : '1';
        this.exchangeRateProductControl.setValue(toSelectExchangeRate);
        let toSelectGuaranteeTime = this.customerOrderDetailModel.GuaranteeTime;
        // this.guaranteeTimeProductControl.setValue(toSelectGuaranteeTime);
        this.guaranteeDatetime = toSelectGuaranteeTime == null ? null : this.customerOrderDetailModel.GuaranteeDatetime;
        this.expirationDateProductControl.setValue(this.customerOrderDetailModel.ExpirationDate);
        this.unitMoneyLabel = this.listUnitMoney.find(x => x.categoryId == this.customerOrderDetailModel.CurrencyUnit).categoryCode;
        let toSelectVat = this.customerOrderDetailModel.Vat != null ? this.customerOrderDetailModel.Vat.toString() : '0';
        this.vatProductControl.setValue(toSelectVat);
        this.discountTypeProductControl.setValue(this.discountTypeList.find(x => x.value == this.customerOrderDetailModel.DiscountType));
        let discountValue = this.customerOrderDetailModel.DiscountValue != null ? this.customerOrderDetailModel.DiscountValue.toString() : '0';
        this.discountValueProductControl.setValue(discountValue);
        /*Tắt validator kiểm tra Chiết khấu*/
        this.discountValueProductControl.setValidators(null);
        this.discountValueProductControl.updateValueAndValidity();
        /*End*/

        //Gán giá trị lại cho các biến lưu số thành tiền
        this.calculatorAmountProduct();
      } else if (this.selectedOrderDetailType == 1) {
        let description = this.customerOrderDetailModel.Description != null ? this.customerOrderDetailModel.Description.trim() : '';
        this.descriptionOCControl.setValue(description);
        let unitOC = this.customerOrderDetailModel.IncurredUnit != null ? this.customerOrderDetailModel.IncurredUnit.trim() : '';
        this.unitOCControl.setValue(unitOC);
        this.quantityOCControl.setValue(this.customerOrderDetailModel.Quantity.toString());
        this.priceOCControl.setValue(this.customerOrderDetailModel.UnitPrice.toString());
        this.unitMoneyOCControl.setValue(this.listUnitMoney.find(x => x.categoryId == this.customerOrderDetailModel.CurrencyUnit));
        let toSelectExchangeRate = this.customerOrderDetailModel.ExchangeRate != null ? this.customerOrderDetailModel.ExchangeRate.toString() : '1';
        this.exchangeRateOCControl.setValue(toSelectExchangeRate);
        let toSelectVat = this.customerOrderDetailModel.Vat != null ? this.customerOrderDetailModel.Vat.toString() : '0';
        this.vatOCControl.setValue(toSelectVat);
        this.discountTypeOCControl.setValue(this.discountTypeList.find(x => x.value == this.customerOrderDetailModel.DiscountType));
        let discountValue = this.customerOrderDetailModel.DiscountValue != null ? this.customerOrderDetailModel.DiscountValue.toString() : '0';
        this.discountValueOCControl.setValue(discountValue);
        /*Tắt validator kiểm tra Chiết khấu*/
        this.discountValueOCControl.setValidators(null);
        this.discountValueOCControl.updateValueAndValidity();
        /*End*/

        //gán giá trị mặc định cho đơn vị tiền
        let toSelectUnitMoneyOC = this.listUnitMoney.find(x => x.categoryId == this.customerOrderDetailModel.CurrencyUnit);
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
    this.unitMoneyProductControl.setValue(this.listUnitMoney.find(x => x.isDefault == true));
    this.exchangeRateProductControl.setValue('1');
    this.unitMoneyLabel = this.listUnitMoney.find(x => x.isDefault == true).categoryCode;
    this.amountProduct = 0;
    this.productNameControl.setValue(product.productName);
    if (product.guaranteeTime != null) {
      // this.guaranteeTimeProductControl.setValue(product.guaranteeTime);
      this.changeGuaranteeTime();
    }
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
      let productUnitName = "";
      let productUnit = this.listUnitProduct.find(x => x.categoryId == productUnitId)
      if (productUnit) {
        productUnitName = productUnit.categoryName
      }
      this.unitProductControl.setValue(productUnitName);

      //Lấy list nhà cung cấp
      this.customerOrderService.getVendorByProductId(product.productId, this.cusGroupId, this.dateOrder).subscribe(response => {
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

      //set giá trị cho Đơn giá nếu sản phẩm có giá trị đơn giá(price1)
      if (product.price1 != null) {
        // this.priceProductControl.setValue(product.price1.toString());
      }

    }
  }
  /*End*/

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

  changePriceInitial() {
    if (!this.IsPriceInitial) {
      this.priceInitialControl.setValue("0");
      this.amountPriceInitialProduct = 0;
    }
    else {
      this.priceInitialElement.nativeElement.focus();
      let priceInitial = this.priceInitialControl.value;
      if (!priceInitial) {
        this.priceInitialControl.setValue('0');
      }
      else {
        this.amountPriceInitialProduct = parseFloat(this.priceInitialControl.value.replace(/,/g, '')) * parseFloat(this.quantityProductControl.value.replace(/,/g, '')) * parseFloat(this.exchangeRateProductControl.value.replace(/,/g, ''));
      }
    }
  }

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
    } else {
      exchangeRate = parseFloat(this.exchangeRateProductControl.value.replace(/,/g, ''));
      if (exchangeRate == 0) {
        this.exchangeRateProductControl.setValue('1');
      }
    }

    this.amountPriceInitialProduct = parseFloat(this.priceInitialControl.value.replace(/,/g, '')) * parseFloat(this.quantityProductControl.value.replace(/,/g, '')) * parseFloat(this.exchangeRateProductControl.value.replace(/,/g, ''));
    this.calculatorAmountProduct();
  }
  /*End*/

  /*Event khi thay đổi thời gian bảo hành*/
  changeGuaranteeTime() {
    // let guaranteeTime = this.guaranteeTimeProductControl.value;
    // if (guaranteeTime) {
    //   //Tính ngày hết hạn bảo hành
    //   let currentTime = (new Date()).getTime();
    //   let addTime = guaranteeTime * 30 * 24 * 60 * 60 * 1000;
    //   let result = currentTime + addTime;
    //   this.guaranteeDatetime = new Date(result);
    // } else {
    //   this.guaranteeDatetime = null;
    // }
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

  /*Event thay đổi đơn vị tiền (OC)*/
  changeUnitMoneyOC(unitMoney: Category) {
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

  /*Event Hủy dialog*/
  cancel() {
    let result: ResultDialog = {
      status: false,  //Hủy
      customerOrderDetailModel: null
    };
    this.ref.close(result);
  }
  /*End*/

  /*Event Lưu dialog*/
  save() {
    let result: ResultDialog = {
      status: true,  //Lưu
      customerOrderDetailModel: new CustomerOrderDetail()
    };

    if (this.selectedOrderDetailType == 0) {
      /*Nếu là thêm Sản phẩm dịch vụ*/

      //reset Form Chi phí khác
      this.resetOtherCostsForm();

      //Tổng số thuộc tính:
      let countTotalAttrProduct = this.listAttrProduct.length;
      //Tổng số thuộc tính đã điền giá trị:
      let countCurrentAttrProduct = this.listAttrProduct.filter(x => x.SelectedAttrValue != null).length;
      //Số lượng sản phẩm
      let quantity = parseFloat(this.quantityProductControl.value.replace(/,/g, ''));
      let priceInitialInp = parseFloat(this.priceInitialControl.value.replace(/,/g, ''));
      // Đơn giá
      let unitPrice = parseFloat(this.priceProductControl.value.replace(/,/g, ''));

      if (countCurrentAttrProduct < countTotalAttrProduct) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Bạn phải chọn đầy đủ thuộc tính cho sản phẩm/dịch vụ' };
        this.showMessage(msg);
      } else if (!this.productForm.valid) {
        Object.keys(this.productForm.controls).forEach(key => {
          if (this.productForm.controls[key].valid == false) {
            this.productForm.controls[key].markAsTouched();
          }
        });
      } else if (quantity <= 0) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Bạn chưa nhập số lượng' };
        this.showMessage(msg);
      } else if (quantity > 999999) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Số lượng không vượt quá 999,999' };
        this.showMessage(msg);
      } else if (unitPrice > 999000000000) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không vượt quá 999,000,000,000 VND' };
        this.showMessage(msg);
      } else if (this.amountProduct > 999000000000) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Thành Tiền (VND) không vượt quá 999,000,000,000 VND' };
        this.showMessage(msg);
      } else if (priceInitialInp <= 0 && this.IsPriceInitial) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Bạn chưa nhập giá vốn' };
        this.showMessage(msg);
      } else {
        let currencyUnit: Category = this.unitMoneyProductControl.value
        result.customerOrderDetailModel.CurrencyUnit = currencyUnit.categoryId; //Đơn vị tiền

        result.customerOrderDetailModel.Description = '';
        let discountType: DiscountType = this.discountTypeProductControl.value;
        result.customerOrderDetailModel.DiscountType = discountType.value;  //Loại chiết khấu

        let discountValue = parseFloat(this.discountValueProductControl.value.replace(/,/g, ''));
        result.customerOrderDetailModel.DiscountValue = discountValue;  //Gía trị chiết khấu

        let exchangeRate = parseFloat(this.exchangeRateProductControl.value.replace(/,/g, ''));
        result.customerOrderDetailModel.ExchangeRate = exchangeRate;  //Tỷ giá

        // let guaranteeTime = parseFloat(this.guaranteeTimeProductControl.value.replace(/,/g, ''));
        // result.customerOrderDetailModel.GuaranteeTime = guaranteeTime;  //Thời gian bảo hành

        result.customerOrderDetailModel.GuaranteeDatetime = this.guaranteeDatetime; //Ngày hết hạn bảo hành

        result.customerOrderDetailModel.ExpirationDate = this.expirationDateProductControl.value; //Ngày hết hạn

        let product: Product = this.productControl.value;
        let productAllAttrName: string = '';
        if (this.listAttrProduct.length > 0) {
          this.listAttrProduct.forEach(item => {
            productAllAttrName += item.SelectedAttrValue.productAttributeCategoryValue + ';'
          });
          productAllAttrName = '(' + productAllAttrName + ')';
        }
        result.customerOrderDetailModel.ExplainStr = product.productName + productAllAttrName;

        result.customerOrderDetailModel.IncurredUnit = 'CCCCC';
        result.customerOrderDetailModel.NameMoneyUnit = currencyUnit.categoryName; //currencyUnit.categoryCode
        let vendor: Vendor = this.vendorControl.value;
        if (vendor !== null && vendor !== undefined) {
          result.customerOrderDetailModel.NameVendor = vendor.vendorCode + ' - ' + vendor.vendorName;
          result.customerOrderDetailModel.VendorId = vendor.vendorId;
        }
        result.customerOrderDetailModel.OrderDetailType = this.selectedOrderDetailType;
        result.customerOrderDetailModel.ProductId = product.productId;
        result.customerOrderDetailModel.ProductNameUnit = this.unitProductControl.value;
        result.customerOrderDetailModel.Quantity = parseFloat(this.quantityProductControl.value.replace(/,/g, ''));

        result.customerOrderDetailModel.OrderProductDetailProductAttributeValue = [];
        let listObjectSelectedProductAttr = this.listAttrProduct.map(x => x.SelectedAttrValue);
        listObjectSelectedProductAttr.forEach(item => {
          var option = new OrderProductDetailProductAttributeValue();
          option.ProductId = product.productId;
          option.ProductAttributeCategoryId = item.productAttributeCategoryId;
          option.ProductAttributeCategoryValueId = item.productAttributeCategoryValueId;

          result.customerOrderDetailModel.OrderProductDetailProductAttributeValue.push(option);
        });

        result.customerOrderDetailModel.SumAmount = this.amountProduct; // this.roundNumber((this.amountProduct + this.amountVatProduct - this.amountDiscountProduct), 2);
        result.customerOrderDetailModel.UnitId = product.productUnitId;
        result.customerOrderDetailModel.ProductCode = product.productCode;
        result.customerOrderDetailModel.ProductName = this.productNameControl.value ? this.productNameControl.value.trim() : '';
        result.customerOrderDetailModel.UnitPrice = parseFloat(this.priceProductControl.value.replace(/,/g, ''));
        result.customerOrderDetailModel.Vat = parseFloat(this.vatProductControl.value.replace(/,/g, ''));
        result.customerOrderDetailModel.AmountDiscount = this.amountDiscountProduct;
        result.customerOrderDetailModel.IsPriceInitial = this.IsPriceInitial;
        result.customerOrderDetailModel.PriceInitial = this.IsPriceInitial ? parseFloat(this.priceInitialControl.value.replace(/,/g, '')) : 0;
        result.customerOrderDetailModel.WarehouseId = this.warehouseIdControl.value ? this.warehouseIdControl.value.warehouseId : null;
        result.customerOrderDetailModel.WareCode = this.warehouseIdControl.value ? this.warehouseIdControl.value.warehouseCode : null;
        result.customerOrderDetailModel.ActualInventory = this.actualInventoryControl.value ? parseFloat(this.actualInventoryControl.value.replace(/,/g, '')) : null;
        result.customerOrderDetailModel.BusinessInventory = this.businessInventoryControl.value ? parseFloat(this.businessInventoryControl.value.replace(/,/g, '')) : null;
        result.customerOrderDetailModel.WarrantyPeriod = this.warrantyPeriodControl.value ? parseFloat(this.warrantyPeriodControl.value.replace(/,/g, '')) : null;
        result.customerOrderDetailModel.GuaranteeTime = this.warrantyPeriodControl.value ? parseFloat(this.warrantyPeriodControl.value.replace(/,/g, '')) : null;

        //Nếu là sửa thì gán lại giá trị sắp xếp
        if (!this.isCreate) {
          result.customerOrderDetailModel.OrderNumber = this.customerOrderDetailModel.OrderNumber;
        }

        this.ref.close(result);
      }
    } else if (this.selectedOrderDetailType == 1) {
      /*Nếu là thêm Chi phí khác*/

      //reset Form Sản phẩm dịch vụ
      this.resetProductForm();

      //Số lượng
      let quantity = parseFloat(this.quantityOCControl.value.replace(/,/g, ''));
      // đơn giá
      let unitPrice = parseFloat(this.priceOCControl.value.replace(/,/g, ''));

      if (!this.otherCostsForm.valid) {
        Object.keys(this.otherCostsForm.controls).forEach(key => {
          if (this.otherCostsForm.controls[key].valid == false) {
            this.otherCostsForm.controls[key].markAsTouched();
          }
        });
      } else if (quantity <= 0) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Bạn chưa nhập số lượng' };
        this.showMessage(msg);
      } else if (quantity > 999999) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Số lượng không được vượt quá 999,999' };
        this.showMessage(msg);
      } else if (unitPrice > 999000000000) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không được vượt quá 999,000,000,000 VND' };
        this.showMessage(msg);
      } else if (this.amountOC > 999000000000) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Thành tiền (VND) không được vượt quá 999,000,000,000 VND' };
        this.showMessage(msg);
      } else {
        let currencyUnit: Category = this.unitMoneyOCControl.value
        result.customerOrderDetailModel.CurrencyUnit = currencyUnit.categoryId; //Đơn vị tiền
        result.customerOrderDetailModel.Description = this.descriptionOCControl.value.trim();

        let discountType: DiscountType = this.discountTypeOCControl.value;
        result.customerOrderDetailModel.DiscountType = discountType.value;  //Loại chiết khấu

        let discountValue = parseFloat(this.discountValueOCControl.value.replace(/,/g, ''));
        result.customerOrderDetailModel.DiscountValue = discountValue;  //Gía trị chiết khấu

        let exchangeRate = parseFloat(this.exchangeRateOCControl.value.replace(/,/g, ''));
        result.customerOrderDetailModel.ExchangeRate = exchangeRate;  //Tỷ giá

        result.customerOrderDetailModel.ExplainStr = result.customerOrderDetailModel.Description;
        result.customerOrderDetailModel.IncurredUnit = this.unitOCControl.value != null ? this.unitOCControl.value.trim() : '';
        result.customerOrderDetailModel.NameMoneyUnit = currencyUnit.categoryCode;
        result.customerOrderDetailModel.NameVendor = "";
        result.customerOrderDetailModel.OrderDetailType = this.selectedOrderDetailType;
        result.customerOrderDetailModel.ProductId = null;
        result.customerOrderDetailModel.ProductNameUnit = "";
        result.customerOrderDetailModel.Quantity = parseFloat(this.quantityOCControl.value.replace(/,/g, ''));
        result.customerOrderDetailModel.OrderProductDetailProductAttributeValue = [];
        result.customerOrderDetailModel.SumAmount = this.roundNumber((this.amountOC + this.amountVatOC - this.amountDiscountOC), 2);
        result.customerOrderDetailModel.UnitId = null;
        result.customerOrderDetailModel.UnitPrice = parseFloat(this.priceOCControl.value.replace(/,/g, ''));
        result.customerOrderDetailModel.Vat = parseFloat(this.vatOCControl.value.replace(/,/g, ''));
        result.customerOrderDetailModel.VendorId = null;
        result.customerOrderDetailModel.AmountDiscount = this.amountDiscountProduct;
        result.customerOrderDetailModel.IsPriceInitial = false;
        result.customerOrderDetailModel.PriceInitial = 0;
        result.customerOrderDetailModel.WarehouseId = null;
        result.customerOrderDetailModel.ActualInventory = 0;
        result.customerOrderDetailModel.BusinessInventory = 0;
        result.customerOrderDetailModel.ProductName = this.descriptionOCControl.value.trim();
        result.customerOrderDetailModel.GuaranteeTime = null;

        //Nếu là sửa thì gán lại giá trị sắp xếp
        if (!this.isCreate) {
          result.customerOrderDetailModel.OrderNumber = this.customerOrderDetailModel.OrderNumber;
        }

        this.ref.close(result);
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
    this.unitMoneyOCControl.setValue(null);
    this.exchangeRateOCControl.setValue('1');
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
    this.unitMoneyProductControl.setValue(this.listUnitMoney.find(x => x.isDefault == true));
    this.exchangeRateProductControl.setValue('0');
    // this.guaranteeTimeProductControl.setValue(null);
    this.guaranteeDatetime = null;
    this.expirationDateProductControl.setValue(null);
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

  showMessage(msg: any) {
    this.messageService.add(msg);
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
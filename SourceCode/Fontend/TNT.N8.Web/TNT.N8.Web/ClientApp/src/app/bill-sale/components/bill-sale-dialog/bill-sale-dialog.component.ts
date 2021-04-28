import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { CustomerOrderService } from '../../../order/services/customer-order.service';
import { QuickCreateProductComponent } from '../../../shared/components/quick-create-product/quick-create-product.component';

interface ResultDialog {
  status: boolean,
  billSaleDetailModel: BillSaleDetailModel,
}

class BillSaleDetailModel {
  billOfSaleDetailId: string;
  billOfSaleId: string;
  vendorId: string;
  vendorName: string;
  productId: string;
  productCode: string;
  quantity: number;
  unitPrice: number;
  currencyUnit: string;
  exchangeRate: number;
  warehouseId: string;
  warehouseCode: string;
  moneyForGoods: number;
  accountId: string;
  accountDiscountId: string;
  vat: number;
  discountType: boolean;
  discountValue: number;
  description: string;
  orderDetailType: number;
  unitId: string;
  unitName: string;
  businessInventory: number;
  productName: string;
  actualInventory: number;
  incurredUnit: string;
  costsQuoteType: number;
  orderDetailId: string;
  orderId: string;
  explainStr: string;
  listBillSaleDetailProductAttribute: Array<BillSaleDetailProductAttributeModel> = [];
}

class BillSaleDetailProductAttributeModel {
  billOfSaleCostProductAttributeId: string;
  orderProductDetailProductAttributeValueId: string;
  orderDetailId: string;
  billOfSaleDetailId: string;
  productId: string;
  productAttributeCategoryId: string;
  productAttributeCategoryValueId: string;
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
  guaranteeTime: number;
  name: string;
  productCodeName: string;
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
  selector: 'app-bill-sale-dialog',
  templateUrl: './bill-sale-dialog.component.html',
  styleUrls: ['./bill-sale-dialog.component.css'],
  providers: [DialogService]
})
export class BillSaleDialogComponent implements OnInit {
  @ViewChild('priceInitial') priceInitialElement: ElementRef;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;

  /*Các biến điều kiện*/
  isCreate: boolean = true; //true: Tạo mới sản phẩm dịch vụ(hoặc chi phí phát sinh), false: Sửa sản phẩm dịch vụ(hoặc chi phí phát sinh)
  selectedOrderDetailType: number = 0;  //0: Sản phẩm dịch vụ, 1: Chi phí phát sinh
  isShowRadioProduct: boolean = true;
  isShowRadioOC: boolean = true;
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
  billSaleDetailModel = new BillSaleDetailModel();
  guaranteeDatetime: Date = null;
  IsPriceInitial: boolean = false;
  warehouse: any = null;
  /*End*/

  /*Form sản phẩm dịch vụ*/
  productForm: FormGroup;
  productControl: FormControl;
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
  nameProductControl: FormControl;
  priceInitialControl: FormControl;
  warrantyPeriodControl: FormControl;
  warehouseIdControl: FormControl;
  actualInventoryControl: FormControl;
  businessInventoryControl: FormControl;
  moneyForGoodsControl: FormControl;
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

  customerGroupId: string = null;
  orderDate: Date = null;
  isEdit: boolean = true;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private customerOrderService: CustomerOrderService,
    public dialogService: DialogService,
  ) {
    this.isCreate = this.config.data.isCreate;
    this.warehouse = this.config.data.warehouse;
    this.customerGroupId = this.config.data.customerGroupId;
    this.orderDate = this.config.data.orderDate;

    if (!this.isCreate) {
      //Nếu là sửa
      this.billSaleDetailModel = this.config.data.billSaleDetailModel;
      let edit: boolean = this.config.data.isEdit;
      if (edit != true && edit != false) {
        this.isEdit = true;
      } else {
        this.isEdit = this.config.data.isEdit;
      }
      this.selectedOrderDetailType = this.billSaleDetailModel.orderDetailType;
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
        this.listProduct.forEach(item => {
          let productCode = item.productCode == null ? "" : item.productCode;
          let productName = item.productName == null ? "" : item.productName;
          item.name = productCode + '-' + productName;
        });
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
    this.unitProductControl = new FormControl({ value: '', disabled: true });
    this.vendorControl = new FormControl(null);
    this.priceInitialControl = new FormControl('0');
    this.quantityProductControl = new FormControl('0');
    this.warrantyPeriodControl = new FormControl(null);
    this.moneyForGoodsControl = new FormControl('0');
    this.priceProductControl = new FormControl('0');
    this.warehouseIdControl = new FormControl(null, [Validators.required]);
    this.actualInventoryControl = new FormControl(null);
    this.businessInventoryControl = new FormControl(null);
    this.unitMoneyProductControl = new FormControl(null);
    this.exchangeRateProductControl = new FormControl('1');
    this.nameProductControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    // this.guaranteeTimeProductControl = new FormControl(null);
    this.expirationDateProductControl = new FormControl(null);
    this.vatProductControl = new FormControl('0');
    this.discountTypeProductControl = new FormControl(null);
    this.discountValueProductControl = new FormControl('0');

    this.productForm = new FormGroup({
      productControl: this.productControl,
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
      moneyForGoodsControl: this.moneyForGoodsControl,
      // guaranteeTimeProductControl: this.guaranteeTimeProductControl,
      expirationDateProductControl: this.expirationDateProductControl,
      vatProductControl: this.vatProductControl,
      discountTypeProductControl: this.discountTypeProductControl,
      discountValueProductControl: this.discountValueProductControl,
      nameProductControl: this.nameProductControl
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
        let toSelectProduct = this.listProduct.find(x => x.productId == this.billSaleDetailModel.productId);
        this.productControl.setValue(toSelectProduct);
        let productUnitName = this.listUnitProduct.find(x => x.categoryId == toSelectProduct.productUnitId).categoryName;
        this.unitProductControl.setValue(productUnitName);
        this.nameProductControl.setValue(this.billSaleDetailModel.productName)
        let wareHouse = this.listWarehouse.find(c => c.warehouseId == this.billSaleDetailModel.warehouseId);
        this.warehouseIdControl.setValue(wareHouse);
        this.actualInventoryControl.setValue(this.billSaleDetailModel.actualInventory);
        this.businessInventoryControl.setValue(this.billSaleDetailModel.businessInventory);

        if (this.warehouse) {
          let wareHouseid = this.listWarehouse.find(c => c.warehouseId == this.warehouse);
          this.warehouseIdControl.setValue(wareHouseid);
        }
        //Lấy list nhà cung cấp
        this.customerOrderService.getVendorByProductId(toSelectProduct.productId, this.customerGroupId, this.orderDate).subscribe(response => {
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
              item.SelectedAttrValue.productAttributeCategoryValueId = this.billSaleDetailModel
                .listBillSaleDetailProductAttribute
                .find(x => x.productAttributeCategoryId == item.AttrName.productAttributeCategoryId)
                .productAttributeCategoryValueId;
              item.SelectedAttrValue.productAttributeCategoryValue = this.listObjectAttrValueProduct
                .find(x => x.productAttributeCategoryValueId == item.SelectedAttrValue.productAttributeCategoryValueId)
                .productAttributeCategoryValue;
            });
            /*End*/

            /*map data vendor*/
            if (this.listVendor.length >= 1) {
              let toSelectVendor = this.listVendor.find(x => x.vendorId == this.billSaleDetailModel.vendorId);
              this.vendorControl.setValue(toSelectVendor);
            }
            /*End*/
          } else {
            let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });

        this.quantityProductControl.setValue(this.billSaleDetailModel.quantity.toString());
        this.priceProductControl.setValue(this.billSaleDetailModel.unitPrice.toString());
        this.unitMoneyProductControl.setValue(this.listUnitMoney.find(x => x.categoryId == this.billSaleDetailModel.currencyUnit));
        let toSelectExchangeRate = this.billSaleDetailModel.exchangeRate != null ? this.billSaleDetailModel.exchangeRate.toString() : '0';
        this.exchangeRateProductControl.setValue(toSelectExchangeRate);
        this.unitMoneyLabel = this.listUnitMoney.find(x => x.categoryId == this.billSaleDetailModel.currencyUnit).categoryCode;
        let toSelectVat = this.billSaleDetailModel.vat != null ? this.billSaleDetailModel.vat.toString() : '0';
        this.vatProductControl.setValue(toSelectVat);
        this.discountTypeProductControl.setValue(this.discountTypeList.find(x => x.value == this.billSaleDetailModel.discountType));
        let discountValue = this.billSaleDetailModel.discountValue != null ? this.billSaleDetailModel.discountValue.toString() : '0';
        this.discountValueProductControl.setValue(discountValue);
        /*Tắt validator kiểm tra Chiết khấu*/
        this.discountValueProductControl.setValidators(null);
        this.discountValueProductControl.updateValueAndValidity();
        /*End*/

        //Gán giá trị lại cho các biến lưu số thành tiền
        this.calculatorAmountProduct();
      } else if (this.selectedOrderDetailType == 1) {
        let description = this.billSaleDetailModel.description != null ? this.billSaleDetailModel.description.trim() : '';
        this.descriptionOCControl.setValue(description);
        let unitOC = this.billSaleDetailModel.incurredUnit != null ? this.billSaleDetailModel.incurredUnit.trim() : '';
        this.unitOCControl.setValue(unitOC);
        this.quantityOCControl.setValue(this.billSaleDetailModel.quantity.toString());
        this.priceOCControl.setValue(this.billSaleDetailModel.unitPrice.toString());
        this.unitMoneyOCControl.setValue(this.listUnitMoney.find(x => x.categoryId == this.billSaleDetailModel.currencyUnit));
        let toSelectExchangeRate = this.billSaleDetailModel.exchangeRate != null ? this.billSaleDetailModel.exchangeRate.toString() : '0';
        this.exchangeRateOCControl.setValue(toSelectExchangeRate);
        let toSelectVat = this.billSaleDetailModel.vat != null ? this.billSaleDetailModel.vat.toString() : '0';
        this.vatOCControl.setValue(toSelectVat);
        this.discountTypeOCControl.setValue(this.discountTypeList.find(x => x.value == this.billSaleDetailModel.discountType));
        let discountValue = this.billSaleDetailModel.discountValue != null ? this.billSaleDetailModel.discountValue.toString() : '0';
        this.discountValueOCControl.setValue(discountValue);
        /*Tắt validator kiểm tra Chiết khấu*/
        this.discountValueOCControl.setValidators(null);
        this.discountValueOCControl.updateValueAndValidity();
        /*End*/

        //gán giá trị mặc định cho đơn vị tiền
        let toSelectUnitMoneyOC = this.listUnitMoney.find(x => x.categoryId == this.billSaleDetailModel.currencyUnit);
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
    this.nameProductControl.setValue(this.productControl.value == null ? null : this.productControl.value.productName);
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
      this.customerOrderService.getVendorByProductId(product.productId, this.customerGroupId, convertToUTCTime(new Date())).subscribe(response => {
        let result: any = response;
        if (result.statusCode == 200) {
          this.listVendor = result.listVendor;
          this.listObjectAttrNameProduct = result.listObjectAttributeNameProduct;
          this.listObjectAttrValueProduct = result.listObjectAttributeValueProduct;
          this.priceProductControl.setValue(result.priceProduct);
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
        this.priceProductControl.setValue(product.price1.toString());
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
      this.amountVatProduct = this.roundNumber((((quantity * price * exchangeRate) - this.amountDiscountProduct) * (vat / 100)), parseInt(this.defaultNumberType, 10));
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
    this.moneyForGoodsControl.setValue(this.roundNumber((quantity * price * exchangeRate - this.amountDiscountProduct), parseInt(this.defaultNumberType, 10)));
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
      billSaleDetailModel: null
    };
    this.ref.close(result);
  }
  /*End*/

  /*Event Lưu dialog*/
  save() {
    let result: ResultDialog = {
      status: true,  //Lưu
      billSaleDetailModel: new BillSaleDetailModel()
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
      // đơn giá
      let unitPrice = parseFloat(this.priceProductControl.value.replace(/,/g, ''));
      // Tiền hàng (VND)
      let amount = parseFloat(this.moneyForGoodsControl.value.replace(/,/g, ''));

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
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Số lượng không được vượt quá 999999' };
        this.showMessage(msg);
      } else if (unitPrice > 999000000000) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không được vượt quá 999,000,000,000 VND' };
        this.showMessage(msg);
      } else if (amount > 999000000000) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Tiền hàng không được vượt quá 999,000,000,000 VND' };
        this.showMessage(msg);
      } else if (priceInitialInp <= 0 && this.IsPriceInitial) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Bạn chưa nhập giá vốn' };
        this.showMessage(msg);
      } else {
        let currencyUnit: Category = this.unitMoneyProductControl.value
        result.billSaleDetailModel.currencyUnit = currencyUnit.categoryId; //Đơn vị tiền

        result.billSaleDetailModel.description = '';
        let discountType: DiscountType = this.discountTypeProductControl.value;
        result.billSaleDetailModel.discountType = discountType.value;  //Loại chiết khấu

        let discountValue = parseFloat(this.discountValueProductControl.value.replace(/,/g, ''));
        result.billSaleDetailModel.discountValue = discountValue;  //Gía trị chiết khấu

        let exchangeRate = parseFloat(this.exchangeRateProductControl.value.replace(/,/g, ''));
        result.billSaleDetailModel.exchangeRate = exchangeRate;  //Tỷ giá



        let product: Product = this.productControl.value;
        let productAllAttrName: string = '';
        if (this.listAttrProduct.length > 0) {
          this.listAttrProduct.forEach(item => {
            productAllAttrName += item.SelectedAttrValue.productAttributeCategoryValue + ';'
          });
          productAllAttrName = '(' + productAllAttrName + ')';
        }
        result.billSaleDetailModel.productName = this.nameProductControl.value;
        result.billSaleDetailModel.explainStr = this.nameProductControl.value;

        result.billSaleDetailModel.incurredUnit = 'CCCCC';
        let vendor: Vendor = this.vendorControl.value;
        if (vendor !== null && vendor !== undefined) {
          result.billSaleDetailModel.vendorName = vendor.vendorCode + ' - ' + vendor.vendorName;
          result.billSaleDetailModel.vendorId = vendor.vendorId;
        }
        result.billSaleDetailModel.orderDetailType = this.selectedOrderDetailType;
        result.billSaleDetailModel.productId = product.productId;
        result.billSaleDetailModel.unitName = this.unitProductControl.value;
        result.billSaleDetailModel.quantity = parseFloat(this.quantityProductControl.value.replace(/,/g, ''));
        result.billSaleDetailModel.moneyForGoods = parseFloat(this.moneyForGoodsControl.value.replace(/,/g, ''));

        result.billSaleDetailModel.listBillSaleDetailProductAttribute = [];
        let listObjectSelectedProductAttr = this.listAttrProduct.map(x => x.SelectedAttrValue);
        listObjectSelectedProductAttr.forEach(item => {
          var option = new BillSaleDetailProductAttributeModel();
          option.productId = product.productId;
          option.productAttributeCategoryId = item.productAttributeCategoryId;
          option.productAttributeCategoryValueId = item.productAttributeCategoryValueId;

          result.billSaleDetailModel.listBillSaleDetailProductAttribute.push(option);
        });

        result.billSaleDetailModel.unitId = product.productUnitId;
        result.billSaleDetailModel.productCode = product.productCode;
        result.billSaleDetailModel.unitPrice = parseFloat(this.priceProductControl.value.replace(/,/g, ''));
        result.billSaleDetailModel.vat = parseFloat(this.vatProductControl.value.replace(/,/g, ''));
        result.billSaleDetailModel.warehouseId = this.warehouseIdControl.value ? this.warehouseIdControl.value.warehouseId : null;
        result.billSaleDetailModel.warehouseCode = this.warehouseIdControl.value ? this.warehouseIdControl.value.warehouseCode : null;
        result.billSaleDetailModel.actualInventory = this.actualInventoryControl.value ? parseFloat(this.actualInventoryControl.value.replace(/,/g, '')) : null;
        result.billSaleDetailModel.businessInventory = this.businessInventoryControl.value ? parseFloat(this.businessInventoryControl.value.replace(/,/g, '')) : null;

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
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Số lượng không được vượt quá 999,000,000,000 VND' };
        this.showMessage(msg);
      } else if (this.amountOC > 999000000000) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Thành tiền (VND) không được vượt quá 999,000,000,000 VND' };
        this.showMessage(msg);
      } else {
        let currencyUnit: Category = this.unitMoneyOCControl.value
        result.billSaleDetailModel.currencyUnit = currencyUnit.categoryId; //Đơn vị tiền
        result.billSaleDetailModel.description = this.descriptionOCControl.value.trim();

        let discountType: DiscountType = this.discountTypeOCControl.value;
        result.billSaleDetailModel.discountType = discountType.value;  //Loại chiết khấu

        let discountValue = parseFloat(this.discountValueOCControl.value.replace(/,/g, ''));
        result.billSaleDetailModel.discountValue = discountValue;  //Gía trị chiết khấu

        let exchangeRate = parseFloat(this.exchangeRateOCControl.value.replace(/,/g, ''));
        result.billSaleDetailModel.exchangeRate = exchangeRate;  //Tỷ giá

        result.billSaleDetailModel.explainStr = result.billSaleDetailModel.description;
        result.billSaleDetailModel.incurredUnit = this.unitOCControl.value != null ? this.unitOCControl.value.trim() : '';
        result.billSaleDetailModel.vendorName = "";
        result.billSaleDetailModel.orderDetailType = this.selectedOrderDetailType;
        result.billSaleDetailModel.productId = null;
        result.billSaleDetailModel.unitName = "";
        result.billSaleDetailModel.quantity = parseFloat(this.quantityOCControl.value.replace(/,/g, ''));
        result.billSaleDetailModel.listBillSaleDetailProductAttribute = [];
        result.billSaleDetailModel.moneyForGoods = this.roundNumber((this.amountOC + this.amountVatOC - this.amountDiscountOC), 2);
        result.billSaleDetailModel.unitId = null;
        result.billSaleDetailModel.unitPrice = parseFloat(this.priceOCControl.value.replace(/,/g, ''));
        result.billSaleDetailModel.vat = parseFloat(this.vatOCControl.value.replace(/,/g, ''));
        result.billSaleDetailModel.vendorId = null;
        result.billSaleDetailModel.warehouseId = null;
        result.billSaleDetailModel.actualInventory = 0;
        result.billSaleDetailModel.businessInventory = 0;
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
        newProduct.name = newProduct.productCodeName
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

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
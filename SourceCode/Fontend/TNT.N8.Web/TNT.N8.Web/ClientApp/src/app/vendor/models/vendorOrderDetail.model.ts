import { VendorOrderProductDetailProductAttributeValueModel } from "./vendorOrderProductDetailProductAttributeValue.model";

export class VendorOrderDetailModel {
  id: number;
  vendorOrderDetailId: string;
  vendorOrderId: string;
  description: string;
  vendorId: string;
  vendorName: string;
  productId: string;
  quantity: number;
  unitId: string;
  unitName: string;
  unitPrice: number;
  productUnitId: string;
  incurredUnit: string;
  currencyUnit: string;
  currencyUnitName: string;
  exchangeRate: number;
  vat: number;
  discountType: boolean;
  discountValue: number;
  sumAmountDiscount: number;
  sumAmountVat: number;
  sumAmount: number;
  productName: string;
  selectedAttributeName: string;
  listVendorOrderProductDetailProductAttributeValue: Array<VendorOrderProductDetailProductAttributeValueModel>;
  vendorOrderProductDetailProductAttributeValue: Array<VendorOrderProductDetailProductAttributeValueModel>;
  orderDetailType: number; //0: SP/DV; 1: Chi phí khác;
  isReshow: boolean;
  procurementRequestItemId: string;
  procurementRequestId: string;
  procurementCode: string;
  cost: number;
  priceWarehouse: number;
  priceValueWarehouse: number;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  active: boolean;
  explainStr: string;
  nameVendor: string;
  productNameUnit: string;
  nameMoneyUnit: string;
  isEditCost: boolean;
  orderNumber: number;

  /*Cờ báo lỗi để hiển thị trên giao diện*/
  isInvalidItemRequest: boolean;
  /*End*/

  constructor() {
    this.listVendorOrderProductDetailProductAttributeValue = [];
    this.vendorOrderProductDetailProductAttributeValue = [];
    this.isInvalidItemRequest = false;
  } 
}


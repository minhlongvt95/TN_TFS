import { QuoteProductDetailProductAttributeValue } from "../models/quote-product-detail-product-attribute-value.model"
export class QuoteDetail {
  quoteDetailId: string;
  vendorId: string;
  quoteId: string;
  productId: string;
  quantity: number; //Số lượng
  unitPrice: number;  //Đơn giá
  currencyUnit: string;
  exchangeRate: number; //Tỷ giá
  vat: number;
  discountType: boolean;
  discountValue: number;  //Chiết khấu
  description: string;
  orderDetailType: number;
  unitId: string;
  incurredUnit: string;
  active: boolean;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  quoteProductDetailProductAttributeValue: Array<QuoteProductDetailProductAttributeValue>;
  explainStr: string;
  nameVendor: string;
  productNameUnit: string;
  nameMoneyUnit: string;
  sumAmount: number;  //Thành tiền
  amountDiscount: number; //Tiền chiết khấu
  priceInitial: number; //Giá vốn
  isPriceInitial: boolean;
  productName: string;
  productCode: string;
  orderNumber: number;
  isPromotionProduct: boolean;

  constructor() {
    this.quoteDetailId = '00000000-0000-0000-0000-000000000000',
    this.vendorId = '',
    this.quoteId = '00000000-0000-0000-0000-000000000000',
    this.productId = '',
    this.quantity = 0,
    this.unitPrice = 0,
    this.currencyUnit = '00000000-0000-0000-0000-000000000000',
    this.exchangeRate = 1,
    this.vat = 0,
    this.discountType = true,
    this.discountValue = 0,
    this.description = '',
    this.orderDetailType = 0,
    this.unitId = '',
    this.incurredUnit = 'CCCCC',
    this.active = true,
    this.createdById = '00000000-0000-0000-0000-000000000000',
    this.createdDate = new Date(),
    this.updatedById = null,
    this.updatedDate = null,
    this.quoteProductDetailProductAttributeValue = [],
    this.explainStr = '',
    this.nameVendor = '',
    this.productNameUnit = '',
    this.nameMoneyUnit = '',
    this.sumAmount = 0,
    this.amountDiscount = 0;
    this.priceInitial = 0;
    this.isPriceInitial = false;
    this.productName = '';
    this.productCode = '';
    this.orderNumber = 0;
    this.isPromotionProduct = false;
  }
}

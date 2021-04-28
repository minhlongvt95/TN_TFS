import { OrderProductDetailProductAttributeValue } from "./order-product-detail-product-attribute-value.model"
export class CustomerOrderDetail {
  OrderDetailId: string;
  VendorId: string;
  OrderId: string;
  ProductId: string;
  Quantity : number;
  UnitPrice : number;
  CurrencyUnit : string;
  ExchangeRate : number;
  Vat: number;
  DiscountType: boolean;
  DiscountValue: number;
  Description: string;
  OrderDetailType: number;
  UnitId: string;
  IncurredUnit: string;
  Active: boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  OrderProductDetailProductAttributeValue: Array<OrderProductDetailProductAttributeValue>;
  ExplainStr: string;
  NameVendor: string;
  ProductNameUnit: string;
  NameMoneyUnit: string;
  SumAmount: number;
  GuaranteeTime: number;
  GuaranteeDatetime: Date;
  AmountDiscount: number; //add by dung
  ExpirationDate: Date;
  WarehouseId: string;
  PriceInitial: number;
  IsPriceInitial: boolean;
  WarrantyPeriod: number;
  ActualInventory: number;
  BusinessInventory: number;
  ProductCode: string;
  ProductName: string;
  WareCode: string;
  OrderNumber: number;

  constructor() {
    this.OrderDetailId = '00000000-0000-0000-0000-000000000000',
    this.VendorId = '',
    this.OrderId = '00000000-0000-0000-0000-000000000000',
    this.ProductId = '',
    this.Quantity = 0,
    this.UnitPrice = 0,
    this.CurrencyUnit = '00000000-0000-0000-0000-000000000000',
    this.ExchangeRate = 1,
    this.Vat = 0,
    this.DiscountType = true,
    this.DiscountValue = 0,
    this.Description = '',
    this.OrderDetailType = 0,
    this.UnitId = '',
    this.IncurredUnit = '',
    this.Active = true,
    this.CreatedById = '00000000-0000-0000-0000-000000000000',
    this.CreatedDate = new Date(),
    this.UpdatedById = null,
    this.UpdatedDate = null,
    this.OrderProductDetailProductAttributeValue = [],
    this.ExplainStr = '',
    this.NameVendor = '',
    this.ProductNameUnit = '',
    this.NameMoneyUnit = '',
    this.SumAmount = 0,
    this.GuaranteeTime = null;
    this.GuaranteeDatetime = null;
    this.AmountDiscount = 0;
    this.ExpirationDate = null;    
    this.WarehouseId = null;
    this.PriceInitial = null;
    this.IsPriceInitial = false;
    this.WarrantyPeriod = null;
    this.ActualInventory = null;
    this.BusinessInventory = null;
    this.OrderNumber = 0;
  }
}

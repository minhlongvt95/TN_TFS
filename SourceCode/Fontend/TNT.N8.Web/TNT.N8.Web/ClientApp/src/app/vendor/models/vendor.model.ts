export class VendorModel {
  VendorId: string;
  VendorName: string;
  VendorCode: string;
  VendorGroupId: string;
  PaymentId: string;
  TotalPurchaseValue: number;
  TotalPayableValue: number;
  NearestDateTransaction: Date;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: boolean;

  constructor() {
    this.VendorId = '00000000-0000-0000-0000-000000000000';
    this.VendorName = '';
    this.VendorCode = '';
    this.VendorGroupId = null;
    this.PaymentId = '00000000-0000-0000-0000-000000000000';
    this.TotalPurchaseValue = 0;
    this.TotalPayableValue = 0;
    this.NearestDateTransaction = null;
    this.CreatedById = '00000000-0000-0000-0000-000000000000';
    this.CreatedDate = new Date();
    this.Active = true;
  }
}

export class ProductVendorMappingModel {
  ProductVendorMappingId: string;
  VendorId: string;
  ProductId: string;
  VendorProductCode: string;
  VendorProductName: string;
  MiniumQuantity: number;
  Price: number;
  MoneyUnitId: string;
  FromDate: Date;
  ToDate: Date;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Actice: boolean;
  UnitPriceId: string;
  VendorName: string;
  ProductName: string;

  constructor() {
    this.ProductVendorMappingId = '00000000-0000-0000-0000-000000000000',
      this.VendorId = '',
      this.ProductId = '',
      this.VendorProductCode = '',
      this.VendorProductName = '',
      this.MiniumQuantity = 0,
      this.Price = 0,
      this.MoneyUnitId = '',
      this.FromDate = new Date(),
      this.ToDate = null,
      this.CreatedById = '00000000-0000-0000-0000-000000000000',
      this.CreatedDate = new Date(),
      this.UpdatedById = null,
      this.UpdatedDate = null,
      this.UnitPriceId = null,
      this.VendorName = '',
      this.ProductName = '',
      this.Actice = true
  }
}

export class SuggestedSupplierQuotesModel {
  suggestedSupplierQuoteId: string;
  suggestedSupplierQuote: string;
  statusId: string;
  vendorId: string;
  personInChargeId: string;
  recommendedDate: Date;
  quoteTermDate: Date;
  objectType: string;
  objectId: string;
  note: string;
  active: boolean;
  createdDate: Date;
  createdById: string;
  updatedDate: Date;
  updatedById: string;
  procurementRequestId: string;
  saleBiddingCode: string;
  quoteCode: string;
  productUnitName: string;
  email: string;

  // ListSuggestedSupplierQuotesDetail: Array<SuggestedSupplierQuotesDetailModel>;

  constructor() {
    this.suggestedSupplierQuoteId = '00000000-0000-0000-0000-000000000000';
    this.suggestedSupplierQuote = '';
    this.statusId = '00000000-0000-0000-0000-000000000000';
    this.vendorId = '00000000-0000-0000-0000-000000000000';
    this.personInChargeId = '00000000-0000-0000-0000-000000000000';
    this.recommendedDate = null;
    this.quoteTermDate = null;
    this.objectId = '00000000-0000-0000-0000-000000000000';
    this.objectType = '';
    this.note = '';
    this.active = true;
    this.createdDate = new Date();
    this.createdById = '00000000-0000-0000-0000-000000000000';
    this.updatedById = null;
    this.updatedDate = null;
    this.procurementRequestId = '00000000-0000-0000-0000-000000000000';
    this.saleBiddingCode = '';
    this.quoteCode = '';
    this.productUnitName = '';
    this.email = '';
  }
}

export class SuggestedSupplierQuotesDetailModel {
  suggestedSupplierQuoteDetailId: string;
  suggestedSupplierQuoteId: string;
  productId: string;
  productCode: string;
  productName: string;
  quantity: number;
  note: string;
  active: boolean;
  createdDate: Date;
  createdById: string;
  updatedDate: Date;
  updatedById: string;

  constructor() {
    this.suggestedSupplierQuoteDetailId = '00000000-0000-0000-0000-000000000000';
    this.suggestedSupplierQuoteId = '00000000-0000-0000-0000-000000000000';
    this.productId = '00000000-0000-0000-0000-000000000000';
    this.productCode = '';
    this.productName = '';
    this.quantity = 0;
    this.note = '';
    this.active = true;
    this.createdById = '00000000-0000-0000-0000-000000000000';
    this.createdDate = new Date();
    this.updatedById = null;
    this.updatedDate = null
  }
}
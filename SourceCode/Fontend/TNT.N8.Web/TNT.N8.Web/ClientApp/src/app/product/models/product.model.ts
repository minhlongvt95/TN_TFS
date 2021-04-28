export class ProductModel {
  ProductId: string;
  ProductCategoryId: string;
  ProductName: string;
  ProductCode: string;
  Price1: number;
  Price2: number;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: Boolean;
  Quantity: number;
  ProductUnitId: string;
  ProductDescription: string;
  Vat: Number;
  MinimumInventoryQuantity: Number;
  ProductMoneyUnitId: string;
  GuaranteeTime: Number;
  ProductCategoryName: string;
  ListVendorName: string;
  //ProductAttribute: Array<ProductAttributeModel>;
  //ProductVendorMapping: Array<ProductVendorMappingModel>;
}

export class ProductModel2 {
  ProductId: string;
  ProductCategoryId: string;
  ProductName: string;
  ProductCode: string;
  Price1: number;
  Price2: number;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: Boolean;
  Quantity: number;
  ProductUnitId: string;
  ProductDescription: string;
  Vat: Number;
  MinimumInventoryQuantity: Number;
  ProductMoneyUnitId: string;
  GuaranteeTime: Number;
  ProductCategoryName: string;
  ListVendorName: string;
  ExWarehousePrice: number;
  CountProductInformation: number;
  CalculateInventoryPricesId: string;
  PropertyId: string;
  WarehouseAccountId: string;
  RevenueAccountId: string;
  PayableAccountId: string;
  ImportTax: number;
  CostPriceAccountId: string;
  AccountReturnsId: string;
  FolowInventory: boolean;
  ManagerSerialNumber: boolean;

  constructor() {
    this.CountProductInformation = 0;
  }
}

export class DetailProductModel {
  productId: string;
  productCategoryId: string;
  productName: string;
  productCode: string;
  price1: number;
  price2: number;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  active: Boolean;
  quantity: number;
  productUnitId: string;
  productDescription: string;
  vat: Number;
  minimumInventoryQuantity: Number;
  productMoneyUnitId: string;
  guaranteeTime: Number;
  productCategoryName: string;
  listVendorName: string;
  exWarehousePrice: number;
  countProductInformation: number;
  calculateInventoryPricesId: string;
  propertyId: string;
  warehouseAccountId: string;
  revenueAccountId: string;
  payableAccountId: string;
  importTax: number;
  costPriceAccountId: string;
  accountReturnsId: string;
  folowInventory: boolean;
  managerSerialNumber: boolean;
  //ProductAttribute: Array<ProductAttributeModel>;
  //ProductVendorMapping: Array<ProductVendorMappingModel>;
  constructor() {
    this.countProductInformation = 0;
  }
}


export class ProductAttributeCategory {
  public ProductAttributeCategoryId: string;
  public ProductAttributeCategoryName: string;
  public CreatedById: string;
  public CreatedDate: Date;
  public UpdatedById: string;
  public UpdatedDate: Date;
  public Active: boolean;
  public ProductAttributeCategoryValue: Array<ProductAttributeCategoryValueModel>;
  constructor() {
    this.ProductAttributeCategoryValue = [];
  }
}

export class ProductAttributeCategoryValueModel {
  public ProductAttributeCategoryValueId: string;
  public ProductAttributeCategoryValue1: string;
  public ProductAttributeCategoryId: string;
  public CreatedById: string;
  public CreatedDate: Date;
  public UpdatedById: string;
  public UpdatedDate: Date;
  public Active: boolean;
}

export class ProductVendorMappingModel {
  VendorId: string;
  VendorName: string;
  VendorCode: string;
  VendorProductName: string;
  MiniumQuantity: number;
  Price: number;
  MoneyUnitId: string;
  MoneyUnitName: string;
  FromDate: Date;
  ToDate: Date;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  OrderNumber: number;
}
export class ProductAttributeModel {
  ProductAttributeCategoryId: string;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
}

export class productMoneyUnitModel {
  public categoryId: string;
  public categoryCode: string;
  public categoryName: string;
  public isDefauld: string;
}

export class productUnitModel {
  public categoryId: string;
  public categoryCode: string;
  public categoryName: string;
  public isDefauld: string;
}

export class vendorModel {
  public vendorId: string;
  public vendorName: string;
  public vendorCode: string;
}

export class warehouseModel {
  public warehouseId: string;
  public warehouseName: string;
  public warehouseCode: string;
  public warehouseParent: string;
  public warehouseNameByLevel: string;
  public listWarehouseNameByLevel: Array<string>;//thêm mới label theo phân cấp
  constructor() {
    this.listWarehouseNameByLevel = [];
  }
}

export class ProductQuantityInWarehouseModel {
  public InventoryReportId: string;
  public WarehouseId: string;
  public ProductId: string;
  public Quantity: number;
  public QuantityMinimum: number;
  public QuantityMaximum: number;
  public Active: boolean;
  public CreatedDate: Date;
  public CreatedById: string;
  public UpdatedDate: Date;
  public UpdatedById: string;
  public StartQuantity: number;
  public OpeningBalance: number;
  public Note: string;
  public ListSerial: Array<Serial>;

  constructor() {
    this.InventoryReportId = '00000000-0000-0000-0000-000000000000';
    this.WarehouseId = '00000000-0000-0000-0000-000000000000';
    this.ProductId = '00000000-0000-0000-0000-000000000000';
    this.Quantity = 0;
    this.QuantityMinimum = 0;
    this.QuantityMaximum = null;
    this.StartQuantity = 0;
    this.OpeningBalance = 0;
    this.Note = null;
    this.ListSerial = [];
    this.Active = true;
    this.CreatedDate = new Date();
    this.CreatedById = '00000000-0000-0000-0000-000000000000';
  }
}

export class Serial {
  public SerialId: string;
  public SerialCode: string;
  public ProductId: string;
  public StatusId: string;
  public WarehouseId: string;
  public Active: boolean;
  public CreatedDate: Date;
  public CreatedById: string;
  public UpdatedDate: Date;
  public UpdatedById: string;

  constructor() {
    this.SerialId = '00000000-0000-0000-0000-000000000000';
    this.ProductId = '00000000-0000-0000-0000-000000000000';
    this.StatusId = '00000000-0000-0000-0000-000000000000';
    this.WarehouseId = '00000000-0000-0000-0000-000000000000';
    this.Active = true;
    this.CreatedDate = new Date();
    this.CreatedById = '00000000-0000-0000-0000-000000000000';
  }
}

export class ProductImageModel {
  public ProductImageId: string;
  public ProductId: string;
  public ImageName: string;
  public ImageSize: string;
  public ImageUrl: string;
  public Active: boolean;
  public CreatedById: string;
  public CreatedDate: Date;
  public UpdatedById: string;
  public UpdatedDate: Date;
}

export class InventoryReportModel {
  public InventoryReportId: string;
  public WarehouseId: string;
  public ProductId: string;
  public Quantity: number;
  public QuantityMinimum: number;
  public QuantityMaximum: number;
  public Note: string;
  public Active: boolean;
  public CreatedDate: Date;
  public CreatedById: string;
  public UpdatedDate: Date;
  public UpdatedById: string;
  public StartQuantity: number;
  public OpeningBalance:number;
}

export class PriceProductModel{
  public PriceProductId: string;
  public ProductId: string;
  public EffectiveDate: Date;
  public PriceVnd: number;
  public MinQuantity: number;
  public PriceForeignMoney: number;
  public CustomerGroupCategory: string;
  public Active: boolean;
  public CreatedDate: Date;
  public CreatedById: string;
  public UpdatedDate: Date;
  public UpdatedById: string;
}


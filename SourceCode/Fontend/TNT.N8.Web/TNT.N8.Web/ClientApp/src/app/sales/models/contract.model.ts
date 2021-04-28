export class Contract {
  ContractId: string;  //done
  ContractCode: string;  //done
  EmployeeId: string;
  MainContractId: string;
  ContractTypeId: string;
  EffectiveDate: Date; //done
  ExpiredDate: Date
  ContractTime: Number
  ContractTimeUnit: string
  ContractDescription: string;  //done
  ContractNote: string; //done
  CustomerId: string; //done
  ObjectType: string; //done
  QuoteId: string;
  CustomerName: string; //done
  ValueContract: number;
  PaymentMethodId: string;  //done
  DiscountType: boolean;  //done
  BankAccountId: string; //done
  Amount: number; //done
  DiscountValue: number;  //done
  StatusId: string; //done
  Active: boolean;  //done
  IsExtend: boolean;  //done
  CreatedById: string; //done
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
}

export class ContractDetail {
  ContractDetailId: string;
  VendorId: string;
  ContractId: string;
  ProductId: string;
  Quantity: number;
  UnitPrice: number;
  CurrencyUnit: string;
  ExchangeRate: number;
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
  ContractProductDetailProductAttributeValue: Array<ContractProductDetailProductAttributeValue>;
  ExplainStr: string;
  NameVendor: string;
  NameProduct: string;
  ProductNameUnit: string;
  NameMoneyUnit: string;
  SumAmount: number;
  GuaranteeTime: number;
  AmountDiscount: number;
  ProductName: string;
  OrderNumber: number;

  constructor() {
    this.ContractDetailId = '00000000-0000-0000-0000-000000000000',
      this.VendorId = '',
      this.ContractId = '00000000-0000-0000-0000-000000000000',
      this.ProductId = '',
      this.Quantity = 0,
      this.UnitPrice = 0,
      this.CurrencyUnit = '',
      this.ExchangeRate = 1,
      this.Vat = 0,
      this.DiscountType = true,
      this.DiscountValue = 0,
      this.Description = '',
      this.OrderDetailType = 0,
      this.UnitId = '',
      this.IncurredUnit = 'CCCCC',
      this.Active = true,
      this.CreatedById = '00000000-0000-0000-0000-000000000000',
      this.CreatedDate = new Date(),
      this.UpdatedById = null,
      this.UpdatedDate = null,
      this.ContractProductDetailProductAttributeValue = [],
      this.ExplainStr = '',
      this.NameVendor = '',
      this.ProductNameUnit = '',
      this.NameMoneyUnit = '',
      this.SumAmount = 0,
      this.GuaranteeTime = 0;
      this.AmountDiscount = 0;
      this.ProductName = '';
      this.OrderNumber = 0;
  }
}

export class ContractProductDetailProductAttributeValue {
  ContractDetailId: string;
  ProductId: string;
  ProductAttributeCategoryId: string;
  ProductAttributeCategoryValueId: string;
  ContractProductDetailProductAttributeValueId: string;
  NameProductAttributeCategoryValue: string;
  NameProductAttributeCategory:string;
  constructor() {
    this.ContractDetailId = '00000000-0000-0000-0000-000000000000',
      this.ProductId = '',
      this.ProductAttributeCategoryId = '',
      this.ProductAttributeCategoryValueId = '',
      this.ContractProductDetailProductAttributeValueId = '00000000-0000-0000-0000-000000000000',
      this.NameProductAttributeCategoryValue = '',
      this.NameProductAttributeCategory = ''
  }
}

export class ContractCostDetail {
  ContractCostDetailId: string;
  CostId: string;
  ContractId: string;
  Quantity: number;
  UnitPrice: number;
  CostName: string;
  CostCode: string;
  Active: boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  SumAmount: number;

  constructor() {
    this.ContractCostDetailId = '00000000-0000-0000-0000-000000000000',
    this.CostId = '',
    this.ContractId = '00000000-0000-0000-0000-000000000000',
    this.Quantity = 0,
    this.UnitPrice = 0,
    this.CostName = '',
    this.Active = true,
    this.CreatedById = '00000000-0000-0000-0000-000000000000',
    this.CreatedDate = new Date(),
    this.UpdatedById = null,
    this.UpdatedDate = null
  }
}


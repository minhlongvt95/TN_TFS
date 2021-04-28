//request models
export class LeadDetailModel {
    public LeadDetailId: string;
    public LeadId: string;
    public VendorId: string;
    public ProductId: string;
    public Quantity: number;
    public UnitPrice: number;
    public CurrencyUnit: string;
    public ExchangeRate: number;
    public Vat: number;
    public DiscountType: boolean;
    public DiscountValue: number;
    public Description: string;
    public OrderDetailType: number;
    public UnitId: string;
    public IncurredUnit: string;
    public Active: boolean;
    public CreatedById: string;
    public CreatedDate: Date;
    public UpdatedById: string;
    public UpdatedDate: Date;

    public ExplainStr: string;
    public NameVendor: string;
    public ProductNameUnit: string;
    public NameMoneyUnit: string;
    public SumAmount: number;
    public AmountDiscount: number; //add by dung
    public OrderNumber: number;

    LeadProductDetailProductAttributeValue: Array<LeadProductDetailProductAttributeValue>;
    public ProductName: string;
    public ProductCode: string;

    constructor() {
        this.LeadDetailId = '00000000-0000-0000-0000-000000000000',
            this.VendorId = '',
            this.LeadId = '00000000-0000-0000-0000-000000000000',
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
            this.IncurredUnit = '',
            this.Active = true,
            this.CreatedById = '00000000-0000-0000-0000-000000000000',
            this.CreatedDate = new Date(),
            this.UpdatedById = null,
            this.UpdatedDate = null,
            this.LeadProductDetailProductAttributeValue = [],
            this.ExplainStr = '',
            this.NameVendor = '',
            this.ProductNameUnit = '',
            this.NameMoneyUnit = '',
            this.SumAmount = 0,
            this.AmountDiscount = 0,
            this.OrderNumber = 0;
    }
}

export class LeadProductDetailProductAttributeValue {
    public LeadProductDetailProductAttributeValue1: string;
    public LeadDetailId: string;
    public ProductId: string;
    public ProductAttributeCategoryId: string;
    public ProductAttributeCategoryValueId: string;
}

//response models
export class leadDetailModel {
    public leadDetailId: string;
    public leadId: string;
    public vendorId: string;
    public productId: string;
    public quantity: number;
    public unitPrice: number;
    public currencyUnit: string;
    public exchangeRate: number;
    public vat: number;
    public discountType: boolean;
    public discountValue: number;
    public description: string;
    public orderDetailType: number;
    public unitId: string;
    public incurredUnit: string;
    public active: boolean;
    public createdById: string;
    public createdDate: Date;
    public updatedById: string;
    public updatedDate: Date;

    public explainStr: string;
    public nameVendor: string;
    public productNameUnit: string;
    public nameMoneyUnit: string;
    public sumAmount: number;
    public amountDiscount: number; //add by dung
    public orderNumber: number;


    leadProductDetailProductAttributeValue: Array<leadProductDetailProductAttributeValue>;
    public productName: string;
    public productCode: string;

}

export class leadProductDetailProductAttributeValue {
    public leadProductDetailProductAttributeValue1: string;
    public leadDetailId: string;
    public productId: string;
    public productAttributeCategoryId: string;
    public productAttributeCategoryValueId: string;
}
import { SaleBiddingDetailProductAttribute } from "../models/product-attribute-category-value.model"
export class CostsQuocte {
    costsQuocteId: string;
    vendorId: string;
    saleBiddingId: string;
    productId: string;
    quantity: number;
    unitPrice: number;
    currencyUnit: string;
    exchangeRate: number;
    vat: number;
    discountType: boolean;
    discountValue: number;
    description: string;
    orderDetailType: number;
    unitId: string;
    incurredUnit: string;
    active: boolean;
    createdById: string;
    createdDate: Date;
    updatedById: string;
    updatedDate: Date;
    saleBiddingDetailProductAttribute: Array<SaleBiddingDetailProductAttribute>;
    nameVendor: string;
    productNameUnit: string;
    nameMoneyUnit: string;
    sumAmount: number;
    amountDiscount: number;
    explainStr: string;
    productName: string;
    productCode: string;
    taxMoney: number; // Tiền thuế
    index: number;
    orderNumber: number;

    constructor() {
        this.costsQuocteId = '00000000-0000-0000-0000-000000000000',
        this.vendorId = '',
        this.saleBiddingId = '00000000-0000-0000-0000-000000000000',
        this.productId = '',
        this.quantity = 0,
        this.unitPrice = 0,
        this.currencyUnit = '',
        this.exchangeRate = 1,
        this.vat = 0,
        this.discountType = true,
        this.discountValue = 0,
        this.description = '',
        this.orderDetailType = 0,
        this.unitId = '',
        this.incurredUnit = '',
        this.active = true,
        this.createdById = '00000000-0000-0000-0000-000000000000',
        this.createdDate = new Date(),
        this.updatedById = null,
        this.updatedDate = null,
        this.saleBiddingDetailProductAttribute = [],
        this.nameVendor = '',
        this.productNameUnit = '',
        this.nameMoneyUnit = '',
        this.sumAmount = 0,
        this.amountDiscount = 0,
        this.index = 0,
        this.taxMoney = 0,
        this.explainStr = "",
        this.orderNumber = 0
    }
}
export class BankReceiptModel {
    BankReceiptInvoiceId: string;
    BankReceiptInvoiceCode: string;
    BankReceiptInvoiceDetail: string;
    BankReceiptInvoicePrice: number;
    BankReceiptInvoicePriceCurrency: string;
    BankReceiptInvoiceExchangeRate: number;
    BankReceiptInvoiceReason: string;
    BankReceiptInvoiceNote: string;
    BankReceiptInvoiceBankAccountId: string;
    BankReceiptInvoiceAmount: number;
    BankReceiptInvoiceAmountText: string;
    BankReceiptInvoicePaidDate: Date;
    VouchersDate: Date;
    OrganizationId: string;
    StatusId: string;
    Active: boolean;
    CreatedById: string;
    CreatedDate: Date;
    UpdatedById: string;
    UpdatedDate: Date;
  }
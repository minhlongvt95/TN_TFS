export class BankPaymentModel {
  BankPayableInvoiceId: string;
  BankPayableInvoiceCode: string;
  BankPayableInvoiceDetail: string;
  BankPayableInvoicePrice: number;
  BankPayableInvoicePriceCurrency: string;
  BankPayableInvoiceExchangeRate: number;
  BankPayableInvoiceReason: string;
  BankPayableInvoiceNote: string;
  BankPayableInvoiceBankAccountId: string;
  BankPayableInvoiceAmount: number;
  BankPayableInvoiceAmountText: string;
  BankPayableInvoicePaidDate: Date;
  VouchersDate: Date;
  OrganizationId: string;
  StatusId: string;
  ReceiveAccountNumber: string;
  ReceiveAccountName: string;
  ReceiveBankName: string;
  ReceiveBranchName: string;
  Active: boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
}

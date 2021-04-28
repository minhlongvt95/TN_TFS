using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class BankReceiptInvoice
    {
        public BankReceiptInvoice()
        {
            BankReceiptInvoiceMapping = new HashSet<BankReceiptInvoiceMapping>();
        }

        public Guid BankReceiptInvoiceId { get; set; }
        public string BankReceiptInvoiceCode { get; set; }
        public string BankReceiptInvoiceDetail { get; set; }
        public decimal? BankReceiptInvoicePrice { get; set; }
        public Guid? BankReceiptInvoicePriceCurrency { get; set; }
        public decimal? BankReceiptInvoiceExchangeRate { get; set; }
        public Guid? BankReceiptInvoiceReason { get; set; }
        public string BankReceiptInvoiceNote { get; set; }
        public Guid? BankReceiptInvoiceBankAccountId { get; set; }
        public decimal? BankReceiptInvoiceAmount { get; set; }
        public string BankReceiptInvoiceAmountText { get; set; }
        public DateTime BankReceiptInvoicePaidDate { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? StatusId { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? VouchersDate { get; set; }

        public BankAccount BankReceiptInvoiceBankAccount { get; set; }
        public Category BankReceiptInvoicePriceCurrencyNavigation { get; set; }
        public Category BankReceiptInvoiceReasonNavigation { get; set; }
        public Organization Organization { get; set; }
        public Category Status { get; set; }
        public ICollection<BankReceiptInvoiceMapping> BankReceiptInvoiceMapping { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class BankPayableInvoice
    {
        public BankPayableInvoice()
        {
            BankPayableInvoiceMapping = new HashSet<BankPayableInvoiceMapping>();
        }

        public Guid BankPayableInvoiceId { get; set; }
        public string BankPayableInvoiceCode { get; set; }
        public string BankPayableInvoiceDetail { get; set; }
        public decimal? BankPayableInvoicePrice { get; set; }
        public Guid? BankPayableInvoicePriceCurrency { get; set; }
        public decimal? BankPayableInvoiceExchangeRate { get; set; }
        public Guid? BankPayableInvoiceReason { get; set; }
        public string BankPayableInvoiceNote { get; set; }
        public Guid? BankPayableInvoiceBankAccountId { get; set; }
        public decimal? BankPayableInvoiceAmount { get; set; }
        public string BankPayableInvoiceAmountText { get; set; }
        public DateTime BankPayableInvoicePaidDate { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? StatusId { get; set; }
        public string ReceiveAccountNumber { get; set; }
        public string ReceiveAccountName { get; set; }
        public string ReceiveBankName { get; set; }
        public string ReceiveBranchName { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? VouchersDate { get; set; }

        public BankAccount BankPayableInvoiceBankAccount { get; set; }
        public Category BankPayableInvoicePriceCurrencyNavigation { get; set; }
        public Category BankPayableInvoiceReasonNavigation { get; set; }
        public Organization Organization { get; set; }
        public Category Status { get; set; }
        public ICollection<BankPayableInvoiceMapping> BankPayableInvoiceMapping { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ReceiptInvoice
    {
        public ReceiptInvoice()
        {
            ReceiptInvoiceMapping = new HashSet<ReceiptInvoiceMapping>();
        }

        public Guid ReceiptInvoiceId { get; set; }
        public string ReceiptInvoiceCode { get; set; }
        public string ReceiptInvoiceDetail { get; set; }
        public Guid? ReceiptInvoiceReason { get; set; }
        public string ReceiptInvoiceNote { get; set; }
        public Guid? RegisterType { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? StatusId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public decimal? UnitPrice { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Amount { get; set; }
        public string AmountText { get; set; }
        public DateTime ReceiptDate { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? VouchersDate { get; set; }

        public Category CurrencyUnitNavigation { get; set; }
        public Organization Organization { get; set; }
        public Category ReceiptInvoiceReasonNavigation { get; set; }
        public Category RegisterTypeNavigation { get; set; }
        public Category Status { get; set; }
        public ICollection<ReceiptInvoiceMapping> ReceiptInvoiceMapping { get; set; }
    }
}

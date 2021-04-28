using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PayableInvoice
    {
        public PayableInvoice()
        {
            PayableInvoiceMapping = new HashSet<PayableInvoiceMapping>();
        }

        public Guid PayableInvoiceId { get; set; }
        public string PayableInvoiceCode { get; set; }
        public string PayableInvoiceDetail { get; set; }
        public decimal? PayableInvoicePrice { get; set; }
        public Guid? PayableInvoicePriceCurrency { get; set; }
        public Guid? PayableInvoiceReason { get; set; }
        public string PayableInvoiceNote { get; set; }
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
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime PaidDate { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? VouchersDate { get; set; }

        public Category CurrencyUnitNavigation { get; set; }
        public Organization Organization { get; set; }
        public Category PayableInvoicePriceCurrencyNavigation { get; set; }
        public Category PayableInvoiceReasonNavigation { get; set; }
        public Category RegisterTypeNavigation { get; set; }
        public Category Status { get; set; }
        public ICollection<PayableInvoiceMapping> PayableInvoiceMapping { get; set; }
    }
}

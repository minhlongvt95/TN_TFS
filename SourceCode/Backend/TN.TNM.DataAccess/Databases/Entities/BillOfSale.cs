using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class BillOfSale
    {
        public Guid BillOfSaLeId { get; set; }
        public string BillOfSaLeCode { get; set; }
        public Guid OrderId { get; set; }
        public DateTime? BillDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? TermsOfPaymentId { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceSymbol { get; set; }
        public Guid? DebtAccountId { get; set; }
        public string Mst { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public Guid? AccountBankId { get; set; }
        public bool Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string CustomerAddress { get; set; }
        public decimal? DiscountValue { get; set; }
        public bool? DiscountType { get; set; }
    }
}

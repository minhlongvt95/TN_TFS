using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class SuggestedSupplierQuotes
    {
        public Guid SuggestedSupplierQuoteId { get; set; }
        public string SuggestedSupplierQuote { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public DateTime? RecommendedDate { get; set; }
        public DateTime? QuoteTermDate { get; set; }
        public string ObjectType { get; set; }
        public Guid? ObjectId { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ProcurementRequestId { get; set; }
        public bool? IsSend { get; set; }
    }
}

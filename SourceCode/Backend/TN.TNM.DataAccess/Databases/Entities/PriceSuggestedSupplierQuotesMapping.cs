using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PriceSuggestedSupplierQuotesMapping
    {
        public Guid PriceSuggestedSupplierQuotesMappingId { get; set; }
        public Guid? ProductVendorMappingId { get; set; }
        public Guid? SuggestedSupplierQuoteId { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}

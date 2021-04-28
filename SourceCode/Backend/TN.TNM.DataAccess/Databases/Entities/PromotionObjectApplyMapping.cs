using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PromotionObjectApplyMapping
    {
        public Guid PromotionObjectApplyMappingId { get; set; }
        public Guid PromotionObjectApplyId { get; set; }
        public Guid PromotionMappingId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public Guid? TenantId { get; set; }
    }
}

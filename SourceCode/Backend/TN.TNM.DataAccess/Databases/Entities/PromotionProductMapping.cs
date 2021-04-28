using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PromotionProductMapping
    {
        public Guid PromotionProductMappingId { get; set; }
        public Guid PromotionMappingId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? TenantId { get; set; }
    }
}

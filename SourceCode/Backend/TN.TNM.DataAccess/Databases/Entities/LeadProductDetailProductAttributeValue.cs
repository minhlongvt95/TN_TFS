using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LeadProductDetailProductAttributeValue
    {
        public Guid LeadProductDetailProductAttributeValue1 { get; set; }
        public Guid? LeadDetailId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductAttributeCategoryId { get; set; }
        public Guid? ProductAttributeCategoryValueId { get; set; }
        public Guid? TenantId { get; set; }
    }
}

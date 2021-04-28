using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class LeadProductDetailProductAttributeValueModel
    {
        public Guid LeadProductDetailProductAttributeValue1 { get; set; }
        public Guid? LeadDetailId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductAttributeCategoryId { get; set; }
        public Guid? ProductAttributeCategoryValueId { get; set; }
        public Guid? TenantId { get; set; }
    }
}

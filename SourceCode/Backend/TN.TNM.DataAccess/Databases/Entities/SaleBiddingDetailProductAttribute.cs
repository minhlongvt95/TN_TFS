using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class SaleBiddingDetailProductAttribute
    {
        public Guid SaleBiddingDetailProductAttributeId { get; set; }
        public Guid? SaleBiddingDetailId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductAttributeCategoryId { get; set; }
        public Guid? ProductAttributeCategoryValueId { get; set; }
        public Guid? TenantId { get; set; }
    }
}

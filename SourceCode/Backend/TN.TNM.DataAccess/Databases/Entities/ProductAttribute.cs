using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProductAttribute
    {
        public Guid ProductAttributeId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public Guid? TenantId { get; set; }

        public Product Product { get; set; }
        public ProductAttributeCategory ProductAttributeCategory { get; set; }
    }
}

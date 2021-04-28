using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProductAttributeCategory
    {
        public ProductAttributeCategory()
        {
            OrderProductDetailProductAttributeValue = new HashSet<OrderProductDetailProductAttributeValue>();
            ProductAttribute = new HashSet<ProductAttribute>();
            ProductAttributeCategoryValue = new HashSet<ProductAttributeCategoryValue>();
            QuoteProductDetailProductAttributeValue = new HashSet<QuoteProductDetailProductAttributeValue>();
            VendorOrderProductDetailProductAttributeValue = new HashSet<VendorOrderProductDetailProductAttributeValue>();
        }

        public Guid ProductAttributeCategoryId { get; set; }
        public string ProductAttributeCategoryName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }
        public Guid? TenantId { get; set; }

        public ICollection<OrderProductDetailProductAttributeValue> OrderProductDetailProductAttributeValue { get; set; }
        public ICollection<ProductAttribute> ProductAttribute { get; set; }
        public ICollection<ProductAttributeCategoryValue> ProductAttributeCategoryValue { get; set; }
        public ICollection<QuoteProductDetailProductAttributeValue> QuoteProductDetailProductAttributeValue { get; set; }
        public ICollection<VendorOrderProductDetailProductAttributeValue> VendorOrderProductDetailProductAttributeValue { get; set; }
    }
}

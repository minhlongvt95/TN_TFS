using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class OrderProductDetailProductAttributeValue
    {
        public Guid OrderDetailId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid ProductAttributeCategoryValueId { get; set; }
        public Guid OrderProductDetailProductAttributeValueId { get; set; }
        public Guid? TenantId { get; set; }

        public CustomerOrderDetail OrderDetail { get; set; }
        public Product Product { get; set; }
        public ProductAttributeCategory ProductAttributeCategory { get; set; }
        public ProductAttributeCategoryValue ProductAttributeCategoryValue { get; set; }
    }
}

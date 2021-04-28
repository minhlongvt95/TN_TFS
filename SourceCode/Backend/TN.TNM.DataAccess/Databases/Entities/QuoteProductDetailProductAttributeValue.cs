using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class QuoteProductDetailProductAttributeValue
    {
        public Guid QuoteDetailId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid ProductAttributeCategoryValueId { get; set; }
        public Guid QuoteProductDetailProductAttributeValueId { get; set; }
        public Guid? TenantId { get; set; }

        public Product Product { get; set; }
        public ProductAttributeCategory ProductAttributeCategory { get; set; }
        public ProductAttributeCategoryValue ProductAttributeCategoryValue { get; set; }
        public QuoteDetail QuoteDetail { get; set; }
    }
}

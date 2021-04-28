using System;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class QuoteProductDetailProductAttributeValueEntityModel
    {
        public Guid QuoteDetailId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid ProductAttributeCategoryValueId { get; set; }
        public Guid QuoteProductDetailProductAttributeValueId { get; set; }

        public string NameProductAttributeCategoryValue { get; set; }
        public string NameProductAttributeCategory { get; set; }

    }
}

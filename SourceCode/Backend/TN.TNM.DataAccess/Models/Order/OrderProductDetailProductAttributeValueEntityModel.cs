using System;

namespace TN.TNM.DataAccess.Models.Order
{
    public class OrderProductDetailProductAttributeValueEntityModel
    {
        public Guid OrderDetailId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid ProductAttributeCategoryValueId { get; set; }
        public Guid OrderProductDetailProductAttributeValueId { get; set; }

        public string NameProductAttributeCategoryValue { get; set; }
        public string NameProductAttributeCategory {get;set;}

    }
}

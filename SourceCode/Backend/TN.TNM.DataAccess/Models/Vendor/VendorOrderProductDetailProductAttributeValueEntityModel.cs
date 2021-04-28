using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Vendor
{
    public class VendorOrderProductDetailProductAttributeValueEntityModel
    {
        public Guid VendorOrderDetailId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid ProductAttributeCategoryValueId { get; set; }
        public Guid OrderProductDetailProductAttributeValueId { get; set; }
        public string NameProductAttributeCategoryValue { get; set; }
        public string NameProductAttributeCategory { get; set; }
        public string ProductAttributeCategoryName { get; set; }
        
        public List<Models.ProductAttributeCategoryValue.ProductAttributeCategoryValueEntityModel> ProductAttributeCategoryValue { get; set; }
        
        public VendorOrderProductDetailProductAttributeValueEntityModel()
        {
            this.ProductAttributeCategoryValue = new List<ProductAttributeCategoryValue.ProductAttributeCategoryValueEntityModel>();
        }
    }
}

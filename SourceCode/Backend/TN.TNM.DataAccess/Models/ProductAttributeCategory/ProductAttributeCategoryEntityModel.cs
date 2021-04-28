using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.ProductAttributeCategoryValue;

namespace TN.TNM.DataAccess.Models.ProductAttributeCategory
{
    public class ProductAttributeCategoryEntityModel
    {
        public Guid ProductAttributeCategoryId { get; set; }
        public string ProductAttributeCategoryName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }
        public List<ProductAttributeCategoryValueEntityModel> ProductAttributeCategoryValue { get; set; }
        //public string ValueString { get; set; }
        
        public ProductAttributeCategoryEntityModel()
        {
            ProductAttributeCategoryValue = new List<ProductAttributeCategoryValueEntityModel>();
        }
    }
}

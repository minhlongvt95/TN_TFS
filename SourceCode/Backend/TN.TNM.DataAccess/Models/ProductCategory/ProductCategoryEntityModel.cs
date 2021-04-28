using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.ProductCategory
{
    public class ProductCategoryEntityModel
    {
        public Guid ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public int? ProductCategoryLevel { get; set; }
        public string ProductCategoryCode { get; set; }
        public string Description { get; set; }
        public Guid? ParentId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }
        public List<ProductCategoryEntityModel> ProductCategoryChildList { get; set; }
        public int CountProductCategory { get; set; }
        public int CountCustomer { get; set; }
        public Guid? CustomerId { get; set; }

        public string ProductCategoryCodeName { get; set; }
    }
}

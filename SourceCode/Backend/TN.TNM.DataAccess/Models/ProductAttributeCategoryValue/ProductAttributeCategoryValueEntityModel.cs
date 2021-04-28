using System;

namespace TN.TNM.DataAccess.Models.ProductAttributeCategoryValue
{
    public class ProductAttributeCategoryValueEntityModel
    {
        public Guid ProductAttributeCategoryValueId { get; set; }
        public string ProductAttributeCategoryValue1 { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }
    }
}

using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory
{
    public class CreateProductCategoryParameter:BaseParameter
    {
        public string ProductCategoryName { get; set; }
        public int? ProducCategoryLevel { get; set; }
        public string ProductCategoryCode { get; set; }
        public string Description { get; set; }
        public Guid? ParentId { get; set; }
    }
}

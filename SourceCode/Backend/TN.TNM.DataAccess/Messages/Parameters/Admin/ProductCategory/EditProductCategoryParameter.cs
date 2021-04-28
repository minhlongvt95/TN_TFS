using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory
{
    public class EditProductCategoryParameter : BaseParameter
    {
        public Guid ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductCategoryCode { get; set; }
        public string Description { get; set; }
        ////public bool Active { get; set; }
        ////public Guid UpdatedById { get; set; }
    }
}

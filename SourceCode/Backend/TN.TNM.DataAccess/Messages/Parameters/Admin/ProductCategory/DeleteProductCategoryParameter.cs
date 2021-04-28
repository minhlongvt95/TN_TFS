using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory
{
    public class DeleteProductCategoryParameter:BaseParameter
    {
        public Guid ProductCategoryId { get; set; }
    }
}

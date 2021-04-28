using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory
{
    public class UpdateActiveProductCategoryParameter : BaseParameter
    {
        public Guid ProductCategoryId { get; set; }
    }
}

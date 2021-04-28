using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory
{
    public class GetProductCategoryByIdParameter : BaseParameter
    {
        public Guid ProductCategoryId {get;set;}
    }
}

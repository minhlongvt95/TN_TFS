using System.Collections.Generic;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.DataAccess.Messages.Results.Admin.ProductCategory
{
    public class GetAllProductCategoryResult : BaseResult
    {
        public List<ProductCategoryEntityModel> ProductCategoryList { get; set; }
    }
}

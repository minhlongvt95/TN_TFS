using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.DataAccess.Messages.Results.Admin.ProductCategory
{
    public class GetProductCategoryByIdResult: BaseResult
    {   
        public ProductCategoryEntityModel ProductCategory { get; set; }

    }
}

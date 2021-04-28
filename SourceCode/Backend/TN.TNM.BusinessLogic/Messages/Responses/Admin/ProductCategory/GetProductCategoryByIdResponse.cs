using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.ProductCategory
{
    public class GetProductCategoryByIdResponse : BaseResponse
    {
        public ProductCategoryModel ProductCategory { get; set; }
    }
}

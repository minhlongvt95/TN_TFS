using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.ProductCategory
{
    public class SearchProductCategoryResponse:BaseResponse
    {
        public List<ProductCategoryModel> ProductCategoryList { get; set; }
    }
}

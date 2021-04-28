using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.ProductCategory
{
    public class GetAllCategoryCodeResponse : BaseResponse
    {
        public List<string> ProductCategoryCodeList { get; set; }
    }
}

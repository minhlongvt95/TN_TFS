using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Category;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Category
{
    public class GetAllCategoryResponse : BaseResponse
    {
        public List<CategoryTypeModel> CategoryTypeList { get; set; }
    }
}

using TN.TNM.BusinessLogic.Models.Category;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Category
{
    public class GetCategoryByIdResponse : BaseResponse
    {
        public CategoryModel Category { get; set; }
        public bool IsCategory { get; set; }
    }
}

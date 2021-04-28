using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Category
{
    public class GetCategoryByIdResult : BaseResult
    {
        public CategoryEntityModel Category { get; set; }
        public bool IsCategory { get; set; }
    }
}

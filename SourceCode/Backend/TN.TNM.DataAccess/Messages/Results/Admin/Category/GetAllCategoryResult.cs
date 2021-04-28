using System.Collections.Generic;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Category
{
    public class GetAllCategoryResult : BaseResult
    {
        public List<CategoryTypeEntityModel> CategoryTypeList { get; set; }
    }
}

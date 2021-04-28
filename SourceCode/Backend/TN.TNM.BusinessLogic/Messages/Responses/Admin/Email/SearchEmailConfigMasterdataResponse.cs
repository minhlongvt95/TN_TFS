using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;


namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Email
{
    public class SearchEmailConfigMasterdataResponse: BaseResponse
    {
        public List<Models.Category.CategoryModel> ListEmailType { get; set; }
        public List<Models.Category.CategoryModel> ListEmailStatus { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetDataSearchEmployeeRequestResponse : BaseResponse
    {
        public List<CategoryModel> ListStatus { get; set; }
        public List<CategoryModel> ListTypeRequest { get; set; }

    }
}

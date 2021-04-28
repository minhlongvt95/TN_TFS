using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.SaleBidding
{
    public class GetMasterDataSearchSaleBiddingResponse : BaseResponse
    {
        public List<CategoryModel> ListStatus { get; set; }
        public List<CategoryModel> ListLeadType { get; set; }
        public List<EmployeeModel> ListPersonalInChange { get; set; }
        public List<CategoryModel> ListContract { get; set; }
    }
}

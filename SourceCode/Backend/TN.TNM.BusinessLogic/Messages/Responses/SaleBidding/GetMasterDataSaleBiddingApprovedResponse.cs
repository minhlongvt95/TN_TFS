using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Responses.SaleBidding
{
    public class GetMasterDataSaleBiddingApprovedResponse:BaseResponse
    {
        public List<SaleBiddingModel> ListSaleBidding { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
        public List<CustomerModel> ListCustomer { get; set; }
        public List<CategoryModel> ListStatus { get; set; }
    }
}

using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.DataAccess.Messages.Results.SaleBidding
{
    public class GetMasterDataSaleBiddingApprovedResult : BaseResult
    {
        public List<SaleBiddingEntityModel> ListSaleBidding { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
    }
}

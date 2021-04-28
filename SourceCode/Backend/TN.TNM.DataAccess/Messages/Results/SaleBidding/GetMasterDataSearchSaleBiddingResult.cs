using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.SaleBidding
{
    public class GetMasterDataSearchSaleBiddingResult : BaseResult
    {
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<CategoryEntityModel> ListLeadType { get; set; }
        public List<EmployeeEntityModel> ListPersonalInChange { get; set; }
        public List<CategoryEntityModel> ListContract { get; set; }
    }
}

using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.SaleBidding
{
    public class GetPersonInChargeByCustomerIdResponse : BaseResponse
    {
        public List<EmployeeModel> ListPersonInCharge { get; set; }
    }
}

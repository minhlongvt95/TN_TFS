using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetEmployeeHighLevelByEmpIdResponse: BaseResponse
    {
        public List<EmployeeModel> ListEmployeeToApprove { get; set; }
        public List<EmployeeModel> ListEmployeeToNotify { get; set; }
    }
}

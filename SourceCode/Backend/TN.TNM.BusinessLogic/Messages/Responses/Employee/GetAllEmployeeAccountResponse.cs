using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetAllEmployeeAccountResponse : BaseResponse
    {
        public IList<EmployeeModel> EmployeeAccounts { get; set; }
    }
}

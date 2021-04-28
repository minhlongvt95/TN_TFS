using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetAllEmployeeResponse : BaseResponse
    {
        public List<EmployeeModel> EmployeeList { get; set; }
        public List<EmployeeModel> listIdentityMapEmpId { get; set; }
    }
}

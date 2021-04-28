using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetEmployeeSalaryByEmpIdResponse : BaseResponse
    {
        public List<EmployeeSalaryModel> ListEmployeeSalary { get; set; }
    }
}

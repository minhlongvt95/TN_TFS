using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeSalaryByEmpIdResult : BaseResult
    {
        public List<EmployeeSalaryEntityModel> ListEmployeeSalary { get; set; }
    }
}

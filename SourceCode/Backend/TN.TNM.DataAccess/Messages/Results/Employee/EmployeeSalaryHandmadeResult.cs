using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class EmployeeSalaryHandmadeResult:BaseResult
    {
        public List<EmployeeMonthySalaryEntityModel> lstEmployeeMonthySalary { get; set; }
    }
}

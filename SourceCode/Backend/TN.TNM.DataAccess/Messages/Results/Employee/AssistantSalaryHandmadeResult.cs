using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class AssistantSalaryHandmadeResult:BaseResult
    {
        public List<EmployeeMonthySalaryEntityModel> lstEmployeeMonthySalary { get; set; }

    }
}

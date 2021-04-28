using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class EmployeeSalaryHandmadeResponse:BaseResponse
    {
        public List<EmployeeMonthySalaryModel> lstEmployeeMonthySalary { get; set; }
    }
}

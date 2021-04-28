using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class FindAssistantMonthySalaryResponse:BaseResponse
    {
        public List<EmployeeMonthySalaryModel> lstEmployeeMonthySalary { get; set; }

    }
}

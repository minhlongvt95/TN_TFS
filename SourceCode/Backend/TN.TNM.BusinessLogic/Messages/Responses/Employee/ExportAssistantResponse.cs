using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class ExportAssistantResponse:BaseResponse
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }
        public List<EmployeeMonthySalaryModel> lstEmployeeMonthySalary { get; set; }
    }
}

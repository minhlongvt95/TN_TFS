using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class EmployeeTimeSheetImportResult : BaseResult
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }
        public List<EmployeeMonthySalaryEntityModel> lstEmployeeMonthySalary { get; set; }
    }
}

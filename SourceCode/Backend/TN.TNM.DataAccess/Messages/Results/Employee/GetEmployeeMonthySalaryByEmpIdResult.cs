using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeMonthySalaryByEmpIdResult : BaseResult
    {
        public EmployeeMonthySalary EmployeeMonthlySalary { get; set; }
    }
}

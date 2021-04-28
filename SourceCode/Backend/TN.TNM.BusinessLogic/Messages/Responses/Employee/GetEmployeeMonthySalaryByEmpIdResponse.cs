using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetEmployeeMonthySalaryByEmpIdResponse : BaseResponse
    {
        public EmployeeMonthySalaryModel EmployeeMonthySalary { get; set; }
    }
}

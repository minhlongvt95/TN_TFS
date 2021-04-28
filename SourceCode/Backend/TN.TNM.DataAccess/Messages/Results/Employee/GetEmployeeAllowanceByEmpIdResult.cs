using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeAllowanceByEmpIdResult : BaseResult
    {
        public EmployeeAllowance EmployeeAllowance { get; set; }
    }
}

using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetEmployeeAllowanceByEmpIdResponse : BaseResponse
    {
        public EmployeeAllowanceModel EmployeeAllowance { get; set; }
    }
}

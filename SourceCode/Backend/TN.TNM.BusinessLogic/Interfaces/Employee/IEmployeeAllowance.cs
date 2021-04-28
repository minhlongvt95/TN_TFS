using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;

namespace TN.TNM.BusinessLogic.Interfaces.Employee
{
    public interface IEmployeeAllowance
    {
        GetEmployeeAllowanceByEmpIdResponse GetEmployeeAllowanceByEmpId(GetEmployeeAllowanceByEmpIdRequest request);
        EditEmployeeAllowanceResponse EditEmployeeAllowance(EditEmployeeAllowanceRequest request);
    }
}

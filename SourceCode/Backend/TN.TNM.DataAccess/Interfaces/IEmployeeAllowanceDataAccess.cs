using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IEmployeeAllowanceDataAccess
    {
        GetEmployeeAllowanceByEmpIdResult GetEmployeeAllowanceByEmpId(GetEmployeeAllowanceByEmpIdParameter parameter);
        EditEmployeeAllowanceResult EditEmployeeAllowance(EditEmployeeAllowanceParameter parameter);
    }
}

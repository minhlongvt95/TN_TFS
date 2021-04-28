using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IEmployeeMonthySalaryDataAccess
    {
        GetEmployeeMonthySalaryByEmpIdResult GetEmployeeMonthySalaryByEmpId(GetEmployeeMonthySalaryByEmpIdParameter parameter);
    }
}

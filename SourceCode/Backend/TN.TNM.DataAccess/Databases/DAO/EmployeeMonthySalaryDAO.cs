using System.Linq;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class EmployeeMonthySalaryDAO : BaseDAO, IEmployeeMonthySalaryDataAccess
    {
        public EmployeeMonthySalaryDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }
        public GetEmployeeMonthySalaryByEmpIdResult GetEmployeeMonthySalaryByEmpId(GetEmployeeMonthySalaryByEmpIdParameter parameter)
        {
            var _empMonthySalary = context.EmployeeMonthySalary.FirstOrDefault(emp => emp.EmployeeId == parameter.EmployeeId
                                                                             && emp.Month == parameter.Month && emp.Year == parameter.Year);
            return new GetEmployeeMonthySalaryByEmpIdResult()
            {
                Message = "Success",
                Status = true,
                EmployeeMonthlySalary = _empMonthySalary == null ? new Entities.EmployeeMonthySalary() : _empMonthySalary
            };
        }
    }
}

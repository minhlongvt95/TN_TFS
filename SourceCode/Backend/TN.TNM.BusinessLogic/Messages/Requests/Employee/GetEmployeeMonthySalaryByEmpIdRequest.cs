using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetEmployeeMonthySalaryByEmpIdRequest : BaseRequest<GetEmployeeMonthySalaryByEmpIdParameter>
    {
        public Guid EmployeeId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public override GetEmployeeMonthySalaryByEmpIdParameter ToParameter()
        {
            return new GetEmployeeMonthySalaryByEmpIdParameter()
            {
                EmployeeId = EmployeeId,
                Year = Year,
                Month = Month
            };
        }
    }
}

using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetEmployeeMonthySalaryByEmpIdParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}

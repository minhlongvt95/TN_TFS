using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetEmployeeSalaryByEmpIdParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}

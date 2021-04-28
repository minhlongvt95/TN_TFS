using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SearchEmployeeInsuranceParameter : BaseParameter
    {
        public Guid? EmployeeId { get; set; }
        public Guid? EmployeeInsuranceId { get; set; }
    }
}

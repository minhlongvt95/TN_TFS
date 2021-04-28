using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetEmployeeRequestByEmpIdParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
    }
}

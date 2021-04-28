using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DisableEmployeeParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
    }
}

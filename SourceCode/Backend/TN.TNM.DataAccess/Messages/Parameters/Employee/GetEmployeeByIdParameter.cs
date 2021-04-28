using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetEmployeeByIdParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public Guid ContactId { get; set; }
    }
}

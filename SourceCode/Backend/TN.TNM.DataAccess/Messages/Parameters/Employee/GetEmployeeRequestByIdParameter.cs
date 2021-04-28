using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetEmployeeRequestByIdParameter : BaseParameter
    {
        public Guid EmployeeRequestId { get; set; }
    }
}

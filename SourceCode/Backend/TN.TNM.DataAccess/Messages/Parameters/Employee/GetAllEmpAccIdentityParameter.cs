using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetAllEmpAccIdentityParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
    }
}

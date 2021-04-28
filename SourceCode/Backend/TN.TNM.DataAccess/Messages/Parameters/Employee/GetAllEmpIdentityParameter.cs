using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetAllEmpIdentityParameter : BaseParameter
    {
        public Guid CurrentEmpId { get; set; }
    }
}

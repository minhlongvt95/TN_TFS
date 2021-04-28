using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class EmployeePermissionMappingParameter : BaseParameter
    {
        public Guid PermissionSetId { get; set; }
        public Guid EmployeeId { get; set; }
    }
}

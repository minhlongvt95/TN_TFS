using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class EditEmployeeDataPermissionParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public Boolean IsManager { get; set; }
    }
}

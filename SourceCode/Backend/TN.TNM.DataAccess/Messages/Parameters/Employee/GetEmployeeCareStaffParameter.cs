using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetEmployeeCareStaffParameter : BaseParameter
    {
        public bool IsManager { get; set; }
        public Guid EmployeeId { get; set; }
    }
}

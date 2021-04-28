using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetEmployeeHighLevelByEmpIdParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public string ModuleCode { get; set; }
    }
}

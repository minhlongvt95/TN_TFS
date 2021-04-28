using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeRequestByEmpIdResult : BaseResult
    {
        public List<EmployeeRequest> ListEmployeeRequest { get; set; }
        public double amountAbsentWithPermission { get; set; }
        public double amountAbsentWithoutPermission { get; set; }
    }
}


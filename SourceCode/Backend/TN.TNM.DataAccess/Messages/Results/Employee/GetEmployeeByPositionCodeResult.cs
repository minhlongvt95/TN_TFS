using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeByPositionCodeResult : BaseResult
    {
        public List<Databases.Entities.Employee> EmployeeList { get; set; }
    }
}

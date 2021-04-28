using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeHighLevelByEmpIdResult : BaseResult
    {
        public List<Category> ListReasons { get; set; }
        public List<Databases.Entities.Employee> ListEmployeeToApprove { get; set; }
        public List<Databases.Entities.Employee> ListEmployeeToNotify { get; set; }
    }
}

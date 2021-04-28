using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetEmployeeWithNotificationPermisisonResult : BaseResult
    {
        public List<Databases.Entities.Employee> EmployeeList { get; set; }
    }
}

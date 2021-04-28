using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetEmployeeManagerResult : BaseResult
    {
        public List<Databases.Entities.Employee> ManagerList { get; set; }
    }
}

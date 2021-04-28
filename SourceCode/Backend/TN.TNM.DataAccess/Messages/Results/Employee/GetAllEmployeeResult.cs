using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetAllEmployeeResult : BaseResult
    {
        public List<Databases.Entities.Employee> EmployeeList { get; set; }
        public List<EmployeeEntityModel> listIdentityMapEmpId { get; set; }
    }
}

using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeByIdResult : BaseResult
    {
        public EmployeeEntityModel Employee { get; set; }
        public ContactEntityModel Contact { get; set; }
        public UserEntityModel User { get; set; }
    }
}

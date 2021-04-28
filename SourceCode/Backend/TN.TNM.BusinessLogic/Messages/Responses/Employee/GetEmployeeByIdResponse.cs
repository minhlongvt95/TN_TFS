using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetEmployeeByIdResponse : BaseResponse
    {
        public EmployeeModel Employee { get; set; }
        public ContactModel Contact { get; set; }
        public UserModel User { get; set; }
    }
}

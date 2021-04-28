using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class EditEmployeeByIdRequest : BaseRequest<EditEmployeeByIdParameter>
    {
        public EmployeeModel Employee { get; set; }
        public ContactModel Contact { get; set; }
        public UserModel User { get; set; }
        public bool IsResetPass { get; set; }

        public override EditEmployeeByIdParameter ToParameter()
        {
            return new EditEmployeeByIdParameter()
            {
                Employee = Employee.ToEntity(),
                Contact = Contact.ToEntity(),
                User = User.ToEntity(),
                IsResetPass = IsResetPass
            };
        }
    }
}

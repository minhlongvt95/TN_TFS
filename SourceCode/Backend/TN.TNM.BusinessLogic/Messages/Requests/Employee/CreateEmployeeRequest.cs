using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class CreateEmployeeRequest : BaseRequest<CreateEmployeeParameter>
    {
        public EmployeeModel Employee { get; set; }
        public ContactModel Contact { get; set; }
        public UserModel User { get; set; }
        public bool IsAccessable { get; set; }
        public override CreateEmployeeParameter ToParameter() => new CreateEmployeeParameter()
        {
            Employee = Employee.ToEntity(),
            Contact = Contact.ToEntity(),
            User = User.ToEntity(),
            IsAccessable = IsAccessable,
            UserId = UserId
        };
    }
}

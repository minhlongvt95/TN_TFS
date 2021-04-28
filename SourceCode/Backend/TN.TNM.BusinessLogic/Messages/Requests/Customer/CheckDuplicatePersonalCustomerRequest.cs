using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class CheckDuplicatePersonalCustomerRequest : BaseRequest<CheckDuplicatePersonalCustomerParameter>
    {
        public CustomerModel Customer { get; set; }
        public ContactModel Contact { get; set; }
        public override CheckDuplicatePersonalCustomerParameter ToParameter()
        {
            return new CheckDuplicatePersonalCustomerParameter()
            {
                UserId = UserId,
                Customer = Customer.ToEntity(),
                Contact = Contact.ToEntity()
            };
        }
    }
}

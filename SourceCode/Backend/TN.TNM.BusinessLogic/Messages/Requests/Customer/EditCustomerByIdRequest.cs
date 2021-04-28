using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class EditCustomerByIdRequest : BaseRequest<EditCustomerByIdParameter>
    {
        public CustomerModel Customer { get; set; }
        public ContactModel Contact { get; set; }
        public List<ContactModel> ContactList { get; set; }
        public override EditCustomerByIdParameter ToParameter()
        {
            List<DataAccess.Databases.Entities.Contact> lst = new List<DataAccess.Databases.Entities.Contact>();
            ContactList.ForEach(contact => {
                lst.Add(contact.ToEntity());
            });
            return new EditCustomerByIdParameter() {
                Contact = Contact.ToEntity(),
                Customer = Customer.ToEntity(),
                UserId = UserId,
                ContactList = lst
            };
        }
    }
}

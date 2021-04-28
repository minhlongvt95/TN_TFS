using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class CreateCustomerRequest : BaseRequest<CreateCustomerParameter>
    {
        public CustomerModel Customer { get; set; }
        public ContactModel Contact { get; set; }
        public List<ContactModel> CustomerContactList { get; set; }
        public bool CreateByLead { get; set; }
        public bool IsFromLead { get; set; } // biến kiểm tra khách hàng định danh hay là khách hàng tiềm năng
        public override CreateCustomerParameter ToParameter()
        {
            List<DataAccess.Databases.Entities.Contact> contactList = new List<DataAccess.Databases.Entities.Contact>();
            CustomerContactList.ForEach(con =>
            {
                contactList.Add(con.ToEntity());
            });
            return new CreateCustomerParameter() {
                UserId = UserId,
                Contact = Contact.ToEntity(),
                Customer = Customer.ToEntity(),
                CustomerContactList = contactList,
                CreateByLead = CreateByLead,
                IsFromLead = IsFromLead
            };
        }
    }
}

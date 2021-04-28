using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class ImportListCustomerRequest: BaseRequest<ImportListCustomerParameter>
    {
        public List<Models.Customer.CustomerModel> ListCustomer { get; set; }
        public List<Models.Contact.ContactModel> ListContact { get; set; }
        public List<Models.Contact.ContactModel> ListContactAdditional { get; set; }
        public bool IsPotentialCustomer { get; set; }
        public override ImportListCustomerParameter ToParameter()
        {
            var customerList = new List<DataAccess.Databases.Entities.Customer>();
            var contactList = new List<DataAccess.Databases.Entities.Contact>();
            var contactAdditional = new List<DataAccess.Databases.Entities.Contact>();
            ListCustomer.ForEach(con =>
            {
                customerList.Add(con.ToEntity());
            });
            ListContact.ForEach(con =>
            {
                contactList.Add(con.ToEntity());
            });
            ListContactAdditional.ForEach(con =>
            {
                contactAdditional.Add(con.ToEntity());
            });
            return new ImportListCustomerParameter()
            {
                ListContact = contactList,
                ListCustomer = customerList,
                ListContactAdditional = contactAdditional,
                IsPotentialCustomer = IsPotentialCustomer,
                UserId = UserId
            };
        }
    }
}

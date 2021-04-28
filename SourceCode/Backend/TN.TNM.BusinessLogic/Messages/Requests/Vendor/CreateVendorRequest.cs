using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class CreateVendorRequest : BaseRequest<CreateVendorParameter>
    {
        public VendorModel Vendor { get; set; }
        public ContactModel Contact { get; set; }
        public List<ContactModel> VendorContactList { get; set; }
        public override CreateVendorParameter ToParameter()
        {
            List<DataAccess.Databases.Entities.Contact> lst = new List<DataAccess.Databases.Entities.Contact>();
            VendorContactList.ForEach(contact => {
                lst.Add(contact.ToEntity());
            });
            return new CreateVendorParameter() {
                Vendor = Vendor.ToEntity(),
                Contact = Contact.ToEntity(),
                VendorContactList = lst,
                UserId = UserId
            };
        }
    }
}

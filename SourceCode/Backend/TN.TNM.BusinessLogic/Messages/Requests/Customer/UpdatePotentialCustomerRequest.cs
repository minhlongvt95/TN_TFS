using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;
using TN.TNM.DataAccess.Models;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class UpdatePotentialCustomerRequest : BaseRequest<UpdatePotentialCustomerParameter>
    {
        public CustomerModel CustomerModel { get; set; }
        public ContactEntityModel ContactModel { get; set; }
        public List<Guid?> ListDocumentIdNeedRemove { get; set; }
        public List<DataAccess.Models.Document.LinkOfDocumentEntityModel> ListLinkOfDocument { get; set; }
        public List<DataAccess.Models.Customer.PotentialCustomerProductEntityModel> ListCustomerProduct { get; set; }
        public List<DataAccess.Models.ContactEntityModel> ListContact { get; set; }
        public override UpdatePotentialCustomerParameter ToParameter()
        {
            return new UpdatePotentialCustomerParameter()
            {
                UserId = UserId,
                Customer = CustomerModel.ToEntity(),
                Contact = ContactModel,
                ListContact = ListContact,
                ListDocumentIdNeedRemove = ListDocumentIdNeedRemove ?? new List<Guid?>(),
                ListLinkOfDocument = ListLinkOfDocument ?? new List<DataAccess.Models.Document.LinkOfDocumentEntityModel>(),
                ListCustomerProduct = ListCustomerProduct ?? new List<PotentialCustomerProductEntityModel>()
            };
        }
    }
}

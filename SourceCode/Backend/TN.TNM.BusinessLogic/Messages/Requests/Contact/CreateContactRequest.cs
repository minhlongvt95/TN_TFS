using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.DataAccess.Messages.Parameters.Contact;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contact
{
    public class CreateContactRequest : BaseRequest<CreateContactParameter>
    {
        public CustomerOtherContactBusinessModel Contact { get; set; }

        public override CreateContactParameter ToParameter()
        {
            var cusContact = new CustomerOtherContactModel();
            cusContact.ContactId = Contact.ContactId;
            cusContact.ObjectId = Contact.ObjectId;
            cusContact.ObjectType = Contact.ObjectType;
            cusContact.FirstName = Contact.FirstName;
            cusContact.LastName = Contact.LastName;
            cusContact.ContactName = Contact.ContactName;
            cusContact.Role = Contact.Role;
            cusContact.Phone = Contact.Phone;
            cusContact.Email = Contact.Email;
            cusContact.Gender = Contact.Gender;
            cusContact.GenderName = Contact.GenderName;
            cusContact.Other = Contact.Other;
            cusContact.DateOfBirth = Contact.DateOfBirth;
            cusContact.Address = Contact.Address;

            return new CreateContactParameter()
            {
                Contact = cusContact,
                UserId = UserId
            };
        }
    }
}

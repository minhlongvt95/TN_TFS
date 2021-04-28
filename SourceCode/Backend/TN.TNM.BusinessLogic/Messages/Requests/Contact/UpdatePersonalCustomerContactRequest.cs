using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.DataAccess.Messages.Parameters.Contact;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contact
{
    public class UpdatePersonalCustomerContactRequest : BaseRequest<UpdatePersonalCustomerContactParameter>
    {
        public PersonalCustomerContactBusinessModel Contact { get; set; }
        public override UpdatePersonalCustomerContactParameter ToParameter()
        {
            var cusContact = new PersonalCustomerContactModel();

            cusContact.ContactId = Contact.ContactId;
            cusContact.Email = Contact.Email;
            cusContact.WorkEmail = Contact.WorkEmail;
            cusContact.OtherEmail = Contact.OtherEmail;
            cusContact.Phone = Contact.Phone;
            cusContact.WorkPhone = Contact.WorkPhone;
            cusContact.OtherPhone = Contact.OtherPhone;
            cusContact.AreaId = Contact.AreaId;
            cusContact.ProvinceId = Contact.ProvinceId;
            cusContact.DistrictId = Contact.DistrictId;
            cusContact.WardId = Contact.WardId;
            cusContact.Address = Contact.Address;
            cusContact.Other = Contact.Other;
            cusContact.Longitude = Contact.Longitude;
            cusContact.Latitude = Contact.Latitude;

            return new UpdatePersonalCustomerContactParameter()
            {
                Contact = cusContact,
                UserId = UserId
            };
        }
    }
}

using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetAllCustomerContactResponse : BaseResponse
    {
        public List<ContactModel> ContactList { get; set; }
    }
}

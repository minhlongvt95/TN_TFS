using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;

namespace TN.TNM.BusinessLogic.Messages.Responses.Contact
{
    public class SearchContactResponse : BaseResponse
    {
        public List<ContactModel> ContactList { get; set; }
    }
}

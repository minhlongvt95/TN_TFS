using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;

namespace TN.TNM.BusinessLogic.Messages.Responses.Contact
{
    public class GetAllContactByObjectTypeResponse : BaseResponse
    {
        public List<ContactModel> ContactList { get; set; }
    }
}

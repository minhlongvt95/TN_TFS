using TN.TNM.BusinessLogic.Models.Contact;

namespace TN.TNM.BusinessLogic.Messages.Responses.Contact
{
    public class GetContactByIdResponse : BaseResponse
    {
        public ContactModel Contact { get; set; }
        public string FullAddress { get; set; }
    }
}

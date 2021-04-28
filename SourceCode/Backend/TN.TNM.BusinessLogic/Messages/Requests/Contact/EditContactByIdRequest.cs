using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.DataAccess.Messages.Parameters.Contact;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contact
{
    public class EditContactByIdRequest : BaseRequest<EditContactByIdParameters>
    {
        public ContactModel Contact { get; set; }
        public override EditContactByIdParameters ToParameter()
        {
            return new EditContactByIdParameters()
            {
                UserId = UserId,
                Contact = Contact.ToEntity()
            };
        }
    }
}

using System;
using TN.TNM.DataAccess.Messages.Parameters.Contact;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contact
{
    public class GetContactByIdRequest : BaseRequest<GetContactByIdParameter>
    {
        public Guid ContactId { get; set; }
        public override GetContactByIdParameter ToParameter()
        {
            return new GetContactByIdParameter()
            {
                UserId = UserId,
                ContactId = ContactId
            };
        }
    }
}

using System;
using TN.TNM.DataAccess.Messages.Parameters.Contact;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contact
{
    public class GetContactByObjectIdRequest : BaseRequest<GetContactByObjectIdParameter>
    {
        public Guid ObjectId { get; set; }
        public override GetContactByObjectIdParameter ToParameter()
        {
            return new GetContactByObjectIdParameter()
            {
                UserId = UserId,
                ObjectId = ObjectId
            };
        }
    }
}

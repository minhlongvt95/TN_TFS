using System;
using TN.TNM.DataAccess.Messages.Parameters.Contact;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contact
{
    public class DeleteContactByIdRequest : BaseRequest<DeleteContactByIdParameter>
    {
       public Guid ContactId { get; set; }
       public Guid ObjectId { get; set; }
       public string ObjectType { get; set; }

        public override DeleteContactByIdParameter ToParameter()
        {
            return new DeleteContactByIdParameter()
            {
                UserId = UserId,
                ContactId = ContactId,
                ObjectId = ObjectId,
                ObjectType = ObjectType
            };
        }
    }
}

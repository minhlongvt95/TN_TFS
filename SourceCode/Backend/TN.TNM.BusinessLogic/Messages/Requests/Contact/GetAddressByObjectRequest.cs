using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Contact;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contact
{
    public class GetAddressByObjectRequest : BaseRequest<GetAddressByObjectParameter>
    {
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }

        public override GetAddressByObjectParameter ToParameter()
        {
            return new GetAddressByObjectParameter()
            {
                ObjectId = ObjectId,
                ObjectType = ObjectType,
                UserId = UserId
            };
        }
    }
}

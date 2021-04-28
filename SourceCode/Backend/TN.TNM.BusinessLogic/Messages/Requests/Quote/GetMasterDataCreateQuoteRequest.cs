using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetMasterDataCreateQuoteRequest : BaseRequest<GetMasterDataCreateQuoteParameter>
    {
        public Guid? ObjectId { get; set; }
        public string ObjectType { get; set; }

        public override GetMasterDataCreateQuoteParameter ToParameter()
        {
            return new GetMasterDataCreateQuoteParameter()
            {
                UserId = UserId,
                ObjectId = ObjectId,
                ObjectType = ObjectType
            };
        }
    }
}

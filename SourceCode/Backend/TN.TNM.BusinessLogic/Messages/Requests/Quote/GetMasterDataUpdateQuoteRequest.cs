using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetMasterDataUpdateQuoteRequest : BaseRequest<GetMasterDataUpdateQuoteParameter>
    {
        public Guid QuoteId { get; set; }

        public override GetMasterDataUpdateQuoteParameter ToParameter()
        {
            return new GetMasterDataUpdateQuoteParameter()
            {
                UserId = UserId,
                QuoteId = QuoteId
            };
        }
    }
}

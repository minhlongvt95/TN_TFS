using System;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class UpdateActiveQuoteRequest : BaseRequest<UpdateActiveQuoteParameter>
    {
        public Guid QuoteId { get; set; }
        public override UpdateActiveQuoteParameter ToParameter()
        {
            return new UpdateActiveQuoteParameter
            {
                QuoteId = QuoteId,
                UserId = this.UserId
            };
        }
    }
}

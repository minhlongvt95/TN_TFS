using System;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetQuoteByIDRequest : BaseRequest<GetQuoteByIDParameter>
    {
        public Guid QuoteId { get; set; }
        public string ObjectType { get; set; }

        public override GetQuoteByIDParameter ToParameter()
        {
            return new GetQuoteByIDParameter
            {
                QuoteId = this.QuoteId,
                ObjectType = this.ObjectType,
                UserId = this.UserId
            };
        }
    }
}

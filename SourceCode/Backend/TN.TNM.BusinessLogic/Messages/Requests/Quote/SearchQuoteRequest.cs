using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class SearchQuoteRequest : BaseRequest<SearchQuoteParameter>
    {
        public string QuoteCode { get; set; }
        public string QuoteName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Guid?> ListStatusQuote { get; set; }
        public bool IsOutOfDate { get; set; }
        public bool IsCompleteInWeek { get; set; }
        public bool IsParticipant { get; set; }

        public override SearchQuoteParameter ToParameter()
        {
            return new SearchQuoteParameter()
            {
                UserId = UserId,
                QuoteCode = QuoteCode,
                QuoteName = QuoteName,
                StartDate = StartDate,
                EndDate = EndDate,
                ListStatusQuote = ListStatusQuote,
                IsOutOfDate = IsOutOfDate,
                IsCompleteInWeek = IsCompleteInWeek,
                IsParticipant = IsParticipant
            };
        }
    }
}

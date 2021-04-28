using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class SearchQuoteParameter : BaseParameter
    {
        public string QuoteCode { get; set; }
        public string QuoteName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Guid?> ListStatusQuote { get; set; }
        public bool IsOutOfDate { get; set; }
        public bool IsCompleteInWeek { get; set; }
        public bool IsParticipant { get; set; }
    }
}

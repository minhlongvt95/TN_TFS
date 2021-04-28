using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class UpdateActiveQuoteParameter : BaseParameter
    {
        public Guid QuoteId { get; set; }
    }
}

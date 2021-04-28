using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class CreateQuoteResult:BaseResult
    {
        public Guid QuoteID { get; set; }
        public List<QuoteDocumentEntityModel> ListQuoteDocument { get; set; }

    }
}

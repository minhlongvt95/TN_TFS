using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class CreateQuoteResponse:BaseResponse
    {
        public Guid QuoteID { get; set; }
        public List<QuoteDocumentModel> ListQuoteDocument { get; set; }
    }
}

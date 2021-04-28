using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetQuoteByIDResponse:BaseResponse
    {
        public QuoteModel QuoteEntityObject { get; set; }
        public List<QuoteDetailModel> ListQuoteDetail { get; set; }
        public List<QuoteDocumentModel> ListQuoteDocument { get; set; }
    }
}

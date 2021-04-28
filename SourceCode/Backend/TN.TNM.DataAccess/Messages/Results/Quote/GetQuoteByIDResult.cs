using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class GetQuoteByIDResult:BaseResult
    {
        public QuoteEntityModel QuoteEntityObject { get; set; }
        public List<QuoteDetailEntityModel> ListQuoteDetail { get; set; }
        public List<QuoteDocumentEntityModel> ListQuoteDocument { get; set; }
    }
}

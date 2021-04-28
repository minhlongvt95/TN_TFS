using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetAllQuoteResponse:BaseResponse
    {
        public List<QuoteModel> QuoteList { get; set; }
    }
}

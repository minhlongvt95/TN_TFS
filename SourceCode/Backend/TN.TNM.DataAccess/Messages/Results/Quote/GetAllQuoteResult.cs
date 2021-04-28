using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class GetAllQuoteResult:BaseResult
    {
        public List<QuoteEntityModel> QuoteList { get; set; }
    }
}

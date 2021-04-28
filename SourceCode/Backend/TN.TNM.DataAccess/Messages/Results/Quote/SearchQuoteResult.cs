using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class SearchQuoteResult : BaseResult
    {
        public List<QuoteEntityModel> ListQuote { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
    }
}

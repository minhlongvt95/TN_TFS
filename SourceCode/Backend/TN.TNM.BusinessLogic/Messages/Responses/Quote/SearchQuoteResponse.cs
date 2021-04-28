using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class SearchQuoteResponse : BaseResponse
    {
        public List<QuoteModel> ListQuote { get; set; }
        public List<CategoryModel> ListStatus { get; set; }
    }
}

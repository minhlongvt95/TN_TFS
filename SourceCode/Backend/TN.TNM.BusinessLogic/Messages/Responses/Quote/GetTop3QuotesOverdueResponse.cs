using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetTop3QuotesOverdueResponse : BaseResponse
    {
        public List<GetTop3QuotesOverdueeModel> QuoteList { get; set; }
    }
}

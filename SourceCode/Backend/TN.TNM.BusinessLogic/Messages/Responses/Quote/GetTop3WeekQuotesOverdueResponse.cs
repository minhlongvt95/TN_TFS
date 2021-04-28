using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetTop3WeekQuotesOverdueResponse : BaseResponse
    {
        public List<GetTop3WeekQuotesOverdueeModel> QuoteList { get; set; }
    }
}

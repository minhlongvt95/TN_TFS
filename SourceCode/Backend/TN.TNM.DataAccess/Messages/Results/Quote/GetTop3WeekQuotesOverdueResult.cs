using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class GetTop3WeekQuotesOverdueResult : BaseResult
    {
        public List<GetTop3WeekQuotesOverdueModel> QuotesOverdueList { get; set; }
    }
}


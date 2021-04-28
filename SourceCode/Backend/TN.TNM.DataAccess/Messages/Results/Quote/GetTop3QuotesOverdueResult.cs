using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class GetTop3QuotesOverdueResult : BaseResult
    {
        public List<GetTop3QuotesOverdueModel> QuotesOverdueList { get; set; }
    }
}


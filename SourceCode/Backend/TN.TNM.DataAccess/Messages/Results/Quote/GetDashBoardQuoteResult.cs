using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class GetDashBoardQuoteResult : BaseResult
    {
        public GetDashBoardQuoteModel DashBoardQuote { get; set; }
    }
}


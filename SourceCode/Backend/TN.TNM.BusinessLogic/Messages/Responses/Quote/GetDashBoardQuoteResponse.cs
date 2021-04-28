using TN.TNM.BusinessLogic.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetDashBoardQuoteResponse : BaseResponse
    {
        public GetDashBoardQuoteeModel DashBoardQuote { get; set; }
    }
}

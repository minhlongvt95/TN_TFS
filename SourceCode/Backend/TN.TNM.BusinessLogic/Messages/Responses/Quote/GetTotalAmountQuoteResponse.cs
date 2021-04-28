using TN.TNM.BusinessLogic.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetTotalAmountQuoteResponse : BaseResponse
    {
        public GetTotalAmountQuoteeModel ToTalAmount { get; set; }
    }
}

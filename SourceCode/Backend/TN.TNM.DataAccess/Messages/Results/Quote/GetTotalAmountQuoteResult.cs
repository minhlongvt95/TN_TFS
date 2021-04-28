using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class GetTotalAmountQuoteResult : BaseResult
    {
        public GetTotalAmountQuoteModel ToTalAmount { get; set; }
    }
}


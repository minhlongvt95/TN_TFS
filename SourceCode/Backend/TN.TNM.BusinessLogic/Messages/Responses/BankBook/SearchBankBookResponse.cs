namespace TN.TNM.BusinessLogic.Messages.Responses.BankBook
{
    public class SearchBankBookResponse : BaseResponse
    {
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
    }
}

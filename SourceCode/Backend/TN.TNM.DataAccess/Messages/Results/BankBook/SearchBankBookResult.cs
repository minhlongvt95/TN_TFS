namespace TN.TNM.DataAccess.Messages.Results.BankBook
{
    public class SearchBankBookResult : BaseResult
    {
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
    }
}

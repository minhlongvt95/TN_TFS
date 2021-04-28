namespace TN.TNM.DataAccess.Messages.Results.CashBook
{
    public class GetSurplusCashBookPerMonthResult : BaseResult
    {
        public decimal? OpeningSurplus { get; set; }
        public decimal? ClosingSurplus { get; set; }
    }
}

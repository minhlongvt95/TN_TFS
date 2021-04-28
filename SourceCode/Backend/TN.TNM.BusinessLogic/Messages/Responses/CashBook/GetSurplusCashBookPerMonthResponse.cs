namespace TN.TNM.BusinessLogic.Messages.Responses.CashBook
{
    public class GetSurplusCashBookPerMonthResponse:BaseResponse
    {
        public decimal? OpeningSurplus { get; set; }
        public decimal? ClosingSurplus { get; set; }
    }
}

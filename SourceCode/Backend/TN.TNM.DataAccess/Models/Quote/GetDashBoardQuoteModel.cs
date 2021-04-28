namespace TN.TNM.DataAccess.Models.Quote
{
    public class GetDashBoardQuoteModel
    {
        public int CountNew { get; set; }
        public int CountInProgress { get; set; }
        public int CountDone { get; set; }
        public int CountWaiting { get; set; }
        public int CountAbort { get; set; }
        public int CountClose { get; set; }

    }
}

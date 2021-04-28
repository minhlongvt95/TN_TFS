using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetDashBoardQuoteParameter : BaseParameter
    {
        public Guid PersonInChangeId { get; set; }
        public int MonthQuote { get; set; }
        public int YearQuote { get; set; }
    }
}

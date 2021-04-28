using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetDataQuoteToPieChartParameter : BaseParameter
    {
        public int MonthQuote { get; set; }
        public int YearQuote { get; set; }
    }
}

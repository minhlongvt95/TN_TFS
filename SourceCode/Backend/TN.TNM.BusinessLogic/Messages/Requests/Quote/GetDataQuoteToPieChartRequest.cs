using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetDataQuoteToPieChartRequest : BaseRequest<GetDataQuoteToPieChartParameter>
    {
        public int MonthQuote { get; set; }
        public int YearQuote { get; set; }

        public override GetDataQuoteToPieChartParameter ToParameter()
        {
            return new GetDataQuoteToPieChartParameter()
            {
                MonthQuote = MonthQuote,
                YearQuote = YearQuote,
                UserId = UserId
            };
        }
    }
}

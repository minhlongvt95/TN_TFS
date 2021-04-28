using System;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetDashBoardQuoteRequest : BaseRequest<GetDashBoardQuoteParameter>
    {
        public Guid PersonInChangeId { get; set; }
        public int MonthQuote { get; set; }
        public int YearQuote { get; set; }

        public override GetDashBoardQuoteParameter ToParameter()
        {
            return new GetDashBoardQuoteParameter
            {
                UserId=this.UserId,
                PersonInChangeId = this.PersonInChangeId,
                MonthQuote = this.MonthQuote,
                YearQuote = this.YearQuote
            };
        }
    }
}

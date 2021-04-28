using System;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetTop3WeekQuotesOverdueRequest : BaseRequest<GetTop3WeekQuotesOverdueParameter>
    {
        public Guid PersonInChangeId { get; set; }

        public override GetTop3WeekQuotesOverdueParameter ToParameter()
        {
            return new GetTop3WeekQuotesOverdueParameter
            {
                PersonInChangeId = this.PersonInChangeId,
                UserId = UserId
            };
        }
    }
}

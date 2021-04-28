using System;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetTop3QuotesOverdueRequest : BaseRequest<GetTop3QuotesOverdueParameter>
    {
        public Guid PersonInChangeId { get; set; }

        public override GetTop3QuotesOverdueParameter ToParameter()
        {
            return new GetTop3QuotesOverdueParameter
            {
                PersonInChangeId = this.PersonInChangeId,
                UserId = UserId
            };
        }
    }
}

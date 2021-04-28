using System;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetTop3PotentialCustomersRequest : BaseRequest<GetTop3PotentialCustomersParameter>
    {
        public Guid PersonInChangeId { get; set; }

        public override GetTop3PotentialCustomersParameter ToParameter()
        {
            return new GetTop3PotentialCustomersParameter
            {
                PersonInChangeId = this.PersonInChangeId,
                UserId=this.UserId
            };
        }
    }
}

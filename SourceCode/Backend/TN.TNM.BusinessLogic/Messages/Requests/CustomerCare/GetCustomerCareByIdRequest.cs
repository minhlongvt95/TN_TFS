using System;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class GetCustomerCareByIdRequest : BaseRequest<GetCustomerCareByIdParameter>
    {
        public Guid CustomerCareId { get; set; }

        public override GetCustomerCareByIdParameter ToParameter()
        {
            return new GetCustomerCareByIdParameter
            {
                CustomerCareId = this.CustomerCareId,
                UserId = this.UserId
            };
        }
    }
}

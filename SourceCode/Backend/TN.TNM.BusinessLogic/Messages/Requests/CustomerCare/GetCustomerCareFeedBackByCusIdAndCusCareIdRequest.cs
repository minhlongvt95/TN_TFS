using System;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class GetCustomerCareFeedBackByCusIdAndCusCareIdRequest : BaseRequest<GetCustomerCareFeedBackByCusIdAndCusCareIdParameter>
    {
        public Guid CustomerId { get; set; }
        public Guid CustomerCareId { get; set; }

        public override GetCustomerCareFeedBackByCusIdAndCusCareIdParameter ToParameter()
        {
            return new GetCustomerCareFeedBackByCusIdAndCusCareIdParameter
            {
                CustomerId = CustomerId,
                CustomerCareId = CustomerCareId,
                UserId = UserId
            };
        }
    }
}

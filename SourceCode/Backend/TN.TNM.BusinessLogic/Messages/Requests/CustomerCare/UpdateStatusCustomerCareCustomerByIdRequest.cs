using System;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class UpdateStatusCustomerCareCustomerByIdRequest : BaseRequest<UpdateStatusCustomerCareCustomerByIdParameter>
    {
        public Guid CustomerCareCustomerId { get; set; }
        public Guid StatusId { get; set; }

        public override UpdateStatusCustomerCareCustomerByIdParameter ToParameter()
        {
            return new UpdateStatusCustomerCareCustomerByIdParameter
            {
                CustomerCareCustomerId = CustomerCareCustomerId,
                StatusId = StatusId,
                UserId = UserId
            };
        }
    }
}

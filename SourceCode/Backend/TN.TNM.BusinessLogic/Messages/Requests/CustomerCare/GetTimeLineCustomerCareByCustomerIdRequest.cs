using System;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class GetTimeLineCustomerCareByCustomerIdRequest : BaseRequest<GetTimeLineCustomerCareByCustomerIdParameter>
    {
        public Guid CustomerId { get; set; }
        public DateTime First_day { get; set; }
        public DateTime Last_day { get; set; }

        public override GetTimeLineCustomerCareByCustomerIdParameter ToParameter()
        {
            return new GetTimeLineCustomerCareByCustomerIdParameter
            {
                CustomerId = CustomerId,
                First_day = First_day,
                Last_day = Last_day,
                UserId = UserId
            };
        } 
    }
}

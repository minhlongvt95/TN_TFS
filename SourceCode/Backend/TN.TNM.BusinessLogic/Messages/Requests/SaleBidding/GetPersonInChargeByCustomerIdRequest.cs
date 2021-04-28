using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class GetPersonInChargeByCustomerIdRequest:BaseRequest<GetPersonInChargeByCustomerIdParameter>
    {
        public Guid CustomerId { get; set; }
        public override GetPersonInChargeByCustomerIdParameter ToParameter()
        {
            return new GetPersonInChargeByCustomerIdParameter()
            {
                CustomerId =CustomerId,
                UserId = UserId
            };
        }
    }
}

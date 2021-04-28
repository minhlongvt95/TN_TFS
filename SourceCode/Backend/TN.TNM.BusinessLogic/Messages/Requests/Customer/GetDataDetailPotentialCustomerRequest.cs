using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetDataDetailPotentialCustomerRequest: BaseRequest<GetDataDetailPotentialCustomerParameter>
    {
        public Guid CustomerId { get; set; }
        public override GetDataDetailPotentialCustomerParameter ToParameter()
        {
            return new GetDataDetailPotentialCustomerParameter
            {
                CustomerId = CustomerId,
                UserId = UserId
            };
        }

    }
}

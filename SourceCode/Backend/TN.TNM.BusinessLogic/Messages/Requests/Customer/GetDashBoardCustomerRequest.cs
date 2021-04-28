using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetDashBoardCustomerRequest : BaseRequest<GetDashBoardCustomerParameter>
    {
        public string CustomerName { get; set; }

        public override GetDashBoardCustomerParameter ToParameter()
        {
            return new GetDashBoardCustomerParameter()
            {
                CustomerName = CustomerName,
                UserId = UserId
            };
        }
    }
}

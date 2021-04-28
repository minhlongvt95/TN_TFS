using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetCustomerDeliveryRequest : BaseRequest<GetCustomerDeliveryParameter>
    {
        public override GetCustomerDeliveryParameter ToParameter()
        {
            return new GetCustomerDeliveryParameter { };
        }
    }
}

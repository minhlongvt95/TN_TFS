using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class CheckExistsCustomerByPhoneRequest : BaseRequest<CheckExistsCustomerByPhoneParameter>
    {
        public string CustomerPhone { get; set; }

        public override CheckExistsCustomerByPhoneParameter ToParameter()
        {
            return new CheckExistsCustomerByPhoneParameter()
            {
                UserId = UserId,
                CustomerPhone = CustomerPhone
            };
        }
    }
}

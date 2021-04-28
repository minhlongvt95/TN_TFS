using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetListCustomerOrderByIdCustomerIdRequest : BaseRequest<GetListCustomerOrderByIdCustomerIdParameter>
    {
        public Guid CustomerId { get; set; }

        public override GetListCustomerOrderByIdCustomerIdParameter ToParameter()
        {
            return new GetListCustomerOrderByIdCustomerIdParameter
            {
                CustomerId = this.CustomerId
            };
        }
    }
}

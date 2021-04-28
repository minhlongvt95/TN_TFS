using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetListCustomerByTypeRequest : BaseRequest<GetListCustomerByTypeParameter>
    {
        public string CustomerType { get; set; }
        public override GetListCustomerByTypeParameter ToParameter()
        {
            return new GetListCustomerByTypeParameter
            {
                CustomerType = CustomerType,
                UserId = UserId
            };
        }
    }
}

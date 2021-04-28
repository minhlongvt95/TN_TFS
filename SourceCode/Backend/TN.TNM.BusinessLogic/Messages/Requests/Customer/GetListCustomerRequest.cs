using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetListCustomerRequest:BaseRequest<GetListCustomerParameter>
    {
        public string CategoryTypeCode { get; set; }
       // public Guid UserId { get; set; }

        public override GetListCustomerParameter ToParameter()
        {
            return new GetListCustomerParameter
            {
                CategoryTypeCode = CategoryTypeCode,
                UserId = UserId
            };
        }
    }
}

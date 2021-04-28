using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class SearchContactCustomerRequest : BaseRequest<SearchContactCustomerParameter>
    {
        public override SearchContactCustomerParameter ToParameter()
        {
            return new SearchContactCustomerParameter
            {
                UserId = this.UserId
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class FilterCustomerRequest : BaseRequest<FilterCustomerParameter>
    {
        public override FilterCustomerParameter ToParameter()
        {
            return new FilterCustomerParameter
            {

            };
        }
    }
}

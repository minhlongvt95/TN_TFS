using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetDataImportPotentialCustomerRequest: BaseRequest<GetDataImportPotentialCustomerParameter>
    {
        public override GetDataImportPotentialCustomerParameter ToParameter()
        {
            return new GetDataImportPotentialCustomerParameter
            {
                UserId = UserId
            };
        }
    }
}

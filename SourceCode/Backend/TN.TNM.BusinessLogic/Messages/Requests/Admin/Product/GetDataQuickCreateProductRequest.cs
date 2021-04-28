using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class GetDataQuickCreateProductRequest: BaseRequest<GetDataQuickCreateProductParameter>
    {
        public override GetDataQuickCreateProductParameter ToParameter()
        {
            return new GetDataQuickCreateProductParameter
            {
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class GetMasterDataPriceProductRequest : BaseRequest<GetMasterDataPriceProductParameter>
    {
        public override GetMasterDataPriceProductParameter ToParameter()
        {
            return new GetMasterDataPriceProductParameter
            {
                UserId = UserId,
            };
        }
    }
}

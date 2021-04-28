using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetMasterDataSearchVendorOrderRequest : BaseRequest<GetMasterDataSearchVendorOrderParameter>
    {
        public override GetMasterDataSearchVendorOrderParameter ToParameter()
        {
            return new GetMasterDataSearchVendorOrderParameter
            {
                UserId = UserId
            };
        }
    }
}

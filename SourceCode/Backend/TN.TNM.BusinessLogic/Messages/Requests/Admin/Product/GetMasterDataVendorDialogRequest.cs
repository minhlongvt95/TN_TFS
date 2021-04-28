using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class GetMasterDataVendorDialogRequest : BaseRequest<GetMasterDataVendorDialogParameter>
    {
        public override GetMasterDataVendorDialogParameter ToParameter()
        {
            return new GetMasterDataVendorDialogParameter
            {
                UserId = UserId
            };
        }
    }
}

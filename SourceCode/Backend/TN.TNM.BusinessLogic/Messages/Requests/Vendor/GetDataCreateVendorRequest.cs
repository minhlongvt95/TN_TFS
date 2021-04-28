using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetDataCreateVendorRequest : BaseRequest<GetDataCreateVendorParameter>
    {
        public override GetDataCreateVendorParameter ToParameter()
        {
            return new GetDataCreateVendorParameter()
            {
                UserId = UserId
            };
        }
    }
}

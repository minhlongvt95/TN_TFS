using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetDataAddVendorOrderDetailRequest: BaseRequest<GetDataAddVendorOrderDetailParameter>
    {
        public override GetDataAddVendorOrderDetailParameter ToParameter()
        {
            return new GetDataAddVendorOrderDetailParameter()
            {
                UserId = UserId
            };
        }
    }
}

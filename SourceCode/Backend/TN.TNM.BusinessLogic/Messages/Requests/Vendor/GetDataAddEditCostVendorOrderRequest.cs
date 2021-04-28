using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetDataAddEditCostVendorOrderRequest : BaseRequest<GetDataAddEditCostVendorOrderParameter>
    {
        public override GetDataAddEditCostVendorOrderParameter ToParameter()
        {
            return new GetDataAddEditCostVendorOrderParameter()
            {
                UserId = UserId,
            };
        }
    }
}

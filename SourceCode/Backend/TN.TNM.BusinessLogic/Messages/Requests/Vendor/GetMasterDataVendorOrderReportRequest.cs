using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetMasterDataVendorOrderReportRequest : BaseRequest<GetMasterDataVendorOrderReportParameter>
    {
        public override GetMasterDataVendorOrderReportParameter ToParameter()
        {
            return new GetMasterDataVendorOrderReportParameter()
            {
                UserId = UserId
            };
        }
    }
}

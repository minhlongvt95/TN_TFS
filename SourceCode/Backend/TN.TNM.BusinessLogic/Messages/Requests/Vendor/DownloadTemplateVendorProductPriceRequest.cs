using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class DownloadTemplateVendorProductPriceRequest : BaseRequest<DownloadTemplateVendorProductPriceParameter>
    {
        public override DownloadTemplateVendorProductPriceParameter ToParameter()
        {
            return new DownloadTemplateVendorProductPriceParameter
            {
                UserId = UserId
            };
        }
    }
}

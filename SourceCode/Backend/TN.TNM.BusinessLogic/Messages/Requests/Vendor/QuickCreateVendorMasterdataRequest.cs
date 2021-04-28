using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class QuickCreateVendorMasterdataRequest: BaseRequest<QuickCreateVendorMasterdataParameter>
    {
        public override QuickCreateVendorMasterdataParameter ToParameter()
        {
            return new QuickCreateVendorMasterdataParameter
            {

            };
        }
    }
}

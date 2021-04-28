using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Receivable.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Receivable.Vendor
{
    public class GetDataSearchReceivableVendorRequest : BaseRequest<GetDataSearchReceivableVendorParameter>
    {
        public override GetDataSearchReceivableVendorParameter ToParameter()
        {
            return new GetDataSearchReceivableVendorParameter
            {
                UserId = UserId
            };
        }
    }
}

using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetDataSearchVendorQuoteRequest : BaseRequest<GetDataSearchVendorQuoteParameter>
    {
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public override GetDataSearchVendorQuoteParameter ToParameter()
        {
            return new GetDataSearchVendorQuoteParameter() {
                UserId = UserId,
                VendorCode = VendorCode,
                VendorName = VendorName,
            };
        }
    }
}

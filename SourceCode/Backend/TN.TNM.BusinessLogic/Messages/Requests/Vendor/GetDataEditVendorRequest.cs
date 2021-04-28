using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetDataEditVendorRequest: BaseRequest<GetDataEditVendorParameter>
    {
        public Guid VendorId { get; set; }
        public override GetDataEditVendorParameter ToParameter()
        {
            return new GetDataEditVendorParameter()
            {
                VendorId = VendorId,
                UserId = UserId
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetDataEditVendorOrderRequest : BaseRequest<GetDataEditVendorOrderParameter>
    {
        public Guid VendorOrderId { get; set; }
        public bool IsAprroval { get; set; }
        public string Description { get; set; }

        public override GetDataEditVendorOrderParameter ToParameter()
        {
            return new GetDataEditVendorOrderParameter()
            {
                VendorOrderId = VendorOrderId,
                IsAprroval = IsAprroval,
                Description = Description,
                UserId = UserId
            };
        }
    }
}

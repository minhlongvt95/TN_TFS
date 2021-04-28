using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class DeleteVendorProductPriceRequest : BaseRequest<DeleteVendorProductPriceParameter>
    {
        public Guid ProductVendorMappingId { get; set; }

        public override DeleteVendorProductPriceParameter ToParameter()
        {
            return new DeleteVendorProductPriceParameter
            {
                UserId = UserId,
                ProductVendorMappingId = ProductVendorMappingId
            };
        }
    }
}

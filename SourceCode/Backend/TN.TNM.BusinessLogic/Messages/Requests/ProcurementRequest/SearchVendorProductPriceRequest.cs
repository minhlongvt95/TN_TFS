using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementRequest
{
    public class SearchVendorProductPriceRequest : BaseRequest<SearchVendorProductPriceParameter>
    {
        public Guid ProductId { get; set; }
        public Guid VendorId { get; set; }
        public int Quantity { get; set; }
        public override SearchVendorProductPriceParameter ToParameter()
        {
            return new SearchVendorProductPriceParameter
            {
                ProductId = ProductId,
                VendorId = VendorId,
                Quantity = Quantity,
                UserId = UserId,
            };
        }
    }
}

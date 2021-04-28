using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class SearchVendorProductPriceRequest : BaseRequest<SearchVendorProductPriceParameter>
    {
        public string ProductName { get; set; }
        public string VendorName { get; set; }
        public override SearchVendorProductPriceParameter ToParameter()
        {
            return new SearchVendorProductPriceParameter
            {
                ProductName = ProductName,
                VendorName = VendorName,
                UserId = UserId,
            };
        }
    }
}

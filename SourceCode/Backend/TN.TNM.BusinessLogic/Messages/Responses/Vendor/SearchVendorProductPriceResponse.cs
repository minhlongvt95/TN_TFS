using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class SearchVendorProductPriceResponse : BaseResponse
    {
        public List<ProductVendorMappingModel> ListVendorProductPrice { get; set; }
    }
}

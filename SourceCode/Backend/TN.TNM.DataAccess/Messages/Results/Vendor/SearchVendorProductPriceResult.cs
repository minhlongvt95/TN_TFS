using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class SearchVendorProductPriceResult : BaseResult
    {
        public List<ProductVendorMappingEntityModel> ListVendorProductPrice { get; set; }
    }
}

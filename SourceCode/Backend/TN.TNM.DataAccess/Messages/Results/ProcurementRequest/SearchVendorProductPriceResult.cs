using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.ProcurementRequest
{
    public class SearchVendorProductPriceResult : BaseResult
    {
        public ProductVendorMappingEntityModel VendorProductPrice { get; set; }
    }
}

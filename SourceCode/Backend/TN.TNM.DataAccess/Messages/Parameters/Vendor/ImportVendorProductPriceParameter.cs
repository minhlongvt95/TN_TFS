using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class ImportVendorProductPriceParameter : BaseParameter
    {
        public List<ProductVendorMappingEntityModel> ListProductVendorMapping { get; set; }
    }
}

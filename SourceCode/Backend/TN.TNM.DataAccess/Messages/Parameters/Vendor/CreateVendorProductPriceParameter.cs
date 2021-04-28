using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class CreateVendorProductPriceParameter : BaseParameter
    {
        public ProductVendorMappingEntityModel ProductVendorMapping { get; set; }
        public List<Guid> ListSuggestedSupplierQuoteId { get; set; }
    }
}

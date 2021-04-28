using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.ProcurementRequest
{
    public class SearchVendorProductPriceResponse : BaseResponse
    {
        public DataAccess.Models.Product.ProductVendorMappingEntityModel VendorProductPrice { get; set; }
    }
}

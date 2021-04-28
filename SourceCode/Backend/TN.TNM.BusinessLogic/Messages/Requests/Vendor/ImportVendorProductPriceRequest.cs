using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class ImportVendorProductPriceRequest : BaseRequest<ImportVendorProductPriceParameter>
    {
        public List<ProductVendorMappingEntityModel> ListProductVendorMapping { get; set; }
        public override ImportVendorProductPriceParameter ToParameter()
        {
            return new ImportVendorProductPriceParameter
            {
                ListProductVendorMapping = ListProductVendorMapping,
                UserId = UserId,
            };
        }
    }
}

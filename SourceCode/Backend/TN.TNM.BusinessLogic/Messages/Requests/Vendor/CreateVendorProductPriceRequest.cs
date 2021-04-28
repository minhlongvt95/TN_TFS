using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class CreateVendorProductPriceRequest : BaseRequest<CreateVendorProductPriceParameter>
    {
        public ProductVendorMappingEntityModel ProductVendorMapping { get; set; }
        public List<Guid> ListSuggestedSupplierQuoteId { get; set; }
        public override CreateVendorProductPriceParameter ToParameter()
        {
            var productVendorMappingEntityModel = new ProductVendorMappingEntityModel
            {
                ProductId = ProductVendorMapping.ProductId,
                VendorId = ProductVendorMapping.VendorId,
                ProductVendorMappingId = ProductVendorMapping.ProductVendorMappingId,
                VendorProductName = ProductVendorMapping.VendorProductName,
                VendorProductCode = ProductVendorMapping.VendorProductCode,
                MoneyUnitId = ProductVendorMapping.MoneyUnitId,
                Price = ProductVendorMapping.Price,
                FromDate = ProductVendorMapping.FromDate,
                ToDate = ProductVendorMapping.ToDate,
                MiniumQuantity = ProductVendorMapping.MiniumQuantity,
            };

            return new CreateVendorProductPriceParameter
            {
                ProductVendorMapping = productVendorMappingEntityModel,
                ListSuggestedSupplierQuoteId = ListSuggestedSupplierQuoteId,
                UserId = UserId
            };
        }
    }
}

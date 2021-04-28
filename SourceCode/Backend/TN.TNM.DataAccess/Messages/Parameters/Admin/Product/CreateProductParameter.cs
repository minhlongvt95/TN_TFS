using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class CreateProductParameter : BaseParameter
    {
        public ProductEntityModel Product { get; set; }
        public List<ProductVendorMappingEntityModel> ListProductVendorMapping { get; set; }
        public List<Models.ProductAttributeCategory.ProductAttributeCategoryEntityModel> ListProductAttributeCategory { get; set; }
        public List<ProductQuantityInWarehouseEntityModel> ListInventoryReport { get; set; }
        public List<Databases.Entities.ProductImage> ListProductImage { get; set; }
        public List<ProductBillOfMaterialsEntityModel> ListProductBillOfMaterials { get; set; }
    }
}

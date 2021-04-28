using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class UpdateProductParameter : BaseParameter
    {
        public Databases.Entities.Product Product { get; set; }
        //public List<Databases.Entities.Vendor> lstVendor { get; set; }
        public List<ProductVendorMappingEntityModel> ListProductVendorMapping { get; set; }
        public List<ProductAttributeCategory> lstProductAttributeCategory { get; set; }
        public List<Guid> listVendor { get; set; }
        public List<ProductQuantityInWarehouseEntityModel> ListInventoryReport { get; set; }
        public List<ProductBillOfMaterialsEntityModel> ListProductBillOfMaterials { get; set; }
    }
}

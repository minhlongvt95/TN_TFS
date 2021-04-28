using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetProductByIDResult : BaseResult
    {
        public Databases.Entities.Product Product { get; set; }
        public List<ProductVendorMapping> LstProductVendorMapping { get; set; }
        public List<ProductAttributeCategory> lstProductAttributeCategory { get; set; }
        public List<CustomerOrderEntityModel> lstCustomerOrder { get; set; }
        public List<ProductImage> ListProductImage { get; set; }
        public List<Models.Product.InventoryReportByProductIdEntityModel> ListInventory { get; set; }
        public bool CanDelete { get; set; }
        public List<Models.Product.ProductBillOfMaterialsEntityModel> ListProductBillOfMaterials { get; set; }
    }
}

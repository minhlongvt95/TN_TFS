using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.ProductAttributeCategory;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class GetProductByIDResponse : BaseResponse
    {
        public ProductModel Product { get; set; }
        public List<ProductVendorMappingModel> LstProductVendorMapping { get; set; }
        public List<ProductAttributeCategoryModel> lstProductAttributeCategory { get; set; }
        public List<CustomerOrderModel> lstCustomerOrder { get; set; }
        public List<DataAccess.Databases.Entities.ProductImage> ListProductImage { get; set; }
        public List<DataAccess.Models.Product.InventoryReportByProductIdEntityModel> ListInventory { get; set; }
        public bool CanDelete { get; set; }
        public List<DataAccess.Models.Product.ProductBillOfMaterialsEntityModel> ListProductBillOfMaterials { get; set; }
    }
}

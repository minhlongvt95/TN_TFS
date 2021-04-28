using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class GetMasterdataCreateProductResponse: BaseResponse
    {
        public List<Models.Vendor.VendorModel> ListVendor { get; set; }
        public List<Models.Category.CategoryModel> ListProductMoneyUnit { get; set; }
        public List<Models.Category.CategoryModel> ListProductUnit { get; set; }
        public List<Models.Category.CategoryModel> ListProperty { get; set; }
        public List<Models.Category.CategoryModel> ListPriceInventory { get; set; }
        public List<Models.WareHouse.WareHouseModel> ListWarehouse { get; set; }
        public List<string> ListProductCode { get; set; }
        public List<string> ListProductUnitName { get; set; }
    }
}

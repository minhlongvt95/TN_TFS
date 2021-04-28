using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class GetListProductResponse: BaseResponse
    {
        public List<Models.Admin.ProductCategoryModel> ListProductCategory { get; set; }
        public List<Models.Vendor.VendorModel> ListVendor { get; set; }
        public List<Models.Category.CategoryModel> ListUnit { get; set; }
        public List<Models.Category.CategoryModel> ListProperty { get; set; }
        public List<Models.Category.CategoryModel> ListPriceInventory { get; set; }
    }
}

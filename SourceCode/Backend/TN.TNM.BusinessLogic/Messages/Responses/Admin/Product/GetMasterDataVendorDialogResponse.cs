using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class GetMasterDataVendorDialogResponse : BaseResponse
    {
        public List<Models.Vendor.VendorModel> ListVendor { get; set; }
        public List<Models.Category.CategoryModel> ListProductMoneyUnit { get; set; }
        public List<Models.Product.ProductModel> ListProduct { get; set; }
        public List<Models.Vendor.SuggestedSupplierQuotesModel> ListSuggestedSupplierQuote { get; set; }
    }
}

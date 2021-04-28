using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetDataQuoteAddEditProductDialogResponse : BaseResponse
    {
        public List<CategoryModel> ListUnitMoney { get; set; }
        public List<CategoryModel> ListUnitProduct { get; set; }
        public List<VendorModel> ListVendor { get; set; }
        public List<ProductModel> ListProduct { get; set; }
        public List<PriceProductModel> ListPriceProduct { get; set; }
    }
}

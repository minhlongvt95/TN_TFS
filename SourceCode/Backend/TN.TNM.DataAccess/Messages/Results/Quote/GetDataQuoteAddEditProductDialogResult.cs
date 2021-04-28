using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class GetDataQuoteAddEditProductDialogResult : BaseResult
    {
        public List<CategoryEntityModel> ListUnitMoney { get; set; }
        public List<CategoryEntityModel> ListUnitProduct { get; set; }
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
        public List<Databases.Entities.PriceProduct> ListPriceProduct { get; set; }
    }
}

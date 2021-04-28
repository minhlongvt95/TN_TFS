using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetMasterDataVendorDialogResult : BaseResult
    {
        public List<Databases.Entities.Vendor> ListVendor { get; set; }
        public List<Databases.Entities.Category> ListProductMoneyUnit { get; set; }
        public List<Databases.Entities.Product> ListProduct { get; set; }
        public List<Databases.Entities.SuggestedSupplierQuotes> ListSuggestedSupplierQuote { get; set; }
    }
}

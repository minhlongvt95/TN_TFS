using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetDataSearchVendorQuoteResult : BaseResult
    {
        //public List<VendorQuoteEntityModel> VendorQuoteList { get; set; }
        public List<SuggestedSupplierQuotesEntityModel> ListVendorQuote { get; set; }
      
    }
}

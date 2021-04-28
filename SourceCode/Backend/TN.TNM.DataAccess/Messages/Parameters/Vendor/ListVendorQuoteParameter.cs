using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class ListVendorQuoteParameter : BaseParameter
    {
        public SuggestedSupplierQuotes SuggestedSupplierQuotes { get; set; }
        public List<SuggestedSupplierQuotesDetail> SuggestedSupplierQuoteDetailList { get; set; }
        public int Index { get; set; }
    }
}

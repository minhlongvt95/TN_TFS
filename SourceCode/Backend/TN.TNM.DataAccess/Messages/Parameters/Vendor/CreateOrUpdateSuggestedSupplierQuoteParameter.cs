using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class CreateOrUpdateSuggestedSupplierQuoteParameter : BaseParameter
    {
        public SuggestedSupplierQuotesEntityModel SuggestedSupplierQuotes { get; set; }
        public List<SuggestedSupplierQuotesDetailEntityModel> ListSuggestedSupplierQuotesDetail { get; set; }
    }
}

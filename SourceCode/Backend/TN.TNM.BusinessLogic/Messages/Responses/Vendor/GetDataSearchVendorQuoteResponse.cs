using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetDataSearchVendorQuoteResponse : BaseResponse
    {
        //public List<VendorQuoteModel> VendorQuoteList { get; set; }
        public List<SuggestedSupplierQuotesModel> ListVendorQuote { get; set; }

    }
}

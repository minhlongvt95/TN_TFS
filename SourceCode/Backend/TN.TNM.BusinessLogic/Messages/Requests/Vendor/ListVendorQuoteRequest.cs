using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class ListVendorQuoteRequest : BaseRequest<ListVendorQuoteParameter>
    {
        public SuggestedSupplierQuotes SuggestedSupplierQuotes { get; set; }
        public List<SuggestedSupplierQuotesDetail> SuggestedSupplierQuoteDetailList { get; set; }
        public int Index { get; set; }
        public override ListVendorQuoteParameter ToParameter()
        {
            return new ListVendorQuoteParameter()
            {
                SuggestedSupplierQuotes = SuggestedSupplierQuotes,
                SuggestedSupplierQuoteDetailList = SuggestedSupplierQuoteDetailList,
                Index = Index,
                UserId = UserId
            };
        }
    }
}

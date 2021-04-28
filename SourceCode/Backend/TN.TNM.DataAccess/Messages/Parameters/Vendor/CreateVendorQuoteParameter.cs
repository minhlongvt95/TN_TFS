using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class CreateVendorQuoteParameter : BaseParameter
    {
        public List<ListVendorQuoteParameter> SuggestedSupplierQuoteList { get; set; }
    }
}

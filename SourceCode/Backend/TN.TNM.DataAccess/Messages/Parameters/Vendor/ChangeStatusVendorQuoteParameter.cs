using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class ChangeStatusVendorQuoteParameter : BaseParameter
    {
        public Guid SuggestedSupplierQuoteId { get; set; }
        public Guid StatusId { get; set; }
    }
}

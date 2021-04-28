using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest
{
    public class SearchVendorProductPriceParameter : BaseParameter
    {
        public Guid ProductId { get; set; }
        public Guid VendorId { get; set; }
        public int Quantity { get; set; }
    }
}

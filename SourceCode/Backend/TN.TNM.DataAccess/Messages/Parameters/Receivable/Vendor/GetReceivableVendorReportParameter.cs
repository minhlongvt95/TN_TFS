using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Receivable.Vendor
{
    public class GetReceivableVendorReportParameter : BaseParameter
    {
        //public string VendorCode { get; set; }
        public List<Guid> VendorCode { get; set; }
        public string VendorName { get; set; }
        public DateTime? ReceivalbeDateTo { get; set; }
    }
}

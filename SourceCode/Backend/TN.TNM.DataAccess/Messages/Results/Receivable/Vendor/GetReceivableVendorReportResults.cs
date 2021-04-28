using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Receivable;

namespace TN.TNM.DataAccess.Messages.Results.Receivable.Vendor
{
    public class GetReceivableVendorReportResults : BaseResult
    {
        public List<ReceivableVendorReportEntityModel> ReceivableVendorReport { get; set; }
        public decimal? TotalPurchase { get; set; }
        public decimal? TotalPaid { get; set; }
       
    }
}

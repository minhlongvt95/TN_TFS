using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Receivable;

namespace TN.TNM.DataAccess.Messages.Results.Receivable.Vendor
{
    public class GetReceivableVendorDetailResults : BaseResult
    {
        public List<ReceivableVendorReportEntityModel> ReceivableVendorDetail { get; set; }
        public List<ReceivableVendorReportEntityModel> ReceiptsList { get; set; }
        public string VendorName { get; set; }
        public decimal? TotalReceivableBefore { get; set; }
        public decimal? TotalReceivableInPeriod { get; set; }
        public decimal? TotalReceivable { get; set; }
        public decimal? TotalValueReceipt { get; set; }
        public decimal? TotalValueOrder { get; set; }
    }
}

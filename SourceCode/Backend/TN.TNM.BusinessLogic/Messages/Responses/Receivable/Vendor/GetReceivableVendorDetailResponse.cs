using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Receivable;

namespace TN.TNM.BusinessLogic.Messages.Responses.Receivable.Vendor
{
    public class GetReceivableVendorDetailResponse : BaseResponse
    {
        public List<ReceivableVendorReportModel> ReceivableVendorDetail { get; set; }
        public List<ReceivableVendorReportModel> ReceiptsList { get; set; }
        public string VendorName { get; set; }
        public decimal? TotalReceivableBefore { get; set; }
        public decimal? TotalReceivableInPeriod { get; set; }
        public decimal? TotalReceivable { get; set; }
        public decimal? TotalValueReceipt { get; set; }
        public decimal? TotalValueOrder { get; set; }
    }
}

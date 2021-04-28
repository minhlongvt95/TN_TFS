using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Receivable;

namespace TN.TNM.BusinessLogic.Messages.Responses.Receivable.Vendor
{
    public class GetReceivableVendorReportResponse : BaseResponse
    {
        public List<ReceivableVendorReportModel> ReceivableVendorReport { get; set; }
        public decimal? TotalPurchase { get; set; }
        public decimal? TotalPaid { get; set; }
    }
}

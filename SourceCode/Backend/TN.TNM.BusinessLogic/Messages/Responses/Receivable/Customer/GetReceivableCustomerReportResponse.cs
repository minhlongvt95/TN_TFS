using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Receivable;

namespace TN.TNM.BusinessLogic.Messages.Responses.Receivable.Customer
{
    public class GetReceivableCustomerReportResponse : BaseResponse
    {
        public List<ReceivableCustomerModel> ReceivableCustomerReport { get; set; }
        public decimal? TotalPurchase { get; set; }
        public decimal? TotalPaid { get; set; }
        public decimal? TotalReceipt { get; set; }
    }
}

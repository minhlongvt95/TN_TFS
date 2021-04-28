using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Receivable;

namespace TN.TNM.DataAccess.Messages.Results.Receivable.Customer
{
    public class GetReceivableCustomerReportResults : BaseResult
    {
        public List<ReceivableCustomerEntityModel> ReceivableCustomerReport { get; set; }
        public decimal? TotalPurchase { get; set; }
        public decimal? TotalPaid { get; set; }
        public decimal? TotalReceipt { get; set; }
    }
}

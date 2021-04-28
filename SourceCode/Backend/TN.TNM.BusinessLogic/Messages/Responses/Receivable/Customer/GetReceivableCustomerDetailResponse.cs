using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Receivable;

namespace TN.TNM.BusinessLogic.Messages.Responses.Receivable.Customer
{
    public class GetReceivableCustomerDetailResponse : BaseResponse
    {
        public List<ReceivableCustomerModel> ReceivableCustomerDetail { get; set; }
        public List<ReceivableCustomerModel> ReceiptsList { get; set; }
        public string CustomerName { get; set; }
        public Guid CustomerContactId { get; set; }
        public decimal? TotalReceivableBefore { get; set; }
        public decimal? TotalReceivableInPeriod { get; set; }
        public decimal? TotalReceivable { get; set; }
        public decimal? TotalPurchaseProduct { get; set; }
        public decimal? TotalReceipt { get; set; }
    }
}

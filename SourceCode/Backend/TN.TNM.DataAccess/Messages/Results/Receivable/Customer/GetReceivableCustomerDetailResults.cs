using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Receivable;

namespace TN.TNM.DataAccess.Messages.Results.Receivable.Customer
{
    public class GetReceivableCustomerDetailResults : BaseResult
    {
        public List<ReceivableCustomerEntityModel> ReceivableCustomerDetail { get; set; }
        public List<ReceivableCustomerEntityModel> ReceiptsList { get; set; }
        public string CustomerName { get; set; }
        public Guid CustomerContactId { get; set; }
        public decimal? TotalReceivableBefore { get; set; }
        public decimal? TotalReceivableInPeriod { get; set; }
        public decimal? TotalReceivable { get; set; }
        public decimal? TotalPurchaseProduct { get; set; }
        public decimal? TotalReceipt { get; set; }
    }
}

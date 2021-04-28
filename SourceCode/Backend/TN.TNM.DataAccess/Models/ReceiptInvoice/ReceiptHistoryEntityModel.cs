using System;

namespace TN.TNM.DataAccess.Models.ReceiptInvoice
{
    public class ReceiptHistoryEntityModel
    {
        public Guid OrderId { get; set; }
        public decimal AmountCollected { get; set; }
        public decimal Amount { get; set; }
    }
}

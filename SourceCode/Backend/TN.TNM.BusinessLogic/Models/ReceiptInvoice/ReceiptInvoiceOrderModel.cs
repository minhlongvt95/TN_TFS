using System;

namespace TN.TNM.BusinessLogic.Models.ReceiptInvoice
{
    public class ReceiptInvoiceOrderModel
    {
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; }
        public decimal AmountCollected { get; set; }    //Số tiền đã thu
        public decimal AmountReceivable { get; set; }   //Số tiền còn phải thu
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
    }
}

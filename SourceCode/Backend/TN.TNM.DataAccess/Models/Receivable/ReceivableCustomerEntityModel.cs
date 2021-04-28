using System;

namespace TN.TNM.DataAccess.Models.Receivable
{
    public class ReceivableCustomerEntityModel
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime? NearestTransaction { get; set; }
        public decimal? TotalSales { get; set; }    //Tổng đặt hàng
        public decimal? TotalPaid { get; set; }     //Tổng thanh toán
        public decimal? TotalUnpaid { get; set; }
        public decimal? TotalReceipt { get; set; }  //Tổng còn phải thu
        public decimal? ReceiptInvoiceValue { get; set; }
        public decimal? OrderValue { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ReceiptId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ProductId { get; set; }
        public Guid CustomerOrderId { get; set; }
        public DateTime? CreateDateOrder { get; set; }
        public DateTime? CreateDateReceiptInvoice { get; set; }
        public string OrderCode { get; set; }
        public string OrderName { get; set; }
        public string CreatedByName { get; set; }
        public string DescriptionReceipt { get; set; }
        public string ReceiptCode { get; set; }
        public Guid? Status { get; set; }
        public string Router { get; set; }
    }
}

using System;

namespace TN.TNM.DataAccess.Models.Receivable
{
    public class ReceivableVendorReportEntityModel
    {
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public DateTime? NearestTransaction { get; set; }
        public decimal? TotalSales { get; set; }
        public decimal? TotalPaid { get; set; }
        public decimal? TotalUnpaid { get; set; }
        public decimal? ReceiptInvoiceValue { get; set; }
        public decimal? OrderValue { get; set; }
        public Guid VendorId { get; set; }
        public Guid ReceiptId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ProductId { get; set; }
        public Guid VendorOrderId { get; set; }
        public DateTime? CreateDateOrder { get; set; }
        public DateTime? CreateDateReceiptInvoice { get; set; }
        public string OrderCode { get; set; }
        public string OrderName { get; set; }
        public string CreatedByName { get; set; }
        public string DescriptionReceipt { get; set; }
        public string ReceiptCode { get; set; }
        public decimal? TotalPurchase { get; set; }
        public Guid? Status { get; set; }
        public string Router { get; set; }
        public Guid? PayableInvoiceId { get; set; }
        public Guid? BankPayableInvoiceId { get; set; }
    }
}

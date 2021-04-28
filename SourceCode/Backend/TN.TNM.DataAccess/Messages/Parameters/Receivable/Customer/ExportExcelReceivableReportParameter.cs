using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Receivable;

namespace TN.TNM.DataAccess.Messages.Parameters.Receivable.Customer
{
    public class ExportExcelReceivableReportParameter : BaseParameter
    {
        public List<ReceivableCustomerEntityModel> ReceivableCustomerDetail { get; set; }
        public List<ReceivableCustomerEntityModel> ReceiptsList { get; set; }
        public Guid CustomerId { get; set; }
        public decimal? TotalReceivableBefore { get; set; }
        public decimal? TotalReceivableInPeriod { get; set; }
        public decimal? TotalReceivable { get; set; }
        public decimal? TotalPurchaseProduct { get; set; }
        public decimal? TotalReceipt { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class SearchReceiptInvoiceRequest : BaseRequest<SearchReceiptInvoiceParameter>
    {
        public string ReceiptInvoiceCode { get; set; }
        public List<Guid?> ReceiptInvoiceReason { get; set; }
        public List<Guid> CreateById { get; set; }
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public List<Guid?> Status { get; set; }
        public List<Guid?> ObjectReceipt { get; set; }

        public override SearchReceiptInvoiceParameter ToParameter()
        {
            return new SearchReceiptInvoiceParameter
            {
                ReceiptInvoiceCode = this.ReceiptInvoiceCode,
                ReceiptInvoiceReason = this.ReceiptInvoiceReason,
                CreateById = this.CreateById,
                CreateDateFrom = this.CreateDateFrom,
                CreateDateTo = this.CreateDateTo,
                Status = this.Status,
                ObjectReceipt = this.ObjectReceipt,
                UserId = this.UserId
            };
        }
    }
}

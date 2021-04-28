using System.Collections.Generic;
using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.DataAccess.Messages.Results.ReceiptInvoice
{
    public class GetOrderByCustomerIdResult : BaseResult
    {
        public List<ReceiptInvoiceOrderModel> listOrder { get; set; }
        public decimal totalAmountReceivable { get; set; }
    }
}

using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice
{
    public class GetOrderByCustomerIdResponse : BaseResponse
    {
        public List<ReceiptInvoiceOrderModel> listOrder { get; set; }
        public decimal totalAmountReceivable { get; set; }
    }
}

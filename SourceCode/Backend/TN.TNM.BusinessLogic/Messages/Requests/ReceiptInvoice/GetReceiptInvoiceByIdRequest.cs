using System;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class GetReceiptInvoiceByIdRequest : BaseRequest<GetReceiptInvoiceByIdParameter>
    {
        public Guid ReceiptInvoiceId { get; set; }
        public override GetReceiptInvoiceByIdParameter ToParameter() => new GetReceiptInvoiceByIdParameter
        {
            ReceiptInvoiceId = ReceiptInvoiceId
        };
    }
}

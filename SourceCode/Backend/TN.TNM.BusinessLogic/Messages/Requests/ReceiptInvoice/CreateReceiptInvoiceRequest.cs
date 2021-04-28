using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.ReceiptInvoice;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;
using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class CreateReceiptInvoiceRequest : BaseRequest<CreateReceiptInvoiceParameter>
    {
        public ReceiptInvoiceModel ReceiptInvoice { get; set; }
        public ReceiptInvoiceMappingModel ReceiptInvoiceMapping { get; set; }
        public List<ReceiptHistoryEntityModel> ReceiptOrderHistory { get; set; }

        public override CreateReceiptInvoiceParameter ToParameter() => new CreateReceiptInvoiceParameter()
        {
            ReceiptInvoice = ReceiptInvoice.ToEntity(),
            ReceiptInvoiceMapping = ReceiptInvoiceMapping.ToEntity(),
            ReceiptOrderHistory = ReceiptOrderHistory,
            UserId = UserId
        };
    }
}

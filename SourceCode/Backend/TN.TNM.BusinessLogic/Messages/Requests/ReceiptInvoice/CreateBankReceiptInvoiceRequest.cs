using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.ReceiptInvoice;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;
using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class CreateBankReceiptInvoiceRequest: BaseRequest<CreateBankReceiptInvoiceParameter>
    {
        public BankReceiptInvoiceModel BankReceiptInvoice { get; set; }
        public BankReceiptInvoiceMappingModel BankReceiptInvoiceMapping { get; set; }
        public List<ReceiptHistoryEntityModel> ReceiptOrderHistory { get; set; }

        public override CreateBankReceiptInvoiceParameter ToParameter()
        {
            return new CreateBankReceiptInvoiceParameter()
            {
                UserId = UserId,
                BankReceiptInvoice = BankReceiptInvoice.ToEntity(),
                BankReceiptInvoiceMapping = BankReceiptInvoiceMapping.ToEntity(),
                ReceiptOrderHistory = ReceiptOrderHistory
            };
        }
    }
}

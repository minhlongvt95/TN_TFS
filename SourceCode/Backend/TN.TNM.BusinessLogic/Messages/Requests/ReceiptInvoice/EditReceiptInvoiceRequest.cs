using System;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class EditReceiptInvoiceRequest : BaseRequest<EditReceiptInvoiceParameter>
    {
        public override EditReceiptInvoiceParameter ToParameter()
        {
            throw new NotImplementedException();
        }
    }
}

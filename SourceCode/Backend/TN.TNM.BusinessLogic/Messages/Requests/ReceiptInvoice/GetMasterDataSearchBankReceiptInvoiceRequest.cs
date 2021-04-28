using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class GetMasterDataSearchBankReceiptInvoiceRequest : BaseRequest<GetMasterDateSearchBankReceiptInvoiceParameter>
    {
        public override GetMasterDateSearchBankReceiptInvoiceParameter ToParameter() => new GetMasterDateSearchBankReceiptInvoiceParameter
        {
            UserId = UserId
        };
    }
}

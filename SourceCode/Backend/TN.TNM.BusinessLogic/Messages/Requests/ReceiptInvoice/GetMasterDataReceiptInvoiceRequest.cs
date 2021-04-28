using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class GetMasterDataReceiptInvoiceRequest : BaseRequest<GetMasterDataReceiptInvoiceParameter>
    {
        public override GetMasterDataReceiptInvoiceParameter ToParameter()
        {
            return new GetMasterDataReceiptInvoiceParameter
            {
                UserId = UserId
            };
        }
    }
}

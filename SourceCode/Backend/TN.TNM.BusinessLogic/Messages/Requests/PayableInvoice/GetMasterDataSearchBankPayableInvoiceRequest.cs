using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class GetMasterDataSearchBankPayableInvoiceRequest : BaseRequest<GetMasterDataSearchBankPayableInvoiceParameter>
    {
        public override GetMasterDataSearchBankPayableInvoiceParameter ToParameter() => new GetMasterDataSearchBankPayableInvoiceParameter
        {
            UserId = UserId
        };
    }
}

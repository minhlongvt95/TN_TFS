using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class GetMasterDataBankPayableInvoiceRequest : BaseRequest<GetMasterDataBankPayableInvoiceParameter>
    {
        public override GetMasterDataBankPayableInvoiceParameter ToParameter() => new GetMasterDataBankPayableInvoiceParameter
        {
            UserId = UserId
        };
    }
}

using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class GetMasterDataPayableInvoiceRequest : BaseRequest<GetMasterDataPayableInvoiceParameter>
    {
        public override GetMasterDataPayableInvoiceParameter ToParameter() => new GetMasterDataPayableInvoiceParameter
        {
            UserId = UserId
        };
    }
}

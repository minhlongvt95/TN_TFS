using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class GetMasterDataPayableInvoiceSearchRequest : BaseRequest<GetMasterDataPayableInvoiceSearchParameter>
    {
        public override GetMasterDataPayableInvoiceSearchParameter ToParameter() => new GetMasterDataPayableInvoiceSearchParameter
        {
            UserId = UserId
        };
    }
}

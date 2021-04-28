using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class GetMasterDataSaleBiddingAddEditProductDialogRequest : BaseRequest<GetMasterDataSaleBiddingAddEditProductDialogParameter>
    {
        public override GetMasterDataSaleBiddingAddEditProductDialogParameter ToParameter()
        {
            return new GetMasterDataSaleBiddingAddEditProductDialogParameter()
            {
                UserId = UserId
            };
        }
    }
}

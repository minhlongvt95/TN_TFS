using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetMasterDataCreateCostRequest : BaseRequest<GetMasterDataCreateCostParameter>
    {
        public override GetMasterDataCreateCostParameter ToParameter()
        {
            return new GetMasterDataCreateCostParameter
            {
                UserId = UserId
            };
        }
    }
}

using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetWareHouseChaRequest : BaseRequest<GetWareHouseChaParameter>
    {
        public override GetWareHouseChaParameter ToParameter()
        {
            return new GetWareHouseChaParameter()
            {
                UserId = UserId,
            };
        }
    }
}

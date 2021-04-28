using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class SearchWareHouseRequest : BaseRequest<SearchWareHouseParameter>
    {
        public override SearchWareHouseParameter ToParameter()
        {
            return new SearchWareHouseParameter()
            {
                UserId = UserId,
            };
        }
    }
}

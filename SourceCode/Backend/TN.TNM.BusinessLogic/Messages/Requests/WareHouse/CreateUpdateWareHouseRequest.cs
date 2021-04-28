using TN.TNM.BusinessLogic.Models.WareHouse;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class CreateUpdateWareHouseRequest : BaseRequest<CreateUpdateWareHouseParameter>
    {
        public WareHouseModel WareHouse { get; set; }
        public override CreateUpdateWareHouseParameter ToParameter()
        {
            return new CreateUpdateWareHouseParameter()
            {
                Warehouse = WareHouse.ToEntity(),
                UserId = UserId,
            };
        }
    }
}

using System;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class RemoveWareHouseRequest : BaseRequest<RemoveWareHouseParameter>
    {
        public Guid WareHouseId { get; set; }
        public override RemoveWareHouseParameter ToParameter()
        {
            return new RemoveWareHouseParameter()
            {
                WareHouseId = WareHouseId,
                UserId = UserId,
            };
        }
    }
}

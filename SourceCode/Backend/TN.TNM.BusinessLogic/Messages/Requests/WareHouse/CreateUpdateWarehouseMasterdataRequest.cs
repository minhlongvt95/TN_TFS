using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class CreateUpdateWarehouseMasterdataRequest: BaseRequest<CreateUpdateWarehouseMasterdataParameter>
    {
        public Guid? WarehouseId { get; set; }
        public override CreateUpdateWarehouseMasterdataParameter ToParameter()
        {
            return new CreateUpdateWarehouseMasterdataParameter
            {
                WarehouseId = WarehouseId,
                UserId = UserId
            };
        }
    }
}

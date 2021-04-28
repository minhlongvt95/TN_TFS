using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetSerialRequest : BaseRequest<GetSerialParameter>
    {
        public Guid? WarehouseId { get; set; }
        public Guid? ProductId { get; set; }

        public override GetSerialParameter ToParameter()
        {
            return new GetSerialParameter
            {
                ProductId = this.ProductId,
                WarehouseId = this.WarehouseId
            };
        }
    }
}

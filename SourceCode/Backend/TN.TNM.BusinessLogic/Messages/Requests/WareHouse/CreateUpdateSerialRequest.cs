using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class CreateUpdateSerialRequest : BaseRequest<CreateUpdateSerialParameter>
    {
        public List<SerialEntityModel> ListSerial { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid ProductId { get; set; }

        public override CreateUpdateSerialParameter ToParameter()
        {
            return new CreateUpdateSerialParameter()
            {
                UserId = UserId,
                ListSerial = ListSerial,
                WarehouseId = WarehouseId,
                ProductId = ProductId
            };
        }
    }
}

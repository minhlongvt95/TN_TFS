using System;

using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class ChangeStatusInventoryDeliveryVoucherRequest : BaseRequest<ChangeStatusInventoryDeliveryVoucherParameter>
    {
        public Guid InventoryDeliveryVoucherId { get; set; }

        public override ChangeStatusInventoryDeliveryVoucherParameter ToParameter()
        {
            return new ChangeStatusInventoryDeliveryVoucherParameter
            {
                InventoryDeliveryVoucherId = this.InventoryDeliveryVoucherId,
                UserId=this.UserId
            };
        }
    }
}

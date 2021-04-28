using System;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class DeleteInventoryDeliveryVoucherRequest : BaseRequest<DeleteInventoryDeliveryVoucherParameter>
    {
        public Guid InventoryDeliveryVoucherId { get; set; }

        public override DeleteInventoryDeliveryVoucherParameter ToParameter()
        {
            return new DeleteInventoryDeliveryVoucherParameter { 
                InventoryDeliveryVoucherId = this.InventoryDeliveryVoucherId 
            };
        }
    }
}

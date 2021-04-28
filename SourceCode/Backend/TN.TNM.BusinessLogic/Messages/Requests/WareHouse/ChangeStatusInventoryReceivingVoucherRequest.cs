using System;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class ChangeStatusInventoryReceivingVoucherRequest : BaseRequest<ChangeStatusInventoryReceivingVoucherParameter>
    {
        public Guid InventoryReceivingVoucherId { get; set; }
        
        public override ChangeStatusInventoryReceivingVoucherParameter ToParameter()
        {
            return new ChangeStatusInventoryReceivingVoucherParameter
            {
                InventoryReceivingVoucherId = this.InventoryReceivingVoucherId,
                UserId=this.UserId
            };
        }
    }
}

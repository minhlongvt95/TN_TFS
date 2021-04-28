using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class CreateOrUpdateInventoryVoucherResponse : BaseResponse
    {
        public Guid InventoryReceivingVoucherId { get; set; }
    }
}

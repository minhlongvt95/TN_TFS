using System;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class CreateOrUpdateInventoryVoucherResult : BaseResult
    {
        public Guid InventoryReceivingVoucherId { get; set; }
    }
}

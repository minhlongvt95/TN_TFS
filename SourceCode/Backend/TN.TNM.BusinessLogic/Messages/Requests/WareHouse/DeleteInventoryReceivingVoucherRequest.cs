using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class DeleteInventoryReceivingVoucherRequest : BaseRequest<DeleteInventoryReceivingVoucherParameter>
    {
        public Guid InventoryReceivingVoucherId { get; set; }

        public override DeleteInventoryReceivingVoucherParameter ToParameter()
        {
            return new DeleteInventoryReceivingVoucherParameter
            {
                InventoryReceivingVoucherId = this.InventoryReceivingVoucherId
            };
        }
    }
}

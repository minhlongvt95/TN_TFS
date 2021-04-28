using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class DeleteInventoryReceivingVoucherParameter:BaseParameter
    {
        public Guid InventoryReceivingVoucherId { get; set; }
    }
}

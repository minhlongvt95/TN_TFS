using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class DeleteInventoryDeliveryVoucherParameter:BaseParameter
    {
        public Guid InventoryDeliveryVoucherId { get; set; }
    }
}

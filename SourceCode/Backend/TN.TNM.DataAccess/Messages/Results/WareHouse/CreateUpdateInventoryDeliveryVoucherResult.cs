using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class CreateUpdateInventoryDeliveryVoucherResult:BaseResult
    {
        public Guid InventoryDeliveryVoucherId { get; set; }
    }
}

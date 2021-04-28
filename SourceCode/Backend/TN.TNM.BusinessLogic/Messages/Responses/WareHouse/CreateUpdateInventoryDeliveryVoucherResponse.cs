using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class CreateUpdateInventoryDeliveryVoucherResponse:BaseResponse
    {
        public Guid InventoryDeliveryVoucherId { get; set; }
    }
}

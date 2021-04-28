using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class GetInventoryDeliveryVoucherByIdResponse:BaseResponse
    {
        public InventoryDeliveryVoucherModel inventoryDeliveryVoucher { get; set; }
        public List<InventoryDeliveryVoucherMappingModel> inventoryDeliveryVoucherMappingModel { get; set; }

    }
}

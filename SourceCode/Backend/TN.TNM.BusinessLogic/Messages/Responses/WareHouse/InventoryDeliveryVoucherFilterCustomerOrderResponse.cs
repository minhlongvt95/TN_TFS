
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class InventoryDeliveryVoucherFilterCustomerOrderResponse:BaseResponse
    {
        public List<CustomerOrderModel> listCustomerOrder { get; set; }

    }
}

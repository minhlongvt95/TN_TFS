using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class InventoryDeliveryVoucherFilterVendorOrderRequest : BaseRequest<InventoryDeliveryVoucherFilterVendorOrderParameter>
    {
        public override InventoryDeliveryVoucherFilterVendorOrderParameter ToParameter()
        {
            return new InventoryDeliveryVoucherFilterVendorOrderParameter { };
        }
    }
}

using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class InventoryDeliveryVoucherFilterVendorOrderResult:BaseResult
    {
        public List<VendorOrder> listVendorOrder { get; set; }

    }
}

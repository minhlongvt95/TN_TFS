
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetTop10WarehouseFromReceivingVoucherResult:BaseResult
    {
       public List<InventoryReceivingVoucherMapping> lstInventoryReceivingVoucherMapping { get; set; }
    }
}

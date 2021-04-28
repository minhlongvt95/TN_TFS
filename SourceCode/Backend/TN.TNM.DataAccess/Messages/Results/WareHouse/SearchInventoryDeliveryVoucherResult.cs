using System.Collections.Generic;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class SearchInventoryDeliveryVoucherResult:BaseResult
    {
        public List<InventoryDeliveryVoucherEntityModel> lstResult { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetListInventoryReceivingVoucherResult:BaseResult
    {
        public List<InventoryReceivingVoucherModel> lstResult { get; set; }
    }
}

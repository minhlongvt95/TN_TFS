using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class CreatePhieuNhapKhoResult : BaseResult
    {
        public Guid InventoryReceivingVoucherId { get; set; }
    }
}

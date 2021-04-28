using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class HuyPhieuNhapKhoParameter : BaseParameter
    {
        public Guid InventoryReceivingVoucherId { get; set; }
    }
}

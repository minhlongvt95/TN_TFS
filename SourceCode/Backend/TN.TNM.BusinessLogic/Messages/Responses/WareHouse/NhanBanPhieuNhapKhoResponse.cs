using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class NhanBanPhieuNhapKhoResponse : BaseResponse
    {
        public Guid InventoryReceivingVoucherId { get; set; }
    }
}

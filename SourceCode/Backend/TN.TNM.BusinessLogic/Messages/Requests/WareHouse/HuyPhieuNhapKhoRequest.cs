using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class HuyPhieuNhapKhoRequest : BaseRequest<HuyPhieuNhapKhoParameter>
    {
        public Guid InventoryReceivingVoucherId { get; set; }

        public override HuyPhieuNhapKhoParameter ToParameter()
        {
            return new HuyPhieuNhapKhoParameter()
            {
                UserId = UserId,
                InventoryReceivingVoucherId = InventoryReceivingVoucherId
            };
        }
    }
}

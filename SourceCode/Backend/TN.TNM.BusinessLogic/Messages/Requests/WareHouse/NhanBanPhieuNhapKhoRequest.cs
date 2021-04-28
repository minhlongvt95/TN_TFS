using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class NhanBanPhieuNhapKhoRequest : BaseRequest<NhanBanPhieuNhapKhoParameter>
    {
        public Guid InventoryReceivingVoucherId { get; set; }

        public override NhanBanPhieuNhapKhoParameter ToParameter()
        {
            return new NhanBanPhieuNhapKhoParameter()
            {
                UserId = UserId,
                InventoryReceivingVoucherId = InventoryReceivingVoucherId
            };
        }
    }
}

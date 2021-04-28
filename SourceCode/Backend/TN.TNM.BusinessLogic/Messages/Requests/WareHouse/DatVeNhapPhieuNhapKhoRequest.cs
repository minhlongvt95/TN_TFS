using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class DatVeNhapPhieuNhapKhoRequest : BaseRequest<DatVeNhapPhieuNhapKhoParameter>
    {
        public Guid InventoryReceivingVoucherId { get; set; }

        public override DatVeNhapPhieuNhapKhoParameter ToParameter()
        {
            return new DatVeNhapPhieuNhapKhoParameter()
            {
                UserId = UserId,
                InventoryReceivingVoucherId = InventoryReceivingVoucherId
            };
        }
    }
}

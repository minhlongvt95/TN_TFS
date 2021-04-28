using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class XoaPhieuNhapKhoRequest : BaseRequest<XoaPhieuNhapKhoParameter>
    {
        public Guid InventoryReceivingVoucherId { get; set; }

        public override XoaPhieuNhapKhoParameter ToParameter()
        {
            return new XoaPhieuNhapKhoParameter()
            {
                UserId = UserId,
                InventoryReceivingVoucherId = InventoryReceivingVoucherId
            };
        }
    }
}

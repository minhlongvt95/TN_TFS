using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class DanhDauCanLamPhieuNhapKhoRequest : BaseRequest<DanhDauCanLamPhieuNhapKhoParameter>
    {
        public Guid InventoryReceivingVoucherId { get; set; }
        public bool Check { get; set; }

        public override DanhDauCanLamPhieuNhapKhoParameter ToParameter()
        {
            return new DanhDauCanLamPhieuNhapKhoParameter()
            {
                UserId = UserId,
                InventoryReceivingVoucherId = InventoryReceivingVoucherId,
                Check = Check
            };
        }
    }
}

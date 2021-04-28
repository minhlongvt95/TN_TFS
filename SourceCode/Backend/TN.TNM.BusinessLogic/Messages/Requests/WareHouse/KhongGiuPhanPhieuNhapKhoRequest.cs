using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class KhongGiuPhanPhieuNhapKhoRequest : BaseRequest<KhongGiuPhanPhieuNhapKhoParameter>
    {
        public List<SanPhamPhieuNhapKhoModel> ListSanPhamPhieuNhapKho { get; set; }
        public Guid InventoryReceivingVoucherId { get; set; }

        public override KhongGiuPhanPhieuNhapKhoParameter ToParameter()
        {
            return new KhongGiuPhanPhieuNhapKhoParameter()
            {
                UserId = UserId,
                InventoryReceivingVoucherId = InventoryReceivingVoucherId,
                ListSanPhamPhieuNhapKho = ListSanPhamPhieuNhapKho
            };
        }
    }
}

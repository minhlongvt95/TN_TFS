using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class KiemTraKhaDungPhieuNhapKhoRequest : BaseRequest<KiemTraKhaDungPhieuNhapKhoParameter>
    {
        public List<SanPhamPhieuNhapKhoModel> ListSanPhamPhieuNhapKho { get; set; }
        public Guid InventoryReceivingVoucherId { get; set; }

        public override KiemTraKhaDungPhieuNhapKhoParameter ToParameter()
        {
            return new KiemTraKhaDungPhieuNhapKhoParameter()
            {
                UserId = UserId,
                ListSanPhamPhieuNhapKho = ListSanPhamPhieuNhapKho,
                InventoryReceivingVoucherId = InventoryReceivingVoucherId
            };
        }
    }
}

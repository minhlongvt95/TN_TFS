using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class KiemTraKhaDungPhieuNhapKhoParameter : BaseParameter
    {
        public List<SanPhamPhieuNhapKhoModel> ListSanPhamPhieuNhapKho { get; set; }
        public Guid InventoryReceivingVoucherId { get; set; }
    }
}

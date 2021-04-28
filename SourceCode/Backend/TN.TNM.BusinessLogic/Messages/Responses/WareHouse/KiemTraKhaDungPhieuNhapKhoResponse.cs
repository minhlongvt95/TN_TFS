using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class KiemTraKhaDungPhieuNhapKhoResponse : BaseResponse
    {
        public List<SanPhamPhieuNhapKhoModel> ListSanPhamPhieuNhapKho { get; set; }
    }
}

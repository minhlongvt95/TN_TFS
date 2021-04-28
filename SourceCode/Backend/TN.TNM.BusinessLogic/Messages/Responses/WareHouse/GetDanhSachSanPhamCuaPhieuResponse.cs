using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class GetDanhSachSanPhamCuaPhieuResponse : BaseResponse
    {
        public List<SanPhamPhieuNhapKhoModel> ListItemDetail { get; set; }
    }
}

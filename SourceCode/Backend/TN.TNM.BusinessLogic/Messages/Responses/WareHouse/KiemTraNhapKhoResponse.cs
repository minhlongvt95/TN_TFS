using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class KiemTraNhapKhoResponse : BaseResponse
    {
        public int Mode { get; set; }
        public List<string> ListMaSanPhamKhongHopLe { get; set; }
    }
}

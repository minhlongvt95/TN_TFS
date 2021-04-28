using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class KiemTraNhapKhoResult : BaseResult
    {
        public int Mode { get; set; }
        public List<string> ListMaSanPhamKhongHopLe { get; set; }
    }
}

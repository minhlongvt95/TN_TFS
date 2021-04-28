using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class SearchListPhieuNhapKhoParameter : BaseParameter
    {
        public string MaPhieu { get; set; }
        public DateTime? FromNgayLapPhieu { get; set; }
        public DateTime? ToNgayLapPhieu { get; set; }
        public DateTime? FromNgayNhapKho { get; set; }
        public DateTime? ToNgayNhapKho { get; set; }
        public List<Guid> ListStatusId { get; set; }
        public List<Guid> ListWarehouseId { get; set; }
        public List<Guid> ListEmployeeId { get; set; }
        public List<Guid> ListProductId { get; set; }
        public string SerialCode { get; set; }
    }
}

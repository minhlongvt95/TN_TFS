using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class SearchListPhieuNhapKhoRequest : BaseRequest<SearchListPhieuNhapKhoParameter>
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

        public override SearchListPhieuNhapKhoParameter ToParameter()
        {
            return new SearchListPhieuNhapKhoParameter()
            {
                UserId = UserId,
                MaPhieu = MaPhieu,
                FromNgayLapPhieu = FromNgayLapPhieu,
                ToNgayLapPhieu = ToNgayLapPhieu,
                FromNgayNhapKho = FromNgayNhapKho,
                ToNgayNhapKho = ToNgayNhapKho,
                ListStatusId = ListStatusId,
                ListWarehouseId = ListWarehouseId,
                ListEmployeeId = ListEmployeeId,
                ListProductId = ListProductId,
                SerialCode = SerialCode
            };
        }
    }
}

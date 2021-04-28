using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class SoKho
    {
        public Guid SoKhoId { get; set; }
        public int LoaiPhieu { get; set; }
        public Guid PhieuId { get; set; }
        public Guid ChiTietPhieuId { get; set; }
        public int ChiTietLoaiPhieu { get; set; }
        public DateTime NgayChungTu { get; set; }
        public string SoChungTu { get; set; }
        public Guid ProductId { get; set; }
        public decimal SoLuong { get; set; }
        public decimal Gia { get; set; }
        public decimal ThanhTien { get; set; }
        public Guid? DoiTac { get; set; }
        public Guid WarehouseId { get; set; }
        public bool CheckGia { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}

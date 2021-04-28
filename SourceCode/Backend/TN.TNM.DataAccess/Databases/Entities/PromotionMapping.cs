using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PromotionMapping
    {
        public Guid PromotionMappingId { get; set; }
        public Guid PromotionId { get; set; }
        public int IndexOrder { get; set; }
        public Guid? HangKhuyenMai { get; set; }
        public decimal SoLuongTang { get; set; }
        public decimal SoTienTu { get; set; }
        public Guid? SanPhamMua { get; set; }
        public decimal SoLuongMua { get; set; }
        public bool ChiChonMot { get; set; }
        public bool LoaiGiaTri { get; set; }
        public decimal GiaTri { get; set; }
        public Guid? TenantId { get; set; }
    }
}

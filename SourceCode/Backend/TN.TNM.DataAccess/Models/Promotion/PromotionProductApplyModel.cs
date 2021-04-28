using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Promotion
{
    public class PromotionProductApplyModel
    {
        public Guid PromotionMappingId { get; set; }
        public int IndexOrder { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductUnitName { get; set; }
        public string PromotionProductName { get; set; }
        public string PromotionProductNameConvert { get; set; }
        public decimal SoLuongTang { get; set; }
        public bool LoaiGiaTri { get; set; }
        public decimal GiaTri { get; set; }
        public decimal SoTienTu { get; set; }
    }
}

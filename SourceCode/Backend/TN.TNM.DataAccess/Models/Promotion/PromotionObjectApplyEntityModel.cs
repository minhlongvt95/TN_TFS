using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Promotion
{
    public class PromotionObjectApplyEntityModel
    {
        public Guid? PromotionObjectApplyId { get; set; }
        public Guid? ObjectId { get; set; }
        public string ObjectType { get; set; }
        public Guid PromotionId { get; set; }
        public int ConditionsType { get; set; }
        public int PropertyType { get; set; }
        public bool? NotMultiplition { get; set; }
        public Guid PromotionMappingId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal SoLuongTang { get; set; }
        public bool LoaiGiaTri { get; set; }
        public decimal GiaTri { get; set; }
        public decimal Amount { get; set; }
        public decimal SoTienTu { get; set; }

        public string PromotionName { get; set; }
        public string ProductUnitName { get; set; }
        public string PromotionProductName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Promotion
{
    public class PromotionApplyModel
    {
        public Guid PromotionId { get; set; }
        public string PromotionName { get; set; }
        public int ConditionsType { get; set; }
        public int PropertyType { get; set; }
        public string PropertyTypeName { get; set; }
        public bool? NotMultiplition { get; set; }
        public List<PromotionProductApplyModel> ListPromotionProductApply { get; set; }
        public List<PromotionProductApplyModel> SelectedPromotionProductApply { get; set; }

        //Trường hợp khuyến mãi theo sản phẩm (mua hàng giảm giá hàng, mua hàng tặng hàng)
        public Guid? PromotionMappingId { get; set; }
        public decimal? SoLuongTang { get; set; } //Tổng số lượng giảm giá (tặng) tối đa
        public bool ChiChonMot { get; set; }
        public List<PromotionProductMappingApplyModel> ListPromotionProductMappingApply { get; set; }
        public List<PromotionProductMappingApplyModel> SelectedPromotionProductMappingApply { get; set; }
        public string SelectedDetail { get; set; }
    }
}

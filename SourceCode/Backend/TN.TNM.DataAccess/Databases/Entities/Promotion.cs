using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Promotion
    {
        public Guid PromotionId { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Description { get; set; }
        public int ConditionsType { get; set; }
        public int PropertyType { get; set; }
        public string FilterContent { get; set; }
        public string FilterQuery { get; set; }
        public bool? NotMultiplition { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public bool CustomerHasOrder { get; set; }
    }
}

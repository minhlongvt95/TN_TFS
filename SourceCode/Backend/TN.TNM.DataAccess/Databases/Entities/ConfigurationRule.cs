using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ConfigurationRule
    {
        public Guid ConfigurationRuleId { get; set; }
        public Guid? PositionId { get; set; }
        public string Description { get; set; }
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public decimal? Money { get; set; }
        public Guid? Type { get; set; }
        public Guid? TenantId { get; set; }
    }
}

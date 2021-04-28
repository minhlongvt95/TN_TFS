using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class NotifiSettingCondition
    {
        public Guid NotifiSettingConditionId { get; set; }
        public Guid? NotifiSettingId { get; set; }
        public Guid InforScreenId { get; set; }
        public int TypeValue { get; set; }
        public Guid NotifiConditionId { get; set; }
        public decimal? NumberValue { get; set; }
        public string StringValue { get; set; }
        public DateTime? DateValue { get; set; }
        public Guid? TenantId { get; set; }
    }
}

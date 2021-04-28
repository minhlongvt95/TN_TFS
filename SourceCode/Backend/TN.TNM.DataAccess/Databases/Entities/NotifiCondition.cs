using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class NotifiCondition
    {
        public Guid NotifiConditionId { get; set; }
        public string NotifiConditionName { get; set; }
        public int TypeValue { get; set; }
        public int TypeCondition { get; set; }
        public Guid? ScreenId { get; set; }
        public Guid? TenantId { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class SystemFeature
    {
        public Guid SystemFeatureId { get; set; }
        public string SystemFeatureName { get; set; }
        public string SystemFeatureCode { get; set; }
        public Guid? TenantId { get; set; }
    }
}

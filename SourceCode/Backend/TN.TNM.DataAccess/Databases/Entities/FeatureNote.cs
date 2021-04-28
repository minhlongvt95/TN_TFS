using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class FeatureNote
    {
        public Guid Id { get; set; }
        public Guid FeatureId { get; set; }
        public string Note { get; set; }
        public Guid? TenantId { get; set; }
    }
}

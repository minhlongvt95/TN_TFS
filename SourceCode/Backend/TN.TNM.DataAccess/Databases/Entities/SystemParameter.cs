using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class SystemParameter
    {
        public Guid SystemParameterId { get; set; }
        public string SystemKey { get; set; }
        public bool? SystemValue { get; set; }
        public string SystemValueString { get; set; }
        public string SystemDescription { get; set; }
        public Guid? TenantId { get; set; }
        public string SystemGroupCode { get; set; }
        public string SystemGroupDesc { get; set; }
        public string Description { get; set; }
    }
}

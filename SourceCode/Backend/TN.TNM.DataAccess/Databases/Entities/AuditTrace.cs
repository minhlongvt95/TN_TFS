using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class AuditTrace
    {
        public Guid TraceId { get; set; }
        public string ActionName { get; set; }
        public string ObjectName { get; set; }
        public string Description { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}

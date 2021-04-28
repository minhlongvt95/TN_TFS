using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LoginAuditTrace
    {
        public Guid LoginAuditTraceId { get; set; }
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public int? Status { get; set; }
    }
}

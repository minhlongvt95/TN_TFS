using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class QuoteParticipantMapping
    {
        public Guid QuoteParticipantMappingId { get; set; }
        public Guid? QuoteId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? TenantId { get; set; }
    }
}

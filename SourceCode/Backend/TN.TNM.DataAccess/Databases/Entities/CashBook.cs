using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CashBook
    {
        public Guid CashBookId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? PaidDate { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? TenantId { get; set; }
    }
}

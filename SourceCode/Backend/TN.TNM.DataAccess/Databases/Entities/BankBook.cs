using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class BankBook
    {
        public Guid BankBookId { get; set; }
        public Guid? BankAccountId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}

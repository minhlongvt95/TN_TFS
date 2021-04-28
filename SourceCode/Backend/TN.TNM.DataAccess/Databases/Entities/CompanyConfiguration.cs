using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CompanyConfiguration
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string TaxCode { get; set; }
        public Guid? BankAccountId { get; set; }
        public string CompanyAddress { get; set; }
        public string ContactName { get; set; }
        public string ContactRole { get; set; }
        public Guid? TenantId { get; set; }

        public BankAccount BankAccount { get; set; }
    }
}

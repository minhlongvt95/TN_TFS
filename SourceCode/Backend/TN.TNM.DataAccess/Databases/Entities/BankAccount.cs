using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class BankAccount
    {
        public BankAccount()
        {
            BankPayableInvoice = new HashSet<BankPayableInvoice>();
            BankReceiptInvoice = new HashSet<BankReceiptInvoice>();
            CompanyConfiguration = new HashSet<CompanyConfiguration>();
        }

        public Guid BankAccountId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankDetail { get; set; }
        public string BranchName { get; set; }
        public string AccountName { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }

        public ICollection<BankPayableInvoice> BankPayableInvoice { get; set; }
        public ICollection<BankReceiptInvoice> BankReceiptInvoice { get; set; }
        public ICollection<CompanyConfiguration> CompanyConfiguration { get; set; }
    }
}

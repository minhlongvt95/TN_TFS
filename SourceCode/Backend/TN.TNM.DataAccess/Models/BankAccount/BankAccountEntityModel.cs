using System;

namespace TN.TNM.DataAccess.Models.BankAccount
{
    public class BankAccountEntityModel
    {
        public Guid? BankAccountId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankDetail { get; set; }
        public string BranchName { get; set; }
        public string AccountName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }

        public string LabelShow { get; set; }
    }
}

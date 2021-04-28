using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models
{
    public class CompanyConfigEntityModel
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
        public CompanyConfigEntityModel() { }
        public CompanyConfigEntityModel(CompanyConfiguration entity)
        {
            CompanyId = entity.CompanyId;
            BankAccountId = entity.BankAccountId;
            CompanyAddress = entity.CompanyAddress;
            CompanyName = entity.CompanyName;
            ContactName = entity.ContactName;
            ContactRole = entity.ContactRole;
            TaxCode = entity.TaxCode;
            Email = entity.Email;
            Phone = entity.Phone;
        }
    }
}

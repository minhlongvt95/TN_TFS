using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.DataAccess.Messages.Results.CompanyConfig
{
    public class GetCompanyConfigResults : BaseResult
    {
        public CompanyConfigEntityModel CompanyConfig { get; set; }
        public List<BankAccountEntityModel> ListBankAccount { get; set; }
    }
}

using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.BusinessLogic.Models.Company;

namespace TN.TNM.BusinessLogic.Messages.Responses.CompanyConfig
{
    public class GetCompanyConfigResponse : BaseResponse
    {
        public CompanyConfigModel CompanyConfig { get; set; }
        public List<BankAccountModel> ListBankAccount { get; set; }
    }
}

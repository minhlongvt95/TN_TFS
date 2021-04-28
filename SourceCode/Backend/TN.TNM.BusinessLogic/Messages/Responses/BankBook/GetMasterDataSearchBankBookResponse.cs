using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.BankBook
{
    public class GetMasterDataSearchBankBookResponse : BaseResponse
    {
        public List<BankAccountModel> ListBankAccount { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
    }
}

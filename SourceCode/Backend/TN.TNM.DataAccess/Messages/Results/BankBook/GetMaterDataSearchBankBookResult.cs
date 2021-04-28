using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.BankBook
{
    public class GetMaterDataSearchBankBookResult : BaseResult
    {
        public List<Databases.Entities.BankAccount> ListBankAccount { get; set; }
        public List<Databases.Entities.Employee> ListEmployee { get; set; }
    }
}

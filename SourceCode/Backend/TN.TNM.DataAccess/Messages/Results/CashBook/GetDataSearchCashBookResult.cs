using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.CashBook
{
    public class GetDataSearchCashBookResult : BaseResult
    {
        public List<Databases.Entities.Employee> ListEmployee { get; set; }
        public List<Organization> ListOrganization { get; set; }
    }
}

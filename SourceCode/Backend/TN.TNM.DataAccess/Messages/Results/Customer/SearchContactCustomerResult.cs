using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class SearchContactCustomerResult : BaseResult
    {
        public List<ContactEntityModel> ListContactCustomer { get; set; }
    }
}

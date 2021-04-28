using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class UpdateCustomerByIdParameter : BaseParameter
    {
        public CustomerEntityModel CustomerModel { get; set; }
        public ContactEntityModel ContactModel { get; set; }
    }
}

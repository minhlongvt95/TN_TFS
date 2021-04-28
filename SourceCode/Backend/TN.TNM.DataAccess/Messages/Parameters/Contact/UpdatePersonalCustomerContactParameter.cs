using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Parameters.Contact
{
    public class UpdatePersonalCustomerContactParameter : BaseParameter
    {
        public PersonalCustomerContactModel Contact { get; set; }
    }
}

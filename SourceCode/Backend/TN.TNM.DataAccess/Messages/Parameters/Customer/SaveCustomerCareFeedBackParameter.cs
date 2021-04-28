using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class SaveCustomerCareFeedBackParameter : BaseParameter
    {
        public SaveCustomerCareFeedBackModel CustomerCareFeedBack { get; set; }
    }
}

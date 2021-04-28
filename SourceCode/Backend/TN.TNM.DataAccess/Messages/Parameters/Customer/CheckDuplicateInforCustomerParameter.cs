using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class CheckDuplicateInforCustomerParameter : BaseParameter
    {
        public Guid? CustomerId { get; set; }
        public int CheckType { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}

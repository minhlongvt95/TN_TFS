using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class CheckDuplicateLeadWithCustomerEntityModel
    {
        public Guid? CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}

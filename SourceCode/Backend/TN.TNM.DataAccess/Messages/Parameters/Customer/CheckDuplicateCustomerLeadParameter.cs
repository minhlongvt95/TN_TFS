using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class CheckDuplicateCustomerLeadParameter:BaseParameter
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public Guid LeadId { get; set; }
        public bool IsUpdateLead { get; set; }
    }
}

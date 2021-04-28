using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class LeadReferenceCustomerModel
    {
        public Guid? CustomerId { get; set; }
        public string CustomerStatus { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int? CustomerType { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string WorkEmail { get; set; }
        public string Address { get; set; }
        public string AddressWard { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public Guid? InvestmentFundId { get; set; }
    }
}

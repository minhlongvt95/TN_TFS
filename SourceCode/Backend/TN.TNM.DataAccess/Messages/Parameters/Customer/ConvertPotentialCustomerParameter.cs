using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class ConvertPotentialCustomerParameter: BaseParameter
    {
        public Guid? CustomerId { get; set; }
        public bool? IsCreateCustomer { get; set; }
        public bool? IsCreateLead { get; set; }
        public Guid? PersonalInChargeId { get; set; } //nếu tạo khách hàng 
        //public DataAccess.Models.Lead.LeadEntityModel LeadModel { get; set; }
        //public DataAccess.Models.ContactEntityModel ContactModel { get; set; }
        //public List<Guid?> ListInterestedGroupId { get; set; }
        public string LeadName { get; set; }
    }
}

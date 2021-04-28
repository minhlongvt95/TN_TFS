using System;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class GetTop3PotentialCustomersModel
    {
        public Guid ContactId { get; set; }
        public Guid LeadId { get; set; }
        public string LeadFirstName { get; set; }
        public string LeadLastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public string PersonInChargeName { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

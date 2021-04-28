using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class LeadEntityModelById
    {
        public Guid LeadId { get; set; }
        public string RequirementDetail { get; set; }
        public Guid? PotentialId { get; set; }
        public Guid? InterestedGroupId { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid StatusId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public string Role { get; set; }
        public bool WaitingForApproval { get; set; }
        public Guid? LeadTypeId { get; set; }
    }
}

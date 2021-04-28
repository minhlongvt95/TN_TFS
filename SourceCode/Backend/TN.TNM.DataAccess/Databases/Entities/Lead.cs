using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Lead
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
        public Guid? TenantId { get; set; }
        public Guid? LeadTypeId { get; set; }
        public Guid? LeadGroupId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? BusinessTypeId { get; set; }
        public Guid? InvestmentFundId { get; set; }
        public decimal? ExpectedSale { get; set; }
        public Guid? ProbabilityId { get; set; }
        public string LeadCode { get; set; }
        public int? CloneCount { get; set; }
        public Guid? GeographicalAreaId { get; set; }
        public Guid? StatusSuportId { get; set; }

        public Company Company { get; set; }
        public Category InterestedGroup { get; set; }
        public Category PaymentMethod { get; set; }
        public Category Potential { get; set; }
        public Category Status { get; set; }
    }
}

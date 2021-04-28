using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class LeadEntityModel
    {
        public Guid? LeadId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? PotentialId { get; set; }
        public Guid? InterestedGroupId { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public string RequirementDetail { get; set; }
        public string FullName { get; set; }
        public string LeadCode { get; set; }
        public string LeadCodeName { get; set; }
        public string PersonInChargeFullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullAddress { get; set; }
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public string AvatarUrl { get; set; }
        public string PersonInChargeAvatarUrl { get; set; }
        public bool NoActivePic { get; set; }
        public bool? Active { get; set; }
        public string Role { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CountLead { get; set; }
        public bool WaitingForApproval { get; set; }
        public bool CanDeleteLead { get; set; }
        public Guid? LeadGroupId { get; set; }
        public int? CloneCount { get; set; }
        public Guid? LeadTypeId { get; set; }

        public Guid? CustomerId { get; set; }
        public Guid? BusinessTypeId { get; set; }
        public Guid? InvestmentFundId { get; set; }
        public Guid? ProbabilityId { get; set; }
        public Guid? GeographicalAreaId { get; set; }

        public string CustomerName { get; set; }
        public string BusinessTypeName { get; set; }
        public string InvestmentFundName { get; set; }
        public string ProbabilityName { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid StatusId { get; set; }
        public decimal? ExpectedSale { get; set; }
        public string UpdatedMonthYear { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Trạng thái xác nhận của cơ hội :
        // 0 : Chưa gắn với HST hay báo giá.
        // 1 : Gắn vs HST
        // 2 : Gắn vs Báo giá
        public int IsStatusConnect { get; set; }

        public Guid? StatusSuportId { get; set; }
        public string StatusSupportName { get; set; }

        public List<LeadDetailModel> ListLeadDetail { get; set; }
    }
}

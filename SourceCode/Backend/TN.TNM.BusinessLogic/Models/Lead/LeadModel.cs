using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.BusinessLogic.Models.Lead
{
    public class LeadModel : BaseModel<DataAccess.Databases.Entities.Lead>
    {
        public Guid? LeadId { get; set; }
        public string RequirementDetail { get; set; }
        public Guid? PotentialId { get; set; }
        public Guid? InterestedGroupId { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public string CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public string LeadCode { get; set; }
        public string Role { get; set; }
        public string FullAddress { get; set; }
        public string FullName { get; set; }
        public string PersonInChargeFullName { get; set; }
        public string PersonInChargeAvatarUrl { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public string AvatarUrl { get; set; }
        public bool NoActivePic { get; set; }
        public bool WaitingForApproval { get; set; }
        public Guid? LeadTypeId { get; set; }
        public Guid? LeadGroupId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? BusinessTypeId { get; set; }
        public Guid? InvestmentFundId { get; set; }
        public Guid? ProbabilityId { get; set; }
        public decimal? ExpectedSale { get; set; }
        public Guid? GeographicalAreaId { get; set; }
        public Guid? ContactId { get; set; }
        public List<LeadDetailModel> ListLeadDetail { get; set; }

        public LeadModel() { }

        public LeadModel(LeadEntityModel model)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(model, this);
        }

        public LeadModel(LeadEntityModelById model)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(model, this);
        }

        public LeadModel(DataAccess.Databases.Entities.Lead entity) : base(entity) {
            //Xu ly sau khi lay tu DB len
        }
        
        public override DataAccess.Databases.Entities.Lead ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.Lead();
            Mapper(this, entity);
            return entity;
        }
    }
}


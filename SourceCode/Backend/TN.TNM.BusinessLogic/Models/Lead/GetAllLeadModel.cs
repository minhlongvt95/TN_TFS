using System;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.BusinessLogic.Models.Lead
{
    public class GetAllLeadModel : BaseModel<DataAccess.Databases.Entities.Lead>
    {
        public Guid? LeadId { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public Guid? StatusId { get; set; }
        public string FullName { get; set; }
        public string PersonInChargeFullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public int CountLead { get; set; }
        public Guid? ContactId { get; set; }
        public bool WaitingForApproval { get; set; }
        public bool CanDeleteLead { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? BusinessTypeId { get; set; }
        public Guid? InvestmentFundId { get; set; }
        public Guid? ProbabilityId { get; set; }
        public decimal? ExpectedSale { get; set; }
        public string CustomerName { get; set; }
        public string BusinessTypeName { get; set; }
        public string InvestmentFundName { get; set; }
        public string ProbabilityName { get; set; }
        public int IsStatusConnect { get; set; }
        public Guid? StatusSuportId { get; set; }
        public string StatusSupportName { get; set; }

        public GetAllLeadModel() { }

        public GetAllLeadModel(LeadEntityModel model)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(model, this);
        }

        public GetAllLeadModel(DataAccess.Databases.Entities.Lead entity) : base(entity)
        {
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

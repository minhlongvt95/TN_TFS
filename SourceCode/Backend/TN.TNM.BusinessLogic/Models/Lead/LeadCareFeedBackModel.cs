using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.BusinessLogic.Models.Lead
{
    public class LeadCareFeedBackModel : BaseModel<LeadCareFeedBack>
    {
        public Guid LeadCareFeedBackId { get; set; }
        public DateTime? FeedBackFromDate { get; set; }
        public DateTime? FeedBackToDate { get; set; }
        public Guid? FeedbackType { get; set; }
        public Guid? FeedBackCode { get; set; }
        public string FeedBackContent { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? LeadCareId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }

        public LeadCareFeedBackModel() { }

        public LeadCareFeedBackModel(LeadCareFeedBackEntityModel LeadCareFeedBackEntityModel)
        {
            Mapper(LeadCareFeedBackEntityModel, this);
        }

        public LeadCareFeedBackModel(LeadCareFeedBack entity) : base(entity)
        {
            Mapper(entity, this);
        }

        public override LeadCareFeedBack ToEntity()
        {
            var entity = new LeadCareFeedBack();
            Mapper(this, entity);
            return entity;
        }
    }
}

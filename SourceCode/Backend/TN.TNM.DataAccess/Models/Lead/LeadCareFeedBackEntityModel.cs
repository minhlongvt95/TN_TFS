using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class LeadCareFeedBackEntityModel
    {
        public LeadCareFeedBackEntityModel(LeadCareFeedBack leadCareFeedBack)
        {
            LeadCareFeedBackId = leadCareFeedBack.LeadCareFeedBackId;
            FeedBackFromDate = leadCareFeedBack.FeedBackFromDate;
            FeedBackToDate = leadCareFeedBack.FeedBackToDate;
            FeedbackType = leadCareFeedBack.FeedbackType;
            FeedBackCode = leadCareFeedBack.FeedBackCode;
            FeedBackContent = leadCareFeedBack.FeedBackContent;
            LeadId = leadCareFeedBack.LeadId;
            LeadCareId = leadCareFeedBack.LeadCareId;
            CreateDate = leadCareFeedBack.CreateDate;
            CreateById = leadCareFeedBack.CreateById;
            UpdateDate = leadCareFeedBack.UpdateDate;
            UpdateById = leadCareFeedBack.UpdateById;
        }
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
    }
}

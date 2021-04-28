using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.CustomerCare
{
    public class CustomerCareFeedBackEntityModel
    {
        public CustomerCareFeedBackEntityModel(CustomerCareFeedBack customerCareFeedBack)
        {
            CustomerCareFeedBackId = customerCareFeedBack.CustomerCareFeedBackId;
            FeedBackFromDate = customerCareFeedBack.FeedBackFromDate;
            FeedBackToDate = customerCareFeedBack.FeedBackToDate;
            FeedbackType = customerCareFeedBack.FeedbackType;
            FeedBackCode = customerCareFeedBack.FeedBackCode;
            FeedBackContent = customerCareFeedBack.FeedBackContent;
            CustomerId = customerCareFeedBack.CustomerId;
            CustomerCareId = customerCareFeedBack.CustomerCareId;
            CreateDate = customerCareFeedBack.CreateDate;
            CreateById = customerCareFeedBack.CreateById;
            UpdateDate = customerCareFeedBack.UpdateDate;
            UpdateById = customerCareFeedBack.UpdateById;
        }
        public Guid CustomerCareFeedBackId { get; set; }
        public DateTime? FeedBackFromDate { get; set; }
        public DateTime? FeedBackToDate { get; set; }
        public Guid? FeedbackType { get; set; }
        public Guid? FeedBackCode { get; set; }
        public string FeedBackContent { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CustomerCareId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
    }
}

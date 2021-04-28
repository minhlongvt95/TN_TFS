using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CustomerCareFeedBack
    {
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
        public Guid? TenantId { get; set; }

        public Customer Customer { get; set; }
        public CustomerCare CustomerCare { get; set; }
    }
}

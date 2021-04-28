using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class SaveLeadCareFeedBackModel
    {
        public Guid LeadId { get; set; }
        public Guid LeadCareId { get; set; }
        public Guid FeedBackCode { get; set; }
        public string FeedBackContent { get; set; }

        public SaveLeadCareFeedBackModel()
        {

        }

        public SaveLeadCareFeedBackModel(Guid _leadID, Guid _leadCareId, Guid _feedBackCode, string _feedbackContent)
        {
            LeadId = _leadID;
            LeadCareId = _leadCareId;
            FeedBackCode = _feedBackCode;
            FeedBackContent = _feedbackContent;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.DataAccess.Messages.Parameters.Lead;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class SaveLeadCareFeedBackRequest : BaseRequest<SaveLeadCareFeedBackParameter>
    {
        public LeadCareFeedBackModel LeadCareFeedBack { get; set; }
        public override SaveLeadCareFeedBackParameter ToParameter()
        {
            return new SaveLeadCareFeedBackParameter()
            {
                UserId = UserId,
                LeadCareFeedBack = new SaveLeadCareFeedBackModel(LeadCareFeedBack.LeadId.Value, LeadCareFeedBack.LeadCareId.Value, LeadCareFeedBack.FeedBackCode.Value, LeadCareFeedBack.FeedBackContent),
            };
        }
    }
}

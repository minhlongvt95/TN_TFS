using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Lead;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetHistoryLeadMeetingResponse : BaseResponse
    {
        public LeadMeetingInforBusinessModel LeadMeetingInfor { get; set; }
    }
}

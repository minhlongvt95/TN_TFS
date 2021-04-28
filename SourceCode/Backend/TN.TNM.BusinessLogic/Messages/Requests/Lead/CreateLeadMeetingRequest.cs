using System;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class CreateLeadMeetingRequest : BaseRequest<CreateLeadMeetingParameter>
    {
        public LeadMeetingModel LeadMeeting { get; set; }
        public override CreateLeadMeetingParameter ToParameter()
        {
            var leadMeeting = new DataAccess.Models.Lead.LeadMeetingEntityModel
            {
                LeadMeetingId = LeadMeeting.LeadMeetingId,
                LeadId = LeadMeeting.LeadId,
                EmployeeId = LeadMeeting.EmployeeId,
                Title = LeadMeeting.Title,
                LocationMeeting = LeadMeeting.LocationMeeting,
                StartDate = LeadMeeting.StartDate,
                StartHours = LeadMeeting.StartHours,
                Content = LeadMeeting.Content,
                EndDate = LeadMeeting.EndDate,
                EndHours = LeadMeeting.EndHours,
                Active = true,
                CreatedById = UserId,
                CreatedDate = new DateTime(),
                Participant = LeadMeeting.Participant
            };

            return new CreateLeadMeetingParameter()
            {
                LeadMeeting = leadMeeting,
                UserId = UserId
            };
        }
    }
}

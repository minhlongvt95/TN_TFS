using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.BusinessLogic.Models.Lead
{
    public class LeadMeetingModel : BaseModel<LeadMeetingEntityModel>
    {
        public Guid? LeadMeetingId { get; set; }
        public Guid LeadId { get; set; }
        public Guid EmployeeId { get; set; }
        public string Title { get; set; }
        public string LocationMeeting { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? StartHours { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EndHours { get; set; }
        public string Content { get; set; }
        public string Participant { get; set; }
        public string LeadName { get; set; }
        public string EmployeeName { get; set; }
        public string CreateByName { get; set; }
        public bool IsShowLink { get; set; }
        public LeadMeetingModel()
        {

        }

        public LeadMeetingModel(LeadMeetingEntityModel model)
        {
            Mapper(model, this);
        }

        public override LeadMeetingEntityModel ToEntity()
        {
            var entity = new LeadMeetingEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}

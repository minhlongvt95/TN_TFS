using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class LeadMeetingEntityModel
    {
        public Guid? LeadMeetingId { get; set; }
        public Guid LeadId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string Title { get; set; }
        public string LocationMeeting { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? StartHours { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EndHours { get; set; }
        public string Content { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public string Participant { get; set; }
        public string LeadName { get; set; }
        public string EmployeeName { get; set; }
        public string CreateByName { get; set; }
        public bool IsShowLink { get; set; }
    }
}

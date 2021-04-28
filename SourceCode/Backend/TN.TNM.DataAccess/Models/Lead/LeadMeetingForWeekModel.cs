using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class LeadMeetingForWeekModel
    {
        public Guid LeadMeetingId { get; set; }
        public Guid EmployeeId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Background { get; set; }
        public DateTime? StartDate { get; set; }
        public TimeSpan? StartHours { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? EndHours { get; set; }
    }
}

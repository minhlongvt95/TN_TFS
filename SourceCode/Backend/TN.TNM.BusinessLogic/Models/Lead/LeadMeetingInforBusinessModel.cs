using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Models.Lead
{
    public class LeadMeetingInforBusinessModel
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeePosition { get; set; }
        public List<LeadMeetingForWeekBusinessModel> Week1 { get; set; }
        public List<LeadMeetingForWeekBusinessModel> Week2 { get; set; }
        public List<LeadMeetingForWeekBusinessModel> Week3 { get; set; }
        public List<LeadMeetingForWeekBusinessModel> Week4 { get; set; }
        public List<LeadMeetingForWeekBusinessModel> Week5 { get; set; }
    }
}

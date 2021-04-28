using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class LeadMeetingInforModel
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeePosition { get; set; }
        public List<LeadMeetingForWeekModel> Week1 { get; set; }
        public List<LeadMeetingForWeekModel> Week2 { get; set; }
        public List<LeadMeetingForWeekModel> Week3 { get; set; }
        public List<LeadMeetingForWeekModel> Week4 { get; set; }
        public List<LeadMeetingForWeekModel> Week5 { get; set; }
    }
}
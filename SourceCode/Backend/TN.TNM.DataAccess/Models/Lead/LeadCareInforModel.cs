using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class LeadCareInforModel
    {
        public string EmployeeName { get; set; }
        public string EmployeePosition { get; set; }
        public Guid EmployeeCharge { get; set; }
        public List<LeadCareForWeekModel> Week1 { get; set; }
        public List<LeadCareForWeekModel> Week2 { get; set; }
        public List<LeadCareForWeekModel> Week3 { get; set; }
        public List<LeadCareForWeekModel> Week4 { get; set; }
        public List<LeadCareForWeekModel> Week5 { get; set; }
    }
}

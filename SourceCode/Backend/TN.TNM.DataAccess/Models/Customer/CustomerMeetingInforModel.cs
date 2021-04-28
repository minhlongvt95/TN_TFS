using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Customer
{
    public class CustomerMeetingInforModel
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeePosition { get; set; }
        public List<CustomerMeetingForWeekModel> Week1 { get; set; }
        public List<CustomerMeetingForWeekModel> Week2 { get; set; }
        public List<CustomerMeetingForWeekModel> Week3 { get; set; }
        public List<CustomerMeetingForWeekModel> Week4 { get; set; }
        public List<CustomerMeetingForWeekModel> Week5 { get; set; }
    }
}

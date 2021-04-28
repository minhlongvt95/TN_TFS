using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Models.Customer
{
    public class CustomerMeetingInforBusinessModel
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeePosition { get; set; }
        public List<CustomerMeetingForWeekBusinessModel> Week1 { get; set; }
        public List<CustomerMeetingForWeekBusinessModel> Week2 { get; set; }
        public List<CustomerMeetingForWeekBusinessModel> Week3 { get; set; }
        public List<CustomerMeetingForWeekBusinessModel> Week4 { get; set; }
        public List<CustomerMeetingForWeekBusinessModel> Week5 { get; set; }
    }
}

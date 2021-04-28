using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Customer
{
    public class CustomerMeetingEntityModel
    {
        public Guid? CustomerMeetingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid EmployeeId { get; set; }
        public string Title { get; set; }
        public string LocationMeeting { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? StartHours { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EndHours { get; set; }
        public string Content { get; set; }
        public string Participants { get; set; }
        public Guid CreatedById { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string CreateByName { get; set; }
        public bool IsShowLink { get; set; }
    }
}

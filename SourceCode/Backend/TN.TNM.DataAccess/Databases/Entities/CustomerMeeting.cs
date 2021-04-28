﻿using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CustomerMeeting
    {
        public Guid CustomerMeetingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid EmployeeId { get; set; }
        public string Title { get; set; }
        public string LocationMeeting { get; set; }
        public DateTime? StartDate { get; set; }
        public TimeSpan? StartHours { get; set; }
        public string Content { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? EndHours { get; set; }
        public string Participants { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Queue
    {
        public Guid QueueId { get; set; }
        public string SendTo { get; set; }
        public string SendContent { get; set; }
        public string Title { get; set; }
        public string Method { get; set; }
        public bool? IsSend { get; set; }
        public DateTime? SenDate { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public string FromTo { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CustomerCareId { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? CustomerMeetingId { get; set; }
        public Guid? EmployeeId { get; set; }
    }
}

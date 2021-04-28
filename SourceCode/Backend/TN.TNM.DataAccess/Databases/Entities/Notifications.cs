using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Notifications
    {
        public Guid NotificationId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string NotificationType { get; set; }
        public Guid ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Viewed { get; set; }
        public Guid? TenantId { get; set; }

        public Employee Receiver { get; set; }
    }
}

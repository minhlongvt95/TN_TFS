using System;

namespace TN.TNM.DataAccess.Models.Queue
{
    public class QueueEntityModel
    {
        public Guid QueueId { get; set; }
        public string SendTo { get; set; }
        public string SendContent { get; set; }
        public string Title { get; set; }
        public Guid Method { get; set; }
        public bool? IsSend { get; set; }
        public DateTime? SenDate { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
    }
}

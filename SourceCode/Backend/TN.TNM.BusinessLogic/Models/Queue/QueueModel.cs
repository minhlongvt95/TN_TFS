using System;

namespace TN.TNM.BusinessLogic.Models.Queue
{
    public class QueueModel : BaseModel<DataAccess.Databases.Entities.Queue>
    {
        public Guid QueueId { get; set; }
        public string FromTo { get; set; }
        public string SendTo { get; set; }
        public string SendContent { get; set; }
        public string Title { get; set; }
        public string Method { get; set; }
        public bool? IsSend { get; set; }
        public DateTime? SenDate { get; set; }
        public Guid? CustomerCareId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }

        public QueueModel() { }

        public QueueModel(DataAccess.Databases.Entities.Queue entity) : base(entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.Queue ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.Queue();
            Mapper(this, entity);
            return entity;
        }

    }
}

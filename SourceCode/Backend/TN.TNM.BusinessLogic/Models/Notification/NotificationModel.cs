using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Models.Notification
{
    public class NotificationModel : BaseModel<Notifications>
    {
        public Guid NotificationId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string NotificationType { get; set; }
        public Guid ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Viewed { get; set; }
        public NotificationModel(){}
        
        public NotificationModel(Notifications entity)
        {
            Mapper(entity, this);
        }
        public override Notifications ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new Notifications();
            Mapper(this, entity);
            return entity;
        }
    }
}

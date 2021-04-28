using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class ActionResourceModel : BaseModel<ActionResource>
    {
        public Guid ActionResourceId { get; set; }
        public string ActionResource1 { get; set; }
        public string Description { get; set; }

        public ActionResourceModel() { }

        public ActionResourceModel(ActionResource entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override ActionResource ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new ActionResource();
            Mapper(this, entity);
            return entity;
        }
    }
}

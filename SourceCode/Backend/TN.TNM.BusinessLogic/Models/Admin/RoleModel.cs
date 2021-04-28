using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class RoleModel : BaseModel<Role>
    {
        public Guid RoleId { get; set; }
        public string RoleValue { get; set; }
        public string Description { get; set; }
        public int UserNumber { get; set; }

        public RoleModel() { }

        public RoleModel(Role entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override Role ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new Role();
            Mapper(this, entity);
            return entity;
        }
    }
}

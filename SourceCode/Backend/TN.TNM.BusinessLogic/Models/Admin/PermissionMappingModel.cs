using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class PermissionMappingModel : BaseModel<PermissionMapping>
    {
        public Guid PermissionMappingId { get; set; }
        public string PermissionMappingName { get; set; }
        public string PermissionMappingCode { get; set; }
        public Guid? UserId { get; set; }
        public Guid? GroupId { get; set; }
        public string UseFor { get; set; }
        public Guid PermissionId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }

        public PermissionMappingModel(PermissionMapping entity) : base(entity)
        {

        }

        public PermissionMappingModel(PermissionMappingEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override PermissionMapping ToEntity()
        {
            var entity = new PermissionMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}

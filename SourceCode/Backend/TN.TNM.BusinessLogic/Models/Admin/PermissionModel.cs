using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class PermissionModel : BaseModel<Permission>
    {
        public Guid? PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public Guid? ParentId { get; set; }
        public string PermissionDescription { get; set; }
        public string Type { get; set; }
        public string IconClass { get; set; }
        public bool? Active { get; set; }
        public List<PermissionModel> PermissionChildList { get; set; }
        public int NumberOfUserHasPermission { get; set; }
        public byte? Sort { get; set; }

        public PermissionModel() { }

        public PermissionModel(Permission entity) : base(entity) { }
        public PermissionModel(PermissionEntityModel entity)
        {
            Mapper(entity, this);
            if (entity.PermissionChildList != null)
            {
                var cList = new List<PermissionModel>();
                entity.PermissionChildList.ForEach(child =>
                {
                    cList.Add(new PermissionModel(child));
                });
                PermissionChildList = cList;
            }
        }
        public override Permission ToEntity()
        {
            var entity = new Permission();
            Mapper(this, entity);
            return entity;
        }
    }
}

using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class PermissionSetModel : BaseModel<PermissionSet>
    {
        public Guid PermissionSetId { get; set; }
        public string PermissionSetName { get; set; }
        public string PermissionSetCode { get; set; }
        public string PermissionSetDescription { get; set; }
        public string PermissionId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public int NumberOfUserHasPermission { get; set; }
        public List<PermissionModel> PermissionList { get; set; }

        public PermissionSetModel() { }
        public PermissionSetModel(PermissionSet entity) : base(entity) { }
        public PermissionSetModel(PermissionSetEntityModel entity)
        {
            Mapper(entity, this);
            if (entity.PermissionList != null)
            {
                var cList = new List<PermissionModel>();
                entity.PermissionList.ForEach(child =>
                {
                    cList.Add(new PermissionModel(child));
                });
                PermissionList = cList;
            }
        }
        public override PermissionSet ToEntity()
        {
            var entity = new PermissionSet();
            Mapper(this, entity);
            return entity;
        }
    }
}

using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class WardModel : BaseModel<Ward>
    {
        public Guid WardId { get; set; }
        public Guid DistrictId { get; set; }
        public string WardName { get; set; }
        public string WardCode { get; set; }
        public string WardType { get; set; }
        public bool? Active { get; set; }

        public WardModel(Ward entity) : base(entity)
        {

        }

        public WardModel(WardEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override Ward ToEntity()
        {
            var entity = new Ward();
            Mapper(this, entity);
            return entity;
        }
    }
}

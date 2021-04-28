using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class AreaModel : BaseModel<GeographicalArea>
    {
        public Guid AreaId { get; set; }
        public string AreaName { get; set; }
        public string AreaCode { get; set; }
        public bool? Active { get; set; }

        public AreaModel(GeographicalArea entity) : base(entity)
        {

        }

        public AreaModel(AreaEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override GeographicalArea ToEntity()
        {
            var entity = new GeographicalArea();
            Mapper(this, entity);
            return entity;
        }
    }
}

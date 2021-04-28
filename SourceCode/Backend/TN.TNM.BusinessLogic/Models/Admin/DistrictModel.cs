using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class DistrictModel : BaseModel<District>
    {
        public Guid DistrictId { get; set; }
        public Guid ProvinceId { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictType { get; set; }
        public bool? Active { get; set; }
        public List<WardModel> WardList { get; set; }

        public DistrictModel(District entity) : base(entity)
        {
            
        }

        public DistrictModel(DistrictEntityModel entity)
        {
            Mapper(entity, this);
            Mapper(entity, this);
            if (entity.WardList != null)
            {
                var cList = new List<WardModel>();
                entity.WardList.ForEach(child =>
                {
                    cList.Add(new WardModel(child));
                });
                this.WardList = cList;
            }
        }

        public override District ToEntity()
        {
            var entity = new District();
            Mapper(this, entity);
            return entity;
        }
    }
}

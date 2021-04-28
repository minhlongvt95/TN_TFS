using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class ProvinceModel : BaseModel<Province>
    {
        public Guid ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string ProvinceCode { get; set; }
        public string ProvinceType { get; set; }
        public bool? Active { get; set; }
        public List<DistrictModel> DistrictList { get; set; }

        public ProvinceModel(Province entity) : base(entity)
        {

        }

        public ProvinceModel(ProvinceEntityModel entity)
        {
            Mapper(entity, this);
            if(entity.DistrictList != null)
            {
                var cList = new List<DistrictModel>();
                entity.DistrictList.ForEach(child =>
                {
                    cList.Add(new DistrictModel(child));
                });
                this.DistrictList = cList;
            }
        }

        public override Province ToEntity()
        {
            var entity = new Province();
            Mapper(this, entity);
            return entity;
        }
    }
}

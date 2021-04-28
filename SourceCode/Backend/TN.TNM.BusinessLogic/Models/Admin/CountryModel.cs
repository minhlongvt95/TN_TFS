using System;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class CountryModel : BaseModel<DataAccess.Databases.Entities.Country>
    {
        public Guid CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public CountryModel(CountryEntityModel entity)
        {
            Mapper(entity, this);
        }

        public CountryModel(DataAccess.Databases.Entities.Country entity) : base(entity)
        {
        }

        public override DataAccess.Databases.Entities.Country ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.Country();
            Mapper(this, entity);
            return entity;
        }
    }
}

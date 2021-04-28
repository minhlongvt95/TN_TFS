using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Models.Workflow
{
    public class SystemFeatureModel : BaseModel<SystemFeature>
    {
        public Guid SystemFeatureId { get; set; }
        public string SystemFeatureName { get; set; }
        public string SystemFeatureCode { get; set; }

        public SystemFeatureModel() { }
        public SystemFeatureModel(SystemFeature entity) : base(entity)
        {
            Mapper(entity, this);
        }
        public override SystemFeature ToEntity()
        {
            var entity = new SystemFeature();
            Mapper(this, entity);
            return entity;
        }
    }
}

using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Models.Case
{
    public class CaseActivitiesModel : BaseModel<CaseActivities>
    {
        public Guid CaseActivitiesId { get; set; }
        public Guid? CaseActivitiesType { get; set; }
        public string CaseActivitiesContent { get; set; }
        public Guid? CaseId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }

        public CaseActivitiesModel() { }

        public CaseActivitiesModel(CaseActivities entity) : base(entity)
        {
            Mapper(entity, this);
        }


        public override CaseActivities ToEntity()
        {
            var entity = new CaseActivities();
            Mapper(this, entity);
            return entity;
        }
    }
}

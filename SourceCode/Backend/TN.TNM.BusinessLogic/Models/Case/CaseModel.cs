using System;

namespace TN.TNM.BusinessLogic.Models.Case
{
    public class CaseModel : BaseModel<DataAccess.Databases.Entities.Case>
    {
        public Guid CaseId { get; set; }
        public Guid? ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string CasteTitle { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? AsignTo { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }


        public CaseModel() { }

        public CaseModel(DataAccess.Databases.Entities.Case entity) : base(entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.Case ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.Case();
            Mapper(this, entity);
            return entity;
        }
    }
}

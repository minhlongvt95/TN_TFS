using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Models.Company
{
    public class SystemParameterModel : BaseModel<SystemParameter>
    {
        public string SystemKey { get; set; }
        public bool? SystemValue { get; set; }
        public string SystemDescription { get; set; }
        public string SystemValueString { get; set; }
        public string SystemGroupCode { get; set; }
        public string SystemGroupDesc { get; set; }
        public string Description { get; set; }

        public SystemParameterModel() { }

        public SystemParameterModel(SystemParameter entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override SystemParameter ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new SystemParameter();
            Mapper(this, entity);
            return entity;
        }
    }
}

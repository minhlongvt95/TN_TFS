using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.BusinessLogic.Models.Lead
{
    public class LeadConvertRateModel:BaseModel<DataAccess.Databases.Entities.Lead>
    {
        public int Count { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public LeadConvertRateModel() { }

        public LeadConvertRateModel(LeadConvertRateEntityModel model)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.Lead ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.Lead();
            Mapper(this, entity);
            return entity;
        }
    }
}

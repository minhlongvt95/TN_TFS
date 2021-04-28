using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Models.CustomerCare
{
    public class GetTotalInteractiveModel : BaseModel<GetTotalInteractiveEntityModel>
    {
        public string CategoryName { get; set; }
        public int Total { get; set; }

        public GetTotalInteractiveModel() { }

        public GetTotalInteractiveModel(GetTotalInteractiveEntityModel entity) : base(entity)
        {
            Mapper(entity, this);
        }
        
        public override GetTotalInteractiveEntityModel ToEntity()
        {
            var entity = new GetTotalInteractiveEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}

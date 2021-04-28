using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Models.CustomerCare
{
    public class GetCharCustomerCSModel : BaseModel<GetCharCustomerCSEntityModel>
    {
        public string Month { get; set; }
        public int TotalCustomerProgram { get; set; }
        public int TotalCustomerCSKH { get; set; }

        public GetCharCustomerCSModel() { }

        public GetCharCustomerCSModel(GetCharCustomerCSEntityModel entity) : base(entity)
        {
            Mapper(entity, this);
        }


        public override GetCharCustomerCSEntityModel ToEntity()
        {
            var entity = new GetCharCustomerCSEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}

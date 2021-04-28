using System;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Models.CustomerCare
{
    public class GetCustomerCareActiveModel : BaseModel<GetCustomerCareActiveEntityModel>
    {
        public Guid CustomerCareId { get; set; }
        public string CustomerCareTitle { get; set; }
        public int CustomerTotal { get; set; }
        public string Status { get; set; }
        public string CategoryCare { get; set; }
        public string DateCreate { get; set; }

        public GetCustomerCareActiveModel() { }

        public GetCustomerCareActiveModel(GetCustomerCareActiveEntityModel entity) : base(entity)
        {
            Mapper(entity, this);
        }


        public override GetCustomerCareActiveEntityModel ToEntity()
        {
            var entity = new GetCustomerCareActiveEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}

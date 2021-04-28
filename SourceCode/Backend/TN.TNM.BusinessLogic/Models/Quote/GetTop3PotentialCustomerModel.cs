using System;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Models.Quote
{
    public class GetTop3PotentialCustomerModel : BaseModel<DataAccess.Databases.Entities.Quote>
    {
        public Guid ContactId { get; set; }
        public Guid LeadId { get; set; }
        public string LeadFirstName { get; set; }
        public string LeadLastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public string PersonInChargeName { get; set; }
        public DateTime? CreatedDate { get; set; }

        public GetTop3PotentialCustomerModel() { }

        public GetTop3PotentialCustomerModel(DataAccess.Databases.Entities.Quote entity) : base(entity)
        {
            // Mapper(entity, this);
        }
        public GetTop3PotentialCustomerModel(GetTop3PotentialCustomersModel model)
        {
            Mapper(model, this);
        }
        public override DataAccess.Databases.Entities.Quote ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.Quote();
            Mapper(this, entity);
            return entity;
        }

    }
}

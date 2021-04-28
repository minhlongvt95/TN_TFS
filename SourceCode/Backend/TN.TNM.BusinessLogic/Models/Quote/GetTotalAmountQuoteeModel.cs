using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Models.Quote
{
    public class GetTotalAmountQuoteeModel : BaseModel<DataAccess.Databases.Entities.Quote>
    {
        public decimal TotalAmount { get; set; }

        public GetTotalAmountQuoteeModel() { }

        public GetTotalAmountQuoteeModel(DataAccess.Databases.Entities.Quote entity) : base(entity)
        {
            // Mapper(entity, this);
        }
        public GetTotalAmountQuoteeModel(GetTotalAmountQuoteModel model)
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

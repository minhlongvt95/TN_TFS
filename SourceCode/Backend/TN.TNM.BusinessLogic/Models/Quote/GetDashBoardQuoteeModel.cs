using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Models.Quote
{
    public class GetDashBoardQuoteeModel : BaseModel<DataAccess.Databases.Entities.Quote>
    {
        public int CountNew { get; set; }
        public int CountInProgress { get; set; }
        public int CountDone { get; set; }
        public int CountWaiting { get; set; }
        public int CountAbort { get; set; }
        public int CountClose { get; set; }

        public GetDashBoardQuoteeModel() { }

        public GetDashBoardQuoteeModel(DataAccess.Databases.Entities.Quote entity) : base(entity)
        {
            // Mapper(entity, this);
        }
        public GetDashBoardQuoteeModel(GetDashBoardQuoteModel model)
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

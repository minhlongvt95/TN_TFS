using System;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Models.Quote
{
    public class GetTop3QuotesOverdueeModel : BaseModel<DataAccess.Databases.Entities.Quote>
    {
        public Guid QuoteId { get; set; }
        public string QuoteCode { get; set; }
        public string QuoteName { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? SendQuoteDate { get; set; }
        public DateTime? IntendedQuoteDate { get; set; }
        public decimal Amount { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string Status { get; set; }


        public GetTop3QuotesOverdueeModel() { }

        public GetTop3QuotesOverdueeModel(DataAccess.Databases.Entities.Quote entity) : base(entity)
        {
            // Mapper(entity, this);
        }
        public GetTop3QuotesOverdueeModel(GetTop3QuotesOverdueModel model)
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

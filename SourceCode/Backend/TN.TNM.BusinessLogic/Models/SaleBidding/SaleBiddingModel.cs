using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.BusinessLogic.Models.SaleBidding
{
    public class SaleBiddingModel : BaseModel<DataAccess.Databases.Entities.SaleBidding>
    {
        public Guid SaleBiddingId { get; set; }
        public string SaleBiddingName { get; set; }
        public Guid LeadId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal ValueBid { get; set; }
        public DateTime StartDate { get; set; }
        public string Address { get; set; }
        public DateTime? BidStartDate { get; set; }
        public string Note { get; set; }
        public Guid PersonInChargeId { get; set; }
        public int EffecTime { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? TypeContractId { get; set; }
        public string FormOfBid { get; set; }
        public Guid? CurrencyUnitId { get; set; }
        public Guid? StatusId { get; set; }
        public string StatusName { get; set; }
        public string TypeContractName { get; set; }
        public int SlowDay { get; set; }
        public string SaleBiddingCode { get; set; }
        public decimal? Ros { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PersonInChargeName { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreateDate { get; set; }
        public string LeadName { get; set; }
        public string LeadCode { get; set; }
        public Guid ContactId { get; set; }
        public Guid? EmployeeId { get; set; }
        public bool IsSupport { get; set; }
        public Guid? UpdatedById { get; set; }

        public List<CostQuoteModel> SaleBiddingDetail { get; set; }

        public SaleBiddingModel() { }

        public SaleBiddingModel(SaleBiddingModel entity)
        {
            Mapper(entity, this);
        }

        public SaleBiddingModel(SaleBiddingEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.SaleBidding ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.SaleBidding();
            Mapper(this, entity);
            return entity;
        }

        public  SaleBiddingEntityModel ToEntityModel()
        {
            var entity = new SaleBiddingEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}

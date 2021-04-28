using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.SaleBidding
{
    public class SaleBiddingEntityModel
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
        public decimal? Ros { get; set; }
        public int SlowDay { get; set; }
        public string SaleBiddingCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PersonInChargeName { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreateDate { get; set; }
        public string LeadName { get; set; }
        public string LeadCode { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? EmployeeId { get; set; }
        public bool IsSupport { get; set; }
        public string SaleBiddingCodeName { get; set; }
        public Guid? UpdatedById { get; set; }


        public List<CostQuoteModel> SaleBiddingDetail { get; set; }
    }
}

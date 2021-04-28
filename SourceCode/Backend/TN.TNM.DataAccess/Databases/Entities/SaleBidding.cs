using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class SaleBidding
    {
        public Guid SaleBiddingId { get; set; }
        public string SaleBiddingName { get; set; }
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
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? StatusId { get; set; }
        public string SaleBiddingCode { get; set; }
        public Guid LeadId { get; set; }
        public Guid? EmployeeId { get; set; }
        public bool IsSupport { get; set; }
        public bool? Active { get; set; }
    }
}

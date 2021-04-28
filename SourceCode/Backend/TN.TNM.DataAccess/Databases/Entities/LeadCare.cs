using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LeadCare
    {
        public Guid LeadCareId { get; set; }
        public string LeadCareCode { get; set; }
        public Guid? EmployeeCharge { get; set; }
        public DateTime? EffecttiveFromDate { get; set; }
        public DateTime? EffecttiveToDate { get; set; }
        public string LeadCareTitle { get; set; }
        public string LeadCareContent { get; set; }
        public Guid? LeadCareContactType { get; set; }
        public string LeadCareContentSms { get; set; }
        public bool? IsSendNow { get; set; }
        public bool? IsEvent { get; set; }
        public DateTime? SendDate { get; set; }
        public TimeSpan? SendHour { get; set; }
        public Guid? LeadCareEvent { get; set; }
        public TimeSpan? LeadCareEventHour { get; set; }
        public string LeadCareContentEmail { get; set; }
        public bool? IsSendEmailNow { get; set; }
        public DateTime? SendEmailDate { get; set; }
        public TimeSpan? SendEmailHour { get; set; }
        public string LeadCareVoucher { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? PercentDiscountAmount { get; set; }
        public Guid? GiftTypeId1 { get; set; }
        public double? GiftTotal1 { get; set; }
        public Guid? GiftTypeId2 { get; set; }
        public double? GiftTotal2 { get; set; }
        public int? LeadCareType { get; set; }
        public int? NumberCode { get; set; }
        public int? YearCode { get; set; }
        public int? MonthCode { get; set; }
        public int? DateCode { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? ActiveDate { get; set; }
        public int? GiftLeadType1 { get; set; }
        public int? GiftLeadType2 { get; set; }
    }
}

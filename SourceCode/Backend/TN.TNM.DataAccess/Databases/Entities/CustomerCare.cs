using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CustomerCare
    {
        public CustomerCare()
        {
            CustomerCareCustomer = new HashSet<CustomerCareCustomer>();
            CustomerCareFeedBack = new HashSet<CustomerCareFeedBack>();
            CustomerCareFilter = new HashSet<CustomerCareFilter>();
        }

        public Guid CustomerCareId { get; set; }
        public string CustomerCareCode { get; set; }
        public Guid? EmployeeCharge { get; set; }
        public DateTime? EffecttiveFromDate { get; set; }
        public DateTime? EffecttiveToDate { get; set; }
        public string CustomerCareTitle { get; set; }
        public string CustomerCareContent { get; set; }
        public Guid? CustomerCareContactType { get; set; }
        public string CustomerCareContentSms { get; set; }
        public bool? IsSendNow { get; set; }
        public bool? IsEvent { get; set; }
        public DateTime? SendDate { get; set; }
        public TimeSpan? SendHour { get; set; }
        public Guid? CustomerCareEvent { get; set; }
        public TimeSpan? CustomerCareEventHour { get; set; }
        public string CustomerCareContentEmail { get; set; }
        public bool? IsSendEmailNow { get; set; }
        public DateTime? SendEmailDate { get; set; }
        public TimeSpan? SendEmailHour { get; set; }
        public string CustomerCareVoucher { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? PercentDiscountAmount { get; set; }
        public decimal? ExpectedAmount { get; set; }
        public Guid? GiftTypeId1 { get; set; }
        public double? GiftTotal1 { get; set; }
        public Guid? GiftTypeId2 { get; set; }
        public double? GiftTotal2 { get; set; }
        public int? CustomerCareType { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public int? ProgramType { get; set; }
        public int? NumberCode { get; set; }
        public int? YearCode { get; set; }
        public int? MonthCode { get; set; }
        public int? DateCode { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? ActiveDate { get; set; }
        public int TypeCustomer { get; set; }
        public int? GiftCustomerType1 { get; set; }
        public int? GiftCustomerType2 { get; set; }

        public ICollection<CustomerCareCustomer> CustomerCareCustomer { get; set; }
        public ICollection<CustomerCareFeedBack> CustomerCareFeedBack { get; set; }
        public ICollection<CustomerCareFilter> CustomerCareFilter { get; set; }
    }
}

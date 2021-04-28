using System;

namespace TN.TNM.DataAccess.Models.CustomerCare
{
    public class CustomerCareEntityModel
    {
        public CustomerCareEntityModel(Databases.Entities.CustomerCare customerCare)
        {

            this.CustomerCareId = customerCare.CustomerCareId;
            this.CustomerCareCode = customerCare.CustomerCareCode;
            this.EmployeeCharge = customerCare.EmployeeCharge;
            this.EffecttiveFromDate = customerCare.EffecttiveFromDate;
            this.EffecttiveToDate = customerCare.EffecttiveToDate;
            this.CustomerCareTitle = customerCare.CustomerCareTitle;
            this.CustomerCareContent = customerCare.CustomerCareContent;
            this.CustomerCareContactType = customerCare.CustomerCareContactType;
            this.CustomerCareContentSms = customerCare.CustomerCareContentSms;
            this.IsSendNow = customerCare.IsSendNow;
            this.IsEvent = customerCare.IsEvent;
            this.SendDate = customerCare.SendDate;
            this.SendHour = customerCare.SendHour;
            this.CustomerCareEvent = customerCare.CustomerCareEvent;
            this.CustomerCareEventHour = customerCare.CustomerCareEventHour;
            this.CustomerCareContentEmail = customerCare.CustomerCareContentEmail;
            this.IsSendEmailNow = customerCare.IsSendEmailNow;
            this.SendEmailDate = customerCare.SendEmailDate;
            this.SendEmailHour = customerCare.SendEmailHour;
            this.CustomerCareVoucher = customerCare.CustomerCareVoucher;
            this.DiscountAmount = customerCare.DiscountAmount;
            this.PercentDiscountAmount = customerCare.PercentDiscountAmount;
            this.GiftCustomerType1 = customerCare.GiftCustomerType1;
            this.GiftTypeId1 = customerCare.GiftTypeId1;
            this.GiftTotal1 = customerCare.GiftTotal1;
            this.GiftCustomerType2 = customerCare.GiftCustomerType2;
            this.GiftTypeId2 = customerCare.GiftTypeId2;
            this.GiftTotal2 = customerCare.GiftTotal2;
            this.CustomerCareType = customerCare.CustomerCareType;
            this.StatusId = customerCare.StatusId;
            this.CreateDate = customerCare.CreateDate;
            this.CreateById = customerCare.CreateById;
            this.UpdateDate = customerCare.UpdateDate;
            this.UpdateById = customerCare.UpdateById;
            this.ExpectedAmount = customerCare.ExpectedAmount;
            this.ProgramType = customerCare.ProgramType;
          
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
        public int? GiftCustomerType1 { get; set; }
        public Guid? GiftTypeId1 { get; set; }
        public double? GiftTotal1 { get; set; }
        public int? GiftCustomerType2 { get; set; }
        public Guid? GiftTypeId2 { get; set; }
        public double? GiftTotal2 { get; set; }
        public int? CustomerCareType { get; set; }
        public int? ProgramType { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public decimal? ExpectedAmount { get; set; }

        public string CustomerCareContactTypeName { get; set; }
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public string EmployeeChargeName { get; set; }


    }
}

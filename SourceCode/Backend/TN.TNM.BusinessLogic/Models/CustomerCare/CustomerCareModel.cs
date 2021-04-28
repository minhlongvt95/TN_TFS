using System;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Models.CustomerCare
{
    public class CustomerCareModel : BaseModel<DataAccess.Databases.Entities.CustomerCare>
    {
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
        public decimal? ExpectedAmount { get; set; }
        public int? ProgramType { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }


        public string CustomerCareContactTypeName { get; set; }
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public string EmployeeChargeName { get; set; }

        public CustomerCareModel() { }

        public CustomerCareModel(CustomerCareEntityModel CustomerCareEntityModel) {
            Mapper(CustomerCareEntityModel, this);
        }


        public CustomerCareModel(DataAccess.Databases.Entities.CustomerCare entity) : base(entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.CustomerCare ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.CustomerCare();
            Mapper(this, entity);
            return entity;
        }
    }
}

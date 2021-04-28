using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.BusinessLogic.Models.Customer
{
    public class CustomerModel : BaseModel<DataAccess.Databases.Entities.Customer>
    {
        public Guid CustomerId { get; set; }
        public Guid ContactId { get; set; }
        public string CustomerCode { get; set; }
        public Guid? CustomerGroupId { get; set; }
        public string CustomerName { get; set; }
        public Guid? CustomerCareStaff { get; set; }
        public Guid? LeadId { get; set; }
        public Guid StatusId { get; set; }
        public Guid? CustomerServiceLevelId { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public short? CustomerType { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? ScaleId { get; set; }
        public decimal? TotalSaleValue { get; set; }
        public decimal? TotalReceivable { get; set; }
        public DateTime? NearestDateTransaction { get; set; }
        public decimal? MaximumDebtValue { get; set; }
        public int? MaximumDebtDays { get; set; }
        public Guid? MainBusinessSector { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public decimal? TotalCapital { get; set; }
        public DateTime? BusinessRegistrationDate { get; set; }
        public Guid? EnterpriseType { get; set; }
        public int? TotalEmployeeParticipateSocialInsurance { get; set; }
        public decimal? TotalRevenueLastYear { get; set; }
        public Guid? BusinessType { get; set; }
        public Guid? BusinessScale { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? IsGraduated { get; set; }
        public bool? Active { get; set; }
        public string PicName { get; set; }
        public Guid? PicContactId { get; set; }
        public string CustomerServiceLevelName { get; set; }
        public string StatusName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerEmailWork { get; set; }
        public string CustomerEmailOther { get; set; }
        public string CustomerPhone { get; set; }
        public string CusAvatarUrl { get; set; }
        public string PicAvatarUrl { get; set; }
        public Guid? CustomerCareCustomerId { get; set; }
        public Guid? CustomerCareCustomerStatusId { get; set; }
        public string CustomerCareCustomerStatusName { get; set; }
        public int CountCustomerInfo { get; set; }
        public string FullAddress { get; set; }
        public string BackgroupStatus { get; set; }
        public bool? IsApproval { get; set; }
        public int? ApprovalStep { get; set; }
        public bool? IsFromLead { get; set; }
        public Guid? InvestmentFundId { get; set; }
        public bool? AllowSendEmail { get; set; }
        public bool? AllowCall { get; set; }
        public string PotentialCustomerPosition { get; set; }
        public bool? IsConverted { get; set; }
        public string TaxCode { get; set; }
        public string CustomerGroup { get; set; }
        public string PersonInCharge { get; set; }
        public string CustomerCompany { get; set; }
        public string CustomerCodeName { get; set; }
        public decimal? Point { get; set; }
        public decimal? PayPoint { get; set; }
        public Guid? StatusCareId { get; set; }
        public Guid? CareStateId { get; set; }
        public Guid? EmployeeTakeCareId { get; set; }
        public DateTime? ContactDate { get; set; }
        public string SalesUpdate { get; set; }
        public string EvaluateCompany { get; set; }
        public string SalesUpdateAfterMeeting { get; set; }

        public List<BankAccountModel> ListBankAccount { get; set; }
        public List<Guid> ListOrderId { get; set; }

        public CustomerModel() { }

        public CustomerModel(DataAccess.Databases.Entities.Customer entity): base(entity) {
            
        }

        public CustomerModel(CustomerEntityModel model)
        {
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.Customer ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.Customer();
            Mapper(this, entity);
            return entity;
        }
    }
}

using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Customer
    {
        public Customer()
        {
            CustomerAdditionalInformation = new HashSet<CustomerAdditionalInformation>();
            CustomerCareCustomer = new HashSet<CustomerCareCustomer>();
            CustomerCareFeedBack = new HashSet<CustomerCareFeedBack>();
            CustomerOrder = new HashSet<CustomerOrder>();
        }

        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public Guid? CustomerGroupId { get; set; }
        public string CustomerName { get; set; }
        public Guid? LeadId { get; set; }
        public Guid StatusId { get; set; }
        public Guid? CustomerServiceLevelId { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public short? CustomerType { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? ScaleId { get; set; }
        public decimal? MaximumDebtValue { get; set; }
        public int? MaximumDebtDays { get; set; }
        public decimal? TotalSaleValue { get; set; }
        public decimal? TotalReceivable { get; set; }
        public DateTime? NearestDateTransaction { get; set; }
        public decimal? TotalCapital { get; set; }
        public DateTime? BusinessRegistrationDate { get; set; }
        public Guid? EnterpriseType { get; set; }
        public int? TotalEmployeeParticipateSocialInsurance { get; set; }
        public decimal? TotalRevenueLastYear { get; set; }
        public Guid? BusinessType { get; set; }
        public Guid? BusinessScale { get; set; }
        public int? NumberCode { get; set; }
        public int? YearCode { get; set; }
        public int? MonthCode { get; set; }
        public int? DateCode { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? MainBusinessSector { get; set; }
        public Guid? CustomerCareStaff { get; set; }
        public bool? IsGraduated { get; set; }
        public int? PortalId { get; set; }
        public Guid? TenantId { get; set; }
        public bool? IsApproval { get; set; }
        public int? ApprovalStep { get; set; }
        public Guid? InvestmentFundId { get; set; }
        public bool? IsFromLead { get; set; }
        public bool? AllowSendEmail { get; set; }
        public bool? AllowCall { get; set; }
        public bool? IsConverted { get; set; }
        public Guid? PotentialCustomerId { get; set; }
        public decimal? Point { get; set; }
        public decimal? PayPoint { get; set; }
        public Guid? CareStateId { get; set; }
        public Guid? StatusCareId { get; set; }
        public Guid? StatusSuportId { get; set; }
        public Guid? EmployeeTakeCareId { get; set; }
        public DateTime? ContactDate { get; set; }
        public string SalesUpdate { get; set; }
        public string EvaluateCompany { get; set; }
        public string SalesUpdateAfterMeeting { get; set; }

        public CustomerServiceLevel CustomerServiceLevel { get; set; }
        public Category Field { get; set; }
        public Category Scale { get; set; }
        public Category Status { get; set; }
        public ICollection<CustomerAdditionalInformation> CustomerAdditionalInformation { get; set; }
        public ICollection<CustomerCareCustomer> CustomerCareCustomer { get; set; }
        public ICollection<CustomerCareFeedBack> CustomerCareFeedBack { get; set; }
        public ICollection<CustomerOrder> CustomerOrder { get; set; }
    }
}

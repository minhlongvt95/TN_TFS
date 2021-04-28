using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Contract
{
    public class ContractEntityModel
    {
        public Guid ContractId { get; set; }
        public Guid? QuoteId { get; set; }
        public Guid? CustomerId { get; set; }
        public string ContractCode { get; set; }
        public Guid? ContractTypeId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? MainContractId { get; set; }
        public string ContractNote { get; set; }
        public string ContractDescription { get; set; }
        public decimal? ValueContract { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public Guid? BankAccountId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int DayLeft { get; set; }
        public bool Active { get; set; }
        public bool IsExtend { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? Amount { get; set; }
        public Guid? StatusId { get; set; }
        public string NameCustomer { get; set; }
        public string NameEmployee { get; set; }
        public string NameStatus { get; set; }
        public string NameCreateBy { get; set; }
        public string StatusCode { get; set; }
        public bool CanDelete { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public int? ContractTime { get; set; }
        public string ContractTimeUnit { get; set; }
        public List<ContractDetailEntityModel> ListDetail { get; set; }
    }
}

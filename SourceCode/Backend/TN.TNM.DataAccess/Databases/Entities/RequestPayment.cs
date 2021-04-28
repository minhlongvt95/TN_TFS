using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class RequestPayment
    {
        public Guid RequestPaymentId { get; set; }
        public string RequestPaymentCode { get; set; }
        public DateTime? RequestPaymentCreateDate { get; set; }
        public string RequestPaymentNote { get; set; }
        public Guid? RequestEmployee { get; set; }
        public string RequestEmployeePhone { get; set; }
        public Guid? RequestBranch { get; set; }
        public Guid? ApproverId { get; set; }
        public Guid? PostionApproverId { get; set; }
        public decimal? TotalAmount { get; set; }
        public Guid? PaymentType { get; set; }
        public string Description { get; set; }
        public int? NumberCode { get; set; }
        public int? YearCode { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public Guid? TenantId { get; set; }
    }
}

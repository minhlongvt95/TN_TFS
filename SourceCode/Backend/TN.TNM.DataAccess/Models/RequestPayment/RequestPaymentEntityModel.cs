using System;

namespace TN.TNM.DataAccess.Models.RequestPayment
{
    public class RequestPaymentEntityModel
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

        public string RequestEmployeeName { get; set; }
        public string StatusName { get; set; }
        public string PaymentName { get; set; }
        public string BranchName { get; set; }

        public RequestPaymentEntityModel() { }

        public RequestPaymentEntityModel(Databases.Entities.RequestPayment entity)
        {
            RequestPaymentId = entity.RequestPaymentId;
            RequestPaymentCode = entity.RequestPaymentCode;
            RequestEmployee = entity.RequestEmployee;
            RequestPaymentNote = entity.RequestPaymentNote;
            RequestEmployeePhone = entity.RequestEmployeePhone;
            RequestBranch = entity.RequestBranch;
            ApproverId = entity.ApproverId;
            PostionApproverId = entity.PostionApproverId;
            TotalAmount = entity.TotalAmount;
            PaymentType = entity.PaymentType;
            Description = entity.Description;
            NumberCode = entity.NumberCode;
            YearCode = entity.YearCode;
            StatusId = entity.StatusId;
            CreateDate = entity.CreateDate;
            CreateById = entity.CreateById;
            UpdateDate = entity.UpdateDate;
            UpdateById = entity.UpdateById;
        }
    }
}

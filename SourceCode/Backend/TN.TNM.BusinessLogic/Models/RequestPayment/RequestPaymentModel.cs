using System;
using TN.TNM.DataAccess.Models.RequestPayment;

namespace TN.TNM.BusinessLogic.Models.RequestPayment
{
    public class RequestPaymentModel : BaseModel<DataAccess.Databases.Entities.RequestPayment>
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

        public RequestPaymentModel() { }

        public RequestPaymentModel(RequestPaymentModel entity)
        {
            Mapper(entity, this);
        }

        public RequestPaymentModel(RequestPaymentEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.RequestPayment ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.RequestPayment();
            Mapper(this, entity);
            return entity;
        }
    }
}

using System;
using TN.TNM.DataAccess.Models.PayableInvoice;

namespace TN.TNM.BusinessLogic.Models.PayableInvoice
{
    public class PayableInvoiceModel : BaseModel<DataAccess.Databases.Entities.PayableInvoice>
    {
        public Guid PayableInvoiceId { get; set; }
        public string PayableInvoiceCode { get; set; }
        public string PayableInvoiceDetail { get; set; }
        public decimal? PayableInvoicePrice { get; set; }
        public Guid? PayableInvoicePriceCurrency { get; set; }
        public Guid? PayableInvoiceReason { get; set; }
        public string PayableInvoiceNote { get; set; }
        public Guid? RegisterType { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? StatusId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public decimal? UnitPrice { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Amount { get; set; }
        public string AmountText { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime PaidDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? ObjectId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? VouchersDate { get; set; }
        public string PayableInvoiceReasonText { get; set; }
        public string CreatedByName { get; set; }
        public string AvatarUrl { get; set; }
        public string ObjectName { get; set; }
        public string StatusName { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public string OrganizationName { get; set; }
        public string CurrencyUnitName { get; set; }
        public PayableInvoiceModel() { }

        public PayableInvoiceModel(PayableInvoiceEntityModel entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }

        public PayableInvoiceModel(DataAccess.Databases.Entities.PayableInvoice entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override DataAccess.Databases.Entities.PayableInvoice ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.PayableInvoice();
            Mapper(this, entity);
            return entity;
        }
    }
}

using System;
using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Models.ReceiptInvoice
{
    public class ReceiptInvoiceModel : BaseModel<DataAccess.Databases.Entities.ReceiptInvoice>
    {
        public Guid ReceiptInvoiceId { get; set; }
        public string ReceiptInvoiceCode { get; set; }
        public string ReceiptInvoiceDetail { get; set; }
        public Guid? ReceiptInvoiceReason { get; set; }
        public string ReceiptInvoiceNote { get; set; }
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
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreateByAvatarUrl { get; set; }
        public string NameReceiptInvoiceReason { get; set; }
        public string NameCreateBy { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string NameObjectReceipt { get; set; }
        public DateTime? VouchersDate { get; set; }
        public string StatusName { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public string OrganizationName { get; set; }
        public string CurrencyUnitName { get; set; }

        public ReceiptInvoiceModel() { }
        public ReceiptInvoiceModel(ReceiptInvoiceEntityModel entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }
        public ReceiptInvoiceModel(DataAccess.Databases.Entities.ReceiptInvoice entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }
        public override DataAccess.Databases.Entities.ReceiptInvoice ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.ReceiptInvoice();
            Mapper(this, entity);
            return entity;
        }
    }
}

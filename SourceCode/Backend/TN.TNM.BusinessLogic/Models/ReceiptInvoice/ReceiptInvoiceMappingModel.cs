using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Models.ReceiptInvoice
{
    public class ReceiptInvoiceMappingModel : BaseModel<ReceiptInvoiceMapping>
    {
        public Guid ReceiptInvoiceMappingId { get; set; }
        public Guid ReceiptInvoiceId { get; set; }
        public Guid? ObjectId { get; set; }
        public short? ReferenceType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public ReceiptInvoiceMappingModel() { }
        public ReceiptInvoiceMappingModel(ReceiptInvoiceMappingEntityModel entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }
        public ReceiptInvoiceMappingModel(ReceiptInvoiceMapping entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }
        public override ReceiptInvoiceMapping ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new ReceiptInvoiceMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}

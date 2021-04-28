using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Models.ReceiptInvoice
{
    public class BankReceiptInvoiceMappingModel: BaseModel<BankReceiptInvoiceMapping>
    {
        public Guid BankReceiptInvoiceMappingId { get; set; }
        public Guid BankReceiptInvoiceId { get; set; }
        public Guid? ObjectId { get; set; }
        public short? ReferenceType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public BankReceiptInvoiceMappingModel() { }
        public BankReceiptInvoiceMappingModel(BankReceiptInvoiceMappingEntityModel entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }

        public BankReceiptInvoiceMappingModel(BankReceiptInvoiceMapping entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override BankReceiptInvoiceMapping ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new BankReceiptInvoiceMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}

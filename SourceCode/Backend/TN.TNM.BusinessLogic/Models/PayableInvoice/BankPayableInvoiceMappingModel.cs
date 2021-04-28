using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.PayableInvoice;

namespace TN.TNM.BusinessLogic.Models.PayableInvoice
{
    public class BankPayableInvoiceMappingModel: BaseModel<BankPayableInvoiceMapping>
    {
        public Guid BankPayableInvoiceMappingId { get; set; }
        public Guid BankPayableInvoiceId { get; set; }
        public Guid? ObjectId { get; set; }
        public short? ReferenceType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public BankPayableInvoiceMappingModel() { }

        public BankPayableInvoiceMappingModel(BankPayableInvoiceMappingEntityModel entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }

        public BankPayableInvoiceMappingModel(BankPayableInvoiceMapping entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override BankPayableInvoiceMapping ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new BankPayableInvoiceMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}

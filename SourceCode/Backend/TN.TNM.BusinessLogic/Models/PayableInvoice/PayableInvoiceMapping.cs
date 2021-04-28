using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.PayableInvoice;

namespace TN.TNM.BusinessLogic.Models.PayableInvoice
{
    public class PayableInvoiceMappingModel : BaseModel<PayableInvoiceMapping>
    {
        public Guid PayableInvoiceMappingId { get; set; }
        public Guid PayableInvoiceId { get; set; }
        public Guid? ObjectId { get; set; }
        public short? ReferenceType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public PayableInvoiceMappingModel() { }

        public PayableInvoiceMappingModel(PayableInvoiceMappingEntityModel entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }

        public PayableInvoiceMappingModel(PayableInvoiceMapping entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override PayableInvoiceMapping ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new PayableInvoiceMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}

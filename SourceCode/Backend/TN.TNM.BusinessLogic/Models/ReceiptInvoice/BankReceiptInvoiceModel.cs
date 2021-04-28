using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Models.ReceiptInvoice
{
    public class BankReceiptInvoiceModel: BaseModel<BankReceiptInvoice>
    {
        public Guid BankReceiptInvoiceId { get; set; }
        public string BankReceiptInvoiceCode { get; set; }
        public string BankReceiptInvoiceDetail { get; set; }
        public decimal? BankReceiptInvoicePrice { get; set; }
        public Guid? BankReceiptInvoicePriceCurrency { get; set; }
        public decimal? BankReceiptInvoiceExchangeRate { get; set; }
        public Guid? BankReceiptInvoiceReason { get; set; }
        public string BankReceiptInvoiceNote { get; set; }
        public Guid? BankReceiptInvoiceBankAccountId { get; set; }
        public decimal? BankReceiptInvoiceAmount { get; set; }
        public string BankReceiptInvoiceAmountText { get; set; }
        public DateTime BankReceiptInvoicePaidDate { get; set; }
        public DateTime? VouchersDate { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? StatusId { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string BankReceiptInvoiceReasonText { get; set; }
        public string AvatarUrl { get; set; }
        public string CreatedByName { get; set; }
        public string ObjectName { get; set; }
        public string StatusName { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public BankReceiptInvoiceModel() { }

        public BankReceiptInvoiceModel(BankReceiptInvoiceEntityModel entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }

        public BankReceiptInvoiceModel(BankReceiptInvoice entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override BankReceiptInvoice ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new BankReceiptInvoice();
            Mapper(this, entity);
            return entity;
        }
    }
}

using System;

namespace TN.TNM.DataAccess.Models.ReceiptInvoice
{
    public class ReceiptInvoiceEntityModel
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
        public Guid? ObjectId { get; set; }
        public string CreateByAvatarUrl { get; set; }
        public string NameReceiptInvoiceReason { get; set; }
        public string NameCreateBy { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string NameObjectReceipt { get; set; }
        public string StatusName { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public string OrganizationName { get; set; }
        public string CurrencyUnitName { get; set; }

        public ReceiptInvoiceEntityModel() { }
        public ReceiptInvoiceEntityModel(Databases.Entities.ReceiptInvoice entity)
        {
            ReceiptInvoiceId = entity.ReceiptInvoiceId;
            ReceiptInvoiceCode = entity.ReceiptInvoiceCode;
            ReceiptInvoiceReason = entity.ReceiptInvoiceReason;
            ReceiptInvoiceDetail = entity.ReceiptInvoiceDetail;
            ReceiptInvoiceNote = entity.ReceiptInvoiceNote;
            RegisterType = entity.RegisterType;
            OrganizationId = entity.OrganizationId;
            StatusId = entity.StatusId;
            RecipientName = entity.RecipientName;
            RecipientAddress = entity.RecipientAddress;
            UnitPrice = entity.UnitPrice;
            CurrencyUnit = entity.CurrencyUnit;
            ExchangeRate = entity.ExchangeRate;
            Amount = entity.Amount;
            AmountText = entity.AmountText;
            Active = entity.Active;
            ReceiptDate = entity.ReceiptDate;
            CreatedById = entity.CreatedById;
            CreatedDate = entity.CreatedDate;
            UpdatedById = entity.UpdatedById;
            UpdatedDate = entity.UpdatedDate;
        }
    }
}

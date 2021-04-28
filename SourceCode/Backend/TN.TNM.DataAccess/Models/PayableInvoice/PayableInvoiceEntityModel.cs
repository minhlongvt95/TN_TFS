using System;

namespace TN.TNM.DataAccess.Models.PayableInvoice
{
    public class PayableInvoiceEntityModel
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
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? VouchersDate { get; set; }

        public Guid? ObjectId { get; set; }
        public DateTime PaidDate { get; set; }
        public string PayableInvoiceReasonText { get; set; }
        public string CreatedByName { get; set; }
        public string AvatarUrl { get; set; }
        public string ObjectName { get; set; }
        public string StatusName { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public string OrganizationName { get; set; }
        public string CurrencyUnitName { get; set; }
        public PayableInvoiceEntityModel() { }
        public PayableInvoiceEntityModel(Databases.Entities.PayableInvoice entity)
        {
            PayableInvoiceId = entity.PayableInvoiceId;
            PayableInvoiceCode = entity.PayableInvoiceCode;
            PayableInvoiceDetail = entity.PayableInvoiceDetail;
            PayableInvoicePrice = entity.PayableInvoicePrice;
            PayableInvoicePriceCurrency = entity.PayableInvoicePriceCurrency;
            PayableInvoiceReason = entity.PayableInvoiceReason;
            PayableInvoiceNote = entity.PayableInvoiceNote;
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
            PaidDate = entity.PaidDate;
            Active = entity.Active;
            CreatedById = entity.CreatedById;
            CreatedDate = entity.CreatedDate;
            UpdatedById = entity.UpdatedById;
            UpdatedDate = entity.UpdatedDate;
        }
    }
}

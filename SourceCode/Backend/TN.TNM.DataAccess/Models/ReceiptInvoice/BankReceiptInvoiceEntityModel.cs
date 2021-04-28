using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.ReceiptInvoice
{
    public class BankReceiptInvoiceEntityModel
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
        public Guid? OrganizationId { get; set; }
        public Guid? StatusId { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string BankReceiptInvoiceReasonText { get; set; }
        public string CreatedByName { get; set; }
        public string AvatarUrl { get; set; }
        public Guid? ObjectId { get; set; }
        public string ObjectName { get; set; }
        public string StatusName { get; set; }
        public string BackgroundColorForStatus { get; set; }

        public BankReceiptInvoiceEntityModel() { }
        public BankReceiptInvoiceEntityModel(BankReceiptInvoice entity)
        {
            BankReceiptInvoiceId = entity.BankReceiptInvoiceId;
            BankReceiptInvoiceCode = entity.BankReceiptInvoiceCode;
            BankReceiptInvoiceDetail = entity.BankReceiptInvoiceDetail;
            BankReceiptInvoicePrice = entity.BankReceiptInvoicePrice;
            BankReceiptInvoicePriceCurrency = entity.BankReceiptInvoicePriceCurrency;
            BankReceiptInvoiceExchangeRate = entity.BankReceiptInvoiceExchangeRate;
            BankReceiptInvoiceReason = entity.BankReceiptInvoiceReason;
            BankReceiptInvoiceNote = entity.BankReceiptInvoiceNote;
            BankReceiptInvoiceAmount = entity.BankReceiptInvoiceAmount;
            BankReceiptInvoiceAmountText = entity.BankReceiptInvoiceAmountText;
            BankReceiptInvoicePaidDate = entity.BankReceiptInvoicePaidDate;
            OrganizationId = entity.OrganizationId;
            StatusId = entity.StatusId;
            Active = entity.Active;
            CreatedById = entity.CreatedById;
            CreatedDate = entity.CreatedDate;
            UpdatedById = entity.UpdatedById;
            UpdatedDate = entity.UpdatedDate;
        }
    }
}

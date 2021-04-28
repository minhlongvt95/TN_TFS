using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.PayableInvoice
{
    public class BankPayableInvoiceEntityModel
    {
        public Guid BankPayableInvoiceId { get; set; }
        public string BankPayableInvoiceCode { get; set; }
        public string BankPayableInvoiceDetail { get; set; }
        public decimal? BankPayableInvoicePrice { get; set; }
        public Guid? BankPayableInvoicePriceCurrency { get; set; }
        public decimal? BankPayableInvoiceExchangeRate { get; set; }
        public Guid? BankPayableInvoiceReason { get; set; }
        public string BankPayableInvoiceNote { get; set; }
        public Guid? BankPayableInvoiceBankAccountId { get; set; }
        public decimal? BankPayableInvoiceAmount { get; set; }
        public string BankPayableInvoiceAmountText { get; set; }
        public DateTime BankPayableInvoicePaidDate { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? StatusId { get; set; }
        public string ReceiveAccountNumber { get; set; }
        public string ReceiveAccountName { get; set; }
        public string ReceiveBankName { get; set; }
        public string ReceiveBranchName { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string BankPayableInvoiceReasonText { get; set; }
        public string CreatedByName { get; set; }
        public string AvatarUrl { get; set; }
        public string ObjectName { get; set; }
        public string StatusName { get; set; }
        public Guid? ObjectId { get; set; }
        public string BackgroundColorForStatus { get; set; }

        public BankPayableInvoiceEntityModel() { }
        public BankPayableInvoiceEntityModel(BankPayableInvoice entity)
        {
            BankPayableInvoiceId = entity.BankPayableInvoiceId;
            BankPayableInvoiceCode = entity.BankPayableInvoiceCode;
            BankPayableInvoiceDetail = entity.BankPayableInvoiceDetail;
            BankPayableInvoicePrice = entity.BankPayableInvoicePrice;
            BankPayableInvoicePriceCurrency = entity.BankPayableInvoicePriceCurrency;
            BankPayableInvoiceExchangeRate = entity.BankPayableInvoiceExchangeRate;
            BankPayableInvoiceReason = entity.BankPayableInvoiceReason;
            BankPayableInvoiceNote = entity.BankPayableInvoiceNote;
            BankPayableInvoiceBankAccountId = entity.BankPayableInvoiceBankAccountId;
            BankPayableInvoiceAmount = entity.BankPayableInvoiceAmount;
            BankPayableInvoiceAmountText = entity.BankPayableInvoiceAmountText;
            BankPayableInvoicePaidDate = entity.BankPayableInvoicePaidDate;
            OrganizationId = entity.OrganizationId;
            StatusId = entity.StatusId;
            ReceiveAccountNumber = entity.ReceiveAccountNumber;
            ReceiveAccountName = entity.ReceiveAccountName;
            ReceiveBankName = entity.ReceiveBankName;
            ReceiveBranchName = entity.ReceiveBranchName;
            Active = entity.Active;
            CreatedById = entity.CreatedById;
            CreatedDate = entity.CreatedDate;
            UpdatedById = entity.UpdatedById;
            UpdatedDate = entity.UpdatedDate;
        }
    }
}

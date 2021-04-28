using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class QuoteEntityModel
    {
        public Guid QuoteId { get; set; }
        public string QuoteCode { get; set; }
        public string QuoteName { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? SendQuoteDate { get; set; }
        public Guid? Seller { get; set; }
        public int? EffectiveQuoteDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public Guid? ObjectTypeId { get; set; }
        public string ObjectType { get; set; }
        public Guid? CustomerContactId { get; set; }
        public Guid? PaymentMethod { get; set; }
        public bool? DiscountType { get; set; }
        public Guid? BankAccountId { get; set; }
        public int? DaysAreOwed { get; set; }
        public decimal? MaxDebt { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public TimeSpan? ReceivedHour { get; set; }
        public string RecipientName { get; set; }
        public string LocationOfShipment { get; set; }
        public string ShippingNote { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientEmail { get; set; }
        public string PlaceOfDelivery { get; set; }
        public decimal? Amount { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateTime? IntendedQuoteDate { get; set; }
        public Guid? StatusId { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? PersonInChargeId { get; set; }

        public string SellerName { get; set; }
        public string SellerFirstName { get; set; }
        public string SellerLastName { get; set; }
        public string SellerAvatarUrl { get; set; }
        public string QuoteStatusName { get; set; }
        public string CustomerName { get; set; }
        public int CountQuoteInOrder { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public int? ApprovalStep { get; set; }
        public bool? IsSendQuote { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? SaleBiddingId { get; set; }
        public string QuoteCodeName { get; set; }
        public string LeadCode { get; set; }
        public string SaleBiddingCode { get; set; }
        public Guid? InvestmentFundId { get; set; }
        public List<QuoteDetailEntityModel> ListDetail { get; set; }
        public List<QuoteCostDetailEntityModel> ListCostDetail { get; set; }
    }
}

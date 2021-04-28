using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Vendor
{
    public class SuggestedSupplierQuotesEntityModel
    {
        public Guid SuggestedSupplierQuoteId { get; set; }
        public string SuggestedSupplierQuote { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public DateTime? RecommendedDate { get; set; }
        public DateTime? QuoteTermDate { get; set; }
        public string ObjectType { get; set; }
        public Guid? ObjectId { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? ProcurementRequestId { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string StatusName { get; set; }
        public string SaleBiddingCode { get; set; }
        public string QuoteCode { get; set; }
        public string Email { get; set; }
        public bool CanDelete { get; set; }

        public List<SuggestedSupplierQuotesDetailEntityModel> ListVendorQuoteDetail { get; set; }

        public SuggestedSupplierQuotesEntityModel()
        {
            this.ListVendorQuoteDetail = new List<SuggestedSupplierQuotesDetailEntityModel>();
        }
    }
}

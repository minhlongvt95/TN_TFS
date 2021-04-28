using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Vendor
{
    public class VendorQuoteEntityModel
    {
        public Guid SuggestedSupplierQuoteId { get; set; }
        public string SuggestedSupplierQuote { get; set; }
        public Guid? StatusId { get; set; }
        public string StatusName { get; set; }
        public Guid? VendorId { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
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
        public bool CanDelete { get; set; }

        public List<SuggestedSupplierQuotesDetail> ListDetail { get; set; }
        
       public VendorQuoteEntityModel()
       {
       }
    }
}

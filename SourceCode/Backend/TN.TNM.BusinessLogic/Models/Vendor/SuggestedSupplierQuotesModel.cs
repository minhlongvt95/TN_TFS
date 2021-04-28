using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Models.Vendor
{
    public class SuggestedSupplierQuotesModel : BaseModel<SuggestedSupplierQuotes>
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

        public List<SuggestedSupplierQuotesDetailModel> ListVendorQuoteDetail { get; set; }

        public SuggestedSupplierQuotesModel()
        {

        }
        public SuggestedSupplierQuotesModel(SuggestedSupplierQuotes entity) : base(entity) { }
        public SuggestedSupplierQuotesModel(SuggestedSupplierQuotesEntityModel model)
        {
            Mapper(model, this);
        }
        public override SuggestedSupplierQuotes ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new SuggestedSupplierQuotes();
            Mapper(this, entity);
            return entity;
        }
    }
}

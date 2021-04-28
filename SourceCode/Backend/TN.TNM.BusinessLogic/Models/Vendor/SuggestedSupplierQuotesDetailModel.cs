using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Models.Vendor
{
    public class SuggestedSupplierQuotesDetailModel : BaseModel<SuggestedSupplierQuotesDetail>
    {
        public Guid SuggestedSupplierQuoteDetailId { get; set; }
        public Guid SuggestedSupplierQuoteId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal? Quantity { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductUnitName { get; set; }

        public SuggestedSupplierQuotesDetailModel()
        {

        }
        public SuggestedSupplierQuotesDetailModel(SuggestedSupplierQuotesDetail entity) : base(entity) { }
        public SuggestedSupplierQuotesDetailModel(SuggestedSupplierQuotesDetailEntityModel model)
        {
            Mapper(model, this);
        }
        public override SuggestedSupplierQuotesDetail ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new SuggestedSupplierQuotesDetail();
            Mapper(this, entity);
            return entity;
        }
    }
}

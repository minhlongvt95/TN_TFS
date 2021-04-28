using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Models.Product
{
    public class ProductVendorMappingModel : BaseModel<DataAccess.Databases.Entities.ProductVendorMapping>
    {
        public Guid ProductVendorMappingId { get; set; }
        public Guid ProductId { get; set; }
        public Guid VendorId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }
        public string VendorProductName { get; set; }
        public string VendorProductCode { get; set; }
        public decimal? MiniumQuantity { get; set; }
        public decimal? Price { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? UnitPriceId { get; set; }
        public Guid? MoneyUnitId { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string ProductName { get; set; }
        public string MoneyUnitName { get; set; }
        public string ProductUnitName { get; set; }
        public string ProductCode { get; set; }
        public int? OrderNumber { get; set; }
        public List<Guid?> ListSuggestedSupplierQuoteId { get; set; }

        //public ProductModel(ProductEntityModel model)
        //{
        //    //Xu ly sau khi lay tu DB len
        //    Mapper(model, this);
        //}
        public ProductVendorMappingModel(ProductVendorMappingEntityModel entity)
        {
            ListSuggestedSupplierQuoteId = new List<Guid?>();
            ListSuggestedSupplierQuoteId = entity.ListSuggestedSupplierQuoteId;
            Mapper(entity, this);
        }
        public ProductVendorMappingModel(DataAccess.Databases.Entities.ProductVendorMapping entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override DataAccess.Databases.Entities.ProductVendorMapping ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.ProductVendorMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}

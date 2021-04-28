using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Models.Product
{
    public class PriceProductModel : BaseModel<DataAccess.Databases.Entities.PriceProduct>
    {
        public Guid PriceProductId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public DateTime EffectiveDate { get; set; }
        public decimal PriceVnd { get; set; }
        public decimal MinQuantity { get; set; }
        public decimal? PriceForeignMoney { get; set; }
        public Guid? CustomerGroupCategory { get; set; }
        public string CustomerGroupCategoryName { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }

        public PriceProductModel()
        {
        }

        public PriceProductModel(PriceProductEntityModel model)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(model, this);
        }


        public PriceProductModel(DataAccess.Databases.Entities.PriceProduct entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override DataAccess.Databases.Entities.PriceProduct ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.PriceProduct();
            Mapper(this, entity);
            return entity;
        }
    }
}

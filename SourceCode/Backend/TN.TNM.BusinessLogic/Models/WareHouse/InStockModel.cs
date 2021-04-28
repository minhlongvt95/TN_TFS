using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Models.WareHouse
{
    public class InStockModel : BaseModel<InStockEntityModel>
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductGroup { get; set; }
        public string ProductUnitName { get; set; }
        public decimal QuantityInStock { get; set; }
        public Guid WareHouseId { get; set; }
        public string WareHouseName { get; set; }
        public decimal? ProductPrice { get; set; }
        public List<Serial> lstSerial { get; set; }
        public InStockModel() { }
        public InStockModel(InStockEntityModel entity)
        {
            Mapper(entity, this);
        }
        public override InStockEntityModel ToEntity()
        {
            var entity = new InStockEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}

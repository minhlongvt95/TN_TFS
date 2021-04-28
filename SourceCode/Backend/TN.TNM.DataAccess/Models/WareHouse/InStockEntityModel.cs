using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.WareHouse
{
    public class InStockEntityModel
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductGroup { get; set; }
        public string ProductUnitName { get; set; }
        public decimal QuantityInStock { get; set; }
        public decimal QuantityInStockMaximum { get; set; }
        public Guid WareHouseId { get; set; }
        public string WareHouseName { get; set; }
        public decimal? ProductPrice { get; set; }
        public List<Serial> lstSerial { get; set; }
    }
}

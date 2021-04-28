using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class UpdateItemInventoryReportParameter : BaseParameter
    {
        public ProductQuantityInWarehouseEntityModel InventoryReport { get; set; }
    }
}

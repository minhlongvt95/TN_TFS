using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class UpdateItemInventoryReportRequest : BaseRequest<UpdateItemInventoryReportParameter>
    {
        public ProductQuantityInWarehouseEntityModel InventoryReport { get; set; }

        public override UpdateItemInventoryReportParameter ToParameter()
        {
            return new UpdateItemInventoryReportParameter()
            {
                UserId = UserId,
                InventoryReport = InventoryReport
            };
        }
    }
}

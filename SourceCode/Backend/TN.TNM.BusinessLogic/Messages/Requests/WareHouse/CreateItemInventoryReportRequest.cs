using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class CreateItemInventoryReportRequest : BaseRequest<CreateItemInventoryReportParameter>
    {
        public ProductQuantityInWarehouseEntityModel InventoryReport { get; set; }

        public override CreateItemInventoryReportParameter ToParameter()
        {
            return new CreateItemInventoryReportParameter()
            {
                UserId = UserId,
                InventoryReport = InventoryReport
            };
        }
    }
}

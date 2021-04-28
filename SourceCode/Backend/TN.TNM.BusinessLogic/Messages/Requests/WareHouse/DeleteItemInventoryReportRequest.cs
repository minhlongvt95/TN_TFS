using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class DeleteItemInventoryReportRequest : BaseRequest<DeleteItemInventoryReportParameter>
    {
        public Guid InventoryReportId { get; set; }

        public override DeleteItemInventoryReportParameter ToParameter()
        {
            return new DeleteItemInventoryReportParameter()
            {
                UserId = UserId,
                InventoryReportId = InventoryReportId
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class CreateItemInventoryReportResponse : BaseResponse
    {
        public Guid InventoryReportId { get; set; }
    }
}

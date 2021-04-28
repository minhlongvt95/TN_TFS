using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class CreateItemInventoryReportResult : BaseResult
    {
        public Guid InventoryReportId { get; set; }
    }
}

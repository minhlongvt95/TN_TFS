using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class DeleteItemInventoryReportParameter : BaseParameter
    {
        public Guid InventoryReportId { get; set; }
    }
}

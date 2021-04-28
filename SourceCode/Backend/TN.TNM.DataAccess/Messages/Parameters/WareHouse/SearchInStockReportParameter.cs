using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class SearchInStockReportParameter : BaseParameter
    {
        public DateTime FromDate { get; set; }
        public string ProductNameCode { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public Guid? WarehouseId { get; set; }
    }
}

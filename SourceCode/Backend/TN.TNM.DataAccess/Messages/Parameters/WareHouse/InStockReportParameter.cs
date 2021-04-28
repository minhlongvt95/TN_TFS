using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class InStockReportParameter:BaseParameter
    {
        public List<Guid> lstProduct { get; set; }
        public List<Guid> lstWarehouse { get; set; }
        public int? FromQuantity { get; set; }
        public int? ToQuantity { get; set; }
        public string SerialCode { get; set; }
    }
}

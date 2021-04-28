using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Vendor
{
    public class VendorOrderReportEntityModel
    {
        public int Stt { get; set; }
        public Guid VendorOrderId { get; set; }
        public string VendorOrderCode { get; set; }
        public DateTime? VendorOrderDate { get; set; }
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal Quantity { get; set; }
        public string StatusName { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public Guid? VendorId { get; set; }
        public string VendorName { get; set; }
        public decimal? PriceWarehouse { get; set; }
        public decimal? PriceValueWarehouse { get; set; }
    }
}

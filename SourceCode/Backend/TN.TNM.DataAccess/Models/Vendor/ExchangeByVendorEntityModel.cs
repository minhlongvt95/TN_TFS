using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Vendor
{
    public class ExchangeByVendorEntityModel
    {
        public string ExchangeType { get; set; } // UNC: "Ủy nhiệm chi:, "PC: Phiếu chi
        public Guid? ExchangeId { get; set; }
        public DateTime? ExchangeDate { get; set; }
        public string ExchangeCode { get; set; }
        public string ExchangeName { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public decimal? ExchangeValue { get; set; }
        public string ExchangeDetail { get; set; }
    }
}

using System;

namespace TN.TNM.DataAccess.Messages.Parameters.SalesReport
{
    public class SalesReportParameter : BaseParameter
    {
        public DateTime? FromMonth { get; set; }
        public DateTime? ToMonth { get; set; }
    }
}

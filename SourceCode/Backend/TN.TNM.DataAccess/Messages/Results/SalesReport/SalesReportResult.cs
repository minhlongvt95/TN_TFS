using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Receivable;

namespace TN.TNM.DataAccess.Messages.Results.SalesReport
{
    public class SalesReportResult : BaseResult
    {
        public List<SalesReportEntityModel> SalesReportList { get; set; }
        public decimal TotalSale { get; set; }
        public decimal TotalCost { get; set; }
    }
}

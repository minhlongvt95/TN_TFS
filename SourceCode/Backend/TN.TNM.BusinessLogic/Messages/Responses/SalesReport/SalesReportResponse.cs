using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Receivable;

namespace TN.TNM.BusinessLogic.Messages.Responses.SalesReport
{
    public class SalesReportResponse : BaseResponse
    {
        public List<SalesReportModel> SalesReportList { get; set; }
        public decimal TotalSale { get; set; }
        public decimal TotalCost { get; set; }
    }
}

using System;
using TN.TNM.DataAccess.Messages.Parameters.SalesReport;

namespace TN.TNM.BusinessLogic.Messages.Requests.SalesReport
{
    public class SalesReportRequest : BaseRequest<SalesReportParameter>
    {
        public DateTime? FromMonth { get; set; }
        public DateTime? ToMonth { get; set; }
        public override SalesReportParameter ToParameter()
        {
            return new SalesReportParameter() {
                UserId = UserId,
                FromMonth = FromMonth,
                ToMonth = ToMonth
            };
        }
    }
}

using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Receivable.Customer
{
    public class GetReceivableCustomerReportParameter
    {
        public string CustomerNameOrCustomerCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}

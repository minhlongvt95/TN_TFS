using System;
using TN.TNM.DataAccess.Messages.Parameters.Receivable.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Receivable.Customer
{
    public class GetReceivableCustomerReportRequest : BaseRequest<GetReceivableCustomerReportParameter>
    {
        public string CustomerNameOrCustomerCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override GetReceivableCustomerReportParameter ToParameter()
        {
            return new GetReceivableCustomerReportParameter
            {
                CustomerNameOrCustomerCode = CustomerNameOrCustomerCode,
                FromDate = FromDate,
                ToDate = ToDate
            };
        }
    }
}

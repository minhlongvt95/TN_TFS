using System;
using TN.TNM.DataAccess.Messages.Parameters.Receivable.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Receivable.Customer
{
    public class GetReceivableCustomerDetailRequest : BaseRequest<GetReceivableCustomerDetailParameter>
    {
        public Guid CustomerId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public override GetReceivableCustomerDetailParameter ToParameter()
        {
            return new GetReceivableCustomerDetailParameter
            {
                CustomerId = CustomerId,
                FromDate = FromDate,
                ToDate = ToDate
            };
        }
    }
}

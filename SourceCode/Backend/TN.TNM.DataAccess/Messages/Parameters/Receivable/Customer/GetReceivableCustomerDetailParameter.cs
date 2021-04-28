using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Receivable.Customer
{
    public class GetReceivableCustomerDetailParameter
    {
        public Guid CustomerId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}

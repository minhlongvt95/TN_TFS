using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class GetHistoryCustomerMeetingParameter : BaseParameter
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid CustomerId { get; set; }
    }
}

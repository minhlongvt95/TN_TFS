using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class GetDataCustomerMeetingByIdParameter : BaseParameter
    {
        public Guid? CustomerMeetingId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class RemoveCustomerMettingParameter : BaseParameter
    {
        public Guid CustomerMeetingId { get; set; }
    }
}

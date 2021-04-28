using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class UpdateCustomerMettingParameter : BaseParameter
    {
        public Guid CustomerMeetingId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

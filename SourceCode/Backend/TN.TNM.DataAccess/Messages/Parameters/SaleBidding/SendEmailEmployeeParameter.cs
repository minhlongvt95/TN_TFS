using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.SaleBidding
{
    public class SendEmailEmployeeParameter:BaseParameter
    {
        public Guid SaleBiddingId { get; set; }
    }
}

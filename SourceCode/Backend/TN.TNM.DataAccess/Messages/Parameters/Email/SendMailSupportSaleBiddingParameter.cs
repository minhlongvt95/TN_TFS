using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Email
{
    public class SendMailSupportSaleBiddingParameter:BaseParameter
    {
        public string SaleBiddingCode { get; set; }
        public string SaleBiddingName { get; set; }
        public List<Guid> ListEmployeeId { get; set; }
    }
}

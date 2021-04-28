using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Email;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Email
{
    public class SendEmailParameter:BaseParameter
    {
        public int SendType { get; set; } 
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }
    }
}

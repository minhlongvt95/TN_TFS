using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Email
{
    public class SendEmailCustomerOrderParameter : BaseParameter
    {
        public Guid OrderId { get; set; } 
    }
}

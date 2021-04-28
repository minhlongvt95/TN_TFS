using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class CreateCustomerMeetingParameter : BaseParameter
    {
        public CustomerMeetingEntityModel CustomerMeeting { get; set; }
    }
}

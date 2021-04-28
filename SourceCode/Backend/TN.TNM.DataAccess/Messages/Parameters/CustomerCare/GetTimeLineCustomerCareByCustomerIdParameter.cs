using System;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class GetTimeLineCustomerCareByCustomerIdParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }
        public DateTime First_day { get; set; }
        public DateTime Last_day { get; set; } 
    }
}

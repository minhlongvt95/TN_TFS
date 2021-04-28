using System;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class GetCustomerCareFeedBackByCusIdAndCusCareIdParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }
        public Guid CustomerCareId { get; set; }
    }
}

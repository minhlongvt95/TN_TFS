using System;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class UpdateStatusCustomerCareCustomerByIdParameter : BaseParameter
    {
        public Guid CustomerCareCustomerId { get; set; }
        public Guid StatusId { get; set; }
    }
}

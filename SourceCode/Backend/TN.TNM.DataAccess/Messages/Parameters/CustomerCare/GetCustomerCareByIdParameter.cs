using System;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class GetCustomerCareByIdParameter:BaseParameter
    {
        public Guid CustomerCareId { get; set; }

    }
}

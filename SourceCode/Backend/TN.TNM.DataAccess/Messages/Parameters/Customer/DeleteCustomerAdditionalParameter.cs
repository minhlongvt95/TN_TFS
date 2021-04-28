using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class DeleteCustomerAdditionalParameter : BaseParameter
    {
        public Guid CustomerAdditionalInformationId { get; set; }
        public Guid CustomerId { get; set; }
    }
}

using System;

namespace TN.TNM.DataAccess.Messages.Parameters.SaleBidding
{
    public class GetPersonInChargeByCustomerIdParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }
    }
}

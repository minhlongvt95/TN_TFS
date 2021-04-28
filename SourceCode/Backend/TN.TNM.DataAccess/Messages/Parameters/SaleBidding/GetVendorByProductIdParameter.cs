using System;

namespace TN.TNM.DataAccess.Messages.Parameters.SaleBidding
{
    public class GetVendorByProductIdParameter : BaseParameter
    {
        public Guid ProductId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.SaleBidding
{
    public class GetVendorMappingParameter:BaseParameter
    {
        public Guid ProductId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.SaleBidding
{
    public class GetMasterDataCreateSaleBiddingParameter:BaseParameter
    {
        public Guid LeadId { get; set; }
    }
}

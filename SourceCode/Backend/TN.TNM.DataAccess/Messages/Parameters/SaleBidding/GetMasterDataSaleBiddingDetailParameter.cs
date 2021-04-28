using System;

namespace TN.TNM.DataAccess.Messages.Parameters.SaleBidding
{
    public class GetMasterDataSaleBiddingDetailParameter : BaseParameter
    {
        public Guid SaleBiddingId { get; set; }
    }
}

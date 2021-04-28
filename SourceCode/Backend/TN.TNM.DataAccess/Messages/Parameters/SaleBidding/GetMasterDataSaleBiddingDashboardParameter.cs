using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.SaleBidding
{
    public class GetMasterDataSaleBiddingDashboardParameter : BaseParameter
    {
        public DateTime? EffectiveDate { get; set; }
        public string SaleBiddingName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}

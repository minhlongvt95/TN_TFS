using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class GetMasterDataSaleBiddingDashboardRequest : BaseRequest<GetMasterDataSaleBiddingDashboardParameter>
    {
        public DateTime? EffectiveDate { get; set; }
        public string SaleBiddingName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public override GetMasterDataSaleBiddingDashboardParameter ToParameter()
        {
            return new GetMasterDataSaleBiddingDashboardParameter
            {
                FromDate = FromDate,
                ToDate = ToDate,
                UserId = UserId,
                EffectiveDate = EffectiveDate,
                SaleBiddingName = SaleBiddingName
            };
        }
    }
}

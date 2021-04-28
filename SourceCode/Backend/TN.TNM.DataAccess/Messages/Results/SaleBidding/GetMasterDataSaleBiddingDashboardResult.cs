using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.DataAccess.Messages.Results.SaleBidding
{
    public class GetMasterDataSaleBiddingDashboardResult : BaseResult
    {
        public List<SaleBiddingEntityModel> ListSaleBiddingWaitApproval { get; set; }
        public List<SaleBiddingEntityModel> ListSaleBiddingExpired { get; set; }
        public List<SaleBiddingEntityModel> ListSaleBiddingSlowStartDate { get; set; }
        public List<SaleBiddingEntityModel> ListSaleBiddingInWeek { get; set; }
        public List<SaleBiddingEntityModel> ListSaleBiddingChart { get; set; }
        public List<CategoryEntityModel> ListTypeContact { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
    }
}

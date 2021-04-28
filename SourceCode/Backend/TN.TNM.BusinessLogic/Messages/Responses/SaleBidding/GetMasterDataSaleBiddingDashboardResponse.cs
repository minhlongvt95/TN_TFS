using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Responses.SaleBidding
{
    public class GetMasterDataSaleBiddingDashboardResponse : BaseResponse
    {
        public List<SaleBiddingModel> ListSaleBiddingWaitApproval { get; set; }
        public List<SaleBiddingModel> ListSaleBiddingExpired { get; set; }
        public List<SaleBiddingModel> ListSaleBiddingSlowStartDate { get; set; }
        public List<SaleBiddingModel> ListSaleBiddingInWeek { get; set; }
        public List<SaleBiddingModel> ListSaleBiddingChart { get; set; }
        public List<CategoryModel> ListTypeContact { get; set; }
        public List<CategoryModel> ListStatus { get; set; }
    }
}

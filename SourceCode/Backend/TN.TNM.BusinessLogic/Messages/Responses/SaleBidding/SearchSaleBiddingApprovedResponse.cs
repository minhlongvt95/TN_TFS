using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Responses.SaleBidding
{
    public class SearchSaleBiddingApprovedResponse:BaseResponse
    {
        public List<SaleBiddingModel> ListSaleBidding { get; set; }
    }
}

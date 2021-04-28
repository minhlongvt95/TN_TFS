using System.Collections.Generic;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.DataAccess.Messages.Results.SaleBidding
{
    public class SearchSaleBiddingResult : BaseResult
    {
        public List<SaleBiddingEntityModel> ListSaleBidding { get; set; }
    }
}

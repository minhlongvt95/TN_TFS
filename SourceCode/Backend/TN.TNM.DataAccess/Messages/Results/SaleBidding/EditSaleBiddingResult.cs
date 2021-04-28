using System.Collections.Generic;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.DataAccess.Messages.Results.SaleBidding
{
    public class EditSaleBiddingResult : BaseResult
    {
        public List<SaleBiddingDetailEntityModel> ListSaleBiddingDetail { get; set; }
    }
}

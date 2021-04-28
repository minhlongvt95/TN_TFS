using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Cost;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetMasterDataCreateCostResponse : BaseResponse
    {
        public List<CategoryModel> ListStatus { get; set; }
        public List<CostModel> ListCost { get; set; }
    }
}

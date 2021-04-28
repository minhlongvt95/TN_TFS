using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Responses.Promotion
{
    public class SearchListPromotionResponse : BaseResponse
    {
        public List<PromotionEntityModel> ListPromotion { get; set; }
    }
}

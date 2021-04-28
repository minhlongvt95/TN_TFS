using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class SearchListPromotionRequest : BaseRequest<SearchListPromotionParameter>
    {
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public DateTime? ExpirationDateFrom { get; set; }
        public DateTime? ExpirationDateTo { get; set; }
        public int? QuantityOrder { get; set; }
        public int? QuantityQuote { get; set; }
        public int? QuantityContract { get; set; }

        public override SearchListPromotionParameter ToParameter()
        {
            return new SearchListPromotionParameter()
            {
                UserId = UserId,
                PromotionCode = PromotionCode,
                PromotionName = PromotionName,
                ExpirationDateFrom = ExpirationDateFrom,
                ExpirationDateTo = ExpirationDateTo,
                QuantityOrder = QuantityOrder,
                QuantityQuote = QuantityQuote,
                QuantityContract = QuantityContract
            };
        }
    }
}

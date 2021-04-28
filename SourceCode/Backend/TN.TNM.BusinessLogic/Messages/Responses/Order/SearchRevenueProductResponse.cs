using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class SearchRevenueProductResponse: BaseResponse
    {
        public List<DataAccess.Models.Order.ProductRevenueEntityModel> ListProductRevenue { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class SearchTopReVenueResponse: BaseResponse
    {
        public List<DataAccess.Models.Order.TopEmployeeRevenueEntityModel> ListTopEmployeeRevenue { get; set; }
    }
}

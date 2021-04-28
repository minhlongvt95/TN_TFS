using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class SearchTopReVenueResult: BaseResult
    {
        public List<DataAccess.Models.Order.TopEmployeeRevenueEntityModel> ListTopEmployeeRevenue { get; set; }
    }
}

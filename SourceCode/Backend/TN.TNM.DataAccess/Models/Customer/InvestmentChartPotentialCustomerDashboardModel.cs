using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Customer
{
    //public class InvestmentChartPotentialCustomerDashboardModel
    //{
    //    public decimal? TotalInvestmentFundRecord { get; set; }
    //    public List<ItemInvestmentChartPotentialCustomerDashboard> ListInvestmentFund { get; set; }
    //}

    public class ItemInvestmentChartPotentialCustomerDashboard
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal? Value { get; set; }
        public decimal? PercentValue { get; set; }
    }
}

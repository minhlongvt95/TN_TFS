using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.CustomerOrder
{
    public class SearchProfitCustomerModel
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public decimal? SaleRevenue { get; set; }
        public decimal? TotalPriceInitial { get; set; }
        public decimal? TotalGrossProfit { get; set; }
        public decimal? TotalProfitPerSaleRevenue { get; set; }
        public decimal? TotalProfitPerPriceInitial { get; set; }

        public SearchProfitCustomerModel()
        {
            this.CustomerCode = "";
            this.CustomerName = "";
            this.SaleRevenue = 0;
            this.TotalPriceInitial = 0;
            this.TotalGrossProfit = 0;
            this.TotalProfitPerSaleRevenue = 0;
            this.TotalProfitPerPriceInitial = 0;
        }
    }
}

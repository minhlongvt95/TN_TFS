using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Order
{
    public class ProductRevenueEntityModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal? OrderCount { get; set; }
        public decimal? ProductInOrderCount { get; set; }
        public decimal? ProductRefundCount { get; set; }
        public decimal? SaleRevenue { get; set; }
        public decimal? SaleRevenueRefund { get; set; }
        public decimal? TotalDiscount { get; set; }
        public decimal? TotalPriceInitial { get; set; }
        public decimal? TotalPriceInitialRefund { get; set; }
        public decimal? TotalGrossProfit { get; set; }
        public decimal? TotalProfitPerSaleRevenue { get; set; }
        public decimal? TotalProfitPerPriceInitial { get; set; }

        public ProductRevenueEntityModel()
        {
            ProductCode = "";
            ProductName = "";
            OrderCount = 0;
            ProductInOrderCount = 0;
            ProductRefundCount = 0;
            SaleRevenue = 0;
            SaleRevenueRefund = 0;
            TotalDiscount = 0;
            TotalPriceInitial = 0;
            TotalPriceInitialRefund = 0;
            TotalGrossProfit = 0;
            TotalProfitPerSaleRevenue = 0;
            TotalProfitPerPriceInitial = 0;
        }
    }
}

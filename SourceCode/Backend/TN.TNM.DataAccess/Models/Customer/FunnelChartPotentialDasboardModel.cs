using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Customer
{
    public class FunnelChartPotentialDasboardModel
    {
        public decimal? TotalPotentialCustomerConverted { get; set; }
        public decimal? TotalLead { get; set; }
        public decimal? TotalQuote { get; set; }
        public decimal? TotalCustomerOrder { get; set; }
        public string PercentPotentialToLead { get; set; }
        public string PercentLeadToQuote { get; set; }
        public string PercentQuoteToOrder { get; set; }

        public FunnelChartPotentialDasboardModel()
        {
            this.TotalPotentialCustomerConverted = 0;
            this.TotalLead = 0;
            this.TotalQuote = 0;
            this.TotalCustomerOrder = 0;
            this.PercentPotentialToLead = "";
            this.PercentLeadToQuote = "";
            this.PercentQuoteToOrder = "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetDataDashboardPotentialCustomerResult: BaseResult
    {
        public List<DataAccess.Models.Customer.ItemInvestmentChartPotentialCustomerDashboard> ListInvestmentFundDasboard { get; set; }
        public List<DataAccess.Models.Customer.CustomerEntityModel> TopNewestCustomer { get; set; }
        public List<DataAccess.Models.Customer.CustomerEntityModel> TopNewestCustomerConverted { get; set; }
        public DataAccess.Models.Customer.FunnelChartPotentialDasboardModel FunnelChartPotentialDasboardModel { get; set; }
    }
}

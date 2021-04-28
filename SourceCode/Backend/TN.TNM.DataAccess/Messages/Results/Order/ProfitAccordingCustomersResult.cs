using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class ProfitAccordingCustomersResult : BaseResult
    {
        public List<ProfitAccordingCustomersModel> ListProfitAccordingCustomers { get; set; }
        public decimal SumProfitMoney { get; set; }
        public decimal SumCapitalMoney { get; set; }
        public decimal SumGrossProfit { get; set; }
        public decimal SumGrossCapitalMoney { get; set; }
        public decimal SumGrossProfitMoney { get; set; }
    }
}

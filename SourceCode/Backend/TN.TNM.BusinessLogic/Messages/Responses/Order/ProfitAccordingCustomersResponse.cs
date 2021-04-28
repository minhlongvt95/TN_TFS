using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class ProfitAccordingCustomersResponse : BaseResponse
    {
        public List<ProfitAccordingCustomersModel> ListProfitAccordingCustomers { get; set; }
        public decimal SumProfitMoney { get; set; }
        public decimal SumCapitalMoney { get; set; }
        public decimal SumGrossProfit { get; set; }
        public decimal SumGrossCapitalMoney { get; set; }
        public decimal SumGrossProfitMoney { get; set; }
    }
}

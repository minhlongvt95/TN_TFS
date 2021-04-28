using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Order
{
    public class ProfitAccordingCustomersModel
    {
        public int STT { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public decimal ProfitMoney { get; set; }    //Doanh thu
        public decimal CapitalMoney { get; set; }    //Tiền vốn
        public decimal GrossProfit { get; set; }    //Lãi gộp
        public decimal GrossProfitMoney { get; set; } // Lãi/Doanh thu
        public decimal GrossCapitalMoney { get; set; } // Lãi/Giá vốn
    }
}

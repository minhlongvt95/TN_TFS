using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class PayOrderByLocalPointResult : BaseResult
    {
        public List<Guid?> ListLocalPointId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CashierName { get; set; } //Nhân viên thu ngân
    }
}

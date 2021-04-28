using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class PayOrderByLocalPointResponse : BaseResponse
    {
        public List<Guid?> ListLocalPointId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CashierName { get; set; } //Nhân viên thu ngân
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Order
{
    public class TopEmployeeRevenueEntityModel
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalProductInOrder { get; set; }
        public decimal? TotalOrder { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.BillSale
{
    public class OrderBillEntityModel
    {
        public Guid? OrderId { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public string  CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public decimal? TotalOrder { get; set; }
        public decimal? TotalQuantity { get; set; }
        public Guid? CustomerId { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetAllCustomerOrderParameter : BaseParameter
    {
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public string ProductCode { get; set; }
        public Guid? Seller { get; set; }
        public DateTime? OrderDateStart { get; set; }
        public DateTime? OrderDateEnd { get; set; }
        public int Vat { get; set; }
        public int? Top3NewOrder { get; set; }
        public List<Guid?> ListOrganizationId { get; set; }
        public string Phone { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class ProfitAccordingCustomersParameter : BaseParameter
    {
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public List<Guid> ListStatusId { get; set; }
        public Guid? QuoteId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? Seller { get; set; }
    }
}

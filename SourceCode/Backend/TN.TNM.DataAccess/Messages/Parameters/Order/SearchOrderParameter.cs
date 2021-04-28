using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class SearchOrderParameter : BaseParameter
    {
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public List<Guid> ListStatusId { get; set; }
        public string Phone { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Vat { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? QuoteId { get; set; }
        public Guid? ContractId { get; set; }
    }
}

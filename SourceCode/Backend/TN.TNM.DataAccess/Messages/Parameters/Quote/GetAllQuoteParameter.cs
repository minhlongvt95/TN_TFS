using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetAllQuoteParameter:BaseParameter
    {
        public string QuoteCode { get; set; }
        //public string CustomerName { get; set; }
        public string ProductCode { get; set; }
        //public Guid? Seller { get; set; }
        public List<Guid?> QuoteStatusId { get; set; }
        //public DateTime? OrderDateStart { get; set; }
        //public DateTime? OrderDateEnd { get; set; }
    }
}

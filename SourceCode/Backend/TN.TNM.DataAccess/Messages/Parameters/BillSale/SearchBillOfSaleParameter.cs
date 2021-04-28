using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.BillSale
{
    public class SearchBillOfSaleParameter : BaseParameter
    {
        public string BillOfSaleCode { get; set; }
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public List<Guid> ListStatusId { get; set; }
        public List<Guid> ListProductId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}

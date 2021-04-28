using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Contract
{
    public class SearchContractParameter : BaseParameter
    {
        public string ContractCode { get; set; }
        public string QuoteCode { get; set; }
        public List<Guid> ListProductId { get; set; }
        public List<Guid> ListEmployeeId { get; set; }
        public List<Guid> ListCutomerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}

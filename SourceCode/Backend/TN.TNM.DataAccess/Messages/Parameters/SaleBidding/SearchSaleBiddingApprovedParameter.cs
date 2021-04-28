using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.SaleBidding
{
    public class SearchSaleBiddingApprovedParameter:BaseParameter
    {
        public string SaleBiddingName { get; set; }
        public string CustomerName { get; set; }
        public List<Guid> EmployeeId { get; set; }
        public DateTime? BidStartDateForm { get; set; }
        public DateTime? BidStartDateTo { get; set; }
        public bool IsApproved { get; set; }
    }
}

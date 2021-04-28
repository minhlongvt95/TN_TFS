using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class SearchSaleBiddingApprovedRequest : BaseRequest<SearchSaleBiddingApprovedParameter>
    {
        public string SaleBiddingName { get; set; }
        public string CustomerName { get; set; }
        public List<Guid> EmployeeId { get; set; }
        public DateTime? BidStartDateForm { get; set; }
        public DateTime? BidStartDateTo { get; set; }
        public bool IsApproved { get; set; }
        public override SearchSaleBiddingApprovedParameter ToParameter()
        {
            return new SearchSaleBiddingApprovedParameter()
            {
                BidStartDateForm = BidStartDateForm,
                BidStartDateTo = BidStartDateTo,
                CustomerName = CustomerName,
                EmployeeId = EmployeeId,
                SaleBiddingName = SaleBiddingName,
                UserId = UserId,
                IsApproved = IsApproved
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetMonthsListRequest : BaseRequest<GetMonthsListParameter>
    {
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }
        public int MonthAdd { get; set; }
        public Guid OrganizationId { get; set; }

        public override GetMonthsListParameter ToParameter() => new GetMonthsListParameter()
        {
            UserId = UserId,
            OrderDateFrom = OrderDateFrom,
            OrderDateTo = OrderDateTo,
            MonthAdd = MonthAdd,
            OrganizationId = OrganizationId
        };
    }
}

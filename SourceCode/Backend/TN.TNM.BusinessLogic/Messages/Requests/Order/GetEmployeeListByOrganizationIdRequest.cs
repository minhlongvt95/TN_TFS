using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetEmployeeListByOrganizationIdRequest : BaseRequest<GetEmployeeListByOrganizationIdParameter>
    {
        public Guid OrganizationId { get; set; }
        public DateTime? OrderDateStart { get; set; }
        public DateTime? OrderDateEnd { get; set; }

        public override GetEmployeeListByOrganizationIdParameter ToParameter() => new GetEmployeeListByOrganizationIdParameter()
        {
            UserId = UserId,
            OrganizationId = OrganizationId,
            OrderDateStart = OrderDateStart,
            OrderDateEnd = OrderDateEnd
        };
    }
}
